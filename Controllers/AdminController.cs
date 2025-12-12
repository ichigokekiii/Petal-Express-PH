using System.Web.Mvc;

namespace Petal_Express_PH.Controllers
{
    public class AdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isAdmin = Session["is_admin"] as bool?;
            if (isAdmin != true)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

<<<<<<< HEAD
        // ============================================================================
        // MAIN VIEWS
        // ============================================================================
=======
>>>>>>> parent of 3c5a50b (Sessions working)
        public ActionResult Index()
        {
            return View();
        }
<<<<<<< HEAD

        public ActionResult Products()
        {
            return View();
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        // NEW: Image Gallery Management Page
        public ActionResult ImageGallery()
        {
            return View();
        }

        // ============================================================================
        // PRODUCTS CRUD - COMPLETE IMPLEMENTATION
        // ============================================================================

        [HttpGet]
        public JsonResult GetProducts()
        {
            try
            {
                var products = (from p in db.Products
                                join c in db.ProductCategories on p.categoryID equals c.categoryID into cats
                                from cat in cats.DefaultIfEmpty()
                                join i in db.Images on p.imageID equals i.imageID into imgs
                                from img in imgs.DefaultIfEmpty()
                                where p.isActive
                                orderby p.createdAt descending
                                select new
                                {
                                    p.productID,
                                    p.name,
                                    p.description,
                                    p.price,
                                    p.stockQuantity,
                                    p.categoryID,
                                    categoryName = cat != null ? cat.categoryName : "Uncategorized",
                                    p.imageID,
                                    imagePath = img != null ? img.imagePath : "/assets/default.png",
                                    p.createdAt,
                                    p.updatedAt
                                }).ToList();

                return Json(new { success = true, data = products }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

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
                                  p.productID,
                                  p.name,
                                  p.description,
                                  p.price,
                                  p.stockQuantity,
                                  p.categoryID,
                                  categoryName = cat != null ? cat.categoryName : "Uncategorized",
                                  p.imageID,
                                  imagePath = img != null ? img.imagePath : "/assets/default.png"
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

        [HttpGet]
        public JsonResult GetCategories()
        {
            try
            {
                var categories = db.ProductCategories
                    .Where(c => c.isActive)
                    .Select(c => new { c.categoryID, c.categoryName })
                    .ToList();

                return Json(new { success = true, data = categories }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveProduct(tblProducts product)
        {
            try
            {
                if (product.productID == 0)
                {
                    product.createdAt = DateTime.Now;
                    product.updatedAt = DateTime.Now;
                    product.isActive = true;
                    if (product.categoryID == 0 || product.categoryID == null) product.categoryID = 1;
                    if (product.imageID == 0 || product.imageID == null) product.imageID = null;

                    db.Products.Add(product);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Product created successfully", productID = product.productID });
                }
                else
                {
                    var existing = db.Products.Find(product.productID);
                    if (existing != null)
                    {
                        existing.name = product.name;
                        existing.description = product.description;
                        existing.price = product.price;
                        existing.stockQuantity = product.stockQuantity;
                        existing.categoryID = product.categoryID;
                        if (product.imageID > 0) existing.imageID = product.imageID;
                        existing.updatedAt = DateTime.Now;
                        db.SaveChanges();

                        return Json(new { success = true, message = "Product updated successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Product not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteProduct(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product != null)
                {
                    product.isActive = false;
                    product.updatedAt = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Product deleted successfully" });
                }
                return Json(new { success = false, message = "Product not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================================
        // IMAGE CMS
        // ============================================================================

        [HttpGet]
        public JsonResult GetAllImages()
        {
            try
            {
                var images = db.Images
                    .Where(i => i.isActive)
                    .OrderByDescending(i => i.createdAt)
                    .Select(i => new
                    {
                        i.imageID,
                        i.imagePath,
                        i.altText,
                        i.createdAt,
                        i.updatedAt,
                        isInUse = db.Products.Any(p => p.imageID == i.imageID && p.isActive)
                    })
                    .ToList();

                return Json(new { success = true, data = images }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UploadProductImage()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    
                    if (!allowedExtensions.Contains(extension))
                        return Json(new { success = false, message = "Invalid file type. Only images are allowed." });

                    if (file.ContentLength > 5 * 1024 * 1024)
                        return Json(new { success = false, message = "File size must be less than 5MB." });

                    var fileName = Guid.NewGuid().ToString("N") + extension;
                    var folderPath = Server.MapPath("~/assets/uploads/");

                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    var fullPath = Path.Combine(folderPath, fileName);
                    file.SaveAs(fullPath);

                    var newImage = new tblImages
                    {
                        imagePath = "/assets/uploads/" + fileName,
                        altText = Path.GetFileNameWithoutExtension(file.FileName),
                        isActive = true,
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now
                    };

                    db.Images.Add(newImage);
                    db.SaveChanges();

                    return Json(new 
                    { 
                        success = true, 
                        image_id = newImage.imageID, 
                        image_path = newImage.imagePath,
                        message = "Image uploaded successfully"
                    });
                }
                return Json(new { success = false, message = "No file uploaded" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteImage(int id)
        {
            try
            {
                var image = db.Images.Find(id);
                if (image != null)
                {
                    var isInUse = db.Products.Any(p => p.imageID == id && p.isActive);
                    if (isInUse)
                        return Json(new { success = false, message = "Cannot delete image. It is currently being used by products." });

                    image.isActive = false;
                    image.updatedAt = DateTime.Now;
                    db.SaveChanges();

                    try
                    {
                        var filePath = Server.MapPath(image.imagePath);
                        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    }
                    catch { }

                    return Json(new { success = true, message = "Image deleted successfully" });
                }
                return Json(new { success = false, message = "Image not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateImageAltText(int imageID, string altText)
        {
            try
            {
                var image = db.Images.Find(imageID);
                if (image != null)
                {
                    image.altText = altText;
                    image.updatedAt = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Alt text updated successfully" });
                }
                return Json(new { success = false, message = "Image not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================================
        // DASHBOARD CHARTS & REPORTS
        // ============================================================================

        [HttpGet]
        public JsonResult GetDashboardData()
        {
            try
            {
                var overviewData = new
                {
                    Products = db.Products.Count(p => p.isActive),
                    Orders = db.Orders.Count(),
                    Users = db.Users.Count(u => u.role.ToLower() != "admin")
                };

                var orderStatusData = db.Orders
                    .GroupBy(o => o.orderStatus)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToList();

                var categoryData = (from p in db.Products
                                    join c in db.ProductCategories on p.categoryID equals c.categoryID
                                    where p.isActive
                                    group p by c.categoryName into g
                                    select new { Category = g.Key, Count = g.Count() })
                                    .ToList();

                var sevenDaysAgo = DateTime.Now.AddDays(-7);
                var revenueData = db.Orders
                    .Where(o => o.createdAt >= sevenDaysAgo && o.paymentStatus == "Paid")
                    .GroupBy(o => System.Data.Entity.DbFunctions.TruncateTime(o.createdAt))
                    .Select(g => new 
                    { 
                        Date = g.Key, 
                        Revenue = g.Sum(o => o.totalAmount),
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                var lowStockProducts = db.Products
                    .Where(p => p.isActive && p.stockQuantity < 10)
                    .Select(p => new { p.name, p.stockQuantity })
                    .Take(5)
                    .ToList();

                var recentOrders = (from o in db.Orders
                                   join u in db.Users on o.userID equals u.userID
                                   orderby o.createdAt descending
                                   select new
                                   {
                                       o.orderID,
                                       customerName = u.firstName + " " + u.lastName,
                                       o.totalAmount,
                                       o.orderStatus,
                                       o.createdAt
                                   }).Take(10).ToList();

                return Json(new
                {
                    success = true,
                    overview = overviewData,
                    orderStats = orderStatusData,
                    catStats = categoryData,
                    revenue = revenueData,
                    lowStock = lowStockProducts,
                    recentOrders = recentOrders
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetStats() { return Json(new List<object>(), JsonRequestBehavior.AllowGet); }
        
        [HttpGet]
        public JsonResult RecentOrders() { return Json(new List<object>(), JsonRequestBehavior.AllowGet); }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
=======
>>>>>>> parent of 3c5a50b (Sessions working)
    }
}
