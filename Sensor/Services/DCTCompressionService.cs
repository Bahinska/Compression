using OpenCvSharp;

namespace Sensor.Services
{
    public static class DCTCompressionService
    {
        public static byte[] Compress(Mat frame)
        {
            if (frame == null || frame.Empty())
                throw new ArgumentNullException(nameof(frame), "Input frame cannot be null or empty.");

            try
            {
                var matrix = MatToMatrix(frame);

                // Apply 2D Discrete Cosine Transform (DCT)
                Accord.Math.CosineTransform.DCT(matrix);


                return MatrixToByteArray(matrix);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Compress: {ex.Message}");
                throw;
            }
        }

        private static double[,] MatToMatrix(Mat mat)
        {
            int rows = mat.Rows;
            int cols = mat.Cols;
            var matrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = mat.At<byte>(i, j);
                }
            }

            return matrix;
        }

        private static byte[] MatrixToByteArray(double[,] matrix)
        {
            int totalElements = matrix.Length;
            var byteArray = new byte[totalElements * sizeof(double)];
            Buffer.BlockCopy(matrix, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
    }
}