using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.DataType
{
    public class Deposit
    {
        public string init_date { get; set; }
        /// <summary>
        /// 0,1出入金 2，3当日信用额度，4,5持久信用额度，6,7换汇
        /// 新增：8调账入佣金，9调账出佣金，10调账入盈亏，11调账出盈亏
        /// </summary>
        public string business_flag { get; set; }
        public string business_no { get; set; }
        public string occur_time { get; set; }
        public string operator_no { get; set; }
        public string account_no { get; set; }
        public string currency_code { get; set; }
        public string occur_balance { get; set; }
        public string remark { get; set; }
        public string audit_flag { get; set; }
        public string occur_date { get; set; }
        /// <summary>
        /// T+1标志
        ///0否，1是
        /// </summary>
        public string tplus_flag { get; set; } = "0";
        public string msg_num { get; set; }

        public Deposit Clone()
        {
            return (Deposit)this.MemberwiseClone();
        }

        public void Update(Deposit deposit)
        {
            init_date = deposit.init_date;
            business_flag = deposit.business_flag;
            business_no = deposit.business_no;
            occur_time = deposit.occur_time;
            operator_no = deposit.operator_no;
            account_no = deposit.account_no;
            currency_code = deposit.currency_code;
            occur_balance = deposit.occur_balance;
            remark = deposit.remark;
            audit_flag = deposit.audit_flag;
            occur_date = deposit.occur_date;
            tplus_flag = deposit.tplus_flag;
            msg_num = deposit.msg_num;
        }
    }

}
