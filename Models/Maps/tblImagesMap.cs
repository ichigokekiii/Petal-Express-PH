using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblImagesMap : EntityTypeConfiguration<tblImages>
    {
        public tblImagesMap()
        {
            ToTable("tbl_images");
            HasKey(i => i.imageID);

            Property(t => t.imageID).HasColumnName("image_id");
            Property(t => t.imagePath).HasColumnName("image_path");
            Property(t => t.altText).HasColumnName("alt_text");
            Property(t => t.isActive).HasColumnName("is_active");
            Property(t => t.createdAt).HasColumnName("created_at");
            Property(t => t.updatedAt).HasColumnName("updated_at");
        }
    }
}