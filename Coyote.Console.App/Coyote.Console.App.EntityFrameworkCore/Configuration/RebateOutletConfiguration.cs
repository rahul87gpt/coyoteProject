using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
  public  class RebateOutletConfiguration : IEntityTypeConfiguration<RebateOutlets>
    {
        public void Configure(EntityTypeBuilder<RebateOutlets> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.RebateOutletCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_ReabateOutlet_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.RebateOutletUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_RebateOutlet_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Store).WithMany(c => c.RebateOutlets)
                .HasForeignKey(c => c.StoreId).HasConstraintName("FK_Rebate_Outlet").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.RebateHeader).WithMany(c => c.RebateOutlets)
              .HasForeignKey(c => c.RebateHeaderId).HasConstraintName("FK_Rebate_Outlet_Header").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
