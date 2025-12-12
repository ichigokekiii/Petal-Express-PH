using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblImages
    {
        public int imageID { get; set; }

        public string imagePath { get; set; }

        public string altText { get; set; }

        public bool isActive { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }
    }
}
