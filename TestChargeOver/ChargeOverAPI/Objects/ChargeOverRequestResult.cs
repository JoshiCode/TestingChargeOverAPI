using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeOverAPI.Objects
{
    public class ChargeOverRequestResult
    {
        public string propertyName { set; get; }
        public string propertyValue { set; get; }
        public bool success { set; get; }
        public Object objectResult { set; get; }
    }
}
