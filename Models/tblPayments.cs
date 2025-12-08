using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petal_Express_PH.Models
{
    [Table("tbl_payments")]
    public class tblPayments
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("method")]
        public string Method { get; set; }

        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("reference_number")]
        public string ReferenceNumber { get; set; }

        [Column("reference_image_path")]
        public string ReferenceImagePath { get; set; }

        [Column("confirmed_by")]
        public string ConfirmedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
