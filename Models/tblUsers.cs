using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Petal_Express_PH.Models
{
    [Table("tbl_users")]
    public class tblUsers
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Column("profile_id")]
        public int? ProfileId { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
