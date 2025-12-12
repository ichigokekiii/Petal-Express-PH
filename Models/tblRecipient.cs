using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petal_Express_PH.Models
{
    [Table("tbl_recipient")]
    public class tblRecipient
    {
        [Key]
        [Column("recipient_id")]
        public int RecipientId { get; set; }

        [Column("order_id")]
        public int? OrderId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("contact_number")]
        public string ContactNumber { get; set; }

        [Column("address_details")]
        public string AddressDetails { get; set; }

        [Column("house_no")]
        public string HouseNo { get; set; }

        [Column("region")]
        public string Region { get; set; }

        [Column("province")]
        public string Province { get; set; }

        [Column("municipality")]
        public string Municipality { get; set; }

        [Column("zip_code")]
        public string ZipCode { get; set; }

        [Column("barangay")]
        public string Barangay { get; set; }

        [Column("special_markers")]
        public string SpecialMarkers { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
