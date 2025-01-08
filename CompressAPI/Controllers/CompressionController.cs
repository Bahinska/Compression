using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompressionController : ControllerBase
    {
        private readonly CompressionService _compressionService;

        public CompressionController(CompressionService compressionService)
        {
            _compressionService = compressionService;
        }

        [HttpPost]
        [Route("compressBack")]
        public async Task<IActionResult> CompressDataAndSendBack()
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    byte[] receivedData = memoryStream.ToArray();
                    byte[] compressedData = await _compressionService.CompressAsync(receivedData);

                    return File(compressedData, "application/octet-stream");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error processing request: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("compressOk")]
        public async Task<IActionResult> CompressDataAndOk()
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    byte[] receivedData = memoryStream.ToArray();
                    byte[] compressedData = await _compressionService.CompressAsync(receivedData);

                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error processing request: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("decompressBack")]
        public async Task<IActionResult> DecompressDataAndSendBack()
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    byte[] receivedData = memoryStream.ToArray();
                    byte[] decompressedData = await _compressionService.DecompressAsync(receivedData);

                    return File(decompressedData, "application/octet-stream");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error processing request: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("decompressOk")]
        public async Task<IActionResult> DecompressDataAndOk()
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    byte[] receivedData = memoryStream.ToArray();
                    byte[] decompressedData = await _compressionService.DecompressAsync(receivedData);

                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error processing request: {ex.Message}");
            }
        }

    }
}
