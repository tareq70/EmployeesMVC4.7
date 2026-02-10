using EmployeesMVC4._7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeesMVC4._7.Controllers
{
    public class DepartmentController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Department
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetDepartments()
        {
            var departments = db.Departments
                                .Select(d => new {
                                    d.DepartmentId,
                                    d.Name
                                }).ToList();

            return Json(new { data = departments }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddDepartment(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(new { success = false, message = "Name is required" });

            var dept = new Department { Name = name };
            db.Departments.Add(dept);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditDepartment(int id, string name)
        {
            var dept = db.Departments.Find(id);
            if (dept == null)
                return Json(new { success = false, message = "Department not found" });

            dept.Name = name;
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteDepartment(int id)
        {
            var dept = db.Departments.Find(id);
            if (dept == null)
                return Json(new { success = false, message = "Department not found" });

            db.Departments.Remove(dept);
            db.SaveChanges();

            return Json(new { success = true });
        }
    }
}