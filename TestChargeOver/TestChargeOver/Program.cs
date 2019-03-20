using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestChargeOver.Objects;
using ChargeOverAPI;

namespace TestChargeOver
{
    class Program
    {
        static void Main(string[] args)
        {
            //ChargeOverConnection.MakePaymentAmountAPI(2, 50.98m);
            //ChargeOverConnection.StoreCustomerACHeCheckAPI(2, "chec", "45678123789", "062346234");

            //ChargeOverConnection.StoreCustomerCreditCardAPI(1, "4111111111111111", "2020", "11", "Not Real Person", "72 E Blue Grass Road", "Willington", "CT", "06279", "United States");
            //ChargeOverConnection.StoreCustomerACHeCheckAPI(3, "chec", "45678123789", "062346234");
   
            var subs = ChargeOverConnection.GetsubscriptionAPI(1600);
            ChargeOverConnection.submitUsageByLineItemAPI("3821", "12", DateTime.Today.AddDays(-2), DateTime.Today.AddDays(-1));

            Console.ReadKey();
        }


    }
}
