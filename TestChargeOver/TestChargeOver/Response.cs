using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestChargeOver.Objects;

namespace TestChargeOver
{
    public class DataReturned
    {
        public int id;
    }
    class Response
    {
        public int code;
        public string status;
        public string message;
        //public List<Base> list;
        //public List<Response> bulk;
        //public Base obj;
        //public int id;
        //public List<string> response;
        public DataReturned response;

        //public Response(int code, string status, string message)
        //{
        //    this.code = code;
        //    this.status = status;
        //    this.message = message;
        //}

        //public void SetList<Base>(List<TestChargeOver.Objects.Base> list)
        //{
        //    this.list = list;
        //}

        //public void SetObj(Base obj)
        //{
        //    this.obj = obj;
        //}

        //public void SetId(int id)
        //{
        //    this.id = id;
        //}
    }
}
