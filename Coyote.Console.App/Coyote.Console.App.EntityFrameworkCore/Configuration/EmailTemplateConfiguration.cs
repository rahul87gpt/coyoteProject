using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.EmailTemplateCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_EmailTemplate_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.EmailTemplateUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_EmailTemplate_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasIndex(p => new { p.Name }).IsUnique().HasName("IX_EmailTemplate_Name");



            builder?.HasData(new List<EmailTemplate> {
                new EmailTemplate
                {
                    Id = 1,
                    Name = "Forgot Password",
                    DisplayName = "Forgot Password",
                    Subject = "Forgot Password",
                    Body = @"Dear %UserFirstName% %UserLastName% <br /> <br />

                             Username: %Username% <br />
			                 You will be required to change your password.  Coyote Application system. <br />
			                 Please click the following URL to change your password: %TemporaryPasswordURL%  <br />
			                 Please do not reply to this email as this message will be undeliverable <br /> <br />
			 

                              Coyote Application Team",
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                },
                 new EmailTemplate
                {
                    Id = 2,
                    Name = "New User Creation",
                    DisplayName = "New User Creation",
                    Subject = "New User Creation",
                    Body = @"Dear %UserFirstName% %UserLastName% <br /> <br />

                         Username: %Username%  <br />
			             You will be required to change your password the first time you log into the Coyote Application system.<br />
			             Coyote Application system. <br />
			             Please click the following URL to change your password: %TemporaryPasswordURL% <br />
			             Please do not reply to this email as this message will be undeliverable <br />
			             <br />

                          Coyote Application Team",
                    CreatedById = 1,
                    UpdatedById = 1,
                    CreatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    UpdatedAt =new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                    IsDeleted = false,
                }
            });
        }
    }
}
