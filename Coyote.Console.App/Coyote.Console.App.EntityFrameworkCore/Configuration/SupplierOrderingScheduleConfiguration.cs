using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Coyote.Console.App.Models.EntityContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class SupplierOrderingScheduleConfiguration : IEntityTypeConfiguration<SupplierOrderingSchedule>
    {
        public void Configure(EntityTypeBuilder<SupplierOrderingSchedule> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.SupplierOrderingCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_SupplierOrdering_Created_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.SupplierOrderingUpdatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_SupplierOrdering_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Store).WithMany(c => c.SupplierOrderingScheduleStore)
               .HasForeignKey(c => c.StoreId).HasConstraintName("FK_SupplierOrder_Store").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Supplier).WithMany(c => c.SupplierOrderingScheduleSupplier)
              .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_SupplierOrder_Supplier").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new SupplierOrderingSchedule
            {
                Id = 1,
                StoreId = 1,
                CreatedById = 1,
                UpdatedById = 1,
                CoverDays=0,
                SupplierId=1,
                CoverDaysDiscountThreshold1=1,
                CoverDaysDiscountThreshold2=2,
                CoverDaysDiscountThreshold3=1,
                DiscountThresholdOne=1,
                DiscountThresholdTwo=1,
                DiscountThresholdThree=1,
                DOWGenerateOrder=1,
                InvoiceOrderOffset=0,
                MultipleOrdersInAWeek = true,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });
        }
    }
}
