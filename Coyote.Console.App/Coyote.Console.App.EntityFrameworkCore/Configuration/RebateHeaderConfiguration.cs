using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class RebateHeaderConfiguration : IEntityTypeConfiguration<RebateHeader>
    {
        public void Configure(EntityTypeBuilder<RebateHeader> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.RebateHeaderCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_ReabateHeader_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.RebateHeaderUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_RebateHeader_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Manufacturer).WithMany(c => c.RebateManufacturer)
                .HasForeignKey(c => c.ManufacturerId).HasConstraintName("FK_Rebate_Manufacturer").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Zone).WithMany(c => c.RebateZone)
              .HasForeignKey(c => c.ZoneId).HasConstraintName("FK_Rebate_Zone").OnDelete(DeleteBehavior.NoAction);
        }
    }
    
}
