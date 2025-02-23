using System;
using System.Threading.Tasks;
using Cpm.Core.Models;

namespace Cpm.Core.Services.Fields
{
    public interface IFieldRepository
    {
        Task<FieldDetails> GetFieldById(string fieldId);
        Task<SiteDetails> GetSiteById(string siteId);

        Task SaveFieldNote(string fieldId, DateTime date, Action<PinnedNote> updateAction);
    }
}