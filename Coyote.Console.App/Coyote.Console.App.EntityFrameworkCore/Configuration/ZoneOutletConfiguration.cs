using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class ZoneOutletConfiguration : IEntityTypeConfiguration<ZoneOutlet>
    {
        public void Configure(EntityTypeBuilder<ZoneOutlet> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.ZoneOutLetCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_ZoneOut_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.ZoneOutLetUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_ZoneOut_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.ZoneMasterListItems).WithMany(c => c.ZoneOutlet)
                .HasForeignKey(c => c.ZoneId).HasConstraintName("FK_MasterListItems_ZoneOut_Zone_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.ZoneOutlets)
               .HasForeignKey(c => c.StoreId).HasConstraintName("FK_Store_ZoneOut_Store_Id").OnDelete(DeleteBehavior.NoAction);
            //builder?.HasIndex(p => new { p.Code, p.ZoneId }).IsUnique().HasName("IX_ZOut_Code_Zone");

            builder?.HasData(new List<ZoneOutlet>
            {
                new ZoneOutlet
                {
                    Id = 1,
                    StoreId = 1,
                    ZoneId = 7,
                    //Code = "ZOne Out1",
                    //Desc = "ZOne Out1",
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