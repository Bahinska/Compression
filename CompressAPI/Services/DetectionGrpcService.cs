using Grpc.Core;
using Sensor.Services;
using SensorApi.Protos;
using ServerAPI.Services;

public class DetectionGrpcService : DetectionService.DetectionServiceBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly WebSocketHandler _webSocketHandler;
    private readonly IEmailSenderExtended _emailSender;

    public DetectionGrpcService(IWebHostEnvironment environment, WebSocketHandler webSocketHandler, IEmailSenderExtended emailSender)
    {
        _environment = environment;
        _webSocketHandler = webSocketHandler;
        _emailSender = emailSender;
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

        await _emailSender.SendEmailWithImageAsync("margoshacatmm11880@gmail.com", "Object detected", decompressedMat.ToBytes());

        await _webSocketHandler.NotifyClientsAsync(decompressedMat.ToBytes());

        return new DetectionResponse
        {
            Message = "Decompressed photo saved successfully."
        };
    }
}