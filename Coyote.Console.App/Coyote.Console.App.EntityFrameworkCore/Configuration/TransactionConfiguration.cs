using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder?.HasOne(o => o.TransactionCreatedBy).WithMany(c => c.TransactionCreatedBy)
                 .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Transaction_Created_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.TransactionUpdatedBy).WithMany(c => c.TransactionUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Transaction_Updated_By").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.Product).WithMany(c => c.TransactionProduct)
               .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_Transaction_Product_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Store).WithMany(c => c.TransactionStores)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Store_Transaction_Store_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Till).WithMany(c => c.TransactionTills)
               .HasForeignKey(c => c.TillId).HasConstraintName("FK_Till_Transaction_Till_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Supplier).WithMany(c => c.TransactionSuppliers)
               .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_Supplier_Transaction_Supplier_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Manufacturer).WithMany(c => c.TransactionManufacturers)
               .HasForeignKey(c => c.ManufacturerId).HasConstraintName("FK_MasterListItem_Transaction_Manufacturer_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Department).WithMany(c => c.TransactionDepartments)
               .HasForeignKey(c => c.DepartmentId).HasConstraintName("FK_Department_Transaction_Department_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Commodity).WithMany(c => c.TransactionCommodity)
               .HasForeignKey(c => c.CommodityId).HasConstraintName("FK_Commodity_Transaction_Commodity_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Category).WithMany(c => c.TransactionCategory)
               .HasForeignKey(c => c.CategoryId).HasConstraintName("FK_MasterListItem_Transaction_Category_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.User).WithMany(c => c.TransactionUsers)
               .HasForeignKey(c => c.UserId).HasConstraintName("FK_User_Transaction_User_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.GroupCode).WithMany(c => c.TransactionGroup)
               .HasForeignKey(c => c.Group).HasConstraintName("FK_MasterListItem_Transaction_Group_Id").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionBuy).WithMany(c => c.TransactionPromoBuys)
               .HasForeignKey(c => c.PromoBuyId).HasConstraintName("FK_Promotion_Transaction_Promotion_Buy").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionSell).WithMany(c => c.TransactionPromoSells)
               .HasForeignKey(c => c.PromoSellId).HasConstraintName("FK_Promotion_Transaction_Promotion_Sell").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
