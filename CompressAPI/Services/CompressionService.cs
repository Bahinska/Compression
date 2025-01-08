using System.IO.Compression;
using Zstandard.Net;

namespace Server.Services
{
    public class CompressionService
    {
        public async Task<byte[]> CompressAsync(byte[] data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (ZstandardStream zstd = new ZstandardStream(output, CompressionMode.Compress))
                {
                    await zstd.WriteAsync(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        public async Task<byte[]> DecompressAsync(byte[] data)
        {
            using (MemoryStream input = new MemoryStream(data))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (ZstandardStream zstd = new ZstandardStream(input, CompressionMode.Decompress))
                    {
                        await zstd.CopyToAsync(output);
                    }
                    return output.ToArray();
                }
            }
        }
    }
}
