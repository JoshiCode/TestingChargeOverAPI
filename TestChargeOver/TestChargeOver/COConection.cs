using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestChargeOver.Objects;

namespace TestChargeOver
{
    public enum OfficeSenseActions 
    {
        CreateCustomer,
        AddCustomerCreditCard,
        AddCustomerACHeCheck,
        MakePayment
    }
    class COConection
    {
        /*These are for the auth */
        private const string Username = "pEy2VGcSzmwaqOlen3BArQdUIiX1tkFu";
        protected const string Password = "9NuV7aRqB8lnF2svQpPIiCY3kScfKDjg";
        protected const string Endpoint = "https://temporal.chargeover.com";

        /*These are type of the methods defined by ChargeOver*/
        private const string MethodCreate = "create";
        private const string MethodFind = "find";
        private const string MethodDelete = "delete";
        private const string MethodModify = "modify";
        private const string MethodAction = "action";
        private const string MethodGet = "get";
        private const string MethodBulk = "bulk";
       
        public static void Settings(string Username, string Password, string Endpoint) 
        {
            //this.Username = Username;
            //this.Password = Password;
            //this.Endpoint = Endpoint;
        } 

        #region Another auth 
        public static string Signature(string url, string data)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < 8; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            string nonce = builder.ToString();
            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            string str = COConection.Username + "||" + url.ToLower() + "||" + nonce + "||" + timestamp + "||" + data;

            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(COConection.Password);
            byte[] messageBytes = encoding.GetBytes(str);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

            byte[] hash = hmacsha256.ComputeHash(messageBytes);

            string signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return @"ChargeOver co_public_key=""" + COConection.Username + @""" co_nonce=""" + nonce + @""" co_timestamp=""" + timestamp + @""" co_signature_method=""HMAC-SHA256"" co_version=""1.0"" co_signature=""" + signature + @""" ";
        }
        #endregion

        #region Request to ChargeOver
            
            private static Tuple<int,string> ChargeOverRequest(string method,string co_url_method, Object obj_data = null) 
            {
                try
                {
                    #region
                    string data = ""; 
                    string httpMethod = "GET";
                    switch (method)
                    {
                        case COConection.MethodCreate:
                            httpMethod = "POST";
                            data = JsonConvert.SerializeObject(obj_data);
                            break;
                        case COConection.MethodAction:
                            httpMethod = "POST";
                            break;
                        case COConection.MethodDelete:
                            httpMethod = "DELETE";
                            break;
                        case COConection.MethodFind:
                        case COConection.MethodGet:
                            httpMethod = "GET";
                            break;
                        case COConection.MethodModify:
                            httpMethod = "PUT";
                            data = JsonConvert.SerializeObject(obj_data);
                            break;
                        //case COConection.MethodBulk:
                        //    httpMethod = "POST";
                        //    data = JsonConvert.SerializeObject(bulk);
                        //    break;
                    }

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint + co_url_method);

                    request.KeepAlive = false;
                    request.ProtocolVersion = HttpVersion.Version10;
                    request.Method = httpMethod;

                    byte[] postBytes = Encoding.ASCII.GetBytes(data);
                    request.ContentType = "application/json";
                    request.ContentLength = postBytes.Length;


                    request.Credentials = new NetworkCredential(COConection.Username, COConection.Password);
                    //request.Headers["Authorization"] = Signature(COConection.Endpoint + co_url_method, data);

                    if (postBytes.Length > 0)
                    {
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(postBytes, 0, postBytes.Length);
                        requestStream.Close();
                    }



                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string httpResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

                
                    int httpCode = (int)response.StatusCode;
                    return new Tuple<int, string>(httpCode, httpResponse);
                    #endregion
                }
                catch (Exception exp)
                {
                    return new Tuple<int, string>(-1, "");
                }
            }

            private static void OfficeSenseChargeOverAction(OfficeSenseActions type, Object data_params) 
            {
               
                string method = "";
                string uri = "";

                switch(type)
                {
                    case OfficeSenseActions.CreateCustomer:
                        method = "create";
                        uri = "/api/v3/customer";
                        break;
                    case OfficeSenseActions.AddCustomerCreditCard:
                        method = "create";
                        uri = "/api/v3/creditcard";
                        break;
                    case OfficeSenseActions.AddCustomerACHeCheck:
                        method = "create";
                        uri = "/api/v3/ach";
                        break;
                    case OfficeSenseActions.MakePayment:
                        method = "create";
                        uri = "/api/v3/transaction?action=pay";
                        break;
                }

                Tuple<int,string> http_response = ChargeOverRequest(method, uri, data_params);


                /*To get the IDS the get need properly*/
                int id;
                Response response_chargeover = JsonConvert.DeserializeObject<Response>(http_response.Item2);
                
                switch (type) 
                { 
                    case OfficeSenseActions.CreateCustomer:
                        id = 9;
                        break;
                    case OfficeSenseActions.AddCustomerCreditCard:
                        id = 2;   
                        break;
                    case OfficeSenseActions.AddCustomerACHeCheck:
                        id = 3;
                        break;
                    case OfficeSenseActions.MakePayment:
                        id = 1;
                        break;
                }
               
            }
        
        #endregion

        #region Create Customer

            public static void CreateCustomerAPI(string company, string bill_addr1, string bill_addr2, string bill_city, string bill_state, string bill_postcode)
            {
                
                Object customer = CustomerObj(company, bill_addr1, bill_addr2, bill_city, bill_state, bill_postcode);
                OfficeSenseChargeOverAction(OfficeSenseActions.CreateCustomer, customer);
                
            }
            private static Object CustomerObj(string company, string bill_addr1, string bill_addr2, string bill_city, string bill_state, string bill_postcode) 
            {             
                Customer customer = new Customer();
                customer.company = company;
                customer.bill_addr1 = bill_addr1;
                customer.bill_addr2 = bill_addr2;
                customer.bill_city = bill_city;
                customer.bill_state = bill_state;
                customer.bill_postcode = bill_postcode;
                
                return customer;
            }       
       
        #endregion

        #region Store Customer's Credit Card
            
            public static void StoreCustomerCreditCardAPI(int customer_id, string number, string expdate_year, string expdate_month, string name, string address, string city, string state, string postcode, string country) 
            {
                Object credicard = CreditCardObj(customer_id, number, expdate_year, expdate_month, name, address, city, state, postcode, country);
                OfficeSenseChargeOverAction(OfficeSenseActions.AddCustomerCreditCard, credicard);
                
            }
            private static Object CreditCardObj(int customer_id, string number, string expdate_year, string expdate_month, string name, string address, string city, string state, string postcode, string country) 
            {
                CC_Customer card = new CC_Customer();
                card.customer_id = customer_id;
                card.number = number;
                card.expdate_year = expdate_year;
                card.expdate_month = expdate_month;
                card.name = name;
                card.address = address;
                card.city = city;
                card.state = state;
                card.postcode = postcode;
                card.country = country;
                return card;
            }

        #endregion

        #region Store Customer's ACH/eCheck Account
            
            public static void StoreCustomerACHeCheckAPI(int customer_id, string type, string number, string routing) 
            {
                Object ach_echeck = ACHeCheckObj(customer_id, type, number, routing);
                OfficeSenseChargeOverAction(OfficeSenseActions.AddCustomerACHeCheck, ach_echeck);
                
            }
            private static Object ACHeCheckObj(int customer_id, string type, string number, string routing) 
            {
                ACHeCheck_Customer ach_echeck = new ACHeCheck_Customer();
                ach_echeck.customer_id = customer_id;
                ach_echeck.type = type;
                ach_echeck.number = number;
                ach_echeck.routing = routing;

                return ach_echeck;
            }

        #endregion
        
        #region Make payment
           
            public static void MakePaymentAPI(int customer_id, decimal amount) 
            {
                Object payment = PaymentObj(customer_id, amount);
                OfficeSenseChargeOverAction(OfficeSenseActions.MakePayment, payment);
                
            }
            private static Object PaymentObj(int customer_id, decimal amount) 
            {
                Payment payment = new Payment();
                payment.customer_id = customer_id;
                payment.amount = amount;

                return payment;
            }
        
        #endregion


    
    }
}
