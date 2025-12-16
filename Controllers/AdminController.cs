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

        // Check if user is admin before every action
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null || Session["UserRole"] == null || Session["UserRole"].ToString() != "admin")
            {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            try
            {
                ViewBag.TotalUsers = db.Users.Count();
                ViewBag.TotalProducts = db.Products.Count();
                ViewBag.TotalOrders = db.Orders.Count();
                ViewBag.TotalCategories = db.ProductCategories.Count();
                ViewBag.RecentUsers = db.Users.OrderByDescending(u => u.created_at).Take(5).ToList();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading dashboard: " + ex.Message;
                return View();
            }
        }

        // API: Get Chart Data
        [HttpGet]
        public JsonResult GetChartData()
        {
            try
            {
                // PIE CHART DATA: Products by Category
                var categoryData = db.Products
                    .Where(p => p.is_active == true && p.category_id != null)
                    .GroupBy(p => p.category_id)
                    .Select(g => new
                    {
                        category_id = g.Key,
                        count = g.Count()
                    })
                    .ToList();

                var categoryLabels = new List<string>();
                var categoryValues = new List<int>();

                foreach (var item in categoryData)
                {
                    var category = db.ProductCategories.Find(item.category_id);
                    if (category != null)
                    {
                        categoryLabels.Add(category.category_name);
                        categoryValues.Add(item.count);
                    }
                }

                // BAR CHART DATA: Top 5 Products by Stock
                var stockData = db.Products
                    .Where(p => p.is_active == true)
                    .OrderByDescending(p => p.stock_quantity)
                    .Take(5)
                    .ToList();

                var stockLabels = stockData.Select(p => p.name.Length > 15 ? p.name.Substring(0, 15) + "..." : p.name).ToList();
                var stockValues = stockData.Select(p => p.stock_quantity).ToList();

                // LINE CHART DATA: User Registrations Last 7 Days
                var today = DateTime.Today;
                var userLabels = new List<string>();
                var userValues = new List<int>();

                for (int i = 6; i >= 0; i--)
                {
                    var date = today.AddDays(-i);
                    var nextDate = date.AddDays(1);

                    var count = db.Users.Where(u =>
                        u.created_at >= date &&
                        u.created_at < nextDate
                    ).Count();

                    userLabels.Add(date.ToString("MMM dd"));
                    userValues.Add(count);
                }

                return Json(new
                {
                    success = true,
                    categoryData = new
                    {
                        labels = categoryLabels,
                        values = categoryValues
                    },
                    stockData = new
                    {
                        labels = stockLabels,
                        values = stockValues
                    },
                    userData = new
                    {
                        labels = userLabels,
                        values = userValues
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Get All Products for PDF Report
        [HttpGet]
        public JsonResult GetAllProductsForReport()
        {
            try
            {
                var products = db.Products
                    .Where(p => p.is_active == true)
                    .OrderBy(p => p.name)
                    .ToList();

                var productList = products.Select(p => new
                {
                    name = p.name,
                    price = p.price,
                    stock_quantity = p.stock_quantity,
                    category_name = p.category_id != null
                        ? db.ProductCategories.Find(p.category_id)?.category_name
                        : "N/A"
                }).ToList();

                return Json(new
                {
                    success = true,
                    products = productList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    products = new List<object>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Products
        public ActionResult Products()
        {
            try
            {
                var products = db.Products
                    .Where(p => p.is_active == true)
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

                // Load categories for modal dropdowns
                ViewBag.Categories = db.ProductCategories.Where(c => c.is_active == true).ToList();

                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading products: " + ex.Message;
                return View(new List<tblProductsModel>());
            }
        }

        // GET: Admin/CreateProduct
        public ActionResult CreateProduct()
        {
            try
            {
                ViewBag.Categories = db.ProductCategories.Where(c => c.is_active == true).ToList();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading form: " + ex.Message;
                return View();
            }
        }

        // POST: Admin/CreateProduct
        [HttpPost]
        public ActionResult CreateProduct(tblProductsModel product, HttpPostedFileBase productImage)
        {
            try
            {
                // Handle image upload
                if (productImage != null && productImage.ContentLength > 0)
                {
                    // Create uploads directory if it doesn't exist
                    string uploadDir = Server.MapPath("~/Assets/Images/Products");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Generate unique filename
                    string fileName = Path.GetFileName(productImage.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                    string filePath = Path.Combine(uploadDir, uniqueFileName);

                    // Save file
                    productImage.SaveAs(filePath);

                    // Create image record
                    var image = new tblImagesModel
                    {
                        image_path = "/Assets/Images/Products/" + uniqueFileName,
                        alt_text = product.name,
                        is_active = true,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    db.Images.Add(image);
                    db.SaveChanges();

                    product.image_id = image.image_id;
                }

                // Set product properties
                product.is_active = true;
                product.created_at = DateTime.Now;
                product.updated_at = DateTime.Now;

                // Save product
                db.Products.Add(product);
                db.SaveChanges();

                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error creating product: " + ex.Message;
                ViewBag.Categories = db.ProductCategories.Where(c => c.is_active == true).ToList();
                return View(product);
            }
        }

        // GET: Admin/EditProduct/5
        public ActionResult EditProduct(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Products");
                }

                ViewBag.Categories = db.ProductCategories.Where(c => c.is_active == true).ToList();

                // Load current image
                if (product.image_id != null)
                {
                    var image = db.Images.Find(product.image_id);
                    ViewBag.CurrentImage = image?.image_path;
                }

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading product: " + ex.Message;
                return RedirectToAction("Products");
            }
        }

        // POST: Admin/EditProduct/5
        [HttpPost]
        public ActionResult EditProduct(tblProductsModel product, HttpPostedFileBase productImage)
        {
            try
            {
                var existingProduct = db.Products.Find(product.product_id);
                if (existingProduct == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Products");
                }

                // Handle image upload if new image provided
                if (productImage != null && productImage.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Assets/Images/Products");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Path.GetFileName(productImage.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                    string filePath = Path.Combine(uploadDir, uniqueFileName);
                    productImage.SaveAs(filePath);

                    var image = new tblImagesModel
                    {
                        image_path = "/Assets/Images/Products/" + uniqueFileName,
                        alt_text = product.name,
                        is_active = true,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    db.Images.Add(image);
                    db.SaveChanges();

                    existingProduct.image_id = image.image_id;
                }

                // Update product
                existingProduct.name = product.name;
                existingProduct.description = product.description;
                existingProduct.price = product.price;
                existingProduct.stock_quantity = product.stock_quantity;
                existingProduct.category_id = product.category_id;
                existingProduct.updated_at = DateTime.Now;

                db.SaveChanges();

                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating product: " + ex.Message;
                ViewBag.Categories = db.ProductCategories.Where(c => c.is_active == true).ToList();
                return View(product);
            }
        }

        // GET: Admin/GetProduct - For Edit Modal
        [HttpGet]
        public JsonResult GetProduct(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." }, JsonRequestBehavior.AllowGet);
                }

                // Get image path
                string imagePath = null;
                if (product.image_id != null)
                {
                    var image = db.Images.Find(product.image_id);
                    imagePath = image?.image_path;
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
                        category_id = product.category_id,
                        image_id = product.image_id,
                        image_path = imagePath
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/DeleteProduct/5
        [HttpPost]
        public JsonResult DeleteProduct(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }

                // Soft delete - set is_active to false
                product.is_active = false;
                product.updated_at = DateTime.Now;
                db.SaveChanges();

                return Json(new { success = true, message = "Product deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting product: " + ex.Message });
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