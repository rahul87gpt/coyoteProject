using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{

    public class EPayConfiguration : IEntityTypeConfiguration<EPay>
    {
        public void Configure(EntityTypeBuilder<EPay> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.EPAYCreated)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_EPAY_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.EPAYUpdated)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_EPAY_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Product).WithMany(c => c.EPAYProducts)
               .HasForeignKey(c => c.ProductID).HasConstraintName("FK_EPAY_Product").OnDelete(DeleteBehavior.NoAction);
        }
    }   
}
