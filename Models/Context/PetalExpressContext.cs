using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Petal_Express_PH.Models;
using Petal_Express_PH.Models.Maps;

namespace Petal_Express_PH.Models.Context
{
    public class PetalExpressContext : DbContext
    {
        static PetalExpressContext()
        {
            Database.SetInitializer<PetalExpressContext>(null);
        }

        public PetalExpressContext() : base("Name=3ite_db") { }

        // ============================================================================
        // SIMPLIFIED DBSETS - ONLY 9 TABLES!
        // ============================================================================

        // User Management (2 tables)
        public DbSet<tblUsers> Users { get; set; }
        public DbSet<tblSessions> Sessions { get; set; }

        // Product Management (2 tables)
        public DbSet<tblProducts> Products { get; set; }
        public DbSet<tblProductCategory> ProductCategories { get; set; }

        // CMS - Image Management (1 table)
        public DbSet<tblImages> Images { get; set; }

        // Cart Management (2 tables)
        public DbSet<tblCart> Carts { get; set; }
        public DbSet<tblCartItems> CartItems { get; set; }

        // Order Management (2 tables)
        public DbSet<tblOrders> Orders { get; set; }
        public DbSet<tblOrderItems> OrderItems { get; set; }

        // ============================================================================
        // MODEL CONFIGURATION (APPLY ALL 9 MAPS)
        // ============================================================================

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // User Maps
            modelBuilder.Configurations.Add(new tblUsersMap());
            modelBuilder.Configurations.Add(new tblSessionsMap());

            // Product Maps
            modelBuilder.Configurations.Add(new tblProductsMap());
            modelBuilder.Configurations.Add(new tblProductCategoryMap());

            // CMS - Image Map
            modelBuilder.Configurations.Add(new tblImagesMap());

            // Cart Maps
            modelBuilder.Configurations.Add(new tblCartMap());
            modelBuilder.Configurations.Add(new tblCartItemsMap());

            // Order Maps
            modelBuilder.Configurations.Add(new tblOrdersMap());
            modelBuilder.Configurations.Add(new tblOrderItemsMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
