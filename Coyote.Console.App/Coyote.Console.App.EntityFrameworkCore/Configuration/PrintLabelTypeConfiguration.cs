using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class PrintLabelTypeConfiguration : IEntityTypeConfiguration<PrintLabelType>
    {
        public void Configure(EntityTypeBuilder<PrintLabelType> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.PrintLabelTypeCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_PrintLabelType_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserUpdatedBy).WithMany(c => c.PrintLabelTypeUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_PrintLabelType_Updated_By").OnDelete(DeleteBehavior.NoAction);
            
            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_PrintLabelType_Delete_Unique");

            //builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_PrintLabelType_Code_Unique");

        }
    }
}

