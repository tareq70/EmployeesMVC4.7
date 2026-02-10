using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeesMVC4._7.Models
{
    public class Department
    {

        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}