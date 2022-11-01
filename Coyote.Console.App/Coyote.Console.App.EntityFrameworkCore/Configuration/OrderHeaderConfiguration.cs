using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class OrderHeaderConfiguration : IEntityTypeConfiguration<OrderHeader>
    {
        public void Configure(EntityTypeBuilder<OrderHeader> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.OrderHeaderCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Order_Header_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.OrderHeaderUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Order_Header_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.OrderHeaderOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_Order_Header_Outlet").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Supplier).WithMany(c => c.OrderHeaderSupplier)
               .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_Supplier_Order_Header_Supplier").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Type).WithMany(c => c.OrderTypeMasterListItem)
                .HasForeignKey(c => c.TypeId).HasConstraintName("FK_MasterListItem_OrderType").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Status).WithMany(c => c.OrderStatusMasterListItem)
                .HasForeignKey(c => c.StatusId).HasConstraintName("FK_MasterListItem_OrderStatus").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.CreationType).WithMany(c => c.OrderCreationTypeMasterListItem)
                .HasForeignKey(c => c.CreationTypeId).HasConstraintName("FK_MasterListItem_OrderCreationType").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.StoreAsSupplier).WithMany(c => c.OrderHeaderStoreAsSuppliers)
               .HasForeignKey(c => c.StoreIdAsSupplier).HasConstraintName("FK_Supplier_Order_Header_StoreIdAsSupplier").OnDelete(DeleteBehavior.NoAction);

            //    builder?.HasIndex(p => new { p.OrderNo, p.CreatedAt }).IsUnique().HasName("IX_OrderHeader_Outlet_OrderNo_Unique");

            //builder?.HasData(new Till
            //{
            //    Id = 1,
            //    Code = "8522",
            //    Desc = "REDCLIFFE TILL 2",
            //    Status = true,
            //    OutletId = 1,
            //    CreatedById = 1,
            //    UpdatedById = 1,
            //    CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
            //    UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
            //    IsDeleted = false,
            //});

        }
    }
}

