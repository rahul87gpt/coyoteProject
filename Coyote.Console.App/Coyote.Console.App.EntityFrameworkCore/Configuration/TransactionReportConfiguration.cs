using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class TransactionReportConfiguration : IEntityTypeConfiguration<TransactionReport>
    {
        public void Configure(EntityTypeBuilder<TransactionReport> builder)
        {

            builder?.HasOne(o => o.TransactionReportCreatedBy).WithMany(c => c.TransactionReportCreatedBy)
               .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_TransactionReport_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.TransactionReportUpdatedBy).WithMany(c => c.TransactionReportUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_TransactionReport_Updated_By").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.Product).WithMany(c => c.TransactionReportProduct)
               .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_TransactionReport_Product_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.TransactionReportStores)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Store_TransactionReport_Outlet_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Till).WithMany(c => c.TransactionReportTills)
               .HasForeignKey(c => c.TillId).HasConstraintName("FK_Till_TransactionReport_Till_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Supplier).WithMany(c => c.TransactionReportSuppliers)
               .HasForeignKey(c => c.SupplierId).HasConstraintName("FK_Supplier_TransactionReport_Supplier_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Manufacturer).WithMany(c => c.TransactionReportManufacturers)
               .HasForeignKey(c => c.ManufacturerId).HasConstraintName("FK_MasterListItem_TransactionReport_Manufacturer_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Department).WithMany(c => c.TransactionReportDepartments)
               .HasForeignKey(c => c.DepartmentId).HasConstraintName("FK_Department_TransactionReport_Department_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Commodity).WithMany(c => c.TransactionReportCommodity)
               .HasForeignKey(c => c.CommodityId).HasConstraintName("FK_Commodity_TransactionReport_Commodity_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Category).WithMany(c => c.TransactionReportCategory)
               .HasForeignKey(c => c.CategoryId).HasConstraintName("FK_MasterListItem_TransactionReport_Category_Id").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.User).WithMany(c => c.TransactionReportUsers)
               .HasForeignKey(c => c.UserId).HasConstraintName("FK_User_TransactionReport_User_Id").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(o => o.PromotionBuy).WithMany(c => c.TransactionReportPromoBuys)
               .HasForeignKey(c => c.PromoBuyId).HasConstraintName("FK_Promotion_TransactionReport_Promotion_Buy").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.PromotionSell).WithMany(c => c.TransactionReportPromoSells)
               .HasForeignKey(c => c.PromoSellId).HasConstraintName("FK_Promotion_TransactionReport_Promotion_Sell").OnDelete(DeleteBehavior.NoAction);


        }
    }
}
