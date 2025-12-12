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
                var user = db.Users.FirstOrDefault(u => u.email.ToLower() == email.ToLower() && u.isActive);

                if (user == null)
                {
                    return Json(new { success = false, error = "Account not found or inactive." });
                }

                if (user.passwordHash != password)
                {
                    return Json(new { success = false, error = "Invalid password." });
                }

                Session["UserID"] = user.userID;
                Session["Email"] = user.email;
                Session["FirstName"] = user.firstName;
                Session["LastName"] = user.lastName;

                string role = (user.role ?? "customer").Trim();
                Session["Role"] = role;

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
                catch { }

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
        // SHOP & PRODUCTS API - FIXED FOR CUSTOMER FRONTEND
        // ============================================================================

        /// <summary>
        /// GET: Get all active products for customer shop page
        /// This returns data in the exact format your Shop.cshtml expects
        /// </summary>
        [HttpGet]
        public JsonResult GetProducts()
        {
            try
            {
                // Query products with images and categories
                var products = (from p in db.Products
                               join c in db.ProductCategories on p.categoryID equals c.categoryID into cats
                               from cat in cats.DefaultIfEmpty()
                               join i in db.Images on p.imageID equals i.imageID into imgs
                               from img in imgs.DefaultIfEmpty()
                               where p.isActive == true
                               orderby p.createdAt descending
                               select new
                               {
                                   ProductId = p.productID,      // Match your frontend property names
                                   Name = p.name,
                                   Description = p.description,
                                   Price = p.price,
                                   StockQuantity = p.stockQuantity,
                                   ImagePath = img != null ? img.imagePath : "/assets/default.png",
                                   CategoryName = cat != null ? cat.categoryName : "Uncategorized"
                               }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GET: Get single product by ID for product detail page
        /// </summary>
        [HttpGet]
        public JsonResult GetProductById(int id)
        {
            try
            {
                var product = (from p in db.Products
                              join c in db.ProductCategories on p.categoryID equals c.categoryID into cats
                              from cat in cats.DefaultIfEmpty()
                              join i in db.Images on p.imageID equals i.imageID into imgs
                              from img in imgs.DefaultIfEmpty()
                              where p.productID == id && p.isActive
                              select new
                              {
                                  ProductId = p.productID,
                                  Name = p.name,
                                  Description = p.description,
                                  Price = p.price,
                                  StockQuantity = p.stockQuantity,
                                  ImagePath = img != null ? img.imagePath : "/assets/default.png",
                                  CategoryName = cat != null ? cat.categoryName : "Uncategorized"
                              }).FirstOrDefault();

                if (product == null)
                    return Json(new { success = false, error = "Product not found" }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, data = product }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
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
