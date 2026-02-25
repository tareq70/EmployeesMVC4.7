using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeesMVC4._7.View_Models
{

    public class InvoiceCallbackVM
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("invoice_number")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        public CustomerInfoInvoice Customer { get; set; }
        public OrderInfo Order { get; set; }
        public ChargeInfo Charge { get; set; }
        public TrackInfo Track { get; set; }
        public List<ErrorInfo> Errors { get; set; } = new List<ErrorInfo>();

    }
    public class ErrorInfo
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class CustomerInfoInvoice
    {
        public string Id { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public PhoneInfo Phone { get; set; }
    }

    public class PhoneInfo
    {
        public string Number { get; set; }
        public string Country_Code { get; set; }
    }

    public class OrderInfo
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public List<OrderItem> Items { get; set; }

    }
    public class OrderItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }


    public class ChargeInfo
    {
        [JsonProperty("status")]
        public string StatusCode { get; set; }

        [JsonProperty("statement_descriptor")]
        public string Statement_Descriptor { get; set; }

        public ReceiptInfo Receipt { get; set; }
    }

    public class ReceiptInfo
    {
        public bool Email { get; set; }
        public bool Sms { get; set; }
    }

    public class TrackInfo
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public List<ActivityInfo> Activity { get; set; }

    }
    public class ActivityInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public long Created { get; set; }
    }
}