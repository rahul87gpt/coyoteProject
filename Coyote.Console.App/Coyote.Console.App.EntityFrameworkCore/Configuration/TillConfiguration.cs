using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class TillConfiguration : IEntityTypeConfiguration<Till>
    {
        public void Configure(EntityTypeBuilder<Till> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.TillCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Till_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.TillUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Till_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.TillOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Till_Outlet").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Keypad).WithMany(c => c.TillKeypad)
               .HasForeignKey(c => c.KeypadId).HasConstraintName("FK_Till_Keypad").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Type).WithMany(c => c.TillTypeMasterListItem)
                .HasForeignKey(c => c.TypeId).HasConstraintName("FK_MasterListItem_TillType").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Till_Delete_Unique");

         //   builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Till_Code_Unique");
        }
    }
}
