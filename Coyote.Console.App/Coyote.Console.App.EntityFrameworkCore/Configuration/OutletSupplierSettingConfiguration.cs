using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class OutletSupplierSettingConfiguration : IEntityTypeConfiguration<OutletSupplierSetting>
    {
        public void Configure(EntityTypeBuilder<OutletSupplierSetting> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.OutletSupplierSettingCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_OutletSupplierSetting_Created_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.OutletSupplierSettingUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_OutletSupplierSetting_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(c => c.Store).WithMany(x => x.OutletSupplierSettingStroes)
                   .HasForeignKey(x => x.StoreId).HasConstraintName("FK_Store_OutletSupplierSetting").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(c => c.Supplier).WithMany(x => x.OutletSupplierSettings)
                .HasForeignKey(x => x.SupplierId).HasConstraintName("FK_Supplier_OutletSupplierSetting").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.StateMasterListItem).WithMany(c => c.OutletSupplierSettingStateList)
               .HasForeignKey(c => c.StateId).HasConstraintName("FK_MasterListItemState_OutletSupplierSetting").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.DivisionMasterListItem).WithMany(c => c.OutletSupplierSettingDivisionList)
              .HasForeignKey(c => c.DivisionId).HasConstraintName("FK_MasterListItemDivision_OutletSupplierSetting").OnDelete(DeleteBehavior.NoAction);


            /// builder?.HasIndex(p => new { p.SupplierId, p.StoreId }).IsUnique().HasName("IX_SupplierStore_Unique");

            builder?.HasData(new OutletSupplierSetting
            {
                Id = 1,
                StoreId = 1,
                SupplierId = 1,
                Status = true,
                Desc = "First OutletSupplierSetting",
                CustomerNumber = "123654",
                StateId = 1,
                DivisionId = 1,
                PhoneNumber = "1234567890",
                UserId = "5462Qt61",
                Password = "testPassword",
                QtyDefault = "1",
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });

        }
    }
}
