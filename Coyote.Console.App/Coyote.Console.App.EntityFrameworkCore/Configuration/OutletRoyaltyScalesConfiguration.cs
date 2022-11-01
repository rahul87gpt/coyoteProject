using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
   public class OutletRoyaltyScalesConfiguration : IEntityTypeConfiguration<OutletRoyaltyScales>
    {
        public void Configure(EntityTypeBuilder<OutletRoyaltyScales> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.OutletRoyaltyScalesCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_OutletRoyaltyScales_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.OutletRoyaltyScalesUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_OutletRoyaltyScales_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.OutletRoyaltyScalesOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_OutletRoyaltyScales_Outlet").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
