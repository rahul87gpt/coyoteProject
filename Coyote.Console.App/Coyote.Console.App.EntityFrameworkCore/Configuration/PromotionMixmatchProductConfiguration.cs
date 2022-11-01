using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionMixmatchProductConfiguration : IEntityTypeConfiguration<PromotionMixmatchProduct>
    {
        public void Configure(EntityTypeBuilder<PromotionMixmatchProduct> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionMixmatchProductCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionMixmatchProductCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionMixmatchProductUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionMixmatchProductUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionMixmatch).WithMany(c => c.PromotionMixmatchProduct)
                .HasForeignKey(c => c.PromotionMixmatchId).HasConstraintName("FK_PromotionMixmatch_PromotionMixmatchProduct").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.PromotionMixmatchProduct)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_PromotionMixmatchProduct").OnDelete(DeleteBehavior.NoAction);


            builder?.HasData(new List<PromotionMixmatchProduct>
            {
                new PromotionMixmatchProduct
                {
                    Id = 1,
                    PromotionMixmatchId=1,
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
