using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    public class Response
    {
        public int transaction_id { get; set; }
        public int gateway_id { get; set; }
        public int currency_id { get; set; }
        public string token { get; set; }
        public string transaction_date { get; set; }
        public int gateway_status { get; set; }
        public string gateway_transid { get; set; }
        public string gateway_msg { get; set; }
        public double amount { get; set; }
        public double fee { get; set; }
        public string transaction_type { get; set; }
        public string transaction_method { get; set; }
        public string transaction_detail { get; set; }
        public string transaction_datetime { get; set; }
        public object void_datetime { get; set; }
        public string transaction_type_name { get; set; }
        public string currency_symbol { get; set; }
        public string currency_iso4217 { get; set; }
        public int customer_id { get; set; }
    }
    public class Transaction
    {
        public int code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public List<Response> response { get; set; }
    }
}
