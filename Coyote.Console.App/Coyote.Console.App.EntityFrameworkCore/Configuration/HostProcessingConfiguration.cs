using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class HostProcessingConfiguration : IEntityTypeConfiguration<HostProcessing>
    {
        public void Configure(EntityTypeBuilder<HostProcessing> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.HostProcessingCreated)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_HostProcessing_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.HostProcessingUpdated)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_HostProcessing_Updated_By").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
