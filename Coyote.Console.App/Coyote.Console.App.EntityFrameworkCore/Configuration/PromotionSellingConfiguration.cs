using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionSellingConfiguration : IEntityTypeConfiguration<PromotionSelling>
    {
        public void Configure(EntityTypeBuilder<PromotionSelling> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionSellingCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionSellingCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionSellingUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionSellingUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Promotion).WithMany(c => c.PromotionSelling)
                .HasForeignKey(c => c.PromotionId).HasConstraintName("FK_Promotion_PromotionSelling").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.PromotionSellingchProduct)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_PromotionSelling").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new List<PromotionSelling>
            {
                new PromotionSelling
                {
                    Id = 1,
                    PromotionId = 4,
                    ProductId = 1,
                    Price = 1,
                    Status = true,
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

