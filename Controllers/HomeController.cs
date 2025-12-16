using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petal_Express_PH.Models;
using Petal_Express_PH.Models.Context;

namespace Petal_Express_PH.Controllers
{
    public class HomeController : Controller
    {
        private PetalExpressContext db = new PetalExpressContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Shop()
        {
            return View();
        }

        public ActionResult ProductDetail()
        {
            return View();
        }

        public ActionResult Schedule()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        // Database Connection Test
        public ActionResult TestDatabase()
        {
            try
            {
                using (var context = new PetalExpressContext())
                {
                    var userCount = context.Users.Count();
                    var productCount = context.Products.Count();
                    var categoryCount = context.ProductCategories.Count();

                    ViewBag.Success = true;
                    ViewBag.Message = "✅ DATABASE CONNECTION SUCCESSFUL!";
                    ViewBag.UserCount = userCount;
                    ViewBag.ProductCount = productCount;
                    ViewBag.CategoryCount = categoryCount;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.Message = "❌ DATABASE CONNECTION FAILED!";
                ViewBag.Error = ex.Message;
                ViewBag.InnerError = ex.InnerException?.Message ?? "No inner exception";
            }

            return View();
        }

        // SIMPLE REGISTER - Saves directly to database (plain text password for development)
        [HttpPost]
        public JsonResult RegisterUser(string email, string password, string name, string phone)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { success = false, message = "Email and password are required." });
                }

                // Check if email already exists
                var existingUser = db.Users.FirstOrDefault(u => u.email == email);
                if (existingUser != null)
                {
                    return Json(new { success = false, message = "This email is already registered." });
                }

                // Split name into first and last name
                string firstName = "";
                string lastName = "";
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var nameParts = name.Split(new[] { ' ' }, 2);
                    firstName = nameParts[0];
                    lastName = nameParts.Length > 1 ? nameParts[1] : "";
                }

                // Create new user (SIMPLE - Plain text password for development)
                var newUser = new tblUsersModel
                {
                    email = email,
                    password_hash = password, // For development - storing plain text
                    first_name = firstName,
                    last_name = lastName,
                    phone_number = phone,
                    role = "customer", // Default role
                    is_active = true,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Registration successful! You can now log in.",
                    user = new
                    {
                        user_id = newUser.user_id,
                        email = newUser.email,
                        name = newUser.first_name + " " + newUser.last_name,
                        role = newUser.role
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Registration failed: " + ex.Message });
            }
        }

        // SIMPLE LOGIN - Checks database (plain text password comparison for development)
        [HttpPost]
        public JsonResult LoginUser(string email, string password)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { success = false, message = "Email and password are required." });
                }

                // Find user (SIMPLE - Plain text password comparison for development)
                var user = db.Users.FirstOrDefault(u =>
                    u.email == email &&
                    u.password_hash == password &&
                    u.is_active == true
                );

                if (user == null)
                {
                    return Json(new { success = false, message = "Invalid email or password." });
                }

                // Create session
                Session["UserId"] = user.user_id;
                Session["UserEmail"] = user.email;
                Session["UserRole"] = user.role;
                Session["UserName"] = user.first_name + " " + user.last_name;

                return Json(new
                {
                    success = true,
                    message = "Login successful!",
                    user = new
                    {
                        user_id = user.user_id,
                        email = user.email,
                        name = user.first_name + " " + user.last_name,
                        role = user.role
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Login failed: " + ex.Message });
            }
        }

        // LOGOUT - Clear session
        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                Session.Clear();
                Session.Abandon();
                return Json(new { success = true, message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Logout failed: " + ex.Message });
            }
        }

        // Check Session
        [HttpGet]
        public JsonResult CheckSession()
        {
            try
            {
                if (Session["UserId"] != null)
                {
                    return Json(new
                    {
                        isLoggedIn = true,
                        user = new
                        {
                            user_id = Session["UserId"],
                            email = Session["UserEmail"],
                            role = Session["UserRole"],
                            name = Session["UserName"]
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { isLoggedIn = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isLoggedIn = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
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