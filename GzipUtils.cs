using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace WowsMq
{
    internal class GzipUtils
    {
        public static byte[] Decompress(string input)
        {
           // byte[] compressed = Convert.FromBase64String(input);
            return Decompress(Encoding.UTF8.GetBytes(input));
            //return Encoding.UTF8.GetString(decompressed);
        }

        public static byte[] Compress(string input)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(input);
            return Compress(encoded);
        }

        public static byte[] Decompress(byte[] input)
        {
            using (var source = new MemoryStream(input))
            {
                byte[] lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new GZipStream(source,
                    CompressionMode.Decompress))
                {
                    var result = new byte[length];
                    decompressionStream.Read(result, 0, length);
                    return result;
                }
            }
        }

        public static byte[] Compress(byte[] input)
        {
            using (var result = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(input.Length);
                result.Write(lengthBytes, 0, 4);

                using (var compressionStream = new GZipStream(result,
                    CompressionMode.Compress))
                {
                    compressionStream.Write(input, 0, input.Length);
                    compressionStream.Flush();

                }
                return result.ToArray();
            }
        }
    }
}
