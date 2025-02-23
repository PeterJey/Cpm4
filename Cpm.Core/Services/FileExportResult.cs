using System.IO;

namespace Cpm.Core.Services
{
    public class FileExportResult
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public object FileName { get; set; }
    }
}