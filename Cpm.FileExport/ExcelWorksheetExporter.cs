using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services;
using Cpm.Core.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optional;

namespace Cpm.FileExport
{
    public class ExcelWorksheetExporter : IFileExporter
    {
        private readonly string[] _initialCols = new[] { "Week", "Commencing", "Labour", "Total" };

        public Task<FileExportResult> Export(GridResultsVm viewModel)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage())
            {
                package.Workbook.Properties.Title = "CPM4 Export";
                package.Workbook.Properties.Author = "CPM4";
                // TODO: add more metadata

                var sheet = package.Workbook.Worksheets.Add("Data grid");

                // ReSharper disable CoVariantArrayConversion
                sheet.Cells[1,1].LoadFromArrays(new object[][]
                    {
                        _initialCols
                            .Concat(viewModel.Series.Columns)
                            .ToArray(),
                    }
                    .Concat(
                        viewModel.Rows
                            .Select(row =>
                            new[] { ToDecimal(row.Week), row.Commencing, ToDecimal(row.Labour), string.Empty }
                                .Concat(row.Values.Select(x => x.Weight).Select(ToDecimal))
                                .ToArray()
                            )
                        )
                );
                // ReSharper restore CoVariantArrayConversion

                DoFormatting(viewModel, sheet);

                DoSums(viewModel, sheet);

                sheet.Cells.AutoFitColumns();

                package.SaveAs(stream);
            }

            stream.Seek(0, SeekOrigin.Begin);

            return Task.FromResult(new FileExportResult
            {
                Stream = stream,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = "grid.xlsx",
            });
        }

        private static object ToDecimal(string text)
        {
            return text
                .SomeNotNull()
                .Filter(v => !string.IsNullOrEmpty(v))
                .Map(v => v.Replace(",", ""))
                .Map(decimal.Parse)
                .ToNullable();
        }

        private void DoSums(GridResultsVm viewModel, ExcelWorksheet sheet)
        {
            var rows = viewModel.Rows.Count;
            var cols = viewModel.Series.Columns.Count;

            Enumerable.Range(2, rows)
                .ForEach(row => sheet.Cells[row, _initialCols.Length].FormulaR1C1 = $"=SUM(RC[1]:RC[{cols}])");

            Enumerable.Range(_initialCols.Length, cols + 1)
                .ForEach(col => sheet.Cells[rows + 2, col].FormulaR1C1 = $"=SUM(R[-1]C:R[{-rows}]C)");
        }

        private void DoFormatting(GridResultsVm viewModel, ExcelWorksheet sheet)
        {
            const string numberFormat = "#,##0";
            const string dateFormat = "dd/mm/yyyy";

            sheet.Row(1).Style.Font.Bold = true;

            sheet.Row(viewModel.Rows.Count + 2).Style.Font.Bold = true;

            sheet.Column(1).Style.Numberformat.Format = numberFormat;

            sheet.Column(2).Style.Numberformat.Format = dateFormat;
            sheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            sheet.Column(_initialCols.Length).Style.Font.Bold = true;

            Enumerable.Range(3, 2 + viewModel.Series.Columns.Count)
                .ForEach(col => sheet.Column(col).Style.Numberformat.Format = numberFormat);
        }
    }
}
