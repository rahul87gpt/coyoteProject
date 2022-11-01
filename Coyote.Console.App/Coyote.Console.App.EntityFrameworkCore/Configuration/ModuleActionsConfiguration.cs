using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class ModuleActionsConfiguration : IEntityTypeConfiguration<ModuleActions>
    {
        public void Configure(EntityTypeBuilder<ModuleActions> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.ControllerActionsCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_ControllerActionsCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.ControllerActionsUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_ControllerActionsUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.ModuleName).WithMany(c => c.ModuleNameMasterListItem)
                .HasForeignKey(c => c.ModuleId).HasConstraintName("FK_User_ControllerActionsModule_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.ActionType).WithMany(c => c.ActionTypeMasterListItem)
                .HasForeignKey(c => c.ActionTypeId).HasConstraintName("FK_User_ControllerActionsAction_Type").OnDelete(DeleteBehavior.NoAction);


            builder?.HasData(new List<ModuleActions>
            {
                new ModuleActions
                {
                    Id = 1,
                    Name="Get",
                    Action ="Get",
                    ModuleId=8,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                },
                new ModuleActions
                {
                    Id = 2,
                    Name="Post",
                    Action ="Post",
                    ModuleId=8,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                },
                new ModuleActions
                {
                    Id = 3,
                    Name="Put",
                    Action ="Put",
                    ModuleId=8,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                },
                new ModuleActions
                {
                    Id = 4,
                    Name="Delete",
                    Action ="Delete",
                    ModuleId=8,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                }
            });
        }
    }
}
