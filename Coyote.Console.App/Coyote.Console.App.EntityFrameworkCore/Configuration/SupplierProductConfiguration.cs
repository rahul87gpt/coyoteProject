using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class SupplierProductConfiguration : IEntityTypeConfiguration<SupplierProduct>
    {
        public void Configure(EntityTypeBuilder<SupplierProduct> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.SupplierProductCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_SupplierProduct_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.SupplierProductUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_SupplierProduct_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Supplier).WithMany(x => x.SupplierProductSupplier)
               .HasForeignKey(x => x.SupplierId).HasConstraintName("FK_Supplier_ProductSupplier").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(c => c.Product).WithMany(x => x.SupplierProduct)
              .HasForeignKey(x => x.ProductId).HasConstraintName("FK_Product_ProductSupplier").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.ProductId, p.SupplierId, p.SupplierItem }).IsUnique().HasName("IX_SupplierProduct_ProductId_SupplierId_SupplierItem_Unique");

            builder?.HasData(new List<SupplierProduct>
            {
                new SupplierProduct
                {
                    Id = 1,
                    SupplierId = 1,
                    ProductId = 1,
                    SupplierItem = "Test",
                    Status = true,
                    CartonCost = 10,
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
