using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
   public class ZonePricingConfiguration : IEntityTypeConfiguration<ZonePricing>
    {
        public void Configure(EntityTypeBuilder<ZonePricing> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.ZonePricingCreatedBy)
                 .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_ZonePricing_Created_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.ZonePricingUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_ZonePricing_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.ProductZonePricing)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_PriceZone").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PriceZoneCostPrice).WithMany(c => c.ZonePricingCostZone)
                .HasForeignKey(c => c.PriceZoneId).HasConstraintName("FK_CostZone_PriceZone").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
