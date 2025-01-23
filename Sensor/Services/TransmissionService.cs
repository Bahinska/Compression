
using System.Collections.Concurrent;
using OpenCvSharp;

namespace Sensor.Services
{
    public class TransmissionService
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task SendDetectedObject(Mat frame, string detectedObject)
        {
            var compressedFrame = DCTCompressionService.Compress(frame);

            var objectContent = new StringContent(
                $"{{\"detectedObject\":\"{detectedObject}\"}}",
                System.Text.Encoding.UTF8,
                "application/json");

            using var content = new MultipartFormDataContent
            {
                { objectContent, "metadata" },
                { new ByteArrayContent(compressedFrame), "frame", "compressedFrame.dct" }
            };

            var response = await httpClient.PostAsync("http://localhost:5000/api/detection", content);
            Console.WriteLine($"Detection and frame send response: {response.StatusCode}");
        }


        public async Task SendVideoFragment(ConcurrentQueue<Mat> videoFragment)
        {
            if (videoFragment.IsEmpty)
            {
                Console.WriteLine("Video fragment is empty. Nothing to send.");
                return;
            }

            try
            {
                using var compressedVideoStream = new MemoryStream();

                while (videoFragment.TryDequeue(out var frame))
                {
                    using var grayFrame = frame.CvtColor(ColorConversionCodes.BGR2GRAY);
                    var compressedFrame = DCTCompressionService.Compress(grayFrame);
                    compressedVideoStream.Write(compressedFrame, 0, compressedFrame.Length);
                    frame.Dispose();
                }

                compressedVideoStream.Position = 0;

                var content = new MultipartFormDataContent
                {
                    { new StreamContent(compressedVideoStream), "video", "video_fragment_compressed.dct" }
                };

                var response = await httpClient.PostAsync("http://localhost:5000/api/compressed-video", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Compressed video fragment successfully sent.");
                }
                else
                {
                    Console.WriteLine($"Failed to send compressed video fragment: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error compressing and sending video fragment: {ex.Message}");
            }
        }
    }
}