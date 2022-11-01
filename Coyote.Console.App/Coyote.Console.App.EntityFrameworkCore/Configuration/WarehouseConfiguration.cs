using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.WarehouseCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Warehouse_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.WarehouseUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Warehouse_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Supplier).WithMany(x => x.WarehousesSupplier)
                .HasForeignKey(x => x.SupplierId).HasConstraintName("FK_Supplier_Warehouse").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.HostFormatMasterListItem).WithMany(x => x.WarehousMasterListItem)
                 .HasForeignKey(x => x.HostFormatId).HasConstraintName("FK_Warehouse_MasterList_Items").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_WarehouseCode_Delete_Unique");

         //   builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_WarehouseCode_Unique");

            builder?.HasData(new List<Warehouse>
            {
                new Warehouse
                {
                    Id = 1,
                    SupplierId = 1,
                    HostFormatId = 7,
                    Code = "WAREHOUSE1",
                    Desc = "WAREHOUSE1",
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
