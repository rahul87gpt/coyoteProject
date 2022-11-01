using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class KeypadLevelConfiguration : IEntityTypeConfiguration<KeypadLevel>
    {
        public void Configure(EntityTypeBuilder<KeypadLevel> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.KeypadLevelCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_KeypadLevel_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.KeypadLevelUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_KeypadLevel_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Keypad).WithMany(c => c.KeypadLevel)
               .HasForeignKey(c => c.KeypadId).HasConstraintName("FK_Keypad_KeypadLevel").OnDelete(DeleteBehavior.NoAction);
            //builder?.HasIndex(p => new { p.Desc }).IsUnique().HasName("IX_KeypadLevel_Desc_Unique");

          }
    }
}

