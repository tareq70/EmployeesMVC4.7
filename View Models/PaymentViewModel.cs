using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeesMVC4._7.View_Models
{
    public class ChargeViewModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public TapResponse Response { get; set; }   
        public CustomerInfo Customer { get; set; }
    }

    public class CustomerInfo
    {
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        public CustomerPhone Phone { get; set; }
    }
    public class CustomerPhone
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
    }
    public class TapResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

}