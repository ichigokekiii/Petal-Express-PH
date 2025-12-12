using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblProductsMap : EntityTypeConfiguration<tblProducts>
    {
        public tblProductsMap()
        {
            ToTable("tbl_products");
            HasKey(i => i.productID);

            // Column Mappings
            Property(t => t.productID).HasColumnName("product_id");
            Property(t => t.categoryID).HasColumnName("category_id");
            Property(t => t.imageID).HasColumnName("image_id");
            Property(t => t.name).HasColumnName("name");
            Property(t => t.description).HasColumnName("description");
            Property(t => t.price).HasColumnName("price");
            Property(t => t.stockQuantity).HasColumnName("stock_quantity");
            Property(t => t.isActive).HasColumnName("is_active");
            Property(t => t.createdAt).HasColumnName("created_at");
            Property(t => t.updatedAt).HasColumnName("updated_at");
        }
    }
}