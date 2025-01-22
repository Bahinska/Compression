using OpenCvSharp;

namespace Sensor.Services.Interfaces
{
    public interface IAnalyzerService
    {
        bool AnalyzeFrame(Mat frame, out string detectedObject);
    }
}
