using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblSessionsModel
    {
        public int session_id { get; set; }

        public int user_id { get; set; }

        public string action { get; set; }

        public string description { get; set; }

        public DateTime created_at { get; set; }
    }
}
