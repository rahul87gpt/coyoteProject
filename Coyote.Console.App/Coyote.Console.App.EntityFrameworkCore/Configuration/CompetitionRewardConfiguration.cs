using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class CompetitionRewardConfiguration : IEntityTypeConfiguration<CompetitionReward>
    {
        public void Configure(EntityTypeBuilder<CompetitionReward> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.CompetitionRewardCreated)
            .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_CompetitionRewardCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.CompetitionRewardUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_CompetitionRewardUpdated_By").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.PromoComp).WithOne(c => c.Rewards)
                .HasForeignKey<CompetitionReward>(c => c.CompPromoId).HasConstraintName("FK_PromotionCompetition_CompetitionReward").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
