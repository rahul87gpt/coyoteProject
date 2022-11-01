using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class JournalDetailConfiguration : IEntityTypeConfiguration<JournalDetail>
    {
        public void Configure(EntityTypeBuilder<JournalDetail> builder)
        {
            builder?.HasOne(o => o.JournalDetailCreatedBy).WithMany(c => c.JournalDetailsCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Journal_Detail_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.JournalDetailUpdatedBy).WithMany(c => c.JournalDetailsUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Journal_Detail_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.JournalHeader).WithMany(c => c.JournalDetails)
               .HasForeignKey(c => c.JournalHeaderId).HasConstraintName("FK_Journal_Header_Journal_Detail_Journal_Header").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.JournalDetailsProduct)
               .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Product_Journal_Detail_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.APN).WithMany(c => c.APNJournalDetail)
               .HasForeignKey(c => c.APNSold).HasConstraintName("FK_APN_Journal_Detail_APN").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Cashier).WithMany(c => c.JournalDetailsCahier)
               .HasForeignKey(c => c.CashierId).HasConstraintName("FK_Cashier_Journal_Detail_Cashier").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionSell).WithMany(c => c.JournalDetailSellPromo)
               .HasForeignKey(c => c.PromoSellId).HasConstraintName("FK_Promotion_Journal_Detail_Promotion_Sell").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionMixMatch).WithMany(c => c.JournalDetailMixMatchPromo)
               .HasForeignKey(c => c.PromoMixMatchId).HasConstraintName("FK_Promotion_Journal_Detail_Promotion_MixMatch").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionOffer).WithMany(c => c.JournalDetailOfferPromo)
               .HasForeignKey(c => c.PromoOfferId).HasConstraintName("FK_Promotion_Journal_Detail_Promotion_Offer").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionCompetition).WithMany(c => c.JournalDetailCompPromo)
               .HasForeignKey(c => c.PromoCompId).HasConstraintName("FK_Promotion_Journal_Detail_Promotion_comp").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.PromotionMemberOffer).WithMany(c => c.JournalDetailMemberOfferPromo)
               .HasForeignKey(c => c.PromoMemeberOfferId).HasConstraintName("FK_Promotion_Journal_Detail_Promotion_MemberOffer").OnDelete(DeleteBehavior.NoAction);

        }
    }
}
