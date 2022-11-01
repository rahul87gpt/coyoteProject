using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class ReportSchedulerLogConfiguration : IEntityTypeConfiguration<ReportSchedulerLog>
    {
        public void Configure(EntityTypeBuilder<ReportSchedulerLog> builder)
        {

            builder?.HasOne(o => o.UserScheduled).WithMany(c => c.SchedulerUserLog)
                .HasForeignKey(c => c.UserId).HasConstraintName("FK_SchedulerUserLog_User").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Scheduler).WithMany(c => c.ReportSchedulerLog)
               .HasForeignKey(c => c.SchedulerId).HasConstraintName("FK_SchedulerLog_Scheduler").OnDelete(DeleteBehavior.NoAction);
        }
    }
}

