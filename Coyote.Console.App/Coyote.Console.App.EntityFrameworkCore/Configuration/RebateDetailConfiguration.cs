using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
   public class RebateDetailConfiguration : IEntityTypeConfiguration<RebateDetails>
    {
        public void Configure(EntityTypeBuilder<RebateDetails> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.RebateDetailsCreatedBy)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_ReabateDetail_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.RebateDetailsUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_RebateDetail_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.RebateProducts)
                .HasForeignKey(c => c.ProductId).HasConstraintName("FK_Rebate_Product").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.RebateHeader).WithMany(c => c.RebateDetails)
              .HasForeignKey(c => c.RebateHeaderId).HasConstraintName("FK_Rebate_Detail_Header").OnDelete(DeleteBehavior.NoAction);
        }
    }
}

