using Microsoft.AspNetCore.Mvc;

namespace ServerAPI.Models
{
    public class DetectionRequest
    {
        [FromForm(Name = "frame")]
        public IFormFile Frame { get; set; }

        [FromForm(Name = "detectedObject")]
        public string DetectedObject { get; set; }
    }
}
