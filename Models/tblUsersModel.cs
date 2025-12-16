using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblUsersModel
    {
        public int user_id { get; set; }

        public string email { get; set; }

        public string password_hash { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string phone_number { get; set; }

        public string role { get; set; }

        public bool is_active { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }
    }
}