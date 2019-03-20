using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestChargeOver.Objects
{
    class Payment : Base
    {
        public int customer_id  { get; set; }
        //public string _comment  { get; set; }
        public decimal amount   { get; set; }
      
    }
}
