using System;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Core.Extensions;
using Cpm.Core.Models;
using Cpm.Core.Services;
using Cpm.Infrastructure.Data;
using Cpm.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cpm.Infrastructure
{
    public class EfHarvestManager : IHarvestManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuditDataProvider _auditDataProvider;

        public EfHarvestManager(
            ApplicationDbContext dbContext,
            IAuditDataProvider auditDataProvider
            )
        {
            _dbContext = dbContext;
            _auditDataProvider = auditDataProvider;
        }

        public async Task SetHarvestedWeight(string fieldId, DateTime date, decimal? value, bool last)
        {
            var field = await _dbContext.Fields
                .Include(x => x.Site)
                .ThenInclude(x => x.Farm)
                .SingleAsync(x => x.FieldId == fieldId);

            var firstDayOfYear = field.Site.Farm.FirstDayOfYear;

            await MakeChange(
                c => c.HarvestRegisters, 
                fieldId,
                value,
                date,
                (values, firstDay) =>
                {
                    if (values.Any())
                    {
                        var lastDay = firstDay.AddDays(values.Length - 1);

                        var lastWeekDays = (int)lastDay.Subtract(lastDay.FirstDayOfWeek(firstDayOfYear)).TotalDays + 1;

                        var trailingNulls = values
                            .Reverse()
                            .TakeWhile(x => !x.HasValue)
                            .Count();

                        if (trailingNulls > 6)
                        {
                            // too much padding, trim everything
                            values = values
                                .SkipNullsFromEnd()
                                .ToArray();
                        }
                        else if (last)
                        {
                            values = values
                                .RightPad(7 - lastWeekDays, null)
                                .ToArray();
                        }
                        else
                        {
                            // padding not needed any more, trim everything
                            values = values
                                .SkipNullsFromEnd()
                                .ToArray();
                        }
                    }

                    return values;
                });
        }

        private async Task MakeChange<TEntity>(
            Func<ApplicationDbContext, DbSet<TEntity>> setSelector, 
            string fieldId,
            decimal? value,
            DateTime date,
            Func<decimal?[], DateTime, decimal?[]> postProcessor
            )
            where TEntity : SerializedValuesRegister, IVersionable, new()
        {
            await _dbContext.Persist(
                setSelector, 
                x => x.FieldId == fieldId,
                _auditDataProvider,
                (register, isNew) =>
                {
                    VirtualArray<decimal?> values;
                    if (isNew)
                    {
                        register.FieldId = fieldId;
                        register.FirstDay = date.Date;
                        values = new VirtualArray<decimal?>();
                        values.Add(value);
                    }
                    else
                    {
                        register.Field = null;

                        var originalValues = JsonConvert.DeserializeObject<decimal?[]>(register.SerializedValues);

                        var trailingNulls = originalValues
                            .Reverse()
                            .TakeWhile(x => !x.HasValue)
                            .Count();

                        var indexOfLastNonNull = originalValues.Length - trailingNulls - 1;

                        values = new VirtualArray<decimal?>(originalValues);

                        var offset = (int)date.Date.Subtract(register.FirstDay.Date).TotalDays;

                        values.SetAt(offset, value);
                    }

                    values.Process(x => x.HasValue && x.Value == 0 ? null : x);
                    values.LeftTrim(x => !x.HasValue);

                    var rawValues = values.ToArray();
                    
                    register.FirstDay = register.FirstDay.AddDays(values.Offset);

                    rawValues = postProcessor(rawValues, register.FirstDay);

                    register.SerializedValues = JsonConvert.SerializeObject(rawValues);
                });
        } 

        public Task SetPlannedPick(string fieldId, DateTime date, decimal? value)
        {
            return MakeChange(
                c => c.PickingPlans,
                fieldId,
                value,
                date,
                (values, firstDay) => values
                    .SkipNullsFromEnd()
                    .ToArray()
                );
        }
    }
}