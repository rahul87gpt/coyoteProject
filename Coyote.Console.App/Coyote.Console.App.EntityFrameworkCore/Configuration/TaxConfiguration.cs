using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class TaxConfiguration : IEntityTypeConfiguration<Tax>
    {
        public void Configure(EntityTypeBuilder<Tax> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.TaxCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Tax_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.TaxUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Tax_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Tax_Delete_Unique");

           // builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Tax_Code_Unique");

            builder?.HasData(new List<Tax>
            {
                new Tax
                {
                    Id = 1,
                    Factor = 1,
                    Code = "codTax1",
                    Desc = "codTax1",
                    Status = true,
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
