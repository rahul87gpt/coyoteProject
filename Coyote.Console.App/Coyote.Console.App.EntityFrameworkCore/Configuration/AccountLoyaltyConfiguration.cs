using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class AccountLoyaltyConfiguration : IEntityTypeConfiguration<AccountLoyalty>
    {
        public void Configure(EntityTypeBuilder<AccountLoyalty> builder)
        {
            builder?.HasOne(o => o.AccountLoyaltyCreatedBy).WithMany(c => c.AccountLoyaltyCreatedBy)
                 .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_AccountLoyalty_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.AccountLoyaltyUpdatedBy).WithMany(c => c.AccountLoyaltyUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_AccountLoyalty_Updated_By").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
