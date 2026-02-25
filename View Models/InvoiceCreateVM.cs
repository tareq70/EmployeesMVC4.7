using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeesMVC4._7.View_Models
{
    public class InvoiceCreateVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string Phone { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "KWD";

        public string Description { get; set; }
    }

}