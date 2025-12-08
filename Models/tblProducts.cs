using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petal_Express_PH.Models
{
    [Table("tbl_products")]
    public class tblProducts
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("bid_id")]
        public int? BidId { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("colleague_id")]
        public int? ColleagueId { get; set; }

        [Column("image_id")]
        public int? ImageId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("is_archive")]
        public bool? IsArchive { get; set; }

        [Column("check_quantity")]
        public int? CheckQuantity { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
