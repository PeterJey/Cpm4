using System.Linq;
using Cpm.Core.Models;
using Cpm.Core.Services.Fields;

namespace Cpm.Core.ViewModels
{
    public class FieldVm
    {
        public string FieldName { get; set; }
        public string Description { get; set; }
        public string Variety { get; set; }

        private FieldVm()
        {
        }

        public static FieldVm FromField(Field field)
        {
            return new FieldVm
            {
                Id = field.FieldId,
                FieldName = field.Name,
                Variety = field.Variety,
                Description = field.FieldScores
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefault()
                    ?.Description,
                Index = field.Order
            };
        }

        public static FieldVm FromFieldDetails(FieldDetails details)
        {
            return new FieldVm
            {
                Id = details.FieldId,
                FieldName = details.FieldName,
                Variety = details.Variety,
                Description = details.Description,
                Index = details.Index
            };
        }

        public int Index { get; set; }

        public static FieldVm Placeholder = new FieldVm
        {
            IsPlaceholder = true
        };

        public bool IsPlaceholder { get; private set; }
        public string Id { get; set; }
    }
}