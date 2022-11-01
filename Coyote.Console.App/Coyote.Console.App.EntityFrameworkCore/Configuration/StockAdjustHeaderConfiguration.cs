using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class StockAdjustHeaderConfiguration : IEntityTypeConfiguration<StockAdjustHeader>
    {
        public void Configure(EntityTypeBuilder<StockAdjustHeader> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.StockAdjustHeaderCreated)
               .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Stock_Adjust_Header_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.StockAdjustHeaderUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Stock_Adjust_Header_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.StockAdjustHeaderOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_Stock_Adjust_Header_Outlet").OnDelete(DeleteBehavior.NoAction); 
 
             

        }
    }
}
