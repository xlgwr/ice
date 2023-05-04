using Ice;
using IceInternal;
using System.Runtime.InteropServices;
using static IceInternal.ByteBuffer;

namespace ConsoleApp1
{
    internal class Program
    {

        static string SCRATCH2_FILES_PATH = @"E:\0git\主备\监控客户端\Log\TmpFileDownload\仲裁服务__391915a4698cef327c3da258815f8952\20230501\";

        static void Main(string[] args)
        {
            try
            {

                string filename = "HeartBeat_3.log";
                string filepath = Path.Combine(SCRATCH2_FILES_PATH, filename);

                if (!File.Exists(filepath))
                {
                    throw new System.Exception($"文件不存在。{filepath}");
                }
                int startTime = Environment.TickCount;
                var dataResult = filepath.GetbytesByFilePath();
                int startTimeGetFile = Environment.TickCount;
                Console.WriteLine($"size:{dataResult.Length},GetBytesOfFilePath:{startTimeGetFile - startTime}ms");

                BZip2.InitIsUseSharpZipLib();

                testIceBZlib(dataResult);


                testIceBZlib(dataResult);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
            Console.WriteLine("Hello, World!");
        }

        static void testIceBZlib(byte[] bytes, int comType = 0, int compEle = 6)
        {
            if (bytes == null || bytes.Length <= 0) return;

            int startTimeGetFile = Environment.TickCount;
            var compBytes = bytes.CompressICE(compEle).Item2;
            int startTimeGetFileByteZip = Environment.TickCount;


            var decompBytes = compBytes.DeCompressICE().Item2;
            int startTimeGetFileByteUnZip = Environment.TickCount;

            Console.WriteLine($"ice zip:size:{bytes.Length},compLevel:{compEle},compress:s:{compBytes.Length},t:{startTimeGetFileByteZip - startTimeGetFile}ms,Decompress:s:{decompBytes.Length},t:{startTimeGetFileByteUnZip - startTimeGetFileByteZip}ms");
        }


    }

    public static class extss
    {

        public static byte[] GetbytesByFilePath(this string logPath)
        {
            byte[] array = null;
            if (!File.Exists(logPath))
            {
                return array;
            }

            using (FileStream fileStream = new FileStream(logPath, FileMode.Open))
            {
                long length = fileStream.Length;
                array = new byte[length];
                fileStream.Read(array, 0, array.Length);
                fileStream.Close();
            }

            return array;
        }
        /// <summary>
        /// ICE 压缩 1:极速 5:标准 7:最大 9:极限
        /// </summary>
        /// <param name="uncompressed"></param>
        /// <param name="compressionLevel">1-9,默认 5，1:极速 5:标准 7:最大 9:极限 </param>
        /// <returns></returns>
        public static Tuple<bool, byte[]> CompressICE(this byte[] uncompressed, int compressionLevel = 6)
        {
            var result = new Tuple<bool, byte[]>(false, null);
            if (!BZip2.supported() || uncompressed == null)
            {
                return result;
            }

            int uncompressedLength = uncompressed.Length;
            int compressedLength = (int)((double)uncompressedLength * 1.01 + 600.0);
            var compressed = new byte[compressedLength];
            int num2 = BZip2.GetCompressBuffer()(compressed, ref compressedLength, uncompressed, uncompressedLength, compressionLevel, 0, 0);
            if (num2 == -8)
            {
                return result;
            }

            if (num2 < 0)
            {
                CompressionException ex = new CompressionException("BZ2_bzBuffToBuffCompress failed");
                ex.reason = BZip2.getBZ2Error(num2);
                throw ex;
            }

            if (compressedLength >= uncompressedLength)
            {
                return result;
            }

            var uncompressedSizeArr = uncompressedLength.putInt();
            var destResutArr = uncompressedSizeArr.Combine(compressed, compressedLength);

            return new Tuple<bool, byte[]>(true, destResutArr);
        }
        /// <summary>
        /// 偏移int计数,用于记录原长
        /// </summary>
        /// <param name="source"></param>
        /// <param name="messageSizeMax"></param>
        /// <returns></returns>
        public static Tuple<bool, byte[]> DeCompressICE(this byte[] source)
        {
            var result = new Tuple<bool, byte[]>(false, null);

            if (!BZip2.supported() || source == null)
            {
                return result;
            }
            int compressedLeng = source.Length - 4;
            byte[] compressed = new byte[compressedLeng];
            System.Buffer.BlockCopy(source, 4, compressed, 0, compressedLeng);

            int uncompressLeng = source.getInt();
            byte[] uncompressedarray = new byte[uncompressLeng];

            int num3 = BZip2.GetDecompressBuffer()(uncompressedarray, ref uncompressLeng, compressed, compressedLeng, 0, 0);
            if (num3 < 0)
            {
                CompressionException ex = new CompressionException("BZ2_bzBuffToBuffDecompress failed");
                ex.reason = BZip2.getBZ2Error(num3);
                throw ex;
            }

            return new Tuple<bool, byte[]>(true, uncompressedarray);
        }
        #region int
        [StructLayout(LayoutKind.Explicit)]
        public struct ValBytesUser
        {
            [FieldOffset(0)]
            public short shortVal;

            [FieldOffset(0)]
            public int intVal;

            [FieldOffset(0)]
            public long longVal;

            [FieldOffset(0)]
            public float floatVal;

            [FieldOffset(0)]
            public double doubleVal;

            [FieldOffset(0)]
            public byte b0;

            [FieldOffset(1)]
            public byte b1;

            [FieldOffset(2)]
            public byte b2;

            [FieldOffset(3)]
            public byte b3;

            [FieldOffset(4)]
            public byte b4;

            [FieldOffset(5)]
            public byte b5;

            [FieldOffset(6)]
            public byte b6;

            [FieldOffset(7)]
            public byte b7;
        }

        /// <summary>
        /// offset 4
        /// </summary>
        /// <param name="_bytes"></param>
        /// <param name="_position"></param>
        /// <param name="_o"></param>
        /// <returns></returns>
        public static unsafe int getInt(this byte[] _bytes, int _position = 0, ByteOrder _o = ByteOrder.LITTLE_ENDIAN)
        {
            _bytes.checkUnderflow(4, _position);
            var _valBytes = default(ValBytesUser);
            if (_o == ByteOrder.LITTLE_ENDIAN)
            {
                fixed (byte* ptr = &_bytes[_position])
                {
                    _valBytes.intVal = *(int*)ptr;
                }
            }
            else
            {
                _valBytes.b3 = _bytes[_position];
                _valBytes.b2 = _bytes[_position + 1];
                _valBytes.b1 = _bytes[_position + 2];
                _valBytes.b0 = _bytes[_position + 3];
            }
            return _valBytes.intVal;
        }
        /// <summary>
        /// offset 2
        /// </summary>
        /// <param name="_bytes"></param>
        /// <param name="pos"></param>
        /// <param name="_o"></param>
        /// <returns></returns>
        public static unsafe short getShort(this byte[] _bytes, int pos = 0, ByteOrder _o = ByteOrder.LITTLE_ENDIAN)
        {
            _bytes.checkUnderflow(2, pos);
            var _valBytes = default(ValBytesUser);
            if (_o == ByteOrder.LITTLE_ENDIAN)
            {
                fixed (byte* ptr = &_bytes[pos])
                {
                    _valBytes.shortVal = *(short*)ptr;
                }
            }
            else
            {
                _valBytes.b1 = _bytes[pos];
                _valBytes.b0 = _bytes[pos + 1];
            }

            return _valBytes.shortVal;
        }

        /// <summary>
        /// Int to Byte for ice
        /// </summary>
        /// <param name="val"></param>
        /// <param name="_o"></param>
        /// <returns></returns>
        public static unsafe byte[] putInt(this int val, ByteOrder _o = ByteOrder.LITTLE_ENDIAN)
        {
            byte[] _bytes = new byte[4];
            int pos = 0;
            var _valBytes = default(ValBytesUser);
            _valBytes.intVal = val;
            if (_o == ByteOrder.LITTLE_ENDIAN)
            {
                fixed (byte* ptr = &_bytes[pos])
                {
                    *(int*)ptr = _valBytes.intVal;
                }
            }
            else
            {
                _bytes[pos + 3] = _valBytes.b0;
                _bytes[pos + 2] = _valBytes.b1;
                _bytes[pos + 1] = _valBytes.b2;
                _bytes[pos] = _valBytes.b3;
            }
            return _bytes;
        }

        private static void checkUnderflow(this byte[] _bytes, int size, int _position = 0)
        {
            if (_position + size > _bytes.Length)
            {
                throw new InvalidOperationException($"buffer underflow {_position} + {size} > {_bytes.Length}");
            }
        }
        /// <summary>
        /// 将数组a,b进行合并
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        private static byte[] Combine(this byte[] a, byte[] b, int blength)
        {
            byte[] c = new byte[a.Length + blength];

            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, blength);

            return c;
        }
        #endregion
    }
}