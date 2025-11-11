using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models
{
    public class tblRegistrationsModel
    {
        public int registrationID { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }
    }
}