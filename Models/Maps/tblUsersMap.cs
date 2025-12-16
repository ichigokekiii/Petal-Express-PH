using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblUsersMap : EntityTypeConfiguration<tblUsersModel>
    {
        public tblUsersMap()
        {
            HasKey(i => i.user_id);
            ToTable("tbl_users");
        }
    }
}
