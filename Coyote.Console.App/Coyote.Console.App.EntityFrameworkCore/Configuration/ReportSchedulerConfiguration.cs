using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class ReportSchedulerConfiguration : IEntityTypeConfiguration<ReportScheduler>
    {
        public void Configure(EntityTypeBuilder<ReportScheduler> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.ReportSchedulerCreatedBy)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_ReportScheduler_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.ReportSchedulerUpdatedBy)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_ReportScheduler_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Report).WithMany(c => c.ReportSchedulerTypes)
                .HasForeignKey(c => c.ReportId).HasConstraintName("FK_MasterListItem_Report").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
