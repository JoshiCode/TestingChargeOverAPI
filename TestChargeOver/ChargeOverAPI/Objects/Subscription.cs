using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    public class Subscription : Base
    {
        public int customer_id { get; set; }
        public string paymethod { get; set; }
        public int terms_id { get; set; }
        public int package_id { get; set; }
        public List<Line_Item> line_items { set; get; }
    }

    public class Line_Item
    {
        public int item_id { get; set; }
        public int line_item_id { get; set; }
        public string item_external_key { get; set; }
        public string item_type { get; set; }
        public bool item_is_usage { get; set; }
    }

    public class SubscriptionResponse
    {
        public int code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public Subscription response { get; set; }
    }
}
