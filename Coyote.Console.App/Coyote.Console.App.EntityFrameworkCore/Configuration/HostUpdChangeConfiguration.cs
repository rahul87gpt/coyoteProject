using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class HostUpdChangeConfiguration : IEntityTypeConfiguration<HostUpdChange>
    {
        public void Configure(EntityTypeBuilder<HostUpdChange> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.HostUpdChangeCreatedBy)
                 .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_HostUpdChange_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.HostUpdChangeUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_HostUpdChange_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Product).WithMany(c => c.HostUpdChanges)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_User_HostUpdChange_ProductId").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Outlets).WithMany(c => c.HostUpdChangeOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_User_HostUpdChange_OutletId").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.ChangeType).WithMany(c => c.HostUpdChangeType)
               .HasForeignKey(c => c.ChangeTypeId).HasConstraintName("FK_User_HostUpdChange_ChangeTypeId").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Promotion).WithMany(c => c.HostUpdChangePromotion)
                           .HasForeignKey(c => c.PromotionId).HasConstraintName("FK_User_HostUpdChange_PromotionId").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.HostUpd).WithMany(c => c.HostUpdHostProcessing)
                           .HasForeignKey(c => c.HostUpdId).HasConstraintName("FK_User_HostUpdChange_HostUpdId").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Host).WithMany(c => c.HostUpdChangeHostSettings)
                           .HasForeignKey(c => c.HostId).HasConstraintName("FK_User_HostUpdChange_HostId").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
