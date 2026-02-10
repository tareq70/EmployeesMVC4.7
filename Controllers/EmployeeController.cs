using EmployeesMVC4._7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeesMVC4._7.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentId", "Name");
            return View();
        }
        public JsonResult GetEmployees()
        {
            var employees = db.Employees
                                .Select(e => new
                                {
                                    e.EmployeeId,
                                    e.Name,
                                    e.Email,
                                    e.DepartmentId,
                                    DepartmentName = e.Department.Name
                                }).ToList();
            return Json(new { data = employees }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddEmployee(string name, string email, int departmentId)
        {
            db.Employees.Add(new Employee { Name = name, Email = email, DepartmentId = departmentId });
            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditEmployee(int id, string name,string email ,int departmentId)
        {
            var emp = db.Employees.Find(id);
            if (emp == null) return Json(new { success = false, message = "Employee not found" });

            emp.Name = name;
            emp.Email = email;
            emp.DepartmentId = departmentId;

            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteEmployee(int id)
        {
            var emp = db.Employees.Find(id);
            if (emp == null) return Json(new { success = false, message = "Employee not found" });

            db.Employees.Remove(emp);
            db.SaveChanges();
            return Json(new { success = true });
        }

    }
}