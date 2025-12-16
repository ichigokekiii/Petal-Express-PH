using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblCartMap : EntityTypeConfiguration<tblCartModel>
    {
        public tblCartMap()
        {
            HasKey(i => i.cart_id);
            ToTable("tbl_cart");
        }
    }
}
