using Grpc.Core;
using Sensor.Services;
using SensorApi.Protos;
using ServerAPI.Services;

public class DetectionGrpcService : DetectionService.DetectionServiceBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly WebSocketHandler _webSocketHandler;

    public DetectionGrpcService(IWebHostEnvironment environment, WebSocketHandler webSocketHandler)
    {
        _environment = environment;
        _webSocketHandler = webSocketHandler;
    }

    public override async Task<DetectionResponse> SendDetectedObject(DetectionRequest request, ServerCallContext context)
    {
        int rows = 480;
        int cols = 480;

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var decompressedPath = Path.Combine(_environment.ContentRootPath, "decompressed_photos", $"{timestamp}.png");

        Directory.CreateDirectory(Path.GetDirectoryName(decompressedPath));

        var compressedFrame = request.Frame.ToByteArray();
        //var decompressedMat = DCTDecompressionService.Decompress(compressedFrame, rows, cols);
        var decompressedMat = WaveletCompressionService.Decompress(compressedFrame, rows, cols);

        decompressedMat.SaveImage(decompressedPath);

        await _webSocketHandler.NotifyClientsAsync(decompressedMat.ToBytes());

        return new DetectionResponse
        {
            Message = "Decompressed photo saved successfully."
        };
    }
}