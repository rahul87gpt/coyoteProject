using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class StockTakeDetailConfiguration : IEntityTypeConfiguration<StockTakeDetail>
    {
        public void Configure(EntityTypeBuilder<StockTakeDetail> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.StockTakeDetailCreated)
                   .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Stock_Take_Detail_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.StockTakeDetailUpdated)
                    .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Stock_Take_Detail_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.StockTakeHeader).WithMany(c => c.StockTakeDetails)
                  .HasForeignKey(c => c.StockTakeHeaderId).HasConstraintName("FK_Stock_Take_Header_Stock_Take_Detail").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.OutletProduct).WithMany(c => c.StockTakeDetailOutletProduct)
                   .HasForeignKey(c => c.OutletProductId).HasConstraintName("FK_Outlet_Product_Stock_Take_Detail").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
