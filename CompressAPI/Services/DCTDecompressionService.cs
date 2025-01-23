using OpenCvSharp;

namespace ServerAPI.Services
{
    public class DCTDecompressionService
    {
        public static Mat Decompress(byte[] compressedFrame, int rows, int cols)
        {
            if (compressedFrame == null || compressedFrame.Length == 0)
                throw new ArgumentNullException(nameof(compressedFrame), "Input compressed frame cannot be null or empty.");

            try
            {
                var matrix = ByteArrayToMatrix(compressedFrame, rows, cols);

                // Apply Inverse 2D Discrete Cosine Transform (IDCT)
                Accord.Math.CosineTransform.IDCT(matrix);

                return MatrixToMat(matrix);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Decompress: {ex.Message}");
                throw;
            }
        }

        private static double[,] ByteArrayToMatrix(byte[] byteArray, int rows, int cols)
        {
            var matrix = new double[rows, cols];
            Buffer.BlockCopy(byteArray, 0, matrix, 0, byteArray.Length);
            return matrix;
        }

        private static Mat MatrixToMat(double[,] matrix)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            var mat = new Mat(rows, cols, MatType.CV_8UC1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    mat.Set(i, j, (byte)Math.Clamp(matrix[i, j], 0, 255));
                }
            }

            return mat;
        }
    }
}
