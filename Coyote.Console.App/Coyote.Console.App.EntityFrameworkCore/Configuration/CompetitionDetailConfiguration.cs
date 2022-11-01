using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class CompetitionDetailConfiguration : IEntityTypeConfiguration<CompetitionDetail>
    {
        public void Configure(EntityTypeBuilder<CompetitionDetail> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.CompetitionDetailsCreated)
               .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_CompetitionDetailsCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.CompetitionDetailsUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_CompetitionDetailsUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.CompetitionZone).WithMany(c => c.CompetitionDetailsZoneMasterListItem)
                .HasForeignKey(c => c.ZoneId).HasConstraintName("FK_CompetitionDetailsZone_MasterListItem").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.CompetitionType).WithMany(c => c.CompetitionDetailsTypeMasterLisItem)
                .HasForeignKey(c => c.TypeId).HasConstraintName("FK_CompetitionDetailsType_MasterListItem").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.CompetitionResetCycle).WithMany(c => c.CompetitionDetailsResetMasterListItem)
                .HasForeignKey(c => c.ResetCycleId).HasConstraintName("FK_CompetitionDetailsReset_MasterListItem").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.RewardType).WithMany(c => c.CompetitionRewardTypeMasterList)
                .HasForeignKey(c => c.RewardTypeId).HasConstraintName("FK_CompetitionDetailsRewardType_MasterListItem").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.TriggerType).WithMany(c => c.CompetitionDetailTriggerTypeMasterList)
                .HasForeignKey(c => c.TriggerTypeId).HasConstraintName("FK_CompetitionDetailsTriggerType_MasterListItem").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.Promotion).WithOne(c => c.CompetitionDetails)
                .HasForeignKey<CompetitionDetail>(c => c.PromotionId).HasConstraintName("FK_CompetitionDetails_Promotion").OnDelete(DeleteBehavior.NoAction);


            //builder?.HasIndex(p => new { p.Code}).IsUnique().HasName("IX_CompetitionDetail_Code_Unique");
            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_CompetitionDetail_Code_IsDeleted_Unique");

        }
    }
}
