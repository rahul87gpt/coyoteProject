using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.DepartmentCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Department_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.DepartmentUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Department_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.MapTypeMasterListItems).WithMany(c => c.DeptMapType)
                .HasForeignKey(c => c.MapTypeId).HasConstraintName("FK_MasterListItem_Department_Dept__Map_Type_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Department_Delete_Unique");

            //builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Department_Code_Unique");

            builder?.HasData(new Department
            {
                Id = 1,
                Code = "Department1",
                Desc = "Department Description",
                Status = true,
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });
        }
    }
}
