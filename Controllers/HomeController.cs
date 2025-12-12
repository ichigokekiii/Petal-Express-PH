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

        // ============================================================================
        // VIEWS (PAGES)
        // ============================================================================
        public ActionResult Index() { return View(); }
        public ActionResult Login() { return View(); }
        public ActionResult Register() { return View(); }
        public ActionResult Shop() { return View(); }
        public ActionResult Cart() { return View(); }
        public ActionResult Payment() { return View(); }
        public ActionResult Profile() { return View(); }
        public ActionResult ProductDetail() { return View(); }
        public ActionResult About() { return View(); }
        public ActionResult Contact() { return View(); }
        public ActionResult Schedule() { return View(); }

        // ============================================================================
        // AUTHENTICATION API (Connects to Angular)
        // ============================================================================

        [HttpPost]
        public JsonResult DoLogin(string email, string password)
        {
            try
            {
                // 1. Find user (Case-insensitive email check)
                var user = db.Users.FirstOrDefault(u => u.email.ToLower() == email.ToLower() && u.isActive);

                if (user == null)
                {
                    return Json(new { success = false, error = "Account not found or inactive." });
                }

                // 2. Check Password (Using the correct property 'passwordHash')
                if (user.passwordHash != password)
                {
                    return Json(new { success = false, error = "Invalid password." });
                }

                // 3. Set Session
                Session["UserID"] = user.userID;
                Session["Email"] = user.email;
                Session["FirstName"] = user.firstName;
                Session["LastName"] = user.lastName;

                string role = (user.role ?? "customer").Trim();
                Session["Role"] = role;

                // 4. Log the Session
                try
                {
                    var loginLog = new tblSessions
                    {
                        userID = user.userID,
                        action = "Login",
                        description = "User logged in via API",
                        createdAt = DateTime.Now
                    };
                    db.Sessions.Add(loginLog);
                    db.SaveChanges();
                }
                catch { /* Ignore logging errors */ }

                // 5. Determine Redirect
                string redirectUrl = role.Equals("admin", StringComparison.OrdinalIgnoreCase)
                                     ? "/Admin/Index"
                                     : "/Home/Index";

                return Json(new { success = true, redirect = redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateUser(tblUsers user)
        {
            try
            {
                if (db.Users.Any(u => u.email == user.email))
                    return Json(new { error = "Email is already registered." });

                user.role = "Customer";
                user.isActive = true;
                user.createdAt = DateTime.Now;
                user.updatedAt = DateTime.Now;

                // Fix: Ensure passwordHash is set if your form sends 'password' or 'passwordHash'
                // If your form sends user.passwordHash directly, this line is fine.
                // If your Model binder fails, you might need to map it manually.

                db.Users.Add(user);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult GetCurrentUser()
        {
            if (Session["UserID"] == null) return Json(null, JsonRequestBehavior.AllowGet);

            int uid = (int)Session["UserID"];
            var user = db.Users.Find(uid);

            if (user == null) return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                user.firstName,
                user.lastName,
                user.email,
                user.phoneNumber,
                user.role
            }, JsonRequestBehavior.AllowGet);
        }

        // ============================================================================
        // CART & SHOP APIs
        // ============================================================================

        [HttpGet]
        public JsonResult GetProducts()
        {
            // Join Products with Images
            var list = (from p in db.Products
                        join c in db.ProductCategories on p.categoryID equals c.categoryID
                        join i in db.Images on p.imageID equals i.imageID into images
                        from img in images.DefaultIfEmpty()
                        where p.isActive == true && c.categoryName != "Services"
                        select new
                        {
                            p.productID,
                            p.name,
                            p.price,
                            p.description,
                            ImagePath = img != null ? img.imagePath : "/assets/default.png"
                        }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // ... You can add Cart/Order methods here as needed ...
    }
}