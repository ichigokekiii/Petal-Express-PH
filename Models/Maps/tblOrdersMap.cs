using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblOrdersMap : EntityTypeConfiguration<tblOrders>
    {
        public tblOrdersMap()
        {
            ToTable("tbl_orders");
            HasKey(i => i.orderID);

            Property(t => t.orderID).HasColumnName("order_id");
            Property(t => t.userID).HasColumnName("user_id");
            Property(t => t.orderStatus).HasColumnName("order_status");
            Property(t => t.shippingStatus).HasColumnName("shipping_status");
            Property(t => t.totalAmount).HasColumnName("total_amount");
            Property(t => t.shippingAddress).HasColumnName("shipping_address");
            Property(t => t.recipientName).HasColumnName("recipient_name");
            Property(t => t.recipientPhone).HasColumnName("recipient_phone");
            Property(t => t.paymentMethod).HasColumnName("payment_method");
            Property(t => t.paymentStatus).HasColumnName("payment_status");
            Property(t => t.estimatedDelivery).HasColumnName("estimated_delivery");
            Property(t => t.createdAt).HasColumnName("created_at");
            Property(t => t.updatedAt).HasColumnName("updated_at");
        }
    }
}