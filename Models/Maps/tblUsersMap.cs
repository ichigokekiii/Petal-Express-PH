using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblUsersMap : EntityTypeConfiguration<tblUsers>
    {
        public tblUsersMap()
        {
            // 1. Point to the correct table
            ToTable("tbl_users");

            // 2. Define the Primary Key
            HasKey(t => t.userID);

            // 3. THE TRANSLATION LAYER (Critical Fix!)
            Property(t => t.userID).HasColumnName("user_id");
            Property(t => t.email).HasColumnName("email");
            Property(t => t.passwordHash).HasColumnName("password_hash");
            Property(t => t.firstName).HasColumnName("first_name");
            Property(t => t.lastName).HasColumnName("last_name");
            Property(t => t.phoneNumber).HasColumnName("phone_number");
            Property(t => t.role).HasColumnName("role");
            Property(t => t.isActive).HasColumnName("is_active");
            Property(t => t.createdAt).HasColumnName("created_at");
            Property(t => t.updatedAt).HasColumnName("updated_at");
        }
    }
}