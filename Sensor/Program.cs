using OpenCvSharp;
using Sensor.Services;
using Sensor.Services.Interfaces;
using System.Collections.Concurrent;


namespace CompressionApp.Client
{
    class Program
    {
        static readonly ConcurrentQueue<(Mat frame, DateTime timestamp)> frameBufferQueue = new ConcurrentQueue<(Mat frame, DateTime timestamp)>();
        static readonly TimeSpan BufferTimeSpan = TimeSpan.FromSeconds(10);
        static readonly SemaphoreSlim semaphoreSlim = new (1, 1);

        static async Task Main(string[] args)
        {
            var videoCaptureService = new VideoCaptureService();
            var transmissionService = new TransmissionService();
            var analyzerService = new OpenCVAnalyzerService();

            videoCaptureService.OnNewFrame += async (s, e) =>
            {
                var frame = e.Frame.Clone();
                var timestamp = DateTime.Now;

                frameBufferQueue.Enqueue((frame, timestamp));
                RemoveOldFrames(timestamp);

                if (analyzerService.AnalyzeFrame(frame, out string detectedObject))
                {
                    Console.WriteLine("Object detected");
                    analyzerService.DisplayObjectRectangle(frame);

                    await semaphoreSlim.WaitAsync();
                    try
                    {
                        _ = Task.Run(() => transmissionService.SendDetectedObject(frame, detectedObject));

                        ConcurrentQueue<Mat> videoFragment = GetVideoFragment(timestamp);
                        _ = Task.Run(() => transmissionService.SendVideoFragment(videoFragment));
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                }

                Cv2.ImShow("Sensor Video Stream", frame);
                Cv2.WaitKey(1);

                e.Frame.Dispose();
            };

            videoCaptureService.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            videoCaptureService.Stop();
            Cv2.DestroyAllWindows();

            ClearAllFrames();
        }

        static void RemoveOldFrames(DateTime currentTimestamp)
        {
            while (!frameBufferQueue.IsEmpty)
            {
                if (frameBufferQueue.TryPeek(out var oldestFrame) && currentTimestamp - oldestFrame.timestamp > BufferTimeSpan)
                {
                    if (frameBufferQueue.TryDequeue(out var oldFrame))
                    {
                        oldFrame.frame.Dispose();
                    }
                }
                else
                {
                    break;
                }
            }
        }

        static ConcurrentQueue<Mat> GetVideoFragment(DateTime detectionTime)
        {
            var fragment = new ConcurrentQueue<Mat>();
            var startTimestamp = detectionTime.AddSeconds(-5);
            var endTimestamp = detectionTime.AddSeconds(5);

            // Adding frames in the range [detectionTime - 5s, detectionTime + 5s]
            foreach (var (frame, timestamp) in frameBufferQueue)
            {
                if (timestamp >= startTimestamp && timestamp <= endTimestamp)
                {
                    fragment.Enqueue(frame.Clone());
                }
            }

            return fragment;
        }

        static void ClearAllFrames()
        {
            while (!frameBufferQueue.IsEmpty)
            {
                if (frameBufferQueue.TryDequeue(out var oldFrame))
                {
                    oldFrame.frame.Dispose();
                }
            }
        }
    }
}