using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class HostSettingsConfiguration : IEntityTypeConfiguration<HostSettings>
    {
        public void Configure(EntityTypeBuilder<HostSettings> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.HostSettingsCreated)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_HostSettings_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.HostSettingsUpdated)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_HostSettings_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Path).WithMany(c => c.HostSettingsFilePath)
               .HasForeignKey(c => c.FilePathID).HasConstraintName("FK_HostSettings_FilePath").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Supplier).WithMany(c => c.HostSettingsSupplier)
               .HasForeignKey(c => c.SupplierID).HasConstraintName("FK_HostSettings_Supplier").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Warehouse).WithMany(c => c.HostSettingsWarehouse)
               .HasForeignKey(c => c.WareHouseID).HasConstraintName("FK_HostSettings_Warehouse").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.HostFormatWareHouse).WithMany(c => c.HostSettingsMasterListItem)
               .HasForeignKey(c => c.HostFormatID).HasConstraintName("FK_HostSettings_MasterListItem").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
