using Microsoft.Extensions.Configuration;
using OpenCvSharp;
using Sensor.Services;

namespace CompressionApp.Client
{
    class Program
    {
        static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        static readonly object frameProcessingLock = new object();
        static bool isProcessing = false;

        static async Task Main(string[] args)
        {
            var videoCaptureService = new VideoCaptureService();
            var transmissionService = new TransmissionService();
            var analyzerService = new OpenCVAnalyzerService();
            var webSocketClient = new WebSocketClient(new Uri("ws://localhost:5039/ws/client"));

            //await webSocketClient.ConnectAsync();

            videoCaptureService.OnNewFrame += async (s, e) =>
            {
                await webSocketClient.SendFrameAsync(e.Frame);

                if (analyzerService.AnalyzeFrame(e.Frame, out string detectedObject))
                {
                    //analyzerService.DisplayObjectRectangle(e.Frame);

                    // Ensure only one processing task runs at a time
                    lock (frameProcessingLock)
                    {
                        if (isProcessing)
                        {
                            return;
                        }

                        isProcessing = true;
                    }

                    //await ProcessDetectionAsync(e.Frame, detectedObject, transmissionService);

                    lock (frameProcessingLock)
                    {
                        isProcessing = false;
                    }
                }

                Cv2.ImShow("Sensor Video Stream", e.Frame);
                Cv2.WaitKey(1);

                e.Frame.Dispose();
            };
            
            videoCaptureService.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            videoCaptureService.Stop();
            await webSocketClient.DisconnectAsync();
            Cv2.DestroyAllWindows();
        }

        static async Task ProcessDetectionAsync(Mat frame, string detectedObject, TransmissionService transmissionService)
        {
            await semaphoreSlim.WaitAsync();
            var squareFrame = VideoCaptureService.CropToSquare(frame.Clone());
            try
            {
                await transmissionService.SendDetectedObjectAsync(frame, detectedObject);
            }
            finally
            {
                squareFrame.Dispose();
                semaphoreSlim.Release();
            }
        }
    }
}