using SlmCommunicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageClient.Exts
{
    public static class RequestPackageOrderExt
    {
        public static RequestPackageOrder Copy(this RequestPackageOrder request, bool isAdd, Dictionary<int, int> addNum)
        {
            var curr = request.Clone() as RequestPackageOrder;
            curr.SlmOrderBody = request.SlmOrderBody?.Clone() as SlmOrderRow;
            curr.PushMatchStateBody = request.PushMatchStateBody?.Clone() as PushMatchStateRow;
            curr.PushOrderStateBody = request.PushOrderStateBody?.Clone() as PushOrderStateRow;
            curr.SendOrderStatusBody = request.SendOrderStatusBody?.Clone() as SendOrderStatusRow;
            curr.SlmOrderJourBody = request.SlmOrderJourBody?.Clone() as SlmOrderJourRow;
            List<SlmMatchRow> slmMatchRows = new List<SlmMatchRow>();
            if (request.SlmMatchBody.HasItem())
            {
                foreach (var item in request.SlmMatchBody)
                {
                    var toadd = item.Clone() as SlmMatchRow;
                    if (isAdd)
                    {
                        toadd.IsKeyAdd(addNum);
                    }
                    slmMatchRows.Add(toadd);
                }
            }
            curr.SlmMatchBody = slmMatchRows.HasItem() ? slmMatchRows.ToArray() : null;

            if (isAdd)
            {
                curr.SlmOrderBody.IsKeyAdd(addNum);
                curr.SlmOrderJourBody.IsKeyAdd(addNum);
                curr.SendOrderStatusBody.IsKeyAdd(addNum);
                curr.PushMatchStateBody.IsKeyAdd(addNum);
                curr.PushOrderStateBody.IsKeyAdd(addNum);
                curr.MsgId = Guid.NewGuid().ToString();
            }

            return curr;
        }
        public static void IsKeyAdd(this SlmMatchRow curr, Dictionary<int, int> addNum)
        {
            if (curr == null)
            {
                return;
            }
            curr.MsgNum += addNum[6];
            curr.OrderNo += addNum[6];
            curr.TradeNo += addNum[6]; 

            addNum[6]++;
        }
        public static void IsKeyAdd(this SlmOrderJourRow curr, Dictionary<int, int> addNum)
        {
            if (curr == null)
            {
                return;
            }
            curr.OrderNo += addNum[2];
            curr.MsgNum += addNum[2];
            addNum[2]++;
        }
        public static void IsKeyAdd(this SendOrderStatusRow curr, Dictionary<int, int> addNum)
        {
            if (curr == null)
            {
                return;
            }
            curr.OrderNo += addNum[3];
            curr.MsgNum += addNum[3];

            addNum[3]++;
        }
        public static void IsKeyAdd(this SlmOrderRow curr, Dictionary<int, int> addNum)
        {
            if (curr == null)
            {
                return;
            }
            curr.OrderNo += addNum[1];
            curr.MsgNum += addNum[1];
            addNum[1]++;
        }
        public static void IsKeyAdd(this PushMatchStateRow curr, Dictionary<int, int> addNum)
        {
            if (curr == null)
            {
                return;
            }
            curr.MsgNum += addNum[4];
            curr.OrderNo += addNum[4];
            curr.TradeNo += addNum[4]; 

            addNum[4]++;
        }
        public static void IsKeyAdd(this PushOrderStateRow curr, Dictionary<int, int> addNum)
        {
            if (curr == null)
            {
                return;
            }
            curr.MsgNum += addNum[5];
            curr.OrderNo += addNum[5];
            addNum[5]++;
        }
    }
}
