using EmployeesMVC4._7.View_Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace EmployeesMVC4._7.Controllers
{
    public class PaymentController : Controller
    {
        private readonly string TapSecretKey = "sk_test_fQdnHI9BSF3T5LreojmUEV62";
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCharge(ChargeViewModel paymentModel)
        {
            var data = new
            {
                amount = paymentModel.Amount,
                currency = "EGP",
                description = "Test Charge",
                customer = new
                {
                    first_name = paymentModel.Customer.FirstName,
                    email = paymentModel.Customer.Email,
                    phone = new
                    {
                        country_code = paymentModel.Customer.Phone.CountryCode,
                        number = paymentModel.Customer.Phone.Number
                    }
                },
                source = new
                {
                    id = "src_all"
                },
                redirect = new
                {
                    url = "https://localhost:44350/payment/callback"
                }
            };
            string jsonData = JsonConvert.SerializeObject(data);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + TapSecretKey;
                try
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers[HttpRequestHeader.Authorization] = $"Bearer {TapSecretKey}";

                    // Send the POST request to create a charge
                    string response = client.UploadString("https://api.tap.company/v2/charges/", "POST", jsonData);
                    dynamic charge = JsonConvert.DeserializeObject(response);
                    string paymentUrl = charge.transaction.url;

                    return Redirect(paymentUrl);
                }
                catch (WebException ex)
                {
                    // Handle error
                    using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                    {
                        string errorResponse = reader.ReadToEnd();
                        // Log or display the error
                        ViewBag.Error = "Payment failed: " + errorResponse;
                    }
                }
            }

            return View("Index");
        }
        public async Task<ActionResult> Callback()
        {
            var chargeId = Request.QueryString["charge_id"] ?? Request.QueryString["tap_id"];

            if (string.IsNullOrEmpty(chargeId))
                return View("Callback", new ChargeViewModel { Status = "ERROR" });

            var charge = await GetChargeFromTap(chargeId);

            if (charge == null)
                return View("Callback", new ChargeViewModel { Status = "NOT FOUND" });


            if (charge.Customer == null)
            {
                charge.Customer = new CustomerInfo { Phone = new CustomerPhone() };

            }

            else if (charge.Customer.Phone == null)
                charge.Customer.Phone = new CustomerPhone();

            return View("Callback", charge);
        }
        private async Task<ChargeViewModel> GetChargeFromTap(string chargeId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TapSecretKey);

                var response = await client.GetAsync($"https://api.tap.company/v2/charges/{chargeId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var charge = JsonConvert.DeserializeObject<ChargeViewModel>(json);

                if (charge.Customer == null)
                    charge.Customer = new CustomerInfo { Phone = new CustomerPhone() };
                else if (charge.Customer.Phone == null)
                    charge.Customer.Phone = new CustomerPhone();

                return charge;
            }
        }
        [HttpGet]
        public ActionResult CreateInvoice()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> CreateInvoice(InvoiceCreateVM model)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TapSecretKey);

                var data = new
                {
                    draft = false,
                    due = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeMilliseconds(),
                    expiry = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeMilliseconds(),

                    description = model.Description,
                    mode = "INVOICE",

                    notifications = new
                    {
                        channels = new[] { "SMS", "EMAIL" },
                        dispatch = true
                    },

                    currencies = new[] { model.Currency },

                    charge = new
                    {
                        receipt = new
                        {
                            email = true,
                            sms = true
                        }
                    },

                    customer = new
                    {
                        first_name = model.FirstName,
                        last_name = model.LastName,
                        email = model.Email,
                        phone = new
                        {
                            country_code = model.CountryCode,
                            number = model.Phone
                        }
                    },

                    order = new
                    {
                        amount = model.Amount,
                        currency = model.Currency,

                        items = new[]
                        {
                            new
                            {
                                name = model.Description ?? "Item",
                                amount = model.Amount,
                                currency = model.Currency,
                                quantity = 1
                            }
                        }
                    },
                    redirect = new
                    {
                        url = "https://localhost:44350/payment/InvoiceCallback"
                    },

                    reference = new
                    {
                        invoice = "INV_" + DateTime.Now.Ticks,
                        order = "ORD_" + DateTime.Now.Ticks
                    }
                };
                
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.tap.company/v2/invoices", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return Content("Tap Error: " + responseJson);

                dynamic invoice = JsonConvert.DeserializeObject(responseJson);

                string paymentUrl = invoice.url;

                return Redirect(paymentUrl);
            }
        }

        //[HttpGet]
        //[HttpPost]
        //public async Task<ActionResult> InvoiceCallback()
        //{
        //    var invoiceId = Request.QueryString["invoice_id"]
        //                    ?? Request.QueryString["tap_id"]
        //                    ?? Request.QueryString["id"];

        //    if (string.IsNullOrEmpty(invoiceId))
        //        return Content("Invoice ID missing");

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization =
        //            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TapSecretKey);

        //        var response = await client.GetAsync($"https://api.tap.company/v2/invoices/{invoiceId}");
        //        if (!response.IsSuccessStatusCode)
        //            return Content("Tap Error: " + await response.Content.ReadAsStringAsync());

        //        var json = await response.Content.ReadAsStringAsync();

        //        System.Diagnostics.Debug.WriteLine("INVOICE JSON: " + json);

        //        //var json = await response.Content.ReadAsStringAsync();
        //        //System.Diagnostics.Debug.WriteLine("INVOICE JSON: " + json);
        //        //return Content(json);

        //        dynamic invoiceRaw = JsonConvert.DeserializeObject(json);
        //        var invoice = JsonConvert.DeserializeObject<InvoiceCallbackVM>(json);

        //        string chargeId = invoiceRaw?.charge?.id ?? invoiceRaw?.charges?[0]?.id;

        //        if (!string.IsNullOrEmpty(chargeId))
        //        {
        //            var chargeResponse = await client.GetAsync($"https://api.tap.company/v2/charges/{chargeId}");
        //            if (chargeResponse.IsSuccessStatusCode)
        //            {
        //                var chargeJson = await chargeResponse.Content.ReadAsStringAsync();
        //                System.Diagnostics.Debug.WriteLine("CHARGE JSON: " + chargeJson);

        //                dynamic chargeData = JsonConvert.DeserializeObject(chargeJson);
        //                invoice.Charge = new ChargeInfo
        //                {
        //                    StatusCode = chargeData.status,
        //                    Statement_Descriptor = chargeData.statement_descriptor,
        //                    Receipt = new ReceiptInfo
        //                    {
        //                        Email = chargeData.receipt?.email ?? false,
        //                        Sms = chargeData.receipt?.sms ?? false
        //                    }
        //                };
        //            }
        //        }

        //        if (invoice.Charge == null)
        //            invoice.Charge = new ChargeInfo { StatusCode = "Pending" };

        //        if (invoice.Customer == null)
        //            invoice.Customer = new CustomerInfoInvoice { Phone = new PhoneInfo() };
        //        else if (invoice.Customer.Phone == null)
        //            invoice.Customer.Phone = new PhoneInfo();

        //        return View("InvoiceCallback", invoice);
        //    }
        //}
        public async Task<ActionResult> InvoiceCallback()
        {
            var invoiceId = Request.QueryString["invoice_id"]
                            ?? Request.QueryString["tap_id"]
                            ?? Request.QueryString["id"];

            if (string.IsNullOrEmpty(invoiceId))
                return Content("Invoice ID missing");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TapSecretKey);

                var response = await client.GetAsync($"https://api.tap.company/v2/invoices/{invoiceId}");
                if (!response.IsSuccessStatusCode)
                    return Content("Tap Error: " + await response.Content.ReadAsStringAsync());

                var json = await response.Content.ReadAsStringAsync();
                dynamic invoiceRaw = JsonConvert.DeserializeObject(json);
                var invoice = JsonConvert.DeserializeObject<InvoiceCallbackVM>(json);

                string chargeId = invoiceRaw?.transactions?[0]?.id;
                

                if (!string.IsNullOrEmpty(chargeId))
                {
                    var chargeResponse = await client.GetAsync($"https://api.tap.company/v2/charges/{chargeId}");
                    if (chargeResponse.IsSuccessStatusCode)
                    {
                        var chargeJson = await chargeResponse.Content.ReadAsStringAsync();
                        dynamic chargeData = JsonConvert.DeserializeObject(chargeJson);

                        invoice.Charge = new ChargeInfo
                        {
                            StatusCode = chargeData.status,
                            Statement_Descriptor = chargeData.statement_descriptor,
                            Receipt = new ReceiptInfo
                            {
                                Email = chargeData.receipt?.email ?? false,
                                Sms = chargeData.receipt?.sms ?? false
                            }
                        };
                    }
                }

                if (invoice.Charge == null)
                    invoice.Charge = new ChargeInfo { StatusCode = "Pending" };

                if (invoice.Customer == null)
                    invoice.Customer = new CustomerInfoInvoice { Phone = new PhoneInfo() };
                else if (invoice.Customer.Phone == null)
                    invoice.Customer.Phone = new PhoneInfo();

                if (invoice.Track?.Activity != null)
                {
                    invoice.Track.Activity = invoice.Track.Activity
                        .OrderBy(a => a.Created)
                        .ToList();
                }
                return View("InvoiceCallback", invoice);
            }
        }
        private async Task<ChargeInfo> GetChargeFromInvoice(string invoiceId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TapSecretKey);

                var response = await client.GetAsync($"https://api.tap.company/v2/charges?invoice={invoiceId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ChargeInfo
                    {
                        StatusCode = "Pending",
                        Statement_Descriptor = "Pending",
                        Receipt = new ReceiptInfo
                        {
                            Email = false,
                            Sms = false
                        }
                    };
                }

                var jsonCharge = await response.Content.ReadAsStringAsync();
               
                dynamic chargesList = JsonConvert.DeserializeObject(jsonCharge);

                if (chargesList?.data != null && chargesList.data.Count > 0)
                {
                    var firstCharge = chargesList.data[0];

                    return new ChargeInfo
                    {
                        StatusCode = firstCharge.status,
                        Statement_Descriptor = firstCharge.statement_descriptor,
                        Receipt = new ReceiptInfo
                        {
                            Email = firstCharge.receipt?.email ?? false,
                            Sms = firstCharge.receipt?.sms ?? false
                        }
                    };
                }
                return new ChargeInfo
                {
                    StatusCode = "Pending",
                    Statement_Descriptor = "Pending",
                    Receipt = new ReceiptInfo
                    {
                        Email = false,
                        Sms = false
                    }
                };
            }
        }
    }
}