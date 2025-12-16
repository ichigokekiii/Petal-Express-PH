using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petal_Express_PH.Models;
using Petal_Express_PH.Models.Context;

namespace Petal_Express_PH.Controllers
{
    public class AdminController : Controller
    {
        private PetalExpressContext db = new PetalExpressContext();

        // Check if user is admin before every action
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if user is logged in and is admin
            if (Session["UserId"] == null || Session["UserRole"] == null || Session["UserRole"].ToString() != "admin")
            {
                // Redirect to login if not admin
                filterContext.Result = new RedirectResult("/Home/Login");
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            try
            {
                // Get dashboard statistics
                ViewBag.TotalUsers = db.Users.Count();
                ViewBag.TotalProducts = db.Products.Count();
                ViewBag.TotalOrders = db.Orders.Count();
                ViewBag.TotalCategories = db.ProductCategories.Count();

                // Calculate total revenue (sum of all order amounts)
                ViewBag.TotalRevenue = db.Orders.Sum(o => (decimal?)o.total_amount) ?? 0;

                // Get recent users (last 5)
                ViewBag.RecentUsers = db.Users
                    .OrderByDescending(u => u.created_at)
                    .Take(5)
                    .ToList();

                // Get admin info
                ViewBag.AdminName = Session["UserName"];
                ViewBag.AdminEmail = Session["UserEmail"];

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading dashboard: " + ex.Message;
                return View();
            }
        }

        // API: Get dashboard stats as JSON
        [HttpGet]
        public JsonResult GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    totalUsers = db.Users.Count(),
                    totalProducts = db.Products.Count(),
                    totalOrders = db.Orders.Count(),
                    totalRevenue = db.Orders.Sum(o => (decimal?)o.total_amount) ?? 0,
                    totalCategories = db.ProductCategories.Count(),
                    
                    // Get counts by role
                    adminCount = db.Users.Count(u => u.role == "admin"),
                    customerCount = db.Users.Count(u => u.role == "customer"),
                    
                    // Get active products
                    activeProducts = db.Products.Count(p => p.is_active == true),
                    
                    // Recent activity
                    recentUsers = db.Users
                        .OrderByDescending(u => u.created_at)
                        .Take(5)
                        .Select(u => new
                        {
                            name = u.first_name + " " + u.last_name,
                            email = u.email,
                            role = u.role,
                            created_at = u.created_at
                        })
                        .ToList()
                };

                return Json(stats, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
