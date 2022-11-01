using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class StoreGroupConfiguration : IEntityTypeConfiguration<StoreGroup>
    {
        public void Configure(EntityTypeBuilder<StoreGroup> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.StoreGroupCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Store_Group_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.StoreGroupUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Store_Group_Updated_By").OnDelete(DeleteBehavior.NoAction);


            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_StoreGroupCode_Delete_Unique");

            builder?.HasData(new List<StoreGroup>
            {
                new StoreGroup
                {
                    Id = 1,
                    Code = "0",
                    Name = "Super Admin Store Group",
                    Status = true,
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
