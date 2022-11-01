using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class ManualSaleItemConfiguration : IEntityTypeConfiguration<ManualSaleItem>
    {
        public void Configure(EntityTypeBuilder<ManualSaleItem> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.ManualSaleItemCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_ManualSaleItem_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.ManualSaleItemUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_ManualSaleItem_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.ManualSaleItemOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_ManualSaleItem_Outlet").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Product).WithMany(c => c.ManualSaleItemProduct)
            .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_ManualSaleItem_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.ManualSale).WithMany(c => c.ManualSaleItem)
           .HasForeignKey(c => c.ManualSaleId).HasConstraintName("FK_ManualSale_ManualSaleItem_ManualSale").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.TypeMasterListItem).WithMany(c => c.ManualSaleItemMasterListItems)
            .HasForeignKey(c => c.TypeId).HasConstraintName("FK_Type_ManualSaleItem_Type").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
