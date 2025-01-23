
using OpenCvSharp;

namespace Sensor.Services
{
    public class VideoCaptureService
    {
        private VideoCapture _videoCapture;
        private int _frameRate;
        public event EventHandler<FrameEventArgs> OnNewFrame;

        public VideoCaptureService(int frameRate = 30)
        {
            _videoCapture = new VideoCapture(0);
            if (!_videoCapture.IsOpened())
                throw new Exception("Cannot open the camera");

            _frameRate = frameRate;
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

                    Task.Delay(1000 / _frameRate).Wait();
                }
            });
        }

        public void Stop()
        {
            _videoCapture.Release();
        }
    }
}
