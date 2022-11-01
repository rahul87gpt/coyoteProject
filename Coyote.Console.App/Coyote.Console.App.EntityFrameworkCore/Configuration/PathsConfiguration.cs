using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PathsConfiguration : IEntityTypeConfiguration<Paths>
    {
        public void Configure(EntityTypeBuilder<Paths> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PathsCreated)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_Paths_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.PathsUpdated)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_Paths_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.PathsOutlet)
               .HasForeignKey(c => c.OutletID).HasConstraintName("FK_Paths_Outlet").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
