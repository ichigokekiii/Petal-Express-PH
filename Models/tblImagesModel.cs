using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblImagesModel
    {
        public int image_id { get; set; }

        public string image_path { get; set; }

        public string alt_text { get; set; }

        public bool is_active { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }
    }
}
