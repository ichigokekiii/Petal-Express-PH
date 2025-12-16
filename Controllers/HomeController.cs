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

        // GET: Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/About
        public ActionResult About()
        {
            return View();
        }

        // GET: Home/Shop
        public ActionResult Shop()
        {
            try
            {
                // Get all active products with their images
                var products = db.Products
                    .Where(p => p.is_active == true && p.stock_quantity > 0)
                    .OrderByDescending(p => p.created_at)
                    .ToList();

                // Load images for all products
                var productImages = new Dictionary<int, string>();
                foreach (var product in products)
                {
                    if (product.image_id != null)
                    {
                        var image = db.Images.Find(product.image_id);
                        if (image != null)
                        {
                            productImages[product.product_id] = image.image_path;
                        }
                    }
                }
                ViewBag.ProductImages = productImages;

                // Get categories for filter
                ViewBag.Categories = db.ProductCategories
                    .Where(c => c.is_active == true)
                    .ToList();

                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading products: " + ex.Message;
                return View(new List<tblProductsModel>());
            }
        }

        // GET: Home/Schedule
        public ActionResult Schedule()
        {
            return View();
        }

        // GET: Home/Contact
        public ActionResult Contact()
        {
            return View();
        }

        // GET: Home/Cart
        public ActionResult Cart()
        {
            return View();
        }

        // GET: Home/Checkout
        public ActionResult Checkout()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // POST: Home/PlaceOrder
        [HttpPost]
        public JsonResult PlaceOrder()
        {
            try
            {
                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, message = "Please login to place an order." });
                }

                // Read JSON from request body
                var requestBody = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
                var orderData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(requestBody);

                int userId = (int)Session["UserId"];

                // Calculate total
                decimal subtotal = 0;
                foreach (var item in orderData.cartItems)
                {
                    subtotal += (decimal)item.price * (int)item.quantity;
                }
                decimal deliveryFee = 50;
                decimal totalAmount = subtotal + deliveryFee;

                // Create order with ALL required fields
                var order = new tblOrdersModel
                {
                    user_id = userId,
                    total_amount = totalAmount,
                    order_status = "pending",
                    shipping_status = "pending", // Add this
                    payment_method = (string)orderData.paymentMethod,
                    payment_status = "pending",
                    shipping_address = $"{orderData.address}, {orderData.city}, {orderData.postalCode}",
                    recipient_name = $"{orderData.firstName} {orderData.lastName}", // Add this
                    recipient_phone = (string)orderData.phone, // Add this
                    estimated_delivery = DateTime.Now.AddDays(3), // Add estimated delivery (3 days from now)
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                db.Orders.Add(order);
                db.SaveChanges();

                // Create order items with product name
                foreach (var item in orderData.cartItems)
                {
                    // Get product to fetch the name
                    var product = db.Products.Find((int)item.productId);
                    if (product != null)
                    {
                        var orderItem = new tblOrderItemsModel
                        {
                            order_id = order.order_id,
                            product_id = (int)item.productId,
                            product_name = product.name, // Store product name
                            quantity = (int)item.quantity,
                            price_at_purchase = (decimal)item.price,
                            created_at = DateTime.Now
                        };
                        db.OrderItems.Add(orderItem);

                        // Update product stock
                        product.stock_quantity -= (int)item.quantity;
                        product.updated_at = DateTime.Now;
                    }
                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Your order has been placed successfully!",
                    orderId = order.order_id
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET: Home/Login
        public ActionResult Login()
        {
            return View();
        }

        // GET: Home/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Home/RegisterUser
        [HttpPost]
        public JsonResult RegisterUser(string email, string password, string firstName, string lastName, string phoneNumber)
        {
            try
            {
                // Check if email already exists
                var existingUser = db.Users.FirstOrDefault(u => u.email == email);
                if (existingUser != null)
                {
                    return Json(new { success = false, message = "Email already registered!" });
                }

                // Create new user
                var user = new tblUsersModel
                {
                    email = email,
                    password_hash = password, // In production, use proper hashing!
                    first_name = firstName,
                    last_name = lastName,
                    phone_number = phoneNumber,
                    role = "customer",
                    is_active = true,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();

                return Json(new { success = true, message = "Registration successful!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Home/LoginUser
        [HttpPost]
        public JsonResult LoginUser(string email, string password)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.email == email && u.password_hash == password && u.is_active == true);

                if (user != null)
                {
                    // Create session
                    Session["UserId"] = user.user_id;
                    Session["UserEmail"] = user.email;
                    Session["UserRole"] = user.role;
                    Session["UserName"] = user.first_name + " " + user.last_name;

                    return Json(new
                    {
                        success = true,
                        message = "Login successful!",
                        role = user.role,
                        name = user.first_name + " " + user.last_name
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid email or password!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Home/Logout
        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                Session.Clear();
                return Json(new { success = true, message = "Logged out successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET: Home/CheckSession
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
                        userId = Session["UserId"],
                        email = Session["UserEmail"],
                        role = Session["UserRole"],
                        name = Session["UserName"]
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { isLoggedIn = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { isLoggedIn = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Get Product by ID
        [HttpGet]
        public JsonResult GetProduct(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product == null || product.is_active == false)
                {
                    return Json(new { success = false, message = "Product not found" }, JsonRequestBehavior.AllowGet);
                }

                // Get image
                string imagePath = null;
                if (product.image_id != null)
                {
                    var image = db.Images.Find(product.image_id);
                    imagePath = image?.image_path;
                }

                // Get category
                string categoryName = null;
                if (product.category_id != null)
                {
                    var category = db.ProductCategories.Find(product.category_id);
                    categoryName = category?.category_name;
                }

                return Json(new
                {
                    success = true,
                    product = new
                    {
                        product_id = product.product_id,
                        name = product.name,
                        description = product.description,
                        price = product.price,
                        stock_quantity = product.stock_quantity,
                        image_path = imagePath,
                        category_name = categoryName
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
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