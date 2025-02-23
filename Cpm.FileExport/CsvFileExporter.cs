using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cpm.Core.Services;
using Cpm.Core.ViewModels;

namespace Cpm.FileExport
{
    public class CsvFileExporter : IFileExporter
    {
        public async Task<FileExportResult> Export(GridResultsVm viewModel)
        {
            var writer = new StreamWriter(new MemoryStream(), Encoding.UTF8, 1024, true);

            writer.WriteCsvLine(
                new [] {"Week", "Commencing", "Labour", "Total"}
                .Concat(viewModel.Series.Columns)
            );

            writer.WriteCsvLine(
                new []
                {
                    "",
                    "",
                    "",
                    viewModel.SummaryHeader.GrandTotal
                }
                .Concat(viewModel.SummaryHeader.Subtotals)
            );

            foreach (var row in viewModel.Rows)
            {
                writer.WriteCsvLine(
                    new[]
                        {
                            row.Week,
                            row.Commencing,
                            row.Labour,
                            row.Total,
                        }
                        .Concat(row.Values.Select(x => x.Weight))
                );
            }

            await writer.FlushAsync();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            return new FileExportResult
            {
                Stream = writer.BaseStream,
                ContentType = "text/csv",
                FileName = "grid.csv",
            };
        }
    }
}
