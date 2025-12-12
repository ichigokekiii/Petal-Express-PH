using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblOrderItems
    {
        public int orderItemID { get; set; }

        public int orderID { get; set; }

        public int productID { get; set; }

        public string productName { get; set; }

        public int quantity { get; set; }

        public decimal priceAtPurchase { get; set; }

        public DateTime createdAt { get; set; }
    }
}
