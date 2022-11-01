using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;


namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class AccessDepartmentConfiguration : IEntityTypeConfiguration<AccessDepartment>
    {

        public void Configure(EntityTypeBuilder<AccessDepartment> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.AccessDepartmentCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_AccessDepartment_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.AccessDepartmentUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_AccessDepartment_Updated_By").OnDelete(DeleteBehavior.NoAction);
            //builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Role_Code");



            builder?.HasData(new List<AccessDepartment>
            {
                new AccessDepartment
                {
                    Id = 1,
                   RoleId=1,
                    DepartmentId=1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                
            });
        }
    }
}
