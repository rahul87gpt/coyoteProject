using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class OrderAuditConfiguration : IEntityTypeConfiguration<OrderAudit>
    {
        public void Configure(EntityTypeBuilder<OrderAudit> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.OrderAuditCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Order_Audit_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.OrderAuditUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Order_Audit_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.OrderAuditOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_Order_Audit_Outlet").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Supplier).WithMany(c => c.OrderAuditSupplier)
               .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_Supplier_Order_Audit_Supplier").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Type).WithMany(c => c.OrderAuditTypeMasterListItem)
                .HasForeignKey(c => c.TypeId).HasConstraintName("FK_MasterListItem_Order_Audit_Type").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Status).WithMany(c => c.OrderAuditStatusMasterListItem)
                .HasForeignKey(c => c.StatusId).HasConstraintName("FK_MasterListItem_Order_Audit_Status").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.NewStatus).WithMany(c => c.OrderAuditNewStatusMasterListItem)
               .HasForeignKey(c => c.NewStatusId).HasConstraintName("FK_MasterListItem_Order_Audit_NewStatus").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
