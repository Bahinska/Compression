using OpenCvSharp;
using Sensor.Services;

namespace CompressionApp.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var videoCaptureService = new VideoCaptureService();
            var dctCompressionService = new DCTCompressionService();
            var transmissionService = new TransmissionService();
            var analyzerService = new OpenCVAnalyzerService();

            videoCaptureService.OnNewFrame += async (s, e) =>
            {
                using (var frame = e.Frame)
                {
                    if (analyzerService.AnalyzeFrame(frame, out string detectedObject))
                    {
                        //await transmissionService.SendDetectedObject(detectedObject);
                        Cv2.PutText(frame, "Detected: " + detectedObject, new Point(10, 30), HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);

                        var objects = analyzerService.GetDetectedObjects(); // Assuming you store the bounding box coordinates here

                        // Draw borders around detected faces or bodies
                        foreach (var ob in objects)
                        {
                            // Draw a rectangle around the detected face or body
                            Cv2.Rectangle(frame, ob, Scalar.Green, 2); // Green color with a thickness of 2
                        }
                    }

                    //Task.Run(async()=>await dctCompressionService.CompressAsync(frame)) ;
                    //await transmissionService.SendData(compressedFrame);

                    // Display the video
                    Cv2.ImShow("Sensor Video Stream", frame);
                    Cv2.WaitKey(1); // Allow OpenCV to process the display
                }
            };

            videoCaptureService.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); // Block the main thread until a key is pressed

            videoCaptureService.Stop();
            Cv2.DestroyAllWindows();
        }
    }

}