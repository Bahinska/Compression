using Grpc.Core;
using SensorApi.Protos;
using ServerAPI.Services;

public class DetectionGrpcService : DetectionService.DetectionServiceBase
{
    private readonly IWebHostEnvironment _environment;

    public DetectionGrpcService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public override async Task<DetectionResponse> SendDetectedObject(DetectionRequest request, ServerCallContext context)
    {
        int rows = 480;
        int cols = 480;

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var decompressedPath = Path.Combine(_environment.ContentRootPath, "decompressed_photos", $"{timestamp}.png");

        Directory.CreateDirectory(Path.GetDirectoryName(decompressedPath));

        var compressedFrame = request.Frame.ToByteArray();
        var decompressedMat = DCTDecompressionService.Decompress(compressedFrame, rows, cols);
        decompressedMat.SaveImage(decompressedPath);

        return new DetectionResponse
        {
            Message = "Decompressed photo saved successfully."
        };
    }
}