using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class StockTakeHeaderConfiguration : IEntityTypeConfiguration<StockTakeHeader>
    {
        public void Configure(EntityTypeBuilder<StockTakeHeader> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.StockTakeHeaderCreated)
               .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Stock_Take_Header_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.StockTakeHeaderUpdated)
                            .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Stock_Take_Header_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.StockTakeHeaderOutlet)
                           .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_Stock_Take_Header_Outlet").OnDelete(DeleteBehavior.NoAction);
        }
    }
}