using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class SendEmailConfiguration : IEntityTypeConfiguration<SendEmail>
    {
        public void Configure(EntityTypeBuilder<SendEmail> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.SendEmailCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_SendEmail_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.SendEmailUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_SendEmail_Updated_By").OnDelete(DeleteBehavior.NoAction);
        }
    }
}
