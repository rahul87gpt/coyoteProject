using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionMemberOfferConfiguration : IEntityTypeConfiguration<PromotionMemberOffer>
    {
        public void Configure(EntityTypeBuilder<PromotionMemberOffer> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionMemberOfferCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionMemberOfferCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionMemberOfferUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionMemberOfferUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Promotion).WithMany(c => c.PromotionMemberOffer)
                .HasForeignKey(c => c.PromotionId).HasConstraintName("FK_Promotion_PromotionMemberOffer").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.PromotionMemberOffer)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_PromotionMemberOffer").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new List<PromotionMemberOffer>
            {
                new PromotionMemberOffer
                {
                    Id = 1,
                    PromotionId = 6,
                    ProductId = 1,
                    Price = 1,
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
