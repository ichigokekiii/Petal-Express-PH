using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblSessions
    {
        // Must match database 'session_id'
        public int sessionID { get; set; }

        // Must match database 'user_id'
        public int userID { get; set; }

        // ✅ This adds the 'action' property needed for the red line
        public string action { get; set; }

        // ✅ This adds the 'description' property needed for the red line
        public string description { get; set; }

        public DateTime createdAt { get; set; }
    }
}