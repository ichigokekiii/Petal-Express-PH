using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblOrderItemsModel
    {
        public int order_item_id { get; set; }

        public int order_id { get; set; }

        public int product_id { get; set; }

        public string product_name { get; set; }

        public int quantity { get; set; }

        public decimal price_at_purchase { get; set; }

        public DateTime created_at { get; set; }
    }
}
