using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petal_Express_PH.Models
{
    [Table("tbl_orders")]
    public class tblOrders
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("recipient_id")]
        public int? RecipientId { get; set; }

        [Column("payment_id")]
        public int? PaymentId { get; set; }

        [Column("order_status")]
        public string OrderStatus { get; set; }

        [Column("shipping_status")]
        public string ShippingStatus { get; set; }

        [Column("order_amount")]
        public decimal? OrderAmount { get; set; }

        [Column("item_count")]
        public int? ItemCount { get; set; }

        [Column("estimated_delivery")]
        public DateTime? EstimatedDelivery { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
