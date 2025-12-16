using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblSessionsMap : EntityTypeConfiguration<tblSessionsModel>
    {
        public tblSessionsMap()
        {
            HasKey(i => i.session_id);
            ToTable("tbl_sessions");
        }
    }
}
