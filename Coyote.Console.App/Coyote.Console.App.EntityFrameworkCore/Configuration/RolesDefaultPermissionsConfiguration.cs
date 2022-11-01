using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class RolesDefaultPermissionsConfiguration : IEntityTypeConfiguration<RolesDefaultPermissions>
    {
        public void Configure(EntityTypeBuilder<RolesDefaultPermissions> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.RolesDefaultPermissionsCreatedBy)
               .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Role_Default_Permission_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.RolesDefaultPermissionsUpdatedBy)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Role_Default_Permission_Updated_By").OnDelete(DeleteBehavior.NoAction);

        }
    }
}
