using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    class CreditDebitCard_Customer : Base
    {
        public int customer_id { get; set; }
        public string number { get; set; }
        public string expdate_year { get; set; }
        public string expdate_month { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
    }
}
