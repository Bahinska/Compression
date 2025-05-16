using Amazon.S3;
using Amazon.S3.Model;
using OpenCvSharp;
using System.Globalization;

namespace ServerAPI.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "detections-lnu";
        private readonly string _region = "eu-north-1";

        public S3Service()
        {
            _s3Client = new AmazonS3Client();
        }

        public async Task<List<string>> GetPhotosAsync(DateTime fromDate, DateTime toDate)
        {
            var photos = new List<string>();

            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "decompressed_photos/"
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                foreach (var s3Object in response.S3Objects)
                {
                    // Parse the date from the object key (assuming the object key contains the date in the format yyyyMMddHHmmssfff)
                    if (DateTime.TryParseExact(
                        s3Object.Key.Substring("decompressed_photos/".Length, 17),
                        "yyyyMMddHHmmssfff",
                        null,
                        DateTimeStyles.None,
                        out DateTime photoDate))
                    {
                        if (photoDate >= fromDate && photoDate <= toDate)
                        {
                            photos.Add($"https://{_bucketName}.s3.{_region}.amazonaws.com/{s3Object.Key}");
                        }
                    }
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while ((bool)response.IsTruncated);

            return photos;
        }

        public async Task UploadImageAsync(Mat image, string s3Key)
        {
            using (var stream = new MemoryStream())
            {
                // Save the image to the memory stream as a PNG
                image.WriteToStream(stream, ".png");

                // Configure the uploaded stream position
                stream.Position = 0;

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = s3Key,
                    InputStream = stream,
                    ContentType = "image/png",
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Failed to upload file to S3");
                }
            }
        }

    }
}
