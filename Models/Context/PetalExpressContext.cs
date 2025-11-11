using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Context
{
    public class PetalExpressContext : DbContext
    {

        static PetalExpressContext()
        {
            Database.SetInitializer<PetalExpressContext>(null);
        }

        public PetalExpressContext() : base("Name=3ite_db") {}
    }
}