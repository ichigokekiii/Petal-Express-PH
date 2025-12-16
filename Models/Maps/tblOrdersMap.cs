using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblOrdersMap : EntityTypeConfiguration<tblOrdersModel>
    {
        public tblOrdersMap()
        {
            HasKey(i => i.order_id);
            ToTable("tbl_orders");
        }
    }
}
