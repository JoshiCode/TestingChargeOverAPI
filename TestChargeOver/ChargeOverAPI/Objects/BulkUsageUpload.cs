using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    public class BulkUsageByLineItem
    {
        public string line_item_id { set; get; }
        public string usage_value { set; get; }
        public DateTime from { set; get; }
        public DateTime to { set; get; }
        public string external_key { set; get; }
        public string custom_1 { set; get; }
        public string custom_2 { set; get; }
        public string custom_3 { set; get; }
        public string custom_4 { set; get; }
        public string custom_5 { set; get; }
        public string custom_6 { set; get; }
        public string custom_7 { set; get; }
        public string custom_8 { set; get; }
        public string custom_9 { set; get; }
        public string custom_10 { set; get; }
    }

    public class BulkUsageBySubscription
    {
        public int package_id  { set; get;}
        public string usage_value { set; get; }
        public DateTime from { set; get; }
        public DateTime to { set; get; }
    }

    public class DataReturned
    {
        public bool result;
    }
    public class ResponseBulkUsage
    {
        public int code;
        public string status;
        public string message;
        public DataReturned response;
    }
}
