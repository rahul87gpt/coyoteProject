using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PromotionCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PromotionCreated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PromotionUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PromotionUpdated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionType).WithMany(c => c.PromotionTypeMasterListItem)
                .HasForeignKey(c => c.PromotionTypeId).HasConstraintName("FK_PromotionType_MasterListItem").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionSource).WithMany(c => c.PromotionSourceMasterListItem)
                .HasForeignKey(c => c.SourceId).HasConstraintName("FK_PromotionSource_MasterListItem").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionZone).WithMany(c => c.PromotionZoneMasterListItem)
                .HasForeignKey(c => c.ZoneId).HasConstraintName("FK_PromotionZone_MasterListItem").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionFrequency).WithMany(c => c.PromotionFrequencyMasterListItem)
                .HasForeignKey(c => c.FrequencyId).HasConstraintName("FK_PromotionFrequency_MasterListItem").OnDelete(DeleteBehavior.NoAction);

            // builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Promotion_Code_Unique");
            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Promotion_Delete_Unique");



            builder?.HasData(new List<Promotion>
            {
                new Promotion
                {
                    Id = 1,
                    Code = "Mixmatch1",
                    Desc = "Mix 1",
                    Availibility = "YYYYYYY",
                    End=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(30),
                    Start=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(-30),
                    FrequencyId = 90,
                    PromotionTypeId=34,
                    SourceId = 34, // Not sure
                    Status=true,
                    ZoneId = 1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                new Promotion
                {
                    Id = 2,
                    Code = "Offer",
                    Desc = "off 1",
                    Availibility = "YYYYYYY",
                    End=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(30),
                    Start=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(-30),
                    FrequencyId = 90,
                    PromotionTypeId=29,
                    SourceId = 29, // Not sure
                    Status=true,
                    ZoneId = 1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                new Promotion
                {
                    Id = 3,
                    Code = "BUYING",
                    Desc = "BUYING 1",
                    Availibility = "YYYYYYY",
                    End=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(30),
                    Start=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(-30),
                    FrequencyId = 90,
                    PromotionTypeId=30,
                    SourceId = 30, // Not sure
                    Status=true,
                    ZoneId = 1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                new Promotion
                {
                    Id = 4,
                    Code = "SELLING",
                    Desc = "SELLING 1",
                    Availibility = "YYYYYYY",
                    End=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(30),
                    Start=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(-30),
                    FrequencyId = 90,
                    PromotionTypeId=31,
                    SourceId = 31, // Not sure
                    Status=true,
                    ZoneId = 1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                new Promotion
                {
                    Id = 5,
                    Code = "COMPITITION",
                    Desc = "COMPITITION 1",
                    Availibility = "YYYYYYY",
                    End=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(30),
                    Start=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(-30),
                    FrequencyId = 90,
                    PromotionTypeId=32,
                    SourceId = 32, // Not sure
                    Status=true,
                    ZoneId = 1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                },
                new Promotion
                {
                    Id = 6,
                    Code = "MEMBEROFFER",
                    Desc = "MEMBEROFFER 1",
                    Availibility = "YYYYYYY",
                    End=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(30),
                    Start=new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay).AddDays(-30),
                    FrequencyId = 90,
                    PromotionTypeId=33,
                    SourceId = 33, // Not sure
                    Status=true,
                    ZoneId = 1,
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
