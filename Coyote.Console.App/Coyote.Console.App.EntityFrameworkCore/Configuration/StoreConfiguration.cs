using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.StoresCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Store_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.StoresUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Store_Updated_By").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.Warehouse).WithMany(c => c.StoreWarehouse)
               .HasForeignKey(c => c.WarehouseId).HasConstraintName("FK_Store_Warehouse_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PriceZone).WithMany(c => c.StorePriceZones)
               .HasForeignKey(c => c.PriceZoneId).HasConstraintName("FK_Store_Price_PriceZones_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.CostZone).WithMany(c => c.StoreCostZones)
               .HasForeignKey(c => c.CostZoneId).HasConstraintName("FK_CostZones_Cost_Store_Id")
               .OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.LabelTypeShelf).WithMany(c => c.StoreDefaultShelf)
            .HasForeignKey(c => c.LabelTypeShelfId).HasConstraintName("FK_Store_LabelTypeShelf_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.LabelTypeShort).WithMany(c => c.StoreDefaultShort)
            .HasForeignKey(c => c.LabelTypeShortId).HasConstraintName("FK_Store_LabelTypeShort_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.LabelTypePromo).WithMany(c => c.StoreDefaultPromo)
            .HasForeignKey(c => c.LabelTypePromoId).HasConstraintName("FK_Store_LabelTypePromo_Id").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.StoreGroups).WithMany(c => c.Stores)
                .HasForeignKey(c => c.GroupId).HasConstraintName("FK_Store_store_group_Store_Group_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.OutletPriceFromOutlet).WithMany(c => c.OutletPriceFromOutletStore)
             .HasForeignKey(c => c.OutletPriceFromOutletId).HasConstraintName("FK_Store_OutletPriceFromOutlet").OnDelete(DeleteBehavior.NoAction);


            builder?.HasIndex(p=> new { p.Code, p.IsDeleted}).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_StoreCode_Delete_Unique");

            builder?.HasData(new List<Store>
            {
                new Store
                {
                    Id = 1,
                    Code = "999999",
                    Desc = "Super Admin Store",
                    GroupId= 1,
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false
                }
            });
        }
    }
}
