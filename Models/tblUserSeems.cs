using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petal_Express_PH.Models
{
    [Table("tbl_user_seems")]
    public class tblUserSeems
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("bid_id")]
        public int? BidId { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
