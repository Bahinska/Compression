using Microsoft.Extensions.Configuration;
using OpenCvSharp;
using Sensor.Services;
using System.Collections.Concurrent;

namespace CompressionApp.Client
{
    class Program
    {
        static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        static readonly object frameProcessingLock = new object();
        static bool isProcessing = false;
        static readonly BlockingCollection<(Mat frame, string detectedObject)> frameQueue = new BlockingCollection<(Mat frame, string detectedObject)>();
        static TransmissionService transmissionService = new TransmissionService();

        static async Task Main(string[] args)
        {
            var videoCaptureService = new VideoCaptureService();
            var analyzerService = new OpenCVAnalyzerService();
            var webSocketClient = new WebSocketClient(new Uri("ws://localhost:5039/ws/client"), new Uri("http://localhost:5039/api/health"));

             webSocketClient.ConnectAsync();
            Task.Run(BackgroundFrameProcessor);

            videoCaptureService.OnNewFrame += async (s, e) =>
            {
                await webSocketClient.SendFrameAsync(e.Frame);

                if (analyzerService.AnalyzeFrame(e.Frame, out string detectedObject))
                {
                    analyzerService.DisplayObjectRectangle(e.Frame);

                    frameQueue.Add((e.Frame.Clone(), detectedObject));
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

        static async Task BackgroundFrameProcessor()
        {
            while (!frameQueue.IsCompleted)
            {
                var (frame, detectedObject) = frameQueue.Take();
                await ProcessDetectionAsync(frame, detectedObject); // Replace null with your transmissionService
                frame.Dispose();
            }
        }

        static async Task ProcessDetectionAsync(Mat frame, string detectedObject)
        {
            await semaphoreSlim.WaitAsync();
            var squareFrame = VideoCaptureService.CropToSquare(frame.Clone());
            try
            {
                await transmissionService.SendDetectedObjectAsync(squareFrame, detectedObject);
            }
            finally
            {
                squareFrame.Dispose();
                semaphoreSlim.Release();
            }
        }
    }
}