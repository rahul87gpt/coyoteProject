using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class OutletTradingHiursConfiguration : IEntityTypeConfiguration<OutletTradingHours>
    {
        public void Configure(EntityTypeBuilder<OutletTradingHours> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.OutletTradingHoursCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_OutTradingHours_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.OutletTradingHoursUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_OutTradingHours_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Outelt).WithMany(c => c.OutletTradingHours)
              .HasForeignKey(c => c.OuteltId).HasConstraintName("FK_Store_OutTradingHours").OnDelete(DeleteBehavior.NoAction);
        }
    }
}