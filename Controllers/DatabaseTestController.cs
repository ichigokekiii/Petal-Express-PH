using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petal_Express_PH.Models;
using Petal_Express_PH.Models.Context;

namespace Petal_Express_PH.Controllers
{
    public class DatabaseTestController : Controller
    {
        private PetalExpressContext db = new PetalExpressContext();

        // Test database connection
        // Navigate to: /DatabaseTest/TestConnection
        public ActionResult TestConnection()
        {
            try
            {
                // Try to connect and count records in each table
                var results = new
                {
                    Success = true,
                    Message = "Database connection successful!",
                    TableCounts = new
                    {
                        Users = db.Users.Count(),
                        Products = db.Products.Count(),
                        Categories = db.ProductCategories.Count(),
                        Images = db.Images.Count(),
                        Carts = db.Carts.Count(),
                        CartItems = db.CartItems.Count(),
                        Orders = db.Orders.Count(),
                        OrderItems = db.OrderItems.Count(),
                        Sessions = db.Sessions.Count()
                    }
                };

                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var error = new
                {
                    Success = false,
                    Message = "Database connection failed!",
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message
                };

                return Json(error, JsonRequestBehavior.AllowGet);
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
