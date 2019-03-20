using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChargeOverAPI.Objects;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ChargeOverAPI
{
    public enum COActions
    {
        CreateCustomer,
        AddCustomerCreditCard,
        AddCustomerACHeCheck,
        MakePayment,
        AddInvoice,
        CreateSubscription,
        InvoinceNowSubs,
        CreateContact,
        DeleteCustomerCreditCard,
        DeleteCustomerACHeCheck,
        PayInvoice,
        GetTotalTransaction,
        SubmitUsageSubscription,
        SubmitUsageLineItem,
        GetSubscription,
        GetInvoice
    }
    public class ChargeOverConnection
    {
        /*These are for the auth */
        //private const string Username = "pEy2VGcSzmwaqOlen3BArQdUIiX1tkFu";
        //protected const string Password = "9NuV7aRqB8lnF2svQpPIiCY3kScfKDjg";
        //protected const string Endpoint = "https://temporal.chargeover.com";

        private const string Username = "78Tvnla2XsSQOtyzCfxqG1uRZ3ejgpLo";
        protected const string Password = "dESOhlc56BfXVko27w9MsvDHui8nWeAI";
        protected const string Endpoint = "http://dev.chargeover.com/api/v3";

        /*These are type of the methods defined by ChargeOver*/
        public const string MethodCreate = "create";
        public const string MethodFind = "find";
        public const string MethodDelete = "delete";
        public const string MethodModify = "modify";
        public const string MethodAction = "action";
        public const string MethodGet = "get";
        public const string MethodBulk = "bulk";

        public void Settings(string Username, string Password, string Endpoint)
        {
            //this.Username = Username;
            //this.Password = Password;
            //this.Endpoint = Endpoint;
        }


        #region Another auth 
        private static string Signature(string url, string data)
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

            string str = ChargeOverConnection.Username + "||" + url.ToLower() + "||" + nonce + "||" + timestamp + "||" + data;

            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(ChargeOverConnection.Password);
            byte[] messageBytes = encoding.GetBytes(str);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

            byte[] hash = hmacsha256.ComputeHash(messageBytes);

            string signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return @"ChargeOver co_public_key=""" + ChargeOverConnection.Username + @""" co_nonce=""" + nonce + @""" co_timestamp="""
                   + timestamp + @""" co_signature_method=""HMAC-SHA256"" co_version=""1.0"" co_signature=""" + signature + @""" ";
        }
        #endregion

        #region Request to ChargeOver
        private static Tuple<int, string> ChargeOverRequest(string method, string co_url_method, Object obj_data = null)
        {
            //try
            //{
            #region
            string data = "";
            string httpMethod = "GET";
            switch (method)
            {
                case ChargeOverConnection.MethodCreate:
                    httpMethod = "POST";
                    data = JsonConvert.SerializeObject(obj_data);
                    break;
                case ChargeOverConnection.MethodAction:
                    httpMethod = "POST";
                    break;
                case ChargeOverConnection.MethodDelete:
                    httpMethod = "DELETE";
                    break;
                case ChargeOverConnection.MethodFind:
                case ChargeOverConnection.MethodGet:
                    httpMethod = "GET";
                    break;
                case ChargeOverConnection.MethodModify:
                    httpMethod = "PUT";
                    data = JsonConvert.SerializeObject(obj_data);
                    break;
                    //case COConection.MethodBulk:
                    //    httpMethod = "POST";
                    //    data = JsonConvert.SerializeObject(bulk);
                    //    break;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint + co_url_method);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Ssl3;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = httpMethod;

            byte[] postBytes = Encoding.ASCII.GetBytes(data);
            request.ContentType = "application/json";
            request.ContentLength = (obj_data != null) ? postBytes.Length : 0;


            //request.Credentials = new NetworkCredential(ChargeOverConnection.Username, ChargeOverConnection.Password);
            request.Headers["Authorization"] = ChargeOverConnection.Signature(ChargeOverConnection.Endpoint + co_url_method, (data.Equals("null") ? "" : data));

            if (postBytes.Length > 0 && obj_data != null)
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
            }


            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string httpResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
                int httpCode = (int)response.StatusCode;

                return new Tuple<int, string>(httpCode, httpResponse);
            }
            catch (WebException ex)
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)ex.Response)
                {

                    string temp = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                    int httpCode = (int)httpResponse.StatusCode;
                   // ex.SendToAirbrake();
                    return new Tuple<int, string>(httpCode, temp);
                }
            }
            #endregion
            //}
            //catch (Exception exp)
            //{
            //    return new Tuple<int, string>(-1, "");
            //}
        }
        private static ChargeOverRequestResult OfficeSenseChargeOverAction(COActions type, Object data_params = null, string para_url = "") 
            {
                //try
                //{
                string method = "";
                string uri = "";

                switch (type)
                {
                    case COActions.CreateCustomer:
                        method = "create";
                        uri = "/api/v3/customer";
                        break;
                    case COActions.AddCustomerCreditCard:
                        method = "create";
                        uri = "/api/v3/creditcard";
                        break;
                    case COActions.AddCustomerACHeCheck:
                        method = "create";
                        uri = "/api/v3/ach";
                        break;
                    case COActions.MakePayment:
                        method = "create";
                        uri = "/api/v3/transaction?action=pay";
                        break;
                    case COActions.AddInvoice:
                        method = "create";
                        uri = "/api/v3/invoice";
                        break;
                    case COActions.CreateSubscription:
                        method = "create";
                        uri = "/api/v3/package";
                        break;
                    case COActions.InvoinceNowSubs:
                        method = "create";
                        uri = "/api/v3/package/" + para_url + "?action=invoice";
                        break;
                    case COActions.CreateContact:
                        method = "create";
                        uri = "/api/v3/user";
                        break;
                    case COActions.DeleteCustomerCreditCard:
                        method = "delete";
                        uri = "/api/v3/creditcard/" + para_url;
                        break;
                    case COActions.DeleteCustomerACHeCheck:
                        method = "delete";
                        uri = "/api/v3/ach/" + para_url;
                        break;
                    case COActions.PayInvoice:
                        method = "create";
                        uri = "/api/v3/invoice/" + para_url + "?action=pay";
                        break;
                    case COActions.GetTotalTransaction:
                        method = "get";
                        uri = "/api/v3/transaction?where=transaction_id:EQUALS:" + para_url;
                        break;
                case COActions.SubmitUsageSubscription:
                    method = "create";
                    uri = "/api/v3/usage";
                    break;
                case COActions.SubmitUsageLineItem:
                    method = "create";
                    uri = "/api/v3/usage";
                    break;
                case COActions.GetSubscription:
                    method = "get";
                    uri = "/api/v3/package/" + para_url;
                    break;
                }

                Tuple<int, string> http_response = ChargeOverRequest(method, uri, data_params);


                /*To get the IDS the get need properly*/
                string property_name = "";

                if (type != COActions.GetTotalTransaction && type != COActions.GetSubscription )
                {
                    #region
                    Response response_chargeover = JsonConvert.DeserializeObject<Response>(http_response.Item2);
                    switch (type)
                    {
                        case COActions.CreateCustomer:
                            property_name = "customer_id";
                            break;
                        case COActions.AddCustomerCreditCard:
                            property_name = "creditcard_id";
                            break;
                        case COActions.AddCustomerACHeCheck:
                            property_name = "ach_id";
                            break;
                        case COActions.MakePayment:
                            property_name = "transaction_id";
                            break;
                        case COActions.AddInvoice:
                            property_name = "invoice_id";
                            break;
                        case COActions.CreateSubscription:
                            property_name = "subscription_id";
                            break;
                        case COActions.InvoinceNowSubs:
                            property_name = "invoice_id";
                            break;
                        case COActions.CreateContact:
                            property_name = "id";
                            break;
                        case COActions.SubmitUsageSubscription:
                            property_name = "id";
                            break;
                    }
                if ((response_chargeover.code == 201 || response_chargeover.code == 200) && response_chargeover.response != null)
                    return new ChargeOverRequestResult() { propertyName = property_name, propertyValue = response_chargeover.response.id.ToString(), success = true, objectResult = null };
                else if ((response_chargeover.code == 201 || response_chargeover.code == 200) && response_chargeover.response == null)
                    return new ChargeOverRequestResult() { propertyName = property_name, propertyValue = "-1", success = true, objectResult = null };
                else if (response_chargeover.code == 400 || response_chargeover.code == 401 || response_chargeover.code == 429 || response_chargeover.code == 500)
                    return new ChargeOverRequestResult() { propertyName = "", propertyValue = "-1", success = false, objectResult = null }; 
                    #endregion
                }
                else if (type == COActions.GetTotalTransaction)
                {
                    #region
                    Transaction response_chargeover = JsonConvert.DeserializeObject<Transaction>(http_response.Item2);
                    return new ChargeOverRequestResult() { propertyName = "", propertyValue = response_chargeover.response.ElementAt(0).amount.ToString(), success =  true, objectResult = null }; 
                    #endregion
                }
                else if(type == COActions.GetSubscription)
                {
                    
                      SubscriptionResponse subscription = JsonConvert.DeserializeObject<SubscriptionResponse>(http_response.Item2);
                     if(subscription.response !=  null)
                       return new ChargeOverRequestResult() { propertyName = "", propertyValue = "", success = true, objectResult = subscription.response };
                    else
                       return new ChargeOverRequestResult() { propertyName = "", propertyValue = "", success = true, objectResult = new Subscription() };

            }

            return new ChargeOverRequestResult();
        }

        #endregion

        #region Create Customer

        public static ChargeOverRequestResult CreateCustomerAPI(string company, int campaign_id, string campaign_details, string address1, string address2, string city, string state, string post_code, string country, string _cName, string _cMail, string _cPhone)
        {

            Object customer = CustomerObj(company, campaign_id, campaign_details, address1, address2, city, state, post_code, country, _cName, _cMail, _cPhone);
            return OfficeSenseChargeOverAction(COActions.CreateCustomer, customer);         

        }
        private static Object CustomerObj(string company, int campaign_id, string campaign_details, string address1, string address2, string city, string state, string post_code, string country, string _cName, string _cMail, string _cPhone)
        {
            Customer customer = new Customer();
            customer.company = company;
            customer.campaign_id = campaign_id;
            customer.campaign_details = campaign_details;
            customer.bill_addr1 = address1;
            customer.bill_addr2 = address2;
            customer.bill_city = city;
            customer.bill_state = state;
            customer.bill_postcode = post_code;
            customer.bill_country = country;
            customer.superuser_name = _cName;
            customer.superuser_email = _cMail;
            customer.superuser_phone = _cPhone;

            return customer;
        }

        #endregion

        #region Contact
        public static ChargeOverRequestResult CreateContactAPI(int customer_id, string first_name, string last_name, string email, string phone)
        {

            Object customer = ContactObj(customer_id, first_name, last_name, email, phone);
            return  OfficeSenseChargeOverAction(COActions.CreateContact, customer);
        }
        private static Object ContactObj(int customer_id, string first_name, string last_name, string email, string phone)
        {
            Contact contact = new Contact();
            contact.customer_id = customer_id;
            contact.first_name = first_name;
            contact.last_name = last_name;
            contact.email = email;
            contact.phone = phone;
            return contact;
        }
        #endregion

        #region Store Customer's Credit Card

        public static ChargeOverRequestResult StoreCustomerCreditCardAPI(int customer_id, string number, string expdate_year, string expdate_month, string name, string address, string city, string state, string postcode, string country)
        {
            Object credicard = CreditCardObj(customer_id, number, expdate_year, expdate_month, name, address, city, state, postcode, country);
            return OfficeSenseChargeOverAction(COActions.AddCustomerCreditCard, credicard);
            
        }
        private static Object CreditCardObj(int customer_id, string number, string expdate_year, string expdate_month, string name, string address, string city, string state, string postcode, string country)
        {
            CreditDebitCard_Customer card = new CreditDebitCard_Customer();
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

        #region Delete Credit Card
        public static ChargeOverRequestResult DeleteCreditCard(int creditcard_id)
        {
            return OfficeSenseChargeOverAction(COActions.DeleteCustomerCreditCard, para_url: creditcard_id.ToString());
        }
        #endregion

        #region
        public static ChargeOverRequestResult DeleteACHeCheck(int ach_id)
        {
            return OfficeSenseChargeOverAction(COActions.DeleteCustomerACHeCheck, para_url: ach_id.ToString());
        }
        #endregion

        #region Store Customer's ACH/eCheck Account

        public static ChargeOverRequestResult StoreCustomerACHeCheckAPI(int customer_id, string type, string number, string routing)
        {
            Object ach_echeck = ACHeCheckObj(customer_id, type, number, routing);
            return OfficeSenseChargeOverAction(COActions.AddCustomerACHeCheck, ach_echeck);

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

        #region Create an Invoice
        public static ChargeOverRequestResult CreateInvoiceClientAPI(int custome_id, Dictionary<int, float> plan_id)
        {
            Object invoice = InvoiceObj(custome_id, plan_id);
            return OfficeSenseChargeOverAction(COActions.AddInvoice, invoice);
        }

        public static Object InvoiceObj(int custome_id, Dictionary<int, float> line)
        {

            List<LineItem> line_items = new List<LineItem>();
            foreach (var item in line)
            {
                line_items.Add(new LineItem() { item_id = item.Key, line_rate = item.Value });
            }
            Invoice invoice = new Invoice();
            invoice.customer_id = custome_id;
            invoice.line_items = line_items;
            return invoice;
        }
        #endregion

        #region Make payment


        public static ChargeOverRequestResult MakePaymentAmountAPI(int customer_id, decimal amount)
        {
            Object payment = PaymenAmountObj(customer_id, amount);
            return OfficeSenseChargeOverAction(COActions.MakePayment, payment);

        }
        private static Object PaymenAmountObj(int customer_id, decimal amount)
        {
            Payment payment = new Payment();
            payment.customer_id = customer_id;
            payment.amount = amount;

            return payment;
        }

        /**********************************************************************************************/

        public static ChargeOverRequestResult MakePaymentInvoiceAPI(int customer_id, int type, int pay_method, int appliedTo)
        {
            Object payment = PaymentInvoiceObj(customer_id, type, pay_method, appliedTo);
            return OfficeSenseChargeOverAction(COActions.MakePayment, payment);

        }
        private static Object PaymentInvoiceObj(int customer_id, int type, int pay_method, int invoice_id)
        {


            PaymentInvoice pay_invoice = new PaymentInvoice();
            pay_invoice.customer_id = customer_id;

            if (type == 0)
                pay_invoice.paymethods = (new List<paymethods>() { new CreditCard() { creditcard_id = pay_method } });
            else if (type == 1)
                pay_invoice.paymethods = (new List<paymethods>() { new ACH { ach_id = pay_method } });

            pay_invoice.applied_to = (new List<Applied_to>() { new Applied_to() { invoice_id = invoice_id } });

            return pay_invoice;

        }

        #endregion

        #region Create Subscription

        public static ChargeOverRequestResult CreatesubscriptionAPI(int custome_id, Dictionary<int, float> line_items, DateTime holduntil)
        {
            Object invoice = SubscriptionObj(custome_id, line_items, holduntil);
            return OfficeSenseChargeOverAction(COActions.CreateSubscription, invoice);
        }
        public static Object SubscriptionObj(int custome_id, Dictionary<int, float> line, DateTime holduntil)
        {

            List<Line_Item> line_items = new List<Line_Item>();
            foreach (var item in line)
            {
                line_items.Add(new Line_Item() { item_id = item.Key });
            }

            //, descrip = "", line_quantity = 1
            Subscription subscription = new Subscription();
            subscription.customer_id = custome_id;
            subscription.line_items = line_items;
            //subscription.holduntil_datetime = holduntil;
            subscription.terms_id = 3;
            subscription.paymethod = "inv";
            return subscription;
        }


        public static ChargeOverRequestResult CreateInvoinceNowSubs(int package_id)
        {
            return OfficeSenseChargeOverAction(COActions.InvoinceNowSubs, para_url: package_id.ToString());
        }
        #endregion

        #region Get Subscription
        public static Subscription GetsubscriptionAPI(int packageId)
        {
            var obj =  OfficeSenseChargeOverAction(COActions.GetSubscription, para_url: packageId.ToString()).objectResult as Subscription;
            return obj;
        }
        #endregion

        #region Pay Invoice

        public static ChargeOverRequestResult PayInvoice(int invoice_id, string number, string cvv, string expdate_year, string expdate_month, string name, string address, string city, string state, string postcode, string country)
        {
            Object card = PayInvoiceObj(number, cvv, expdate_year, expdate_month, name, address, city, state, postcode, country);
            return OfficeSenseChargeOverAction(COActions.PayInvoice, card, invoice_id.ToString());

        }

        public static Object PayInvoiceObj(string number, string cvv, string expdate_year, string expdate_month, string name, string address, string city, string state, string postcode, string country)
        {
            CCinvoice card = new CCinvoice();
            card.number = number;
            card.cvv = cvv;
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

        #region Total Transaction 
        public static string getTranstAmount(int transaction_id)
        {
            return OfficeSenseChargeOverAction(COActions.GetTotalTransaction, null, transaction_id.ToString()).propertyValue;
        }

        #endregion

        #region Billing
        public static ChargeOverRequestResult submitUsagerBySubscriptionAPI(int package_id, string usage_value, DateTime from, DateTime to)
        {
            Object usage = BillingUsageBySubscriptionObj(package_id, usage_value, from, to);
            return OfficeSenseChargeOverAction(COActions.SubmitUsageSubscription, usage);
        }

        public static Object BillingUsageBySubscriptionObj(int package_id, string usage_value, DateTime from, DateTime to)
        {
            BulkUsageBySubscription billing = new BulkUsageBySubscription();
            billing.package_id = package_id;
            billing.usage_value = usage_value;
            billing.from = from;
            billing.to = to;
            return billing;
        }
        public static ChargeOverRequestResult submitUsageByLineItemAPI(string line_item_id, string usage_value, DateTime from, DateTime to)
        {
            Object usage = BillingUsageByLineItemObj(line_item_id, usage_value, from, to);
            return OfficeSenseChargeOverAction(COActions.SubmitUsageLineItem, usage);
        }
        public static Object BillingUsageByLineItemObj(string line_item_id, string usage_value, DateTime from, DateTime to)
        {
            BulkUsageByLineItem billing = new BulkUsageByLineItem();
            billing.line_item_id = line_item_id;
            billing.usage_value = usage_value;
            billing.from = from;
            billing.to = to;
            return billing;
        }
        #endregion
    }
}
