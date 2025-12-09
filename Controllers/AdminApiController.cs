using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petal_Express_PH.Models.Context;
using System;
using System.Collections.Generic;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Controllers
{
    public class AdminApiController : Controller
    {
        private readonly PetalExpressContext _db = new PetalExpressContext();

        private class StatDto
        {
            public string title { get; set; }
            public string value { get; set; }
            public string delta { get; set; }
        }

        [HttpGet]
        public ActionResult RecentOrders()
        {
            var data = (from o in _db.Orders
                        join u in _db.Users on o.UserId equals u.UserId
                        orderby o.CreatedAt descending
                        select new
                        {
                            Id = o.OrderId,
                            UserId = o.UserId,
                            Customer = (u.FirstName + " " + u.LastName).Trim(),
                            Items = o.ItemCount,
                            Total = o.OrderAmount,
                            Status = o.OrderStatus,
                            CreatedAt = o.CreatedAt
                        })
                        .Take(10)
                        .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Orders()
        {
            var data = _db.Orders.OrderByDescending(o => o.CreatedAt).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Products()
        {
            var data = _db.Products.OrderByDescending(p => p.CreatedAt).Select(p => new {
                p.ProductId,
                p.Name,
                p.Description,
                p.CategoryId,
                p.ImageId,
                Price = p.Price,
                p.CheckQuantity,
                p.IsArchive,
                p.BidId,
                p.CreatedAt
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Users()
        {
            var data = _db.Users.OrderByDescending(u => u.CreatedAt).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DashboardCharts()
        {
            // Aggregate quantities by product id server-side
            var itemAgg = _db.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Quantity) })
                .ToList();

            var productIds = itemAgg.Select(x => x.ProductId).Distinct().ToList();
            var productMap = _db.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.Name, p.CategoryId })
                .ToList()
                .ToDictionary(p => p.ProductId, p => new { p.Name, p.CategoryId });

            // Group by category in-memory to avoid MySQL GroupBy alias issues
            var categoryAgg = itemAgg
                .GroupBy(x => productMap.ContainsKey(x.ProductId) ? productMap[x.ProductId].CategoryId : (int?)null)
                .Select(g => new {
                    CategoryId = g.Key,
                    Qty = g.Sum(z => z.Qty),
                    ProductCount = g.Select(z => z.ProductId).Distinct().Count()
                })
                .OrderByDescending(x => x.Qty)
                .Take(6)
                .ToList();

            var totalQty = categoryAgg.Sum(cs => cs.Qty);
            var categoryLabels = categoryAgg.Select(cs => {
                var name = cs.CategoryId.HasValue ? ("Category " + cs.CategoryId.Value) : "Uncategorized";
                var pct = totalQty > 0 ? Math.Round((cs.Qty * 100.0) / totalQty, 1) : 0.0;
                return string.Format("{0} — {1} units across {2} products ({3}%)", name, cs.Qty, cs.ProductCount, pct);
            }).ToArray();
            var categoryCounts = categoryAgg.Select(cs => cs.Qty).ToArray();

            // Top products by total quantity sold (server-side agg already done)
            var topProducts = itemAgg
                .OrderByDescending(x => x.Qty)
                .Take(5)
                .ToList();
            var topProductLabels = topProducts.Select(tp => {
                var name = productMap.ContainsKey(tp.ProductId) ? productMap[tp.ProductId].Name : ("#" + tp.ProductId);
                return name + " — " + tp.Qty + " sold";
            }).ToArray();
            var topProductSales = topProducts.Select(tp => tp.Qty).ToArray();

            return Json(new {
                categoryLabels = categoryLabels,
                categoryCounts = categoryCounts,
                topProductLabels = topProductLabels,
                topProductSales = topProductSales
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStats()
        {
            var ordersCount = _db.Orders.Count();
            var totalRevenue = _db.Orders.Sum(o => (decimal?)o.OrderAmount) ?? 0m;
            var customersCount = _db.Users.Count();
            var productsCount = _db.Products.Count();
            var stats = new List<StatDto>{
                new StatDto { title = "Orders", value = ordersCount.ToString(), delta = string.Empty },
                new StatDto { title = "Revenue", value = "$" + totalRevenue.ToString("N0"), delta = string.Empty },
                new StatDto { title = "Customers", value = customersCount.ToString(), delta = string.Empty },
                new StatDto { title = "Products", value = productsCount.ToString(), delta = string.Empty }
            };
            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        // Upload image, save file and DB record in tbl_images, return new image_id
        [HttpPost]
        public ActionResult UploadImage()
        {
            if (Request.Files.Count == 0)
            {
                Response.StatusCode = 400;
                return Json(new { error = "No file uploaded." });
            }
            var file = Request.Files[0];
            if (file == null || file.ContentLength == 0)
            {
                Response.StatusCode = 400;
                return Json(new { error = "Empty file." });
            }
            var uploadsDir = Server.MapPath("~/Assets/Uploads");
            if (!System.IO.Directory.Exists(uploadsDir)) System.IO.Directory.CreateDirectory(uploadsDir);
            var fileName = Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(file.FileName);
            var fullPath = System.IO.Path.Combine(uploadsDir, fileName);
            file.SaveAs(fullPath);
            var relPath = "/Assets/Uploads/" + fileName;

            // Insert into tbl_images using provided schema
            _db.Database.ExecuteSqlCommand(
                "INSERT INTO tbl_images (image_path, is_archive, is_active, created_at) VALUES (@p0, 0, 1, @p1)",
                relPath, DateTime.UtcNow
            );
            var newId = _db.Database.SqlQuery<int>("SELECT LAST_INSERT_ID()").FirstOrDefault();
            return Json(new { image_id = newId, image_path = relPath });
        }

        [HttpPost]
        public ActionResult CreateProduct(tblProducts dto)
        {
            if (dto == null) { Response.StatusCode = 400; return Json(new { error = "Invalid payload." }); }

            var entity = new tblProducts
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                ImageId = dto.ImageId,
                BidId = dto.BidId,
                CheckQuantity = dto.CheckQuantity,
                Price = dto.Price,
                IsArchive = dto.IsArchive ?? false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Products.Add(entity);
            _db.SaveChanges();

            var result = new
            {
                entity.ProductId,
                entity.Name,
                entity.Description,
                entity.CategoryId,
                entity.ImageId,
                Price = entity.Price,
                entity.CheckQuantity,
                entity.IsArchive,
                entity.CreatedAt
            };
            return Json(result);
        }

        [HttpPost]
        public ActionResult UpdateProduct(tblProducts dto)
        {
            if (dto == null || dto.ProductId <= 0) { Response.StatusCode = 400; return Json(new { error = "Invalid product id." }); }

            var entity = _db.Products.FirstOrDefault(p => p.ProductId == dto.ProductId);
            if (entity == null) { Response.StatusCode = 404; return Json(new { error = "Product not found." }); }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.CategoryId = dto.CategoryId;
            entity.Price = dto.Price;
            entity.ImageId = dto.ImageId;
            entity.CheckQuantity = dto.CheckQuantity;
            entity.IsArchive = dto.IsArchive;
            entity.BidId = dto.BidId;
            entity.UpdatedAt = DateTime.UtcNow;

            _db.SaveChanges();

            var result = new
            {
                entity.ProductId,
                entity.Name,
                entity.Description,
                entity.CategoryId,
                entity.ImageId,
                Price = entity.Price,
                entity.CheckQuantity,
                entity.IsArchive,
                entity.CreatedAt
            };
            return Json(result);
        }
    }
}
