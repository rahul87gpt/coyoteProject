using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.ProductCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Product_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.ProductUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Product_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Department).WithMany(x => x.ProductDept)
                .HasForeignKey(x => x.DepartmentId).HasConstraintName("FK_Department_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Supplier).WithMany(x => x.ProductsSupplier)
                .HasForeignKey(x => x.SupplierId).HasConstraintName("FK_Supplier_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Commodity).WithMany(x => x.ProductsCommodity)
                .HasForeignKey(x => x.CommodityId).HasConstraintName("FK_Commodity_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Tax).WithMany(x => x.ProductsTax)
               .HasForeignKey(x => x.TaxId).HasConstraintName("FK_Tax_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.GroupMasterListItem).WithMany(x => x.ProductGroupMasterListItem)
                 .HasForeignKey(x => x.GroupId).HasConstraintName("FK_ProductGroup_MasterList_Items").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.CategoryMasterListItem).WithMany(x => x.ProductCategoryMasterListItem)
                    .HasForeignKey(x => x.CategoryId).HasConstraintName("FK_ProductCategory_MasterList_Items").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.ManufacturerMasterListItem).WithMany(x => x.ProductManufacturerMasterListItem)
                    .HasForeignKey(x => x.ManufacturerId).HasConstraintName("FK_ProductManufacturer_MasterList_Items").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.TypeMasterListItem).WithMany(x => x.ProductTypeMasterListItem)
                    .HasForeignKey(x => x.TypeId).HasConstraintName("FK_ProductType_MasterList_Items").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.NationalRangeMasterListItem).WithMany(x => x.ProductNationalRangeMasterListItem)
                    .HasForeignKey(x => x.NationalRangeId).HasConstraintName("FK_ProductNationalRange_MasterList_Items").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.UnitMeasureMasterListItem).WithMany(x => x.ProductUnitMeasureMasterListItem)
                    .HasForeignKey(x => x.UnitMeasureId).HasConstraintName("FK_ProductUnitMeasure_MasterList_Items").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Number }).IsUnique().HasName("IX_ProductNumber_Unique");

            builder?.HasData(new Product
            {
                Id = 1,
                Number = 1,
                Desc = "First prod1",
                SupplierId = 1,
                Status = true,
                CartonQty = 12,
                CartonCost = 12,
                DepartmentId = 1,
                CommodityId = 1,
                TaxId = 1,
                GroupId = 4,
                CategoryId = 2,
                ManufacturerId = 3,
                TypeId = 63,
                NationalRangeId = 5,
                UnitMeasureId = 6,
                AccessOutletIds = "1",
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });
        }
    }
}
