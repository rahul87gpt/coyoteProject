using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class UserLogConfiguration : IEntityTypeConfiguration<UserLog>
    {
        public void Configure(EntityTypeBuilder<UserLog> builder)
        {
            builder?.HasOne(o => o.ActionById).WithMany(c => c.LogCreatedBy)
               .HasForeignKey(c => c.ActionBy).HasConstraintName("FK_User_UserLog_Created_By").OnDelete(DeleteBehavior.NoAction);
        }
    }
}