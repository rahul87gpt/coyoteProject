using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class POSMessageConfiguration : IEntityTypeConfiguration<POSMessages>
    {
        public void Configure(EntityTypeBuilder<POSMessages> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.POSMessageCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_POSMsg_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.POSMessageUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_POSMsg_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Zone).WithMany(c => c.POSMessagesZone)
               .HasForeignKey(c => c.ZoneId).HasConstraintName("FK_Zone_POSMsg_ZoneId").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new POSMessages
            {
                Id = 1,
                DayParts = "YYYYYYYYYYYYYYYYYYYYYYY",
                DisplayType = "1",
                ZoneId=1,
                POSMessage = "Message for POS",
                Priority = 1,
                ReferenceId = "Prod_1",
                ReferenceType = "Product",
                DateFrom = DateTime.UtcNow.AddDays(-10),
                DateTo = DateTime.UtcNow.AddDays(5),
                Desc = "Prod/Promo Desc",
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            }); ;
        }
    }
}

