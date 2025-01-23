using OpenCvSharp;

namespace Sensor.Services
{
    public class FrameEventArgs : EventArgs
    {
        public Mat Frame { get; set; }

    }
}

