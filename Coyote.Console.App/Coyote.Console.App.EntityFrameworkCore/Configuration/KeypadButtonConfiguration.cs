using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class KeypadButtonConfiguration : IEntityTypeConfiguration<KeypadButton>
    {
        public void Configure(EntityTypeBuilder<KeypadButton> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.KeypadButtonCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_KeypadButton_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.KeypadButtonUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_KeypadButton_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Keypad).WithMany(c => c.KeypadButton)
               .HasForeignKey(c => c.KeypadId).HasConstraintName("FK_Keypad_KeypadButton").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.KeypadLevel).WithMany(c => c.KeypadButton)
             .HasForeignKey(c => c.LevelId).HasConstraintName("FK_KeypadButton_KeypadLevel").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.BtnKeypadLevel).WithMany(c => c.ButtonKeypadLevel)
           .HasForeignKey(c => c.BtnKeypadLevelId).HasConstraintName("FK_KeypadButton_ButtonTypeLevel").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Product).WithMany(c => c.ButtonnTypeProduct)
          .HasForeignKey(c => c.ProductId).HasConstraintName("FK_KeypadButton_ButtonTypeProduct").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.Category).WithMany(c => c.ButtonTypeCategory)
          .HasForeignKey(c => c.CategoryId).HasConstraintName("FK_KeypadButton_ButtonTypeCategory").OnDelete(DeleteBehavior.NoAction);

            builder?.HasOne(o => o.ButtonType).WithMany(c => c.KeypadButtonsMasterListItem)
            .HasForeignKey(c => c.Type).HasConstraintName("FK_KeypadButton_ButtonType").OnDelete(DeleteBehavior.NoAction);
        }
    }
}


