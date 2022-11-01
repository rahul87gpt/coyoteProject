using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class CashierConfiguration : IEntityTypeConfiguration<Cashier>
    {
        public void Configure(EntityTypeBuilder<Cashier> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.CashierCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Cashier_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.CashierUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Cashier_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Type).WithMany(c => c.CashierTypeMasterListItem)
                .HasForeignKey(c => c.TypeId).HasConstraintName("FK_MasterListItem_CashierType").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.StoreGroup).WithMany(c => c.CashierStoreGroup)
                .HasForeignKey(c => c.StoreGroupId).HasConstraintName("FK_StoreGroup_CashierStoreGroup").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Outlet).WithMany(c => c.CashierStores)
                .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Store_CashierStore").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Zone).WithMany(c => c.CashierZoneMasterListItem)
                .HasForeignKey(c => c.ZoneId).HasConstraintName("FK_MasterListItem_CashierZone").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.AccessLevel).WithMany(c => c.CashierAccessLevelMasterListItem)
                .HasForeignKey(c => c.AccessLevelId).HasConstraintName("FK_MasterListItem_CashierAccesLevel").OnDelete(DeleteBehavior.NoAction);

            //builder?.HasIndex(p => new { p.Number }).IsUnique().HasName("IX_CashierNumber_Unique");

            builder?.HasData(new Cashier
            {
                Id = 1,
                Number = 1,
                FirstName = "cashier1",
                Surname = "cash sur",
                Email = "cashier@coyote.com",
                TypeId = 41,
                StoreGroupId = 1,
                OutletId = 1,
                ZoneId = 1,
                Status = true,
                Password = "123456654",
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });

        }
    }
}
