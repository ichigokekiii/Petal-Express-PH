using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblOrderItemsMap : EntityTypeConfiguration<tblOrderItemsModel>
    {
        public tblOrderItemsMap()
        {
            HasKey(i => i.order_item_id);
            ToTable("tbl_orderitems");
        }
    }
}
