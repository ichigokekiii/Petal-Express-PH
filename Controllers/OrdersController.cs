using System;
using System.Linq;
using System.Web.Mvc;
using Petal_Express_PH.Models.Context;
using Petal_Express_PH.Models;
using System.Collections.Generic;

namespace Petal_Express_PH.Controllers
{
    public class CartItemRow
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrdersController : Controller
    {
        private readonly PetalExpressContext _db = new PetalExpressContext();

        // DTOs
        public class CreateOrderRequest
        {
            public int user_id { get; set; }
            public tblRecipient recipient { get; set; }
            public tblPayments payment { get; set; }
            public tblOrderItems[] items { get; set; }
        }

        private int? GetCurrentUserId()
        {
            var email = Session["user_email"] as string;
            if (string.IsNullOrEmpty(email)) return null;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            return user?.UserId;
        }

        [HttpGet]
        public ActionResult Orders()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Home");
            var orders = _db.Orders.Where(o => o.UserId == userId.Value)
                                   .OrderByDescending(o => o.CreatedAt)
                                   .ToList();
            return View(orders);
        }

        // Returns current user details, orders, and items for PDF report
        [HttpGet]
        public ActionResult GetMyOrdersReportData()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return new HttpStatusCodeResult(401);

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId.Value);
            var orders = _db.Orders.Where(o => o.UserId == userId.Value)
                                   .OrderByDescending(o => o.CreatedAt)
                                   .ToList();
            var orderIds = orders.Select(o => o.OrderId).ToList();
            var items = _db.OrderItems.Where(oi => orderIds.Contains(oi.OrderId)).ToList();
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = _db.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

            var productMap = products.ToDictionary(p => p.ProductId, p => new { Name = p.Name, Price = p.Price ?? 0m });

            var orderDtos = orders.Select(o => {
                var orderItems = items.Where(i => i.OrderId == o.OrderId).Select(i => {
                    var unit = productMap.ContainsKey(i.ProductId) ? productMap[i.ProductId].Price : (i.CostPrice ?? 0m);
                    var name = productMap.ContainsKey(i.ProductId) ? productMap[i.ProductId].Name : "";
                    var qty = i.Quantity;
                    var line = unit * qty;
                    return new {
                        ProductId = i.ProductId,
                        ProductName = name,
                        Quantity = qty,
                        UnitPrice = unit,
                        LineTotal = line
                    };
                }).ToList();
                var computedTotal = orderItems.Sum(x => x.LineTotal);
                return new {
                    OrderId = o.OrderId,
                    CreatedAt = o.CreatedAt,
                    OrderStatus = o.OrderStatus,
                    OrderAmount = computedTotal,
                    ItemCount = orderItems.Sum(x => x.Quantity),
                    Items = orderItems
                };
            }).ToList();

            var totalAllOrders = orderDtos.Sum(x => x.OrderAmount);

            return Json(new
            {
                user = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber
                },
                orders = orderDtos,
                totalAllOrders = totalAllOrders
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateFromCart()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return new HttpStatusCodeResult(401);
            var now = DateTime.UtcNow;

            var cartId = _db.Database.SqlQuery<int>("SELECT cart_id FROM tbl_cart WHERE user_id = @p0 LIMIT 1", userId.Value).FirstOrDefault();
            if (cartId <= 0) { return new HttpStatusCodeResult(400); }
            var cartItems = _db.Database.SqlQuery<CartItemRow>("SELECT product_id AS ProductId, quantity AS Quantity FROM tbl_cart_items WHERE cart_id = @p0", cartId).ToList();
            if (cartItems.Count == 0) return new HttpStatusCodeResult(400);

            var order = new tblOrders
            {
                UserId = userId.Value,
                OrderStatus = "Created",
                ShippingStatus = "Pending",
                OrderAmount = 0m,
                ItemCount = cartItems.Sum(ci => ci.Quantity),
                EstimatedDelivery = null,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Orders.Add(order);
            _db.SaveChanges();

            decimal total = 0m;
            foreach (var ci in cartItems)
            {
                var product = _db.Products.Find(ci.ProductId);
                var price = (product != null && product.Price.HasValue) ? product.Price.Value : 0m;
                var oi = new tblOrderItems
                {
                    OrderId = order.OrderId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    CostPrice = price,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                total += price * ci.Quantity;
                _db.OrderItems.Add(oi);
            }
            order.OrderAmount = total;
            _db.SaveChanges();

            _db.Database.ExecuteSqlCommand("DELETE FROM tbl_cart_items WHERE cart_id = @p0", cartId);
            _db.Database.ExecuteSqlCommand("UPDATE tbl_cart SET updated_at = @p0 WHERE cart_id = @p1", now, cartId);

            return Json(new { order_id = order.OrderId, amount = order.OrderAmount });
        }

        [HttpPost]
        public ActionResult Create(CreateOrderRequest req)
        {
            if (req == null || req.items == null || req.items.Length == 0)
                return new HttpStatusCodeResult(400, "Invalid order request");

            var now = DateTime.UtcNow;

            var order = new tblOrders
            {
                UserId = req.user_id,
                OrderStatus = "Created",
                ShippingStatus = "Pending",
                OrderAmount = 0m,
                ItemCount = req.items.Sum(i => i.Quantity),
                EstimatedDelivery = null,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Orders.Add(order);
            _db.SaveChanges();

            decimal total = 0m;
            foreach (var item in req.items)
            {
                var product = _db.Products.Find(item.ProductId);
                if (product == null) continue;
                var price = product.Price ?? item.CostPrice ?? 0m;
                total += price * item.Quantity;
                item.OrderId = order.OrderId;
                item.CostPrice = price;
                item.CreatedAt = now;
                item.UpdatedAt = now;
                _db.OrderItems.Add(item);
            }
            order.OrderAmount = total;
            _db.SaveChanges();

            if (req.recipient != null)
            {
                req.recipient.UserId = req.user_id;
                req.recipient.OrderId = order.OrderId;
                req.recipient.CreatedAt = now;
                req.recipient.UpdatedAt = now;
                _db.Recipients.Add(req.recipient);
                _db.SaveChanges();
                order.RecipientId = req.recipient.RecipientId;
                _db.SaveChanges();
            }

            if (req.payment != null)
            {
                req.payment.OrderId = order.OrderId;
                req.payment.Amount = order.OrderAmount;
                req.payment.PaymentDate = now;
                req.payment.CreatedAt = now;
                req.payment.UpdatedAt = now;
                _db.Payments.Add(req.payment);
                _db.SaveChanges();
                order.PaymentId = req.payment.PaymentId;
                _db.SaveChanges();
            }

            return Json(new { order_id = order.OrderId, amount = order.OrderAmount });
        }
    }
}
