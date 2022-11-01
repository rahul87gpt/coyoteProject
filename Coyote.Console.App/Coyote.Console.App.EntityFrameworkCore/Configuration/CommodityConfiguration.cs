using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class CommodityConfiguration : IEntityTypeConfiguration<Commodity>
    {
        public void Configure(EntityTypeBuilder<Commodity> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.CommodityCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Commodity_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.CommodityUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Commodity_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Departments).WithMany(c => c.Commodity)
                .HasForeignKey(c => c.DepartmentId).HasConstraintName("FK_Commodity_Department_Dept_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Category).WithMany(c => c.CommodityTypeCategory)
               .HasForeignKey(c => c.CategoryId).HasConstraintName("FK_Commodity_MasterListItems_Id").OnDelete(DeleteBehavior.NoAction);


            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Commodity_Delete_Unique");

            //builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Commodity_Code_Unique");

            builder?.HasData(new Commodity
            {
                Id = 1,
                Code = "Comodity1",
                Desc = "Comodity Description",
                Status = true,
                DepartmentId = 1,
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });
        }
    }
}
