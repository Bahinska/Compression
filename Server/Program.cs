using OfficeOpenXml;

namespace CompressionApp.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Console.WriteLine("Starting server...");
            await Server.Start();
        }
    }
}