using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblProductCategory
    {
        public int categoryID { get; set; }

        public string categoryName { get; set; }

        public string description { get; set; }

        public bool isActive { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }
    }
}
