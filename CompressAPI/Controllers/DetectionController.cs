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
        public async Task<IActionResult> ProcessDetection([FromForm] DetectionRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.DetectedObject))
            {
                return BadRequest("Invalid request data");
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var photoPath = Path.Combine(_environment.ContentRootPath, "photos", $"{timestamp}.dct");
            var metadataPath = Path.Combine(_environment.ContentRootPath, "photos", $"{timestamp}.json");

            Directory.CreateDirectory(Path.GetDirectoryName(photoPath));

            // Save metadata
            await System.IO.File.WriteAllTextAsync(metadataPath, request.DetectedObject);

            // Save compressed frame
            using (var stream = new FileStream(photoPath, FileMode.Create))
            {
                await request.Frame.CopyToAsync(stream);
            }

            return Ok("Photo and metadata processed successfully.");
        }

        [HttpPost("compressed-video")]
        public async Task<IActionResult> ProcessVideo([FromForm] IFormFile video)
        {
            if (video == null || video.Length == 0)
            {
                return BadRequest("Invalid video file");
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var videoPath = Path.Combine(_environment.WebRootPath, "videos", $"{timestamp}.dct");

            Directory.CreateDirectory(Path.GetDirectoryName(videoPath));

            // Save compressed video
            using (var stream = new FileStream(videoPath, FileMode.Create))
            {
                await video.CopyToAsync(stream);
            }

            return Ok("Compressed video processed successfully.");
        }

        [HttpPost("decompress-photo")]
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

        [HttpPost("decompress-video")]
        public async Task<IActionResult> DecompressVideo([FromForm] IFormFile video, [FromForm] int rows, [FromForm] int cols)
        {
            if (video == null || video.Length == 0)
            {
                return BadRequest("Invalid video file");
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var decompressedVideoPath = Path.Combine(_environment.WebRootPath, "decompressed_videos", $"{timestamp}.avi");

            Directory.CreateDirectory(Path.GetDirectoryName(decompressedVideoPath));

            using (var memoryStream = new MemoryStream())
            {
                await video.CopyToAsync(memoryStream);
                var compressedVideo = memoryStream.ToArray();

                var frameSize = rows * cols * sizeof(double);
                var frames = new List<Mat>();

                for (int i = 0; i < compressedVideo.Length; i += frameSize)
                {
                    var compressedFrame = new byte[frameSize];
                    Array.Copy(compressedVideo, i, compressedFrame, 0, frameSize);

                    var decompressedMat = DCTDecompressionService.Decompress(compressedFrame, rows, cols);
                    frames.Add(decompressedMat);
                }

                using (var writer = new VideoWriter(decompressedVideoPath, FourCC.XVID, 30, new OpenCvSharp.Size(cols, rows), false))
                {
                    foreach (var frame in frames)
                    {
                        writer.Write(frame);
                        frame.Dispose();
                    }
                }
            }

            return Ok("Decompressed video saved successfully.");
        }
    }
}