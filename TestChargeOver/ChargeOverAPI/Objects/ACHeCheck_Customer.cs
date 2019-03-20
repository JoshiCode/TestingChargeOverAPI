using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    class ACHeCheck_Customer : Base
    {
        public int customer_id { get; set; }
        public string type { get; set; }
        public string number { get; set; }
        public string routing { get; set; }
    }
}
