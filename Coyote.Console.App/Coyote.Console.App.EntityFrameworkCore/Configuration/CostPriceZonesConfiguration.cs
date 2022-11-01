using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class CostPriceZonesConfiguration : IEntityTypeConfiguration<CostPriceZones>
    {
        public void Configure(EntityTypeBuilder<CostPriceZones> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.CostPriceZonesCreated)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_CostPriceZones_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.CostPriceZonesUpdated)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_CostPriceZones_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.HostSettings).WithMany(c => c.CostPriceZonesHostSettings)
               .HasForeignKey(c => c.HostSettingID).HasConstraintName("FK_CostPriceZones_HostSettings").OnDelete(DeleteBehavior.NoAction);         
        }
    }
}
