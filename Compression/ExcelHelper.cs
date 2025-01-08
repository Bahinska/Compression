using System;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

public static class ExcelHelper
{
    private static readonly string ExcelFilePath = @"\\MARGO\Shared\Results.xlsx"; // Adjust the shared network path

    public static void SaveResultsToExcel(object[] values, bool newLine = false)
    {
        // Apply file lock to avoid simultaneous write access
        lock (typeof(ExcelHelper))
        {
            FileInfo fileInfo = new FileInfo(ExcelFilePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Results");

                int row;
                if (newLine)
                {
                    row = worksheet.Dimension?.Rows + 1 ?? 1;
                }
                else
                {
                    row = worksheet.Dimension?.Rows ?? 1;
                }

                // If it is the first record, add headers
                if (row == 1)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = $"Column {i + 1}";
                        worksheet.Cells[row, 1, row, values.Length].Style.Font.Bold = true;
                    }
                    row++;
                }

                for (int i = 0; i < values.Length; i++)
                {
                    worksheet.Cells[row, i + 1].Value = values[i];
                }

                package.Save();
            }
        }
    }
}