using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblSessionsMap : EntityTypeConfiguration<tblSessions>
    {
        public tblSessionsMap()
        {
            ToTable("tbl_sessions");
            HasKey(i => i.sessionID);

            Property(t => t.sessionID).HasColumnName("session_id");
            Property(t => t.userID).HasColumnName("user_id");
            Property(t => t.action).HasColumnName("action");
            Property(t => t.description).HasColumnName("description");
            Property(t => t.createdAt).HasColumnName("created_at");
        }
    }
}