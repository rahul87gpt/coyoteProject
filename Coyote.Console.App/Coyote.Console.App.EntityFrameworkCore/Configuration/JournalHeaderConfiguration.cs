using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class JournalHeaderConfiguration : IEntityTypeConfiguration<JournalHeader>
    {
        public void Configure(EntityTypeBuilder<JournalHeader> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.JournalHeaderCreated)
                 .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_Journal_Header_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.JournalHeaderUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_Journal_Header_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Store).WithMany(c => c.JournalHeaderOutlet)
               .HasForeignKey(c => c.OutletId).HasConstraintName("FK_Outlet_Journal_Header_Outlet").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Till).WithMany(c => c.JournalHeaderTill)
               .HasForeignKey(c => c.TillId).HasConstraintName("FK_Till_Journal_Header_Till").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.Cashier).WithMany(c => c.JournalHeaderCashier)
               .HasForeignKey(c => c.CashierId).HasConstraintName("FK_Cashier_Journal_Header_Cashier").OnDelete(DeleteBehavior.NoAction);



            builder?.HasIndex(p => new { p.HeaderDate, p.HeaderTime, p.OutletId, p.TillId, p.TransactionNo }).IsUnique().HasName("IX_JournalHeader_HeaderDate_HeaderTime_Outlet_Till_TransactionNo_Unique");


        }
    }
}
