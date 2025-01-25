using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using ServerAPI.Services;
using ServerAPI.Models;

namespace SensorApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class DetectionController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public DetectionController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        [HttpPost("detection")]
        public async Task<IActionResult> DecompressPhoto([FromForm] DetectionRequest request)
        {
            int rows = 480;
            int cols = 480;

            if (request.Frame == null || request.Frame.Length == 0)
            {
                return BadRequest("Invalid photo file");
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var decompressedPath = Path.Combine(_environment.ContentRootPath, "decompressed_photos", $"{timestamp}.png");

            Directory.CreateDirectory(Path.GetDirectoryName(decompressedPath));

            using (var memoryStream = new MemoryStream())
            {
                await request.Frame.CopyToAsync(memoryStream);
                var compressedFrame = memoryStream.ToArray();

                var decompressedMat = DCTDecompressionService.Decompress(compressedFrame, rows, cols);
                decompressedMat.SaveImage(decompressedPath);
            }

            return Ok("Decompressed photo saved successfully.");
        }

        
    }
}