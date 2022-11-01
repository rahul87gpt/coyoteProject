using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
   public  class OutletBudgetConfiguration : IEntityTypeConfiguration<OutletBudget>
    {
        public void Configure(EntityTypeBuilder<OutletBudget> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.OutletBudgetCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_OutletBudget_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.OutletBudgetUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_OutletBudget_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Store).WithMany(c => c.OutletBudgetTarget)
                .HasForeignKey(c => c.StoreId).HasConstraintName("FK_OutletBudget_Store_Id").OnDelete(DeleteBehavior.NoAction);
        }
    }
}