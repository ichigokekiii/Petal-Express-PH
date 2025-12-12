using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblCartMap : EntityTypeConfiguration<tblCart>
    {
        public tblCartMap()
        {
            ToTable("tbl_cart");
            HasKey(i => i.cartID);

            Property(t => t.cartID).HasColumnName("cart_id");
            Property(t => t.userID).HasColumnName("user_id");
            Property(t => t.createdAt).HasColumnName("created_at");
            Property(t => t.updatedAt).HasColumnName("updated_at");
        }
    }
}