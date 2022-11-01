using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class AccountTransactionConfiguration : IEntityTypeConfiguration<AccountTransaction>
    {
        public void Configure(EntityTypeBuilder<AccountTransaction> builder)
        {
            builder?.HasOne(o => o.AccountTransactionCreatedBy).WithMany(c => c.AccountTransactionCreatedBy)
                 .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_AccountTransaction_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.AccountTransactionUpdatedBy).WithMany(c => c.AccountTransactionUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_AccountTransaction_Updated_By").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.Product).WithMany(c => c.AccountTransactionProduct)
               .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_AccountTransaction_Product_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.AccountTransactionStores)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Store_AccountTransaction_Outlet_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Till).WithMany(c => c.AccountTransactionTills)
               .HasForeignKey(c => c.TillId).HasConstraintName("FK_Till_AccountTransaction_Till_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Supplier).WithMany(c => c.AccountTransactionSuppliers)
               .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_Supplier_AccountTransaction_Supplier_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Manufacturer).WithMany(c => c.AccountTransactionManufacturers)
               .HasForeignKey(c => c.ManufacturerId).HasConstraintName("FK_MasterListItem_AccountTransaction_Manufacturer_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Department).WithMany(c => c.AccountTransactionDepartments)
               .HasForeignKey(c => c.DepartmentId).HasConstraintName("FK_Department_AccountTransaction_Department_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Commodity).WithMany(c => c.AccountTransactionCommodity)
               .HasForeignKey(c => c.CommodityId).HasConstraintName("FK_Commodity_AccountTransaction_Commodity_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Category).WithMany(c => c.AccountTransactionCategory)
               .HasForeignKey(c => c.CategoryId).HasConstraintName("FK_MasterListItem_AccountTransaction_Category_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.User).WithMany(c => c.AccountTransactionUsers)
               .HasForeignKey(c => c.UserId).HasConstraintName("FK_User_AccountTransaction_User_Id").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.PromotionBuy).WithMany(c => c.AccountTransactionPromoBuys)
               .HasForeignKey(c => c.PromoBuyId).HasConstraintName("FK_Promotion_AccountTransaction_Promotion_Buy").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.PromotionSell).WithMany(c => c.AccountTransactionPromoSells)
               .HasForeignKey(c => c.PromoSellId).HasConstraintName("FK_Promotion_AccountTransaction_Promotion_Sell").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
