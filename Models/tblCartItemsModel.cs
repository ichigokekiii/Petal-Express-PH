using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblCartItemsModel
    {
        public int cart_item_id { get; set; }

        public int cart_id { get; set; }

        public int product_id { get; set; }

        public int quantity { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }
    }
}
