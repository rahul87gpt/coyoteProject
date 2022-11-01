using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
  public class SchedulerUserConfiguration : IEntityTypeConfiguration<SchedulerUser>
    {
        public void Configure(EntityTypeBuilder<SchedulerUser> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.SchedulerUserCreatedBy)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_SchedulerUser_Created_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.SchedulerUserUpdatedBy)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_SchedulerUser_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.UserScheduled).WithMany(c => c.SchedulerUser)
                .HasForeignKey(c => c.UserId).HasConstraintName("FK_SchedulerUser_User").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Scheduler).WithMany(c => c.SchedulerUser)
               .HasForeignKey(c => c.SchedulerId).HasConstraintName("FK_SchedulerUser_Scheduler").OnDelete(DeleteBehavior.NoAction);
        }
    }
}

