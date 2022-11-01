using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class StockAdjustDetailConfiguration : IEntityTypeConfiguration<StockAdjustDetail>
    {
        public void Configure(EntityTypeBuilder<StockAdjustDetail> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.StockAdjustDetailCreated)
               .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Stock_Adjust_Detail_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.StockAdjustDetailUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Stock_Adjust_Detail_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.StockAdjustHeader).WithMany(c => c.StockAdjustDetails)
              .HasForeignKey(c => c.StockAdjustHeaderId).HasConstraintName("FK_Stock_Adjust_Header_Stock_Adjust_Detail").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Product).WithMany(c => c.StockAdjustDetailProduct)
               .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_Stock_Adjust_Detail").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.OutletProduct).WithMany(c => c.StockAdjustDetailOutletProduct)
               .HasForeignKey(c => c.OutletProductId).HasConstraintName("FK_Outlet_Product_Stock_Adjust_Detail").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Reason).WithMany(c => c.StockAdjustDetailListItem)
                .HasForeignKey(c => c.ReasonId).HasConstraintName("FK_MasterListItem_StockAdjustDetail_Type").OnDelete(DeleteBehavior.NoAction); 

            //builder?.HasIndex(p => new { p.StockAdjustHeaderId, p.LineNo, p.CreatedAt }).IsUnique().HasName("IX_StockAdjustDetail_StockAdjustHeaderId_LineNo_Created_Unique");
           
            builder?.HasIndex(p => new { p.StockAdjustHeaderId, p.ProductId, p.LineNo, p.CreatedAt }).IsUnique().HasName("IX_StockAdjustDetail_StockAdjustHeaderId_ProductId_Created_Unique");
        }
    }
}
