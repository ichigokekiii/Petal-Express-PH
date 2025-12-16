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