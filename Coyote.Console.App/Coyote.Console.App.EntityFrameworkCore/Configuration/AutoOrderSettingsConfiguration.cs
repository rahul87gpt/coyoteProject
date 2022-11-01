using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class AutoOrderSettingsConfiguration : IEntityTypeConfiguration<AutoOrderSettings>
    {
        public void Configure(EntityTypeBuilder<AutoOrderSettings> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.AutoOrderSettingsCreatedBy)
              .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_AutoOrderSettings_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.AutoOrderSettingsUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_AutoOrderSettings_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(c => c.Store).WithMany(x => x.AutoOrderSettings)
                   .HasForeignKey(x => x.StoreId).HasConstraintName("FK_Store_AutoOrderSettings").OnDelete(DeleteBehavior.NoAction); 
            builder?.HasOne(c => c.Supplier).WithMany(x => x.AutoOrderSettings)
                .HasForeignKey(x => x.SupplierId).HasConstraintName("FK_Supplier_AutoOrderSettings").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
