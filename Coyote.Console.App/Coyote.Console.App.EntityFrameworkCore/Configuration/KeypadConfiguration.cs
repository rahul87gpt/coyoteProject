using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class KeypadConfiguration : IEntityTypeConfiguration<Keypad>
    {
        public void Configure(EntityTypeBuilder<Keypad> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.KeypadCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Keypad_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.KeypadUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Keypad_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.KeypadOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Keypad_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_Keypad_Delete_Unique");

          //  builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_Keypad_Code_Unique");

            builder?.HasData(new Keypad
            {
                Id = 1,
                Code = "792_Keypad",
                Desc = "792 Keypad",
                Status = true,
                OutletId = 1,
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            });

        }
    }
}
