using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Petal_Express_PH.Models.Maps
{
    public class tblImagesMap : EntityTypeConfiguration<tblImagesModel>
    {
        public tblImagesMap()
        {
            HasKey(i => i.image_id);
            ToTable("tbl_images");
        }
    }
}
