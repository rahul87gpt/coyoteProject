using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class BulkOrderFromTabletConfiguration : IEntityTypeConfiguration<BulkOrderFromTablet>
    {
        public void Configure(EntityTypeBuilder<BulkOrderFromTablet> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.BulkOrderFromTabletCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_BulkOrderFromTablet_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.BulkOrderFromTabletUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_BulkOrderFromTablet_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.BulkOrderFromTabletOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_BulkOrderFromTablet_Outlet").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
