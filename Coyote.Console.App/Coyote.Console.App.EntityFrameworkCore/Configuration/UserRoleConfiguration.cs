using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.UserRolesCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_User_Roles_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.UserRolesUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_User_Roles_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Roles).WithMany(c => c.UserRoles)
               .HasForeignKey(c => c.RoleId).HasConstraintName("FK_User_Roles_Roles_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserRoleList).WithMany(c => c.RolesList)
                .HasForeignKey(c => c.UserId).HasConstraintName("FK_User_User_Roles_User_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new List<UserRoles>
            {
                new UserRoles
                {
                    Id = 1,
                    UserId = 1,
                    RoleId = 1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    IsDefault = true
                },
                 new UserRoles
                {
                    Id = 2,
                    UserId = 2,
                    RoleId = 2,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    IsDefault = true
                },
                new UserRoles
                {
                    Id = 3,
                    UserId = 3,
                    RoleId = 3,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    IsDefault = true
                },
                new UserRoles
                {
                    Id = 4,
                    UserId = 4,
                    RoleId = 3,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    IsDefault = true
                }
            });
        }
    }
}
