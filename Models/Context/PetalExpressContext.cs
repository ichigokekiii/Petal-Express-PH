using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
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

        // DbSets for all tables
        public DbSet<tblUsersModel> Users { get; set; }
        public DbSet<tblProductsModel> Products { get; set; }
        public DbSet<tblProductCategoryModel> ProductCategories { get; set; }
        public DbSet<tblImagesModel> Images { get; set; }
        public DbSet<tblCartModel> Carts { get; set; }
        public DbSet<tblCartItemsModel> CartItems { get; set; }
        public DbSet<tblOrdersModel> Orders { get; set; }
        public DbSet<tblOrderItemsModel> OrderItems { get; set; }
        public DbSet<tblSessionsModel> Sessions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Apply all table mappings
            modelBuilder.Configurations.Add(new tblUsersMap());
            modelBuilder.Configurations.Add(new tblProductsMap());
            modelBuilder.Configurations.Add(new tblProductCategoryMap());
            modelBuilder.Configurations.Add(new tblImagesMap());
            modelBuilder.Configurations.Add(new tblCartMap());
            modelBuilder.Configurations.Add(new tblCartItemsMap());
            modelBuilder.Configurations.Add(new tblOrdersMap());
            modelBuilder.Configurations.Add(new tblOrderItemsMap());
            modelBuilder.Configurations.Add(new tblSessionsMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}