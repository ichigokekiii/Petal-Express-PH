using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblCartItemsMap : EntityTypeConfiguration<tblCartItemsModel>
    {
        public tblCartItemsMap()
        {
            HasKey(i => i.cart_item_id);
            ToTable("tbl_cart_items");
        }
    }
}
