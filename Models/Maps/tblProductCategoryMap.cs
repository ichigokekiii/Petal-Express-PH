using System.Data.Entity.ModelConfiguration;
using Petal_Express_PH.Models;

namespace Petal_Express_PH.Models.Maps
{
    public class tblProductCategoryMap : EntityTypeConfiguration<tblProductCategory>
    {
        public tblProductCategoryMap()
        {
            ToTable("tbl_productcategory");
            HasKey(i => i.categoryID);

            Property(t => t.categoryID).HasColumnName("category_id");
            Property(t => t.categoryName).HasColumnName("category_name");
            Property(t => t.description).HasColumnName("description");
            Property(t => t.isActive).HasColumnName("is_active");
            Property(t => t.createdAt).HasColumnName("created_at");
            Property(t => t.updatedAt).HasColumnName("updated_at");
        }
    }
}