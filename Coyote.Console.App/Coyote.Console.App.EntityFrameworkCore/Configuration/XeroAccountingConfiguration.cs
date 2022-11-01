using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
   public class XeroAccountingConfiguration : IEntityTypeConfiguration<XeroAccount>
    {
        public void Configure(EntityTypeBuilder<XeroAccount> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.XeroAccountCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_XeroAccount_Created_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.XeroAccountUpdated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_XeroAccounting_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Store).WithMany(c => c.XeroAccountingStores)
               .HasForeignKey(c => c.StoreId).HasConstraintName("FK_XeroAccounting_Store").OnDelete(DeleteBehavior.NoAction);

            //builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_XeroAccountingCode_Unique");

            builder?.HasData(new XeroAccount
            {
                Id = 1,
                Desc = "First prod1",
                StoreId=1,
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });
        }
    }
}
