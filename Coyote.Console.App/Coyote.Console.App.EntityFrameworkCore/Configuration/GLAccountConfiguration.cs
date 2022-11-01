using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Coyote.Console.App.Models;
using Coyote.Console.App.EntityFrameworkCore.Constants;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
   public class GLAccountConfiguration : IEntityTypeConfiguration<GLAccount>
    {
        public void Configure(EntityTypeBuilder<GLAccount> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.GLAccountCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_GLAccount_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.GLAccountUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_GLAccount_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.TypeMasterListItem).WithMany(c => c.AccountTypeMasterListItem)
                .HasForeignKey(c => c.TypeId).HasConstraintName("FK_MasterListItem_AccountType").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Store).WithMany(c => c.GLAccountStores)
                .HasForeignKey(c => c.StoreId).HasConstraintName("FK_Store_GLAccountStore").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Supplier).WithMany(c => c.GLAccountSupplier)
                .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_Supplier_GLAccountSupplier").OnDelete(DeleteBehavior.NoAction);

            builder?.HasData(new GLAccount
            {
                Id = 1,
                TypeId = 41,
                SupplierId = 1,
                StoreId = 1,
                AccountSystem="XERO",
                Company=1,
                AccountNumber="123",
                Desc= "GLAccount",
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });

        }
    }
}
