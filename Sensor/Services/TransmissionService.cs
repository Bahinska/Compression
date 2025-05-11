using Grpc.Core;
using Grpc.Net.Client;
using OpenCvSharp;
using Sensor.Protos;

namespace Sensor.Services
{
    public class TransmissionService
    {
        private readonly DetectionService.DetectionServiceClient _grpcClient;

        public TransmissionService()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7246"); //grpc server address
            _grpcClient = new DetectionService.DetectionServiceClient(channel);
        }
        public async Task SendDetectedObjectAsync(Mat frame, string detectedObject)
        {
            //var compressedFrame = DCTCompressionService.Compress(frame);
            var compressedFrame = WaveletCompressionService.Compress(frame);

            var request = new DetectionRequest
            {
                DetectedObject = detectedObject,
                Frame = Google.Protobuf.ByteString.CopyFrom(compressedFrame)
            };

            try
            {
                var response = await _grpcClient.SendDetectedObjectAsync(request);
                Console.WriteLine(response.Message);
            }
            catch (RpcException rpcEx)
            {
                Console.WriteLine($"gRPC error: {rpcEx.Status.StatusCode} - {rpcEx.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending detected object via gRPC: {ex.Message}");
            }
        }
    }
}