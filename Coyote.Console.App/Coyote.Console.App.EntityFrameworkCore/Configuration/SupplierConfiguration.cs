using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.SupplierCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Supplier_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.SupplierUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Supplier_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Supplier_Delete_Unique");

            //builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Supplier_Code");

            builder?.HasData(new List<Supplier>
            {
                new Supplier
                {
                    Id = 1,
                    Code = "Supp1",
                    Desc = "Supplier1",
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
