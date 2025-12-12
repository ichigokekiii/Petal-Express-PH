using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblCartItemsMap : EntityTypeConfiguration<tblCartItems>
    {
        public tblCartItemsMap()
        {
            ToTable("tbl_cart_items");
            HasKey(i => i.cartItemID);

            Property(t => t.cartItemID).HasColumnName("cart_item_id");
            Property(t => t.cartID).HasColumnName("cart_id");
            Property(t => t.productID).HasColumnName("product_id");
            Property(t => t.quantity).HasColumnName("quantity");
            Property(t => t.createdAt).HasColumnName("created_at");
            Property(t => t.updatedAt).HasColumnName("updated_at");
        }
    }
}