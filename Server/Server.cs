using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Zstandard.Net;

namespace CompressionApp.Server
{
    public class Server
    {
        public static async Task Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClient(client);
            }
        }

        private static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = 5000;
            stream.WriteTimeout = 5000;

            MemoryStream memoryStream = new MemoryStream();

            try
            {
                // Reading incoming data
                await stream.CopyToAsync(memoryStream);
                byte[] receivedData = memoryStream.ToArray();
                Console.WriteLine($"Received data size: {receivedData.Length}");

                // Measuring time for compression
                Stopwatch stopwatch = Stopwatch.StartNew();
                byte[] compressedData = Compress(receivedData);
                stopwatch.Stop();
                Console.WriteLine($"Compression time: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Compressed data size: {compressedData.Length}");

                // Save results in shared Excel file
                ExcelHelper.SaveResultsToExcel(new object[]{
                    receivedData.Length,
                    compressedData.Length,
                    stopwatch.ElapsedMilliseconds},
                    true
                );

                // Sending compressed data back to client
                await stream.WriteAsync(compressedData, 0, compressedData.Length);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error in network stream: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private static byte[] Compress(byte[] dataToCompress)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (ZstandardStream zstd = new ZstandardStream(output, CompressionMode.Compress))
                {
                    zstd.Write(dataToCompress, 0, dataToCompress.Length);
                }
                return output.ToArray();
            }
        }
    }
}