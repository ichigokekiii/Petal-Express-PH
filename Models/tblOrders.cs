using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblOrders
    {
        public int orderID { get; set; }

        public int userID { get; set; }

        public string orderStatus { get; set; }

        public string shippingStatus { get; set; }

        public decimal totalAmount { get; set; }

        public string shippingAddress { get; set; }

        public string recipientName { get; set; }

        public string recipientPhone { get; set; }

        public string paymentMethod { get; set; }

        public string paymentStatus { get; set; }

        public DateTime? estimatedDelivery { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }
    }
}
