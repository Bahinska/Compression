
using OpenCvSharp;

namespace Sensor.Services
{
    public class VideoCaptureService
    {
        private VideoCapture _videoCapture;

        public event EventHandler<FrameEventArgs> OnNewFrame;

        public VideoCaptureService()
        {
            _videoCapture = new VideoCapture(0); // Open the first camera
            if (!_videoCapture.IsOpened())
                throw new Exception("Cannot open the camera");
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (_videoCapture.IsOpened())
                {
                    Mat frame = new Mat();
                    _videoCapture.Read(frame);
                    if (!frame.Empty())
                    {
                        OnNewFrame?.Invoke(this, new FrameEventArgs { Frame = frame });
                    }
                }
            });
        }

        public void Stop()
        {
            _videoCapture.Release();
        }
    }
}
