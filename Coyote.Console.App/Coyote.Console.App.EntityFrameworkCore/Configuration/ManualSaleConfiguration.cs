using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class ManualSaleConfiguration : IEntityTypeConfiguration<ManualSale>
    {
        public void Configure(EntityTypeBuilder<ManualSale> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.ManualSaleCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_ManualSale_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.ManualSaleUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_ManualSale_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.TypeMasterListItem).WithMany(c => c.ManualSaleMasterListItems)
               .HasForeignKey(c => c.TypeId).HasConstraintName("FK_Type_ManualSale_Type").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
