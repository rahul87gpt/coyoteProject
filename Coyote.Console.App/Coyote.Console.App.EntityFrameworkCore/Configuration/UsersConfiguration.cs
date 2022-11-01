using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class UsersConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.Created)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_User_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.Updated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_User_Updated_By").OnDelete(DeleteBehavior.NoAction);

          //  builder?.HasIndex(p => new { p.UserName, p.IsDeleted }).IsUnique().HasFilter("UserName IS NOT NULL AND IsDeleted = 0").HasName("IX_UserName_Delete_Unique");
           // builder?.HasIndex(p => new { p.UserName }).IsUnique().HasName("IX_User_Name_Unique");

            builder?.HasData(new List<Users>
            {
                new Users
                {
                    Id = 1,
                    FirstName="SuperAdmin",
                    LastName="SuperAdmin",
                    Email = "SuperAdmin@coyote.com",
                    Password="8kx7T0Uf3VYimsY1g/7aaA==",
                    Status = true,
                    IsResetPassword=false,
                    ZoneIds = "657",
                    StoreIds = "7",
                    PlainPassword="cdn@12345",
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    UserName ="SuperAdmin",
                },
                new Users
                {
                    Id = 2,
                    FirstName="USER1",
                    LastName="W",
                    Email = "user1@cdnsol.com",
                    Password="8kx7T0Uf3VYimsY1g/7aaA==",
                    Status = true,
                    IsResetPassword=false,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    UserName= "User1"
                },
                new Users
                {
                    Id = 3,
                    FirstName="COYOTE",
                    LastName="test",
                    Email = "coyote@mailinator.com",
                    Password="8kx7T0Uf3VYimsY1g/7aaA==",
                    Status = true,
                    IsResetPassword=false,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    UserName="COYOTE_Test"
                },
                new Users
                {
                    Id = 4,
                    FirstName="swagger",
                    LastName="test",
                    Email = "user@example.com",
                    Password="QIWtwazDYZU8/oVq2Xufwg==",
                    Status = true,
                    IsResetPassword=false,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                    UserName="Swagger"
                }
            });
        }
    }
}
