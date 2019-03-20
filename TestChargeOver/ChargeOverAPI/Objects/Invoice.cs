using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    class Invoice : Base
    {
        public int customer_id { set; get; }
        public List<LineItem> line_items { set; get; }
    }
}
