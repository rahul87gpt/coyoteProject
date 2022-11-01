using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class OutletProductConfiguration : IEntityTypeConfiguration<OutletProduct>
    {
        public void Configure(EntityTypeBuilder<OutletProduct> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.OutletProductCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_OutletProduct_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.OutletProductUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_OutletProduct_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Store).WithMany(x => x.OutletProducts)
                   .HasForeignKey(x => x.StoreId).HasConstraintName("FK_Store_OutletProduct").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Product).WithMany(x => x.OutletProductProduct)
                  .HasForeignKey(x => x.ProductId).HasConstraintName("FK_Product_OutletProduct").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Supplier).WithMany(x => x.OutletProductSupplier)
                .HasForeignKey(x => x.SupplierId).HasConstraintName("FK_Supplier_OutletProduct").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(c => c.PromotionMixMatch1).WithMany(x => x.OutletProductPromotionMixMatch1)
                .HasForeignKey(x => x.PromoMixMatch1Id).HasConstraintName("FK_Promo_MixMatch_1_OutletProduct").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.PromotionMixMatch2).WithMany(x => x.OutletProductPromotionMixMatch2)
                .HasForeignKey(x => x.PromoMixMatch2Id).HasConstraintName("FK_Promo_MixMatch_2_OutletProduct").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(c => c.PromotionOffer1).WithMany(x => x.OutletProductPromotionOffer1)
                .HasForeignKey(x => x.PromoOffer1Id).HasConstraintName("FK_Promo_Offer_1_OutletProduct").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.PromotionOffer2).WithMany(x => x.OutletProductPromotionOffer2)
                .HasForeignKey(x => x.PromoOffer2Id).HasConstraintName("FK_Promo_Offer_2_OutletProduct").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.PromotionOffer3).WithMany(x => x.OutletProductPromotionOffer3)
                .HasForeignKey(x => x.PromoOffer3Id).HasConstraintName("FK_Promo_Offer_3_OutletProduct").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.PromotionOffer4).WithMany(x => x.OutletProductPromotionOffer4)
                .HasForeignKey(x => x.PromoOffer4Id).HasConstraintName("FK_Promo_Offer_4_OutletProduct").OnDelete(DeleteBehavior.NoAction);


            builder?.HasOne(c => c.PromotionMember).WithMany(x => x.OutletProductPromotionMember)
                .HasForeignKey(x => x.PromoMemeberOfferId).HasConstraintName("FK_Promo_MemberOffer_OutletProduct").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(c => c.PromotionBuy).WithMany(x => x.OutletProductPromotionBuy)
                .HasForeignKey(x => x.PromoBuyId).HasConstraintName("FK_Promo_Buy_OutletProduct").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.PromotionSell).WithMany(x => x.OutletProductPromotionSell)
                .HasForeignKey(x => x.PromoSellId).HasConstraintName("FK_Promo_Sell_OutletProduct").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(c => c.PromotionComp).WithMany(x => x.OutletProductPromotionCompetition)
               .HasForeignKey(x => x.PromoCompId).HasConstraintName("FK_Promo_Comp_OutletProduct").OnDelete(DeleteBehavior.NoAction);


            builder?.HasIndex(p => new { p.ProductId, p.StoreId }).IsUnique().HasName("IX_ProductStore_Unique");


            builder?.HasData(new OutletProduct
            {
                Id = 1,
                StoreId = 1,
                ProductId = 1,
                SupplierId = 1,
                Status = true,
                NormalPrice1 = 1,
                QtyOnHand = 0,
                MinOnHand = 0,
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });

        }
    }
}
