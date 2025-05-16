using Grpc.Core;
using Sensor.Services;
using SensorApi.Protos;
using ServerAPI.Services;

public class DetectionGrpcService : DetectionService.DetectionServiceBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly WebSocketHandler _webSocketHandler;
    private readonly IEmailSenderExtended _emailSender;
    private readonly S3Service _s3Service;

    public DetectionGrpcService(IWebHostEnvironment environment, WebSocketHandler webSocketHandler,
        IEmailSenderExtended emailSender, S3Service s3Service)
    {
        _environment = environment;
        _webSocketHandler = webSocketHandler;
        _emailSender = emailSender;
        _s3Service = s3Service;
    }

    public override async Task<DetectionResponse> SendDetectedObject(DetectionRequest request, ServerCallContext context)
    {
        int rows = 480;
        int cols = 480;

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        string s3Key = $"decompressed_photos/{timestamp}.png";

        var compressedFrame = request.Frame.ToByteArray();
        var decompressedMat = WaveletCompressionService.Decompress(compressedFrame, rows, cols);

        // Upload the decompressed image to S3
        await _s3Service.UploadImageAsync(decompressedMat, s3Key);

        // Convert the decompressedMat to a byte array for email and websockets
        var decompressedBytes = decompressedMat.ToBytes();

        await _emailSender.SendEmailWithImageAsync("margoshacatmm11880@gmail.com", "Object detected", decompressedBytes);
        await _webSocketHandler.NotifyClientsAsync(decompressedBytes);

        return new DetectionResponse
        {
            Message = "Decompressed photo uploaded to S3 successfully."
        };
    }
}
