namespace TestProject1
{
    using CommunityToolkit.HighPerformance;
    using CommunityToolkit.HighPerformance.Buffers;
    using SlmCommunicator.Server;
    using SlmCommunicator;
    using Solomon.Log;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using Ice;
    using System.Numerics;
    using System.Reflection;
    using System.Text;

    [TestClass]
    public class UnitTest1
    {
        static UnitTest1()
        {
            initLog();
        }
        static void initLog()
        {
            int divTime = "05:30".Replace(":", "").TryToInt();
            var logPath = LogTool.GetDivLogPath(divTime);
            int err = LogHelper.Init(logPath, 50);

        }

        [TestMethod]
        public void TestMethod1()
        {
            int toAll = 120 * 10000;
            //测试字典性能
            var streamDic = new ConcurrentDictionary<int, StreamMessageData>();
            var comm = CommunicatorServerHelper.GetInstance();
            var _io = comm.GetInstanceOs();
            var _v = comm.GetEncodingVersion();

            var slmorder = new SlmOrderRow() { AccountNo = "10001", MsgNum = 101, InitDate = 10102 };
            var byteList = slmorder.readByteSeq(_io, _v);

            InputStream bs = new InputStream(_io, _v);
            Stopwatch sp = new Stopwatch();

            //ConcurrentDictionary
            sp.Start();
            for (int i = 0; i < toAll; i++)
            {
                var dd = new StreamMessageData(PackageOrderType.Orders, byteList, bs);
                streamDic.TryAdd(i, dd);
            }
            sp.Stop();

            LogHelper.WriteLog("testTime", $"计数：{toAll},Time:{sp.ElapsedMilliseconds}ms, tps:{toAll * 1000 / sp.ElapsedMilliseconds} t/s,ConcurrentDictionary<int, StreamMessageData>");

            //Queue
            var streamQueue = new Queue<StreamMessageData>();
            sp.Restart();
            for (int i = 0; i < toAll; i++)
            {
                var dd = new StreamMessageData(PackageOrderType.Orders, byteList, bs);
                streamQueue.Enqueue(dd);
            }
            sp.Stop();

            LogHelper.WriteLog("testTime", $"计数：{toAll},Time:{sp.ElapsedMilliseconds}ms, tps:{toAll * 1000 / sp.ElapsedMilliseconds} t/s,Queue<StreamMessageData>");


            //no add to dic
            sp.Restart();
            for (int i = 0; i < toAll; i++)
            {
                var dd = new StreamMessageData(PackageOrderType.Orders, byteList, bs);
                //streamDic.TryAdd(i, dd);
            }
            sp.Stop();
            LogHelper.WriteLog("testTime", $"计数：{toAll},Time:{sp.ElapsedMilliseconds}ms, tps:{toAll * 1000 / sp.ElapsedMilliseconds} t/s,no add to dic,new StreamMessageData");

            //SpanOwner<StreamMessageData> buffer
            SpanOwner<StreamMessageData> buffer = SpanOwner<StreamMessageData>.Allocate(toAll);
            sp.Restart();
            for (int i = 0; i < toAll; i++)
            {
                var dd = new StreamMessageData(PackageOrderType.Orders, byteList, bs);
                buffer.Span[i] = dd;
            }
            sp.Stop();

            LogHelper.WriteLog("testTime", $"计数：{toAll},Time:{sp.ElapsedMilliseconds}ms, tps:{toAll * 1000 / sp.ElapsedMilliseconds} t/s,SpanOwner<StreamMessageData> buffer");

            //StreamMessageData[] bufferArray
            StreamMessageData[] bufferArray = new StreamMessageData[toAll];
            sp.Restart();
            for (int i = 0; i < toAll; i++)
            {
                var dd = new StreamMessageData(PackageOrderType.Orders, byteList, bs);
                bufferArray[i] = dd;
            }
            sp.Stop();

            LogHelper.WriteLog("testTime", $"计数：{toAll},Time:{sp.ElapsedMilliseconds}ms, tps:{toAll * 1000 / sp.ElapsedMilliseconds} t/s,StreamMessageData[]");

            //Dictionary
            var byteDic = new Dictionary<int, byte[]>();
            sp.Restart();
            for (int i = 0; i < toAll; i++)
            {
                //var dd = new StreamMessageData(PackageOrderType.Orders, byteList, bs);
                byteDic.TryAdd(i, byteList);
            }
            sp.Stop();

            LogHelper.WriteLog("testTime", $"计数：{toAll},Time:{sp.ElapsedMilliseconds}ms, tps:{toAll * 1000 / sp.ElapsedMilliseconds}s,Dictionary<int, byte[]> add same byte[]");

            //Dictionary
            var tmpQueueint = new Queue<int>();
            sp.Restart();
            for (int i = 0; i < toAll; i++)
            {
                //var dd = new StreamMessageData(PackageOrderType.Orders, byteList, bs);
                tmpQueueint.Enqueue(i);
            }
            sp.Stop();

            LogHelper.WriteLog("testTime", $"计数：{toAll},Time:{sp.ElapsedMilliseconds}ms, tps:{toAll * 1000 / sp.ElapsedMilliseconds}s,Queue<int> Enqueue i");


            Assert.IsTrue(streamDic.Count > 0);

            //Console.ReadLine();
        }

        [TestMethod]
        public void BenchmarkVectorTest()
        {
            StringBuilder st = new StringBuilder();
            OutputEnvironment(st, "当前环境");
            Benchmark(st, "Test");
            LogHelper.WriteLog("testTime", st.ToString());
            Assert.IsTrue(true);

        }
        #region Benchmark
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="tw"></param>
        /// <param name="indent"></param>
        void Benchmark(StringBuilder tw, string indent)
        {
            //count：浮点数组的长度。
            //loops：测试所用的外循环次数。
            //countMFlops：每次测试运算量是多少 MFLOPS（百万次浮点运算）。
            //src：测试所用的浮点数组。
            //tickBegin：记录测试开始的时刻。测试计时用的是 Environment.TickCount，它以毫秒为单位.
            //msUsed：测试所用的毫秒数。
            //mFlops：该函数的浮点性能。单位是 MFLOPS/ s（百万次浮点运算 / 秒）。
            //mFlopsBase：基本算法的浮点性能。单位是 MFLOPS/ s（百万次浮点运算 / 秒）。
            //scale：性能提高倍数。既 当前算法的性能，是基本算法的多少倍。

            if (null == tw) return;
            if (null == indent) indent = "";
            //string indentNext = indent + "\t";
            // init.
            int tickBegin, msUsed;
            double mFlops; // MFLOPS/s .
            double scale;
            float rt;
            const int count = 1024 * 4;
            const int loops = 100 * 10000;
            //const int loops = 1;
            const double countMFlops = count * (double)loops / (1000.0 * 1000);
            float[] src = new float[count];
            for (int i = 0; i < count; ++i)
            {
                src[i] = i;
            }
            tw.AppendLine(indent + string.Format("Benchmark: \tcount={0}, loops={1}, countMFlops={2}", count, loops, countMFlops));
            // SumBase.
            tickBegin = Environment.TickCount;
            rt = SumBase(src, count, loops);
            msUsed = Environment.TickCount - tickBegin;
            mFlops = countMFlops * 1000 / msUsed;
            tw.AppendLine(indent + string.Format("SumBase:\t{0}\t# msUsed={1}, MFLOPS/s={2}", rt, msUsed, mFlops));
            double mFlopsBase = mFlops;
            // SumVector4.
            tickBegin = Environment.TickCount;
            rt = SumVector4(src, count, loops);
            msUsed = Environment.TickCount - tickBegin;
            mFlops = countMFlops * 1000 / msUsed;
            scale = mFlops / mFlopsBase;
            tw.AppendLine(indent + string.Format("SumVector4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
            // SumVectorT.
            tickBegin = Environment.TickCount;
            rt = SumVectorT(src, count, loops);
            msUsed = Environment.TickCount - tickBegin;
            mFlops = countMFlops * 1000 / msUsed;
            scale = mFlops / mFlopsBase;
            tw.AppendLine(indent + string.Format("SumVectorT:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
        }

        float SumBase(float[] src, int count, int loops)
        {
            float rt = 0;
            for (int j = 0; j < loops; j++)
            {
                for (int i = 0; i < count; i++)
                {
                    rt += src[i];
                }
            }
            return rt;
        }
        float SumVector4(float[] src, int count, int loops)
        {
            float rt = 0; // Result.
            const int VectorWidth = 4;
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector4 vrt = Vector4.Zero; // Vector result.
            int p; // Index for src data.
            int i;
            // Load.
            Vector4[] vsrc = new Vector4[cntBlock]; // Vector src.
            p = 0;
            for (i = 0; i < vsrc.Length; ++i)
            {
                vsrc[i] = new Vector4(src[p], src[p + 1], src[p + 2], src[p + 3]);
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j)
            {
                // Vector processs.
                for (i = 0; i < cntBlock; ++i)
                {
                    // Equivalent to scalar model: rt += src[i];
                    vrt += vsrc[i]; // Add.
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i)
                {
                    rt += src[p + i];
                }
            }
            // Reduce.
            rt += vrt.X + vrt.Y + vrt.Z + vrt.W;
            return rt;
        }
        float SumVectorT(float[] src, int count, int loops)
        {
            float rt = 0; // Result.
            int VectorWidth = Vector<float>.Count; // Block width.
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector<float> vrt = Vector<float>.Zero; // Vector result.
            int p; // Index for src data.
            int i;
            // Load.
            Vector<float>[] vsrc = new Vector<float>[cntBlock]; // Vector src.
            p = 0;
            for (i = 0; i < vsrc.Length; ++i)
            {
                vsrc[i] = new Vector<float>(src, p);
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j)
            {
                // Vector processs.
                for (i = 0; i < cntBlock; ++i)
                {
                    vrt += vsrc[i]; // Add.
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i)
                {
                    rt += src[p + i];
                }
            }
            // Reduce.
            for (i = 0; i < VectorWidth; ++i)
            {
                rt += vrt[i];
            }
            return rt;
        }
        #region tostring
        /// <summary>
        /// Is release make.
        /// </summary>
        public static readonly bool IsRelease =
#if DEBUG
    false
#else
    true
#endif
            ;
        void OutputEnvironment(StringBuilder tw, string indent)
        {
            //IsRelease: 是不是以 Release方式编译的程序。
            //EnvironmentVariable(PROCESSOR_IDENTIFIER): CPU型号标识。
            //Environment.ProcessorCount: 逻辑处理器数量。
            //Environment.Is64BitOperatingSystem: 是不是64位操作系统。
            //Environment.Is64BitProcess: 当前进程是不是64位的。
            //Environment.OSVersion: 操作系统的版本。
            //Environment.Version: .NET运行环境的版本。
            //RuntimeEnvironment.GetRuntimeDirectory: .NET基础库的运行路径。
            //RuntimeInformation.FrameworkDescription: .NET平台的版本。
            //BitConverter.IsLittleEndian: 是不是小端方式。
            //IntPtr.Size: 指针的大小。32位时为4，64位时为8。
            //Vector.IsHardwareAccelerated: Vector<T> 是否支持硬件加速。
            //Vector<byte>.Count: Vector<byte> 的元素个数、总位数。
            //Vector<float>.Count: Vector<float> 的元素个数、总位数。
            //Vector<double>.Count: Vector<double> 的元素个数、总位数。
            //Vector4.Assembly.CodeBase: Vector4 所属程序集的路径。
            //Vector<T>.Assembly.CodeBase: Vector<T> 所属程序集的路径。

            if (null == tw) return;
            if (null == indent) indent = "";
            //string indentNext = indent + "\t";
            tw.AppendLine(indent + string.Format("IsRelease:\t{0}", IsRelease));
            tw.AppendLine(indent + string.Format("EnvironmentVariable(PROCESSOR_IDENTIFIER):\t{0}", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")));
            tw.AppendLine(indent + string.Format("Environment.ProcessorCount:\t{0}", Environment.ProcessorCount));
            tw.AppendLine(indent + string.Format("Environment.Is64BitOperatingSystem:\t{0}", Environment.Is64BitOperatingSystem));
            tw.AppendLine(indent + string.Format("Environment.Is64BitProcess:\t{0}", Environment.Is64BitProcess));
            tw.AppendLine(indent + string.Format("Environment.OSVersion:\t{0}", Environment.OSVersion));
            tw.AppendLine(indent + string.Format("Environment.Version:\t{0}", Environment.Version));
            // tw.AppendLine(indent + string.Format("RuntimeEnvironment.GetSystemVersion:\t{0}", System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion())); // Same Environment.Version
            tw.AppendLine(indent + string.Format("RuntimeEnvironment.GetRuntimeDirectory:\t{0}", System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()));
#if (NET47 || NET462 || NET461 || NET46 || NET452 || NET451 || NET45 || NET40 || NET35 || NET20) || (NETSTANDARD1_0)
#else
            tw.AppendLine(indent + string.Format("RuntimeInformation.FrameworkDescription:\t{0}", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription));
#endif
            tw.AppendLine(indent + string.Format("BitConverter.IsLittleEndian:\t{0}", BitConverter.IsLittleEndian));
            tw.AppendLine(indent + string.Format("IntPtr.Size:\t{0}", IntPtr.Size));
            tw.AppendLine(indent + string.Format("Vector.IsHardwareAccelerated:\t{0}", Vector.IsHardwareAccelerated));
            tw.AppendLine(indent + string.Format("Vector<byte>.Count:\t{0}\t# {1}bit", Vector<byte>.Count, Vector<byte>.Count * sizeof(byte) * 8));
            tw.AppendLine(indent + string.Format("Vector<float>.Count:\t{0}\t# {1}bit", Vector<float>.Count, Vector<float>.Count * sizeof(float) * 8));
            tw.AppendLine(indent + string.Format("Vector<double>.Count:\t{0}\t# {1}bit", Vector<double>.Count, Vector<double>.Count * sizeof(double) * 8));
            Assembly assembly = typeof(Vector4).GetTypeInfo().Assembly;
            // tw.AppendLine(string.Format("Vector4.Assembly:\t{0}", assembly));
            tw.AppendLine(string.Format("Vector4.Assembly.CodeBase:\t{0}", assembly.CodeBase));
            assembly = typeof(Vector<float>).GetTypeInfo().Assembly;
            tw.AppendLine(string.Format("Vector<T>.Assembly.CodeBase:\t{0}", assembly.CodeBase));
        }
        #endregion
        #endregion
    }
}