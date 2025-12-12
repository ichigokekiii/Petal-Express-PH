using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblOrderItemsMap : EntityTypeConfiguration<tblOrderItems>
    {
        public tblOrderItemsMap()
        {
            ToTable("tbl_orderitems");
            HasKey(i => i.orderItemID);

            Property(t => t.orderItemID).HasColumnName("order_item_id");
            Property(t => t.orderID).HasColumnName("order_id");
            Property(t => t.productID).HasColumnName("product_id");
            Property(t => t.productName).HasColumnName("product_name");
            Property(t => t.quantity).HasColumnName("quantity");
            Property(t => t.priceAtPurchase).HasColumnName("price_at_purchase");
            Property(t => t.createdAt).HasColumnName("created_at");
        }
    }
}