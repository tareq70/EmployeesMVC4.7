using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeesMVC4._7.Models
{
    public class Employee
    {

        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
    }
}