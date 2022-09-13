using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.DataType;

namespace Trade.DataType
{
    public class CacheTradeDBEx
    {
        public PackageCacheDBEx PackageData;

        public SlmOrder[] Orders;

        public SlmOrderJour[] Orderjours;

        public SlmMatch[] Trades;

        public SlmPosition[] PrePositions;

        public SlmPosition[] Positions;

        public Deposit[] Deposits;
    }
}
