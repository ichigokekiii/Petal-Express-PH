using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblOrdersModel
    {
        public int order_id { get; set; }

        public int user_id { get; set; }

        public string order_status { get; set; }

        public string shipping_status { get; set; }

        public decimal total_amount { get; set; }

        public string shipping_address { get; set; }

        public string recipient_name { get; set; }

        public string recipient_phone { get; set; }

        public string payment_method { get; set; }

        public string payment_status { get; set; }

        public DateTime? estimated_delivery { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }
    }
}
