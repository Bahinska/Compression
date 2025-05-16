using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Services;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly S3Service _s3Service;

        public PhotosController(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos([FromQuery]DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest("Invalid date range: 'fromDate' must be earlier than 'toDate'.");
                }

                var photos = await _s3Service.GetPhotosAsync(fromDate, toDate);
                return Ok(photos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
