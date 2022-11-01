using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class CompetitionTriggerConfiguration : IEntityTypeConfiguration<CompetitionTrigger>
    {
        public void Configure(EntityTypeBuilder<CompetitionTrigger> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.CompetitionTriggerCreated)
             .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_CompetitionTriggerCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.CompetitionTriggerUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_CompetitionTriggerUpdated_By").OnDelete(DeleteBehavior.NoAction);

         
            builder?.HasOne(o => o.TriggerProductGroup).WithMany(c => c.CompetitionTriggerGroupMasterList)
                .HasForeignKey(c => c.TriggerProductGroupID).HasConstraintName("FK_CompetitionTriggerGroup_MasterListItem").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromoComp).WithOne(c => c.Triggers)
               .HasForeignKey<CompetitionTrigger>(c => c.CompPromoId).HasConstraintName("FK_PromotionCompetition_CompetitionTrigger").OnDelete(DeleteBehavior.NoAction);




        }
    }
}
