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

    }
}