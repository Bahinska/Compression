using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensor.Services
{
    public class TransmissionService
    {
        private HttpClient httpClient;

        public TransmissionService()
        {
            httpClient = new HttpClient();
        }

        public async Task SendData(byte[] compressedFrame)
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(compressedFrame), "frame", "frame.dct");
            await httpClient.PostAsync("http://localhost:5000/api/data", content);
        }

        public async Task SendDetectedObject(string detectedObject)
        {
            var content = new StringContent($"{{\"detectedObject\":\"{detectedObject}\"}}", System.Text.Encoding.UTF8, "application/json");
            await httpClient.PostAsync("http://localhost:5000/api/detection", content);
        }
    }
}
