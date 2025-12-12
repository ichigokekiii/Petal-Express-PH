using System;
using System.Collections.Generic;
using System.IO;
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

        // ============================================================================
        // MAIN VIEW
        // ============================================================================
        // ============================================================================
        // ADMIN VIEWS (Standard MVC)
        // ============================================================================

        // The Dashboard is now a standalone page
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            return View();
        }

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

        // ============================================================================
        // PRODUCTS CRUD
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
                                    ImagePath = img != null ? img.imagePath : "/assets/default.png" // PascalCase for Frontend
                                }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet); }
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
                    // Default Category if missing
                    if (product.categoryID == 0) product.categoryID = 1;
                    db.Products.Add(product);
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
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
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
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        // ============================================================================
        // IMAGE UPLOAD (CMS)
        // ============================================================================

        [HttpPost]
        public JsonResult UploadProductImage()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
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

                    return Json(new { success = true, image_id = newImage.imageID, image_path = newImage.imagePath });
                }
                return Json(new { success = false, message = "No file" });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        // ... Stats and RecentOrders methods ...
        [HttpGet]
        public JsonResult GetStats() { return Json(new List<object>(), JsonRequestBehavior.AllowGet); }
        [HttpGet]
        public JsonResult RecentOrders() { return Json(new List<object>(), JsonRequestBehavior.AllowGet); }

        // ============================================================================
        // DASHBOARD CHARTS & REPORTS API
        // ============================================================================

        [HttpGet]
        public JsonResult GetDashboardData()
        {
            try
            {
                // 1. Chart 1 Data: Overview Counts (The 3 things you asked for)
                var overviewData = new
                {
                    Products = db.Products.Count(p => p.isActive),
                    Orders = db.Orders.Count(),
                    Users = db.Users.Count() // Assuming Role != 'Admin' if you want customers only
                };

                // 2. Chart 2 Data: Orders by Status
                var orderStatusData = db.Orders
                    .GroupBy(o => o.orderStatus)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToList();

                // 3. Chart 3 Data: Products by Category
                // (Join Products to Categories to get names)
                var categoryData = (from p in db.Products
                                    join c in db.ProductCategories on p.categoryID equals c.categoryID
                                    where p.isActive
                                    group p by c.categoryName into g
                                    select new { Category = g.Key, Count = g.Count() })
                                    .ToList();

                return Json(new
                {
                    overview = overviewData,
                    orderStats = orderStatusData,
                    catStats = categoryData
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        
        }
    }
}