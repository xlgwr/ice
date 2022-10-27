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
            sp.Start();
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
    }
}