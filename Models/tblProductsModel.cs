using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblProductsModel
    {
        public int product_id { get; set; }

        public int? category_id { get; set; }

        public int? image_id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public decimal price { get; set; }

        public int stock_quantity { get; set; }

        public bool is_active { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }
    }
}