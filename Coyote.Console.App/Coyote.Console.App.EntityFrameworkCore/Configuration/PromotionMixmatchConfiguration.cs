using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionMixmatchConfiguration : IEntityTypeConfiguration<PromotionMixmatch>
    {
        public void Configure(EntityTypeBuilder<PromotionMixmatch> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionMixmatchCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionMixmatchCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionMixmatchUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionMixmatchUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Promotion).WithMany(c => c.PromotionMixmatch)
                .HasForeignKey(c => c.PromotionId).HasConstraintName("FK_Promotion_PromotionMixmatch").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new List<PromotionMixmatch>
            {
                new PromotionMixmatch
                {
                    Id = 1,
                    PromotionId = 1,
                    Status=true,
                    Amt1=1,
                    Amt2=1,
                    CumulativeOffer=false,
                    DiscPcnt1=10,
                    DiscPcnt2=20,
                    Qty1=2,
                    Qty2=2,
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
