using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionOfferProductConfiguration : IEntityTypeConfiguration<PromotionOfferProduct>
    {
        public void Configure(EntityTypeBuilder<PromotionOfferProduct> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionOfferProductCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionOfferProductCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionOfferProductUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionOfferProductUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionOffer).WithMany(c => c.PromotionOfferProduct)
                .HasForeignKey(c => c.PromotionOfferId).HasConstraintName("FK_PromotionOffer_PromotionOfferProduct").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.PromotionOfferProduct)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_PromotionOfferProduct").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new List<PromotionOfferProduct>
            {
                new PromotionOfferProduct
                {
                    Id = 1,
                    PromotionOfferId=1,
                    ProductId = 1,
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
