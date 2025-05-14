using Amazon.S3;
using Amazon.S3.Model;
using Grpc.Core;
using OpenCvSharp;
using Sensor.Services;
using SensorApi.Protos;
using ServerAPI.Services;

public class DetectionGrpcService : DetectionService.DetectionServiceBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly WebSocketHandler _webSocketHandler;
    private readonly IEmailSenderExtended _emailSender;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "detections-lnu";

    public DetectionGrpcService(IWebHostEnvironment environment, WebSocketHandler webSocketHandler,
        IEmailSenderExtended emailSender)
    {
        _environment = environment;
        _webSocketHandler = webSocketHandler;
        _emailSender = emailSender;
        _s3Client = new AmazonS3Client();
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
        await UploadImageToS3(decompressedMat, s3Key);

        // Convert the decompressedMat to a byte array for email and websockets
        var decompressedBytes = decompressedMat.ToBytes();

        await _emailSender.SendEmailWithImageAsync("margoshacatmm11880@gmail.com", "Object detected", decompressedBytes);
        await _webSocketHandler.NotifyClientsAsync(decompressedBytes);

        return new DetectionResponse
        {
            Message = "Decompressed photo uploaded to S3 successfully."
        };
    }

    private async Task UploadImageToS3(Mat image, string s3Key)
    {
        using (var stream = new MemoryStream())
        {
            // Save the image to the memory stream as a PNG
            image.WriteToStream(stream, ".png");

            // Configure the uploaded stream position
            stream.Position = 0;

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = s3Key,
                InputStream = stream,
                ContentType = "image/png",
            };

            var response = await _s3Client.PutObjectAsync(putRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Failed to upload file to S3");
            }
        }
    }
}

public static class MatExtensions
{
    public static void WriteToStream(this Mat mat, Stream stream, string format)
    {
        Cv2.ImEncode(format, mat, out byte[] buf);
        stream.Write(buf, 0, buf.Length);
    }
}
