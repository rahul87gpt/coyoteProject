using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.RolesCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_role_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.RolesUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_role_Updated_By").OnDelete(DeleteBehavior.NoAction);
          //  builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Role_Code");

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Role_Delete_Unique");

            builder?.HasData(new List<Roles>
            {
                new Roles
                {
                    Id = 1,
                    Code = "SuperAdmin",
                    Name = "SuperAdmin",
                    Type = "SuperAdmin",
                    Status = true,
                    PermissionSet="*",
                    PermissionDeptSet="",
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                 new Roles
                {
                    Id = 2,
                    Code = "Admin",
                    Name = "Admin",
                    Type = "Admin",
                    Status = true,
                    PermissionSet="*",
                    PermissionDeptSet="",
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                  new Roles
                {
                    Id = 3,
                    Code = "Super",
                    Name = "Super",
                    Type = "Super",
                    Status = true,
                    PermissionSet="*",
                    PermissionDeptSet="",
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                }
            });
        }
    }
}
