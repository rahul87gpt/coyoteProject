using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class APNConfiguration : IEntityTypeConfiguration<APN>
    {
        public void Configure(EntityTypeBuilder<APN> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.APNCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_APN_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.APNUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_APN_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Number }).IsUnique().HasName("IX_APNNumber_Unique");

            builder?.HasData(new APN
            {
                Id = 1,
                Number = 1,
                Status=true,
                ProductId = 1,
                CreatedById = 1,
                Desc = "Test",
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });

        }
    }
}