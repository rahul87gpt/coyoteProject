using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
   public class CSCPeriodConfiguration : IEntityTypeConfiguration<CSCPeriod>
    {
        public void Configure(EntityTypeBuilder<CSCPeriod> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.CSCPeriodCreated)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_CSCPeriod_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.CSCPeriodUpdated)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_CSCPeriod_Updated_By").OnDelete(DeleteBehavior.NoAction);        
        }
    }
}
