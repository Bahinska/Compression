using OpenCvSharp;
using Sensor.Services.Interfaces;

namespace Sensor.Services
{
    public class OpenCVAnalyzerService : IAnalyzerService
    {
        private readonly CascadeClassifier faceCascade;
        private readonly CascadeClassifier bodyCascade;
        private List<Rect> detectedObjects;

        public OpenCVAnalyzerService()
        {
            var baseDirectory = AppContext.BaseDirectory;
            faceCascade = new CascadeClassifier(Path.Combine(baseDirectory, "..\\..\\..\\Data\\haarcascade_frontalface_default.xml"));
            bodyCascade = new CascadeClassifier(Path.Combine(baseDirectory, "..\\..\\..\\Data\\haarcascade_fullbody.xml"));
            detectedObjects = new List<Rect>();
        }

        public bool AnalyzeFrame(Mat frame, out string detectedObject)
        {
            var grayFrame = new Mat();
            Cv2.CvtColor(frame, grayFrame, ColorConversionCodes.BGR2GRAY);

            // Reset detected objects list
            detectedObjects.Clear();

            var faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 4, HaarDetectionTypes.ScaleImage);
            var body = bodyCascade.DetectMultiScale(grayFrame, 1.1, 4, HaarDetectionTypes.ScaleImage);

            if (faces.Length > 0)
            {
                detectedObjects.AddRange(faces); // Add all face rectangles
                detectedObject = "face";
                return true;
            }

            if (body.Length > 0)
            {
                detectedObjects.AddRange(body); // Add all body rectangles
                detectedObject = "body";
                return true;
            }

            detectedObject = string.Empty;
            return false;
        }

        public List<Rect> GetDetectedObjects()
        {
            return detectedObjects; // Return the list of detected objects (faces, bodies, etc.)
        }
    }

}
