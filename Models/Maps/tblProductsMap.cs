using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblProductsMap : EntityTypeConfiguration<tblProductsModel>
    {
        public tblProductsMap()
        {
            HasKey(i => i.product_id);
            ToTable("tbl_products");
        }
    }
}
