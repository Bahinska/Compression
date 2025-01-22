using OpenCvSharp;

namespace Sensor.Services
{
    internal class DCTCompressionService
    {
        /// <summary>
        /// Compresses an OpenCV Mat object using the Discrete Cosine Transform (DCT).
        /// </summary>
        /// <param name="frame">The input Mat frame.</param>
        /// <returns>A byte array representing the compressed frame.</returns>
        public async Task<byte[]> CompressAsync(Mat frame)
        {
            if (frame == null || frame.Empty())
                throw new ArgumentNullException(nameof(frame), "Input frame cannot be null or empty.");

            // Convert the Mat to a 2D double array (grayscale pixel values)
            double[,] matrix = MatToMatrix(frame);

            // Apply 2D Discrete Cosine Transform (DCT)
            Accord.Math.CosineTransform.DCT(matrix);

            // Convert the transformed matrix into a byte array
            return MatrixToByteArray(matrix);
        }

        /// <summary>
        /// Converts a Mat object to a 2D double[,] array representing grayscale pixel values.
        /// </summary>
        /// <param name="mat">The input Mat object.</param>
        /// <returns>A 2D double[,] array of pixel values.</returns>
        private double[,] MatToMatrix(Mat mat)
        {
            // Convert to grayscale if not already
            if (mat.Type() != MatType.CV_8UC1)
                mat = mat.CvtColor(ColorConversionCodes.BGR2GRAY);

            int rows = mat.Rows;
            int cols = mat.Cols;

            // Create a 2D double array to hold pixel values
            double[,] matrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = mat.At<byte>(i, j); // Get pixel intensity
                }
            }

            return matrix;
        }

        /// <summary>
        /// Converts a 2D double[,] matrix to a byte array for serialization.
        /// </summary>
        /// <param name="matrix">The 2D double[,] matrix.</param>
        /// <returns>A byte array representing the serialized matrix.</returns>
        private byte[] MatrixToByteArray(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            // Calculate the total number of elements
            int totalElements = rows * cols;

            // Create a buffer for the byte array
            byte[] byteArray = new byte[totalElements * sizeof(double)];

            // Flatten the 2D array and copy the values as bytes
            Buffer.BlockCopy(matrix, 0, byteArray, 0, byteArray.Length);

            return byteArray;
        }
    }
}
