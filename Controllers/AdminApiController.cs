using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petal_Express_PH.Models.Context;
using System;

namespace Petal_Express_PH.Controllers
{
    public class AdminApiController : Controller
    {
        private readonly PetalExpressContext _db = new PetalExpressContext();

        [HttpGet]
        public ActionResult RecentOrders()
        {
            var data = _db.Orders.OrderByDescending(o => o.CreatedAt).Take(10)
                .Select(o => new { Id = o.OrderId, Customer = o.UserId, Items = o.ItemCount, Total = o.OrderAmount, Status = o.OrderStatus })
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
    }
}
