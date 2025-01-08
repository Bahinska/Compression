using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Threading.Tasks;
using Zstandard.Net;

namespace CompressionApp.Client
{
    public class Client
    {
        public static async Task Start()
        {
            byte[] dataToSend = File.ReadAllBytes("original_data.txt");

            Stopwatch roundTripStopwatch = Stopwatch.StartNew();

            using (TcpClient client = new TcpClient("localhost", 8080))
            {
                NetworkStream stream = client.GetStream();
                stream.ReadTimeout = 5000;
                stream.WriteTimeout = 5000;

                try
                {
                    // Sending data
                    Stopwatch sendStopwatch = Stopwatch.StartNew();
                    stream.Write(dataToSend, 0, dataToSend.Length);
                    stream.Flush();
                    client.Client.Shutdown(SocketShutdown.Send);
                    sendStopwatch.Stop();
                    Console.WriteLine($"Send time: {sendStopwatch.ElapsedMilliseconds} ms");

                    // Receiving compressed data
                    Stopwatch receiveStopwatch = Stopwatch.StartNew();
                    MemoryStream memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    receiveStopwatch.Stop();
                    byte[] receivedData = memoryStream.ToArray();
                    Console.WriteLine($"Receive time: {receiveStopwatch.ElapsedMilliseconds} ms");

                    // Measuring decompression time
                    Stopwatch decompressionStopwatch = Stopwatch.StartNew();
                    byte[] decompressedData = Decompress(receivedData);
                    decompressionStopwatch.Stop();
                    Console.WriteLine($"Decompression time: {decompressionStopwatch.ElapsedMilliseconds} ms");

                    File.WriteAllBytes("received_data.txt", decompressedData);
                    Console.WriteLine("Data received and saved as 'received_data.txt'");

                    // Save results in shared Excel file
                    ExcelHelper.SaveResultsToExcel(new object[]{
                        sendStopwatch.ElapsedMilliseconds,
                        receiveStopwatch.ElapsedMilliseconds,
                        decompressionStopwatch.ElapsedMilliseconds,
                        roundTripStopwatch.ElapsedMilliseconds }
                    );
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error in network stream: {ex.Message}");
                }
            }

            roundTripStopwatch.Stop();
            Console.WriteLine($"Round-trip time: {roundTripStopwatch.ElapsedMilliseconds} ms");
        }

        private static byte[] Decompress(byte[] dataToDecompress)
        {
            using (MemoryStream input = new MemoryStream(dataToDecompress))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (ZstandardStream zstd = new ZstandardStream(input, CompressionMode.Decompress))
                    {
                        zstd.CopyTo(output);
                    }
                    return output.ToArray();
                }
            }
        }
    }
}