using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblProductCategoryModel
    {
        public int category_id { get; set; }

        public string category_name { get; set; }

        public string description { get; set; }

        public bool is_active { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }
    }
}
