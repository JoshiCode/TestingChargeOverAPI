using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    class Payment : Base
    {
        public int customer_id { get; set; }
        public decimal amount { get; set; }
    }

    class PaymentInvoice : Base
    {
        public int customer_id { get; set; }
        public List<paymethods> paymethods { get; set; }
        public List<Applied_to> applied_to { get; set; }

    }

    public class paymethods { }
    public class CreditCard : paymethods
    {
        public int creditcard_id { get; set; }
    }
    public class ACH : paymethods
    {
        public int ach_id { get; set; }
    }


    public class Applied_to
    {
        public int invoice_id { get; set; }
    }

    public class Package
    {
        public int package_id { get; set; }
    }


}
