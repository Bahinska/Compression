using OpenCvSharp;

namespace Sensor.Services
{
    public class TransmissionService
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task SendDetectedObjectAsync(Mat frame, string detectedObject)
        {
            var compressedFrame = DCTCompressionService.Compress(frame);

            var objectContent = new StringContent(
                $"{{\"detectedObject\":\"{detectedObject}\"}}",
                System.Text.Encoding.UTF8,
                "application/json");

            using var content = new MultipartFormDataContent
            {
                { objectContent, "detectedObject" },
                { new ByteArrayContent(compressedFrame), "frame", "compressedFrame.dct" }
            };

            var response = await httpClient.PostAsync("https://localhost:7246/api/detection", content);
            Console.WriteLine($"Detection and frame send response: {response.StatusCode}");
        }
    }
}