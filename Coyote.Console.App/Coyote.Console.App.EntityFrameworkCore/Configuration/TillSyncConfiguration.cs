using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class TillSyncConfiguration : IEntityTypeConfiguration<TillSync>
    {
        public void Configure(EntityTypeBuilder<TillSync> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.TillSyncCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_TillSyncCreated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.TillSyncUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_TillSyncUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Till).WithMany(c => c.SyncTills)
                .HasForeignKey(c => c.TillId).HasConstraintName("FK_Till_TillsSync").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Store).WithMany(c => c.OutletTillSync)
                .HasForeignKey(c => c.StoreId).HasConstraintName("FK_Store_TillsSync").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
