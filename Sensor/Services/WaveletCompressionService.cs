using OpenCvSharp;
using System;
using System.Diagnostics;
using Accord.Math.Wavelets;

namespace Sensor.Services
{
    public static class WaveletCompressionService
    {
        public static byte[] Compress(Mat frame)
        {
            if (frame == null || frame.Empty())
                throw new ArgumentNullException(nameof(frame), "Input frame cannot be null or empty.");

            try
            {
                var matrix = MatToMatrix(frame);

                // Засікання часу перед початком стиснення
                var stopwatch = Stopwatch.StartNew();

                // Визначаємо кількість ітерацій для CDF97 (Wavelet Transform)
                int levels = CalculateLevels(matrix.GetLength(0));

                // Apply 2D Wavelet Transform (CDF 9/7)
                var wavelet = new CDF97(5);
                wavelet.Forward(matrix);

                // Зупинка таймера після завершення стиснення
                stopwatch.Stop();
                Console.WriteLine($"Compression took {stopwatch.ElapsedMilliseconds} ms");

                return MatrixToByteArray(matrix);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Compress: {ex.Message}");
                throw;
            }
        }

        public static Mat Decompress(byte[] compressedFrame, int rows, int cols)
        {
            if (compressedFrame == null || compressedFrame.Length == 0)
                throw new ArgumentNullException(nameof(compressedFrame), "Input compressed frame cannot be null or empty.");

            try
            {
                var matrix = ByteArrayToMatrix(compressedFrame, rows, cols);

                // Засікання часу перед початком декомпресування
                var stopwatch = Stopwatch.StartNew();

                // Визначаємо кількість ітерацій для CDF97 (Wavelet Transform)
                int levels = CalculateLevels(rows);

                // Apply Inverse 2D Wavelet Transform (CDF 9/7)
                var wavelet = new CDF97(5);
                wavelet.Backward(matrix);

                // Зупинка таймера після завершення декомпресування
                stopwatch.Stop();
                Console.WriteLine($"Decompression took {stopwatch.ElapsedMilliseconds} ms");

                return MatrixToMat(matrix);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Decompress: {ex.Message}");
                throw;
            }
        }

        private static int CalculateLevels(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "The size must be greater than zero.");

            return (int)Math.Floor(Math.Log(size, 2)); // Рівні ітерацій - логарифм за основою 2 від розміру
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
            int totalElements = matrix.GetLength(0) * matrix.GetLength(1);
            var flatMatrix = new double[totalElements];
            Buffer.BlockCopy(matrix, 0, flatMatrix, 0, flatMatrix.Length * sizeof(double));
            var byteArray = new byte[flatMatrix.Length * sizeof(double)];
            Buffer.BlockCopy(flatMatrix, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }

        private static double[,] ByteArrayToMatrix(byte[] byteArray, int rows, int cols)
        {
            var flatMatrix = new double[rows * cols];
            Buffer.BlockCopy(byteArray, 0, flatMatrix, 0, byteArray.Length);
            var matrix = new double[rows, cols];
            Buffer.BlockCopy(flatMatrix, 0, matrix, 0, flatMatrix.Length * sizeof(double));
            return matrix;
        }

        private static Mat MatrixToMat(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
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