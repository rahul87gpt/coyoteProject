using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class SystemControlConfiguration : IEntityTypeConfiguration<SystemControls>
    {
        public void Configure(EntityTypeBuilder<SystemControls> builder)
        {
            builder?.HasOne(o => o.UserCreatedBy).WithMany(c => c.SystemControlCreatedBy)
                .HasForeignKey(c => c.CreatedBy).HasConstraintName("FK_User_SysControl_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UserModifiedBy).WithMany(c => c.SystemControlUpdatedBy)
                .HasForeignKey(c => c.ModifiedBy).HasConstraintName("FK_User_SysControl_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder.HasData(new SystemControls
            {
                ID=1,
                Name = "Test Controll",
                SerialNo = "12345",
                LicenceKey = "123",
                MassPriceUpdate = "REAL",
                TillJournal = Common.TillJournal.Stopped,
                MaxStores = 20,
                Color = "Blue(Default)",
                NumberFactor = 1000000,
                CreatedBy = 1,
                ModifiedBy = 1,
                CreatedDate =  new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                ModifiedDate = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsActive = Common.Status.Active,
            });

        }
    }
}
