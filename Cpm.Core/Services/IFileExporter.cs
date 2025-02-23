using System.Threading.Tasks;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services
{
    public interface IFileExporter
    {
        Task<FileExportResult> Export(GridResultsVm viewModel);
    }
}
