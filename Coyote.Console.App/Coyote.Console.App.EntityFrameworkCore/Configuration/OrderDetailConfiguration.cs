using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.OrderDetailCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Order_Detail_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.OrderDetailUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Order_Detail_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.OrderHeader).WithMany(c => c.OrderDetail)
              .HasForeignKey(c => c.OrderHeaderId).HasConstraintName("FK_Order_Header_Order_Detail").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Product).WithMany(c => c.OrderDetailProduct)
               .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_Order_Detail").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.SupplierProduct).WithMany(c => c.OrderDetailSupplierItem)
               .HasForeignKey(c => c.SupplierProductId).HasConstraintName("FK_SupplierProduct_Order_Detail").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.OrderType).WithMany(c => c.OrderDetailTypeMasterListItem)
                .HasForeignKey(c => c.OrderTypeId).HasConstraintName("FK_MasterListItem_OrderDetail_Type").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Supplier).WithMany(c => c.OrderDetailCheaperSupplier)
                .HasForeignKey(c => c.CheaperSupplierId).HasConstraintName("FK_Supplier_OrderDetail").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.OrderHeaderId, p.LineNo, p.CreatedAt }).IsUnique().HasName("IX_OrderDetail_OrderHeaderId_LineNo_Time_Unique");
            //builder?.HasIndex(p => new { p.OrderHeaderId, p.ProductId, p.CreatedAt }).IsUnique().HasName("IX_OrderDetail_OrderHeaderId_ProductId_Time_Unique");


        }
    }
}


