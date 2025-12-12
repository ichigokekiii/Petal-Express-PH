using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Context
{
    public class PetalExpressContext : DbContext
    {

        static PetalExpressContext()
        {
            Database.SetInitializer<PetalExpressContext>(null);
        }

        public PetalExpressContext() : base("Name=3ite_db") {}

        // DbSets for CRUD
        public DbSet<tblUsers> Users { get; set; }
        public DbSet<tblProducts> Products { get; set; }
        public DbSet<tblOrders> Orders { get; set; }
        public DbSet<tblOrderItems> OrderItems { get; set; }
        public DbSet<tblPayments> Payments { get; set; }
        public DbSet<tblRecipient> Recipients { get; set; }
        public DbSet<tblSystem> Systems { get; set; }
        public DbSet<tblUserSeems> UserSeems { get; set; }
    }
}