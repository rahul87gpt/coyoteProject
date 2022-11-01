using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionBuyingConfiguration : IEntityTypeConfiguration<PromotionBuying>
    {
        public void Configure(EntityTypeBuilder<PromotionBuying> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionBuyingCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionBuyingCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionBuyingUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionBuyingUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Promotion).WithMany(c => c.PromotionBuying)
                .HasForeignKey(c => c.PromotionId).HasConstraintName("FK_Promotion_PromotionBuying").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.PromotionBuying)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_PromotionBuying").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Supplier).WithMany(c => c.PromotionBuying)
                .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_Supplier_PromotionBuying").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new List<PromotionBuying>
            {
                new PromotionBuying
                {
                    Id = 1,
                    PromotionId = 3,
                    ProductId = 1,
                    CartonCost=12,
                    CartonQty=12,
                    SupplierId = 1,
                    Status=true,
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
