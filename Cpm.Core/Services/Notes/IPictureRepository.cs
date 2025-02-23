using System;
using System.IO;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Notes
{
    public interface IPictureRepository
    {
        Task Store(string siteId, string fieldId, DateTime date, string tag, Stream stream, string originalFileName);
        string GetFullUrl(string siteId, string fieldId, DateTime date, string id, string fileExtension);
        string GetThumbUrl(string siteId, string fieldId, DateTime date, string id);
        Task DeletePicture(string fieldId, DateTime date, string id);
    }
}