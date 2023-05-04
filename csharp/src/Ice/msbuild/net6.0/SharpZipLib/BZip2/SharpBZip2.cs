using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SharpZipLib.BZip2
{
    /// <summary>
    /// An example class to demonstrate compression and decompression of BZip2 streams.
    /// </summary>
    public static class SharpBZip2
    {
        #region ice
        public static int CompressBuffer(byte[] dest,
                                ref int destLen,
                                byte[] source,
                                int sourceLen,
                                int blockSize100k,
                                int verbosity,
                                int workFactor)
        {
            using (MemoryStream destStream = new MemoryStream(dest))
            using (MemoryStream sourceStream = new MemoryStream(source))
            {
                Compress(sourceStream, destStream, false, blockSize100k);
                destLen = (int)destStream.Position;
            }
            return 0;
        }
        public static int DecompressBuffer(byte[] dest,
                                  ref int destLen,
                                  byte[] source,
                                  int sourceLen,
                                  int small,
                                  int verbosity)
        {
            using (MemoryStream destStream = new MemoryStream(dest))
            using (MemoryStream sourceStream = new MemoryStream(source))
            {
                Decompress(sourceStream, destStream, false);
                destLen = (int)destStream.Position;
            }
            return 0;
        }
        #endregion
        /// <summary>
        /// Decompress the <paramref name="inStream">input</paramref> writing
        /// uncompressed data to the <paramref name="outStream">output stream</paramref>
        /// </summary>
        /// <param name="inStream">The readable stream containing data to decompress.</param>
        /// <param name="outStream">The output stream to receive the decompressed data.</param>
        /// <param name="isStreamOwner">Both streams are closed on completion if true.</param>
        public static void Decompress(Stream inStream, Stream outStream, bool isStreamOwner)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));

            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream));

            try
            {
                using (BZip2InputStream bzipInput = new BZip2InputStream(inStream))
                {
                    bzipInput.IsStreamOwner = isStreamOwner;
                    Core.StreamUtils.Copy(bzipInput, outStream, new byte[4096]);
                }
            }
            finally
            {
                if (isStreamOwner)
                {
                    // inStream is closed by the BZip2InputStream if stream owner
                    outStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Compress the <paramref name="inStream">input stream</paramref> sending
        /// result data to <paramref name="outStream">output stream</paramref>
        /// </summary>
        /// <param name="inStream">The readable stream to compress.</param>
        /// <param name="outStream">The output stream to receive the compressed data.</param>
        /// <param name="isStreamOwner">Both streams are closed on completion if true.</param>
        /// <param name="level">Block size acts as compression level (1 to 9) with 1 giving
        /// the lowest compression and 9 the highest.</param>
        public static void Compress(Stream inStream, Stream outStream, bool isStreamOwner, int level)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));

            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream));
            try
            {
                using (BZip2OutputStream bzipOutput = new BZip2OutputStream(outStream, level))
                {
                    bzipOutput.IsStreamOwner = isStreamOwner;
                    Core.StreamUtils.Copy(inStream, bzipOutput, new byte[4096]);
                }
            }
            finally
            {
                if (isStreamOwner)
                {
                    // outStream is closed by the BZip2OutputStream if stream owner
                    inStream.Dispose();
                }
            }
        }
    }
}
