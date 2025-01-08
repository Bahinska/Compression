using OfficeOpenXml;
using System.Diagnostics;

namespace CompressionApp.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //await Client.Start();

            HttpClient client = new HttpClient();
            byte[] rawData = File.ReadAllBytes("original_data.txt");

            long compressBackTime = 0;
            long compressOkTime = 0;
            long decompressBackTime = 0;
            long decompressOkTime = 0;

            int num = 1;

            for (int i = 0; i < num; i++)
            {
                try
                {
                    var rawContent = new ByteArrayContent(rawData);
                    //compressBack

                    Stopwatch compressBackStopwatch = Stopwatch.StartNew();
                    HttpResponseMessage compressBackResponse =
                        await client.PostAsync("http://localhost:5196/api/compression/compressBack", rawContent);
                    compressBackStopwatch.Stop();
                    byte[] compressedData = await compressBackResponse.Content.ReadAsByteArrayAsync();

                    Console.WriteLine($"Raw: {rawData.Length / (1024*1024)}   Compressed: {compressedData.Length / (1024 * 1024)}");
                    compressBackTime += compressBackStopwatch.ElapsedMilliseconds;

                    //compressOk
                    Stopwatch compressOkStopwatch = Stopwatch.StartNew();
                    HttpResponseMessage compresisOkResponse =
                        await client.PostAsync("http://localhost:5196/api/compression/compressOk", rawContent);
                    compressOkStopwatch.Stop();
                    compressOkTime += compressOkStopwatch.ElapsedMilliseconds;

                    //decompressBack
                    var compressedContent = new ByteArrayContent(compressedData);

                    Stopwatch decompressBackStopwatch = Stopwatch.StartNew();
                    HttpResponseMessage decompressBackResponse =
                        await client.PostAsync("http://localhost:5196/api/compression/decompressBack",
                            compressedContent);
                    decompressBackStopwatch.Stop();
                    byte[] decompressedData = await decompressBackResponse.Content.ReadAsByteArrayAsync();
                    decompressBackTime += decompressBackStopwatch.ElapsedMilliseconds;

                    //decompressOk

                    Stopwatch decompressOkStopwatch = Stopwatch.StartNew();
                    HttpResponseMessage decompressOkResponse =
                        await client.PostAsync("http://localhost:5196/api/compression/decompressOk", compressedContent);
                    decompressOkStopwatch.Stop();
                    decompressOkTime += decompressOkStopwatch.ElapsedMilliseconds;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            // Save results in shared Excel file
            ExcelHelper.SaveResultsToExcel(new object[]
                {
                    compressBackTime /num,
                    compressOkTime /num,
                    decompressBackTime /num,
                    decompressOkTime /num
        }, true
            );
        }
    }
}