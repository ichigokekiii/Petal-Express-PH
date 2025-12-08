using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Petal_Express_PH.Models.Context;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Controllers
{
    public class HomeController : Controller
    {
        private readonly PetalExpressContext _db = new PetalExpressContext();

        private class ImageRow
        {
            public int image_id { get; set; }
            public string image_path { get; set; }
        }

        private class AddToCartRequest { public int productId { get; set; } public int qty { get; set; } }
        private class UpdateQtyRequest { public int productId { get; set; } public int qty { get; set; } }
        private class RemoveRequest { public int productId { get; set; } }

        private bool IsLoggedIn()
        {
            // Treat user as logged in if a server-side session email exists or admin is active
            var isAdmin = Session["is_admin"] as bool?;
            var userEmail = Session["user_email"] as string;
            return (isAdmin == true) || !string.IsNullOrEmpty(userEmail);
        }

        private ActionResult RequireLogin()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            return null;
        }

        public ActionResult Index()
        {
            var isAdmin = Session["is_admin"] as bool?;
            if (isAdmin == true) return RedirectToAction("Index", "Admin");
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Shop()
        {
            var gate = RequireLogin();
            if (gate != null) return gate;
            return View();
        }

        public ActionResult ProductDetail(int id)
        {
            var gate = RequireLogin();
            if (gate != null) return gate;
            ViewBag.ProductId = id;
            return View();
        }

        public ActionResult Schedule()
        {
            var gate = RequireLogin();
            if (gate != null) return gate;
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Cart()
        {
            var gate = RequireLogin();
            if (gate != null) return gate;
            return View();
        }

        public ActionResult Profile()
        {
            var gate = RequireLogin();
            if (gate != null) return gate;
            return View();
        }

        public ActionResult Payment()
        {
            var gate = RequireLogin();
            if (gate != null) return gate;
            return View();
        }

        public ActionResult Login() 
        { 
            var isAdmin = Session["is_admin"] as bool?;
            if (isAdmin == true) return RedirectToAction("Index", "Admin");
            if (IsLoggedIn()) return RedirectToAction("Index", "Home");
            return View(); 
        }

        public ActionResult Register() 
        { 
            var isAdmin = Session["is_admin"] as bool?;
            if (isAdmin == true) return RedirectToAction("Index", "Admin");
            if (IsLoggedIn()) return RedirectToAction("Index", "Home");
            return View(); 
        }

        // DTOs
        public class RegisterRequest
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
        }
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class UpdateProfileRequest
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
            public string Password { get; set; } // optional
        }

        // Basic CRUD APIs (JSON) for demo
        [HttpGet]
        public ActionResult GetUsers()
        {
            var users = _db.Users.ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        private static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(32);
                return $"PBKDF2$10000${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
            }
        }

        private static bool VerifyPassword(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored) || string.IsNullOrEmpty(password)) return false;
            try
            {
                var parts = stored.Split('$');
                if (parts.Length != 4) return false;
                int iterations = int.Parse(parts[1]);
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] storedHash = Convert.FromBase64String(parts[3]);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
                byte[] testHash = pbkdf2.GetBytes(storedHash.Length);
                uint diff = (uint)storedHash.Length ^ (uint)testHash.Length;
                for (int i = 0; i < storedHash.Length && i < testHash.Length; i++)
                    diff |= (uint)(storedHash[i] ^ testHash[i]);
                return diff == 0;
            }
            catch { return false; }
        }

        [HttpPost]
        public ActionResult CreateUser(RegisterRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.FirstName) || string.IsNullOrWhiteSpace(req.LastName) || string.IsNullOrWhiteSpace(req.Password))
            {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
                return Json(new { error = "First name, last name, password, and email are required." });
            }

            var exists = _db.Users.Any(u => u.Email == req.Email);
            if (exists)
            {
                Response.StatusCode = 400;
                return Json(new { error = "Email is already registered." });
            }

            var now = DateTime.UtcNow;

            using (var tx = _db.Database.BeginTransaction())
            {
                try
                {
                    var nextId = _db.Database.SqlQuery<int>("SELECT IFNULL(MAX(user_id),0)+1 FROM tbl_users").FirstOrDefault();

                    _db.Database.ExecuteSqlCommand(
                        "INSERT INTO tbl_users (user_id, first_name, last_name, email, phone_number, created_at) VALUES (@p0, @p1, @p2, @p3, @p4, @p5)",
                        nextId, req.FirstName, req.LastName, req.Email, req.PhoneNumber, now
                    );

                    var pwdHash = HashPassword(req.Password);
                    _db.Database.ExecuteSqlCommand(
                        "INSERT INTO tbl_user_seems (user_id, first_name, last_name, password_hash, token, is_active, created_at, updated_at) VALUES (@p0, @p1, @p2, @p3, NULL, 0, @p4, @p4)",
                        nextId, req.FirstName, req.LastName, pwdHash, now
                    );

                    // Create bound cart
                    _db.Database.ExecuteSqlCommand(
                        "INSERT INTO tbl_cart (user_id, created_at, updated_at) VALUES (@p0, @p1, @p1)",
                        nextId, now
                    );

                    tx.Commit();
                    return Json(new { user_id = nextId, email = req.Email });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    Response.StatusCode = 500;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Registration failed.", detail = ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult DoLogin(LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email))
            {
                Response.StatusCode = 400;
                return Json(new { error = "Email is required." });
            }

            // Admin credentials protection
            if (!string.IsNullOrWhiteSpace(req.Password) && req.Email == "admin@test.com" && req.Password == "admin1234")
            {
                Session["is_admin"] = true;
                return Json(new { redirect = Url.Action("Index", "Admin") });
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == req.Email);
            if (user == null)
            {
                Response.StatusCode = 401;
                return Json(new { error = "Invalid credentials" });
            }

            // find security record and verify password
            var sec = _db.UserSeems.FirstOrDefault(s => s.UserId == user.UserId);
            if (sec == null || string.IsNullOrEmpty(req.Password) || !VerifyPassword(req.Password, sec.PasswordHash))
            {
                Response.StatusCode = 401;
                return Json(new { error = "Invalid credentials" });
            }

            // mark user session and set active in tbl_user_seems
            Session["user_email"] = user.Email;
            sec.IsActive = true;
            if (string.IsNullOrEmpty(sec.Token)) sec.Token = Guid.NewGuid().ToString("N");
            sec.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Json(new { redirect = Url.Action("Index", "Home") });
        }

        [HttpPost]
        public ActionResult Logout()
        {
            var email = Session["user_email"] as string;
            if (!string.IsNullOrEmpty(email))
            {
                var user = _db.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    var sec = _db.UserSeems.FirstOrDefault(s => s.UserId == user.UserId);
                    if (sec != null)
                    {
                        sec.IsActive = false;
                        sec.UpdatedAt = DateTime.UtcNow;
                        _db.SaveChanges();
                    }
                }
            }
            Session["is_admin"] = null;
            Session["user_email"] = null;
            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public ActionResult GetProducts()
        {
            var products = _db.Products.ToList();
            var images = _db.Database.SqlQuery<ImageRow>("SELECT image_id, image_path FROM tbl_images").ToList();
            var imgDict = images.ToDictionary(i => i.image_id, i => i.image_path);

            var result = products.Select(p => new
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                ImageId = p.ImageId,
                ImagePath = (p.ImageId.HasValue && imgDict.ContainsKey(p.ImageId.Value)) ? imgDict[p.ImageId.Value] : null,
                Price = p.Price ?? 0m,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProduct(int id)
        {
            var p = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (p == null) return HttpNotFound();
            string imagePath = null;
            if (p.ImageId.HasValue)
            {
                imagePath = _db.Database.SqlQuery<string>("SELECT image_path FROM tbl_images WHERE image_id = @p0", p.ImageId.Value).FirstOrDefault();
            }
            var dto = new
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                ImagePath = imagePath,
                Price = p.Price ?? 0m
            };
            return Json(dto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateProduct(tblProducts product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
                return Json(new { error = "Product name is required." });
            }

            // Validate FKs exist before insert to avoid MySQL FK violations
            // If related records are missing, return a clear error for the client to insert them first.
            if (product.ImageId.HasValue)
            {
                var imageExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_images WHERE image_id = @p0", product.ImageId.Value).FirstOrDefault() > 0;
                if (!imageExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid image_id. Insert the image into tbl_images first and use its id." });
                }
            }
            if (product.CategoryId.HasValue)
            {
                var categoryExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_productcategory WHERE category_id = @p0", product.CategoryId.Value).FirstOrDefault() > 0;
                if (!categoryExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid category_id. Insert the category into tbl_categories first and use its id." });
                }
            }
            if (product.BidId.HasValue)
            {
                var bidExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_bids WHERE bid_id = @p0", product.BidId.Value).FirstOrDefault() > 0;
                if (!bidExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid bid_id. Insert the bid into tbl_bids first and use its id." });
                }
            }
            if (product.ColleagueId.HasValue)
            {
                var colleagueExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_colleagues WHERE colleague_id = @p0", product.ColleagueId.Value).FirstOrDefault() > 0;
                if (!colleagueExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid colleague_id. Insert the colleague into tbl_colleagues first and use its id." });
                }
            }

            product.CreatedAt = DateTime.UtcNow;
            _db.Products.Add(product);
            _db.SaveChanges();
            return Json(product);
        }

        [HttpPost]
        public ActionResult UpdateProduct(int id, tblProducts updated)
        {
            var product = _db.Products.Find(id);
            if (product == null) return HttpNotFound();

            // Validate FKs before update
            if (updated.ImageId.HasValue)
            {
                var imageExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_images WHERE image_id = @p0", updated.ImageId.Value).FirstOrDefault() > 0;
                if (!imageExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid image_id. Insert the image into tbl_images first and use its id." });
                }
            }
            if (updated.CategoryId.HasValue)
            {
                var categoryExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_categories WHERE category_id = @p0", updated.CategoryId.Value).FirstOrDefault() > 0;
                if (!categoryExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid category_id. Insert the category into tbl_categories first and use its id." });
                }
            }
            if (updated.BidId.HasValue)
            {
                var bidExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_bids WHERE bid_id = @p0", updated.BidId.Value).FirstOrDefault() > 0;
                if (!bidExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid bid_id. Insert the bid into tbl_bids first and use its id." });
                }
            }
            if (updated.ColleagueId.HasValue)
            {
                var colleagueExists = _db.Database.SqlQuery<int>("SELECT COUNT(*) FROM tbl_colleagues WHERE colleague_id = @p0", updated.ColleagueId.Value).FirstOrDefault() > 0;
                if (!colleagueExists)
                {
                    Response.StatusCode = 400;
                    Response.TrySkipIisCustomErrors = true;
                    return Json(new { error = "Invalid colleague_id. Insert the colleague into tbl_colleagues first and use its id." });
                }
            }

            product.Name = updated.Name;
            product.Description = updated.Description;
            product.CategoryId = updated.CategoryId;
            product.ImageId = updated.ImageId;
            product.BidId = updated.BidId;
            product.ColleagueId = updated.ColleagueId;
            product.CheckQuantity = updated.CheckQuantity;
            product.IsArchive = updated.IsArchive;
            product.Price = updated.Price; // ensure price is updated
            product.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();
            return Json(product);
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null) return HttpNotFound();
            _db.Products.Remove(product);
            _db.SaveChanges();
            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public ActionResult GetCurrentUser()
        {
            var email = Session["user_email"] as string;
            if (string.IsNullOrEmpty(email)) return new HttpStatusCodeResult(401);
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return HttpNotFound();
            var sec = _db.UserSeems.FirstOrDefault(s => s.UserId == user.UserId);
            return Json(new {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                HasSecurity = sec != null
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateProfile(UpdateProfileRequest req)
        {
            var email = Session["user_email"] as string;
            if (string.IsNullOrEmpty(email)) { Response.StatusCode = 401; return Json(new { error = "Not logged in" }); }
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return HttpNotFound();

            if (!string.IsNullOrWhiteSpace(req.FirstName)) user.FirstName = req.FirstName;
            if (!string.IsNullOrWhiteSpace(req.LastName)) user.LastName = req.LastName;
            if (!string.IsNullOrWhiteSpace(req.PhoneNumber)) user.PhoneNumber = req.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            if (!string.IsNullOrWhiteSpace(req.Password))
            {
                var sec = _db.UserSeems.FirstOrDefault(s => s.UserId == user.UserId);
                if (sec == null)
                {
                    sec = new tblUserSeems
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PasswordHash = HashPassword(req.Password),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Token = null,
                        IsActive = true
                    };
                    _db.UserSeems.Add(sec);
                }
                else
                {
                    sec.FirstName = user.FirstName;
                    sec.LastName = user.LastName;
                    sec.PasswordHash = HashPassword(req.Password);
                    sec.UpdatedAt = DateTime.UtcNow;
                }
                _db.SaveChanges();
            }

            return new HttpStatusCodeResult(200);
        }

        private int? GetCurrentUserId()
        {
            var email = Session["user_email"] as string;
            if (string.IsNullOrEmpty(email)) return null;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            return user?.UserId;
        }
        private int EnsureUserCart(int userId)
        {
            var cartId = _db.Database.SqlQuery<int>("SELECT cart_id FROM tbl_cart WHERE user_id = @p0 LIMIT 1", userId).FirstOrDefault();
            if (cartId > 0) return cartId;
            var now = DateTime.UtcNow;
            _db.Database.ExecuteSqlCommand("INSERT INTO tbl_cart (user_id, created_at, updated_at) VALUES (@p0, @p1, @p1)", userId, now);
            cartId = _db.Database.SqlQuery<int>("SELECT cart_id FROM tbl_cart WHERE user_id = @p0 LIMIT 1", userId).FirstOrDefault();
            return cartId;
        }

        private class CartItemRow
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpGet]
        public ActionResult GetCart()
        {
            var userId = GetCurrentUserId();
            if (userId == null) { Response.StatusCode = 401; return Json(new { error = "Not logged in" }, JsonRequestBehavior.AllowGet); }
            var cartId = EnsureUserCart(userId.Value);
            var items = _db.Database.SqlQuery<CartItemRow>("SELECT product_id AS ProductId, quantity AS Quantity FROM tbl_cart_items WHERE cart_id = @p0", cartId).ToList();
            var prodIds = items.Select(i => i.ProductId).ToList();
            var prods = _db.Products.Where(p => prodIds.Contains(p.ProductId)).ToList();
            var images = _db.Database.SqlQuery<ImageRow>("SELECT image_id, image_path FROM tbl_images").ToList();
            var imgDict = images.ToDictionary(i => i.image_id, i => i.image_path);
            var result = items.Select(i => {
                var p = prods.FirstOrDefault(x => x.ProductId == i.ProductId);
                string imagePath = null;
                if (p != null && p.ImageId.HasValue && imgDict.ContainsKey(p.ImageId.Value)) imagePath = imgDict[p.ImageId.Value];
                var price = p != null && p.Price.HasValue ? p.Price.Value : 0m;
                return new { ProductId = i.ProductId, Name = p != null ? p.Name : "Unknown", Price = price, Quantity = i.Quantity, ImagePath = imagePath, LineTotal = price * i.Quantity };
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int qty)
        {
            if (qty <= 0) qty = 1;
            var userId = GetCurrentUserId();
            if (userId == null) { return new HttpStatusCodeResult(401); }
            var cartId = EnsureUserCart(userId.Value);
            var now = DateTime.UtcNow;
            var existing = _db.Database.SqlQuery<int>("SELECT quantity FROM tbl_cart_items WHERE cart_id = @p0 AND product_id = @p1", cartId, productId).FirstOrDefault();
            if (existing > 0)
            {
                _db.Database.ExecuteSqlCommand("UPDATE tbl_cart_items SET quantity = quantity + @p0, updated_at = @p1 WHERE cart_id = @p2 AND product_id = @p3", qty, now, cartId, productId);
            }
            else
            {
                _db.Database.ExecuteSqlCommand("INSERT INTO tbl_cart_items (cart_id, product_id, quantity, created_at, updated_at) VALUES (@p0, @p1, @p2, @p3, @p3)", cartId, productId, qty, now);
            }
            _db.Database.ExecuteSqlCommand("UPDATE tbl_cart SET updated_at = @p0 WHERE cart_id = @p1", now, cartId);
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult UpdateCartQty(int productId, int qty)
        {
            if (qty <= 0) qty = 1;
            var userId = GetCurrentUserId();
            if (userId == null) { return new HttpStatusCodeResult(401); }
            var cartId = EnsureUserCart(userId.Value);
            var now = DateTime.UtcNow;
            _db.Database.ExecuteSqlCommand("UPDATE tbl_cart_items SET quantity = @p0, updated_at = @p1 WHERE cart_id = @p2 AND product_id = @p3", qty, now, cartId, productId);
            _db.Database.ExecuteSqlCommand("UPDATE tbl_cart SET updated_at = @p0 WHERE cart_id = @p1", now, cartId);
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) { return new HttpStatusCodeResult(401); }
            var cartId = EnsureUserCart(userId.Value);
            var now = DateTime.UtcNow;
            _db.Database.ExecuteSqlCommand("DELETE FROM tbl_cart_items WHERE cart_id = @p0 AND product_id = @p1", cartId, productId);
            _db.Database.ExecuteSqlCommand("UPDATE tbl_cart SET updated_at = @p0 WHERE cart_id = @p1", now, cartId);
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult ClearCart()
        {
            var userId = GetCurrentUserId();
            if (userId == null) { return new HttpStatusCodeResult(401); }
            var cartId = EnsureUserCart(userId.Value);
            var now = DateTime.UtcNow;
            _db.Database.ExecuteSqlCommand("DELETE FROM tbl_cart_items WHERE cart_id = @p0", cartId);
            _db.Database.ExecuteSqlCommand("UPDATE tbl_cart SET updated_at = @p0 WHERE cart_id = @p1", now, cartId);
            return new HttpStatusCodeResult(200);
        }
    }
}