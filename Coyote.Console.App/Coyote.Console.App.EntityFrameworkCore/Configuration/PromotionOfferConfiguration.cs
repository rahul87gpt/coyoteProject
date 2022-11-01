using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionOfferConfiguration : IEntityTypeConfiguration<PromotionOffer>
    {
        public void Configure(EntityTypeBuilder<PromotionOffer> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionOfferCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionOfferCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionOfferUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionOfferUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Promotion).WithMany(c => c.PromotionOffer)
                .HasForeignKey(c => c.PromotionId).HasConstraintName("FK_Promotion_PromotionOffer").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new List<PromotionOffer>
            {
                new PromotionOffer
                {
                    Id = 1,
                    PromotionId = 1,
                    Group = 1,
                    Group1Price="Free",
                    Group1Qty =1,
                    Group2Price = "1.0",
                    Group2Qty =2,
                    TotalPrice = 20,
                    TotalQty = 3,
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
