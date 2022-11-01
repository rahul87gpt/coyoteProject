using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionCompetitionConfiguration : IEntityTypeConfiguration<PromotionCompetition>
    {
        public void Configure(EntityTypeBuilder<PromotionCompetition> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionCompetitionCreated)
               .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionCompetitionCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionCompetitionUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionCompetitionUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.CompetitionDetail).WithMany(c => c.PromotionCompetitions)
             .HasForeignKey(c => c.CompetitionId).HasConstraintName("FK_CompetitionDetail_PromotionCompetition").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.PromotionCompetitionProduct)
               .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_PromotionCompetition").OnDelete(DeleteBehavior.NoAction);



            //builder?.HasOne(o => o.Triggers).WithOne(c => c.PromoComp)
            //    .HasForeignKey<PromotionCompetition>(c => c.Id).HasConstraintName("FK_PromotionCompetition_CompetitionTrigger").OnDelete(DeleteBehavior.NoAction);

            //builder?.HasOne(o => o.Rewards).WithOne(c => c.PromoComp)
            //    .HasForeignKey<PromotionCompetition>(c => c.Id).HasConstraintName("FK_PromotionCompetition_CompetitionReward").OnDelete(DeleteBehavior.NoAction);

        }
    }
}
