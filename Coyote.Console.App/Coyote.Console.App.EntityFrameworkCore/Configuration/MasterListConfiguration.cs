using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class MasterListConfiguration : IEntityTypeConfiguration<MasterList>
    {
        public void Configure(EntityTypeBuilder<MasterList> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.MasterListCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_MasterList_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.MasterListUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_MasterList_Updated_By").OnDelete(DeleteBehavior.NoAction);

            builder?.HasIndex(p => new { p.Code, p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_MasterList_Delete_Unique");

           // builder?.HasIndex(p => new { p.Code }).IsUnique().HasName("IX_MasterList_Code");

            var list = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("ZONE", "ZONE"),
                new Tuple<string, string>("CATEGORY", "CATEGORY"),
                new Tuple<string, string>("MANUFACTURER", "MANUFACTURER"),
                new Tuple<string, string>("GROUP", "GROUP"),
                new Tuple<string, string>("NATIONALRANGE", "NATIONAL RANGE"),
                new Tuple<string, string>("UNITOFMEASURE", "UNIT OF MEASURE"),
                new Tuple<string, string>("WAREHOUSEHOSTFORMAT", "WAREHOUSE HOST FORMAT"),
                new Tuple<string, string>("CONTROLLER", "MODULE"),
                new Tuple<string, string>("ACTION", "PERMISSION"),
                new Tuple<string, string>("PROMOTYPE", "PROMO TYPE"),
            
                new Tuple<string, string>("ACCESSDEPT", "ACCESSDEPT"),
                new Tuple<string, string>("ACCESSGROUP", "ACCESSGROUP"),
                new Tuple<string, string>("ACCESSMENU", "ACCESSMENU"),
                new Tuple<string, string>("ADJUSTCODE", "ADJUSTCODE"),
                new Tuple<string, string>("ADJUSTMENT", "ADJUSTMENT"),
                new Tuple<string, string>("ADJUSTMENTI", "ADJUSTMENTI"),
                new Tuple<string, string>("CASHIERTYPE", "CASHIERTYPE"),
                //list.Add(new Tuple<string, string>("CATEGORY", "CATEGORY"));
                new Tuple<string, string>("COMMODITY", "COMMODITY"),
                new Tuple<string, string>("COSTZONE", "COSTZONE"),
                new Tuple<string, string>("CRUSERFLAGS", "CRUSERFLAGS"),
                new Tuple<string, string>("DEBTORTERM", "DEBTORTERM"),
                new Tuple<string, string>("DEBTORTERMS", "DEBTORTERMS"),
                new Tuple<string, string>("DEPARTMENT", "DEPARTMENT"),
                new Tuple<string, string>("ENTITY_EDIT", "ENTITY_EDIT"),
                //list.Add(new Tuple<string, string>("GROUP", "GROUP"));
                new Tuple<string, string>("HOST", "HOST"),
                new Tuple<string, string>("HOSTUPD", "HOSTUPD"),
                new Tuple<string, string>("HOSTUPD_CHG", "HOSTUPD_CHG"),
                new Tuple<string, string>("JNL_POSTING_ON", "JNL_POSTING_ON"),
                new Tuple<string, string>("KEYPAD", "KEYPAD"),
                new Tuple<string, string>("KEYPAD_BTNS", "KEYPAD_BTNS"),
                new Tuple<string, string>("KEYPAD_LEVS", "KEYPAD_LEVS"),
                new Tuple<string, string>("LABELTYPE", "LABELTYPE"),
                new Tuple<string, string>("MANUALSALE", "MANUALSALE"),
                new Tuple<string, string>("MANUALSALEI", "MANUALSALEI"),
                //list.Add(new Tuple<string, string>("MANUFACTURER", "MANUFACTURER"));
                new Tuple<string, string>("MEMBERCLASS", "MEMBERCLASS"),
                new Tuple<string, string>("MEMBEROFFER", "MEMBEROFFER"),
                new Tuple<string, string>("MIXMATCH", "MIXMATCH"),
                new Tuple<string, string>("MM_OLD", "MM_OLD"),
                new Tuple<string, string>("OCCUPATION", "OCCUPATION"),
                new Tuple<string, string>("OFFER", "OFFER"),
                new Tuple<string, string>("OUTLET_BUDGETS", "OUTLET_BUDGETS"),
                new Tuple<string, string>("OUTLET_FIFO", "OUTLET_FIFO"),
                new Tuple<string, string>("OUTLET_SUPPLIER", "OUTLET_SUPPLIER"),
                new Tuple<string, string>("PATH", "PATH"),
                new Tuple<string, string>("PDELOAD", "PDELOAD"),
                new Tuple<string, string>("PRICEZONE", "PRICEZONE"),
                new Tuple<string, string>("PROMOTION", "PROMOTION"),
                new Tuple<string, string>("ROYALTY_SCALES", "ROYALTY_SCALES"),
                new Tuple<string, string>("SAV_AUTO_ORDER", "SAV_AUTO_ORDER"),
                new Tuple<string, string>("STOCKTAKE", "STOCKTAKE"),
                new Tuple<string, string>("STOCKTAKEI", "STOCKTAKEI"),
                new Tuple<string, string>("STOREGROUP", "STOREGROUP"),
                new Tuple<string, string>("SUBRANGE", "SUBRANGE"),
                new Tuple<string, string>("SUPPLIER", "SUPPLIER"),
                new Tuple<string, string>("SYNCTILL", "SYNCTILL"),
                new Tuple<string, string>("SYSCONTROLS", "SYSCONTROLS"),
                new Tuple<string, string>("TAXCODE", "TAXCODE"),
                new Tuple<string, string>("USERLOG", "USERLOG"),
                new Tuple<string, string>("USERTYPE", "USERTYPE"),
                new Tuple<string, string>("WAREHOUSE", "WAREHOUSE"),
                new Tuple<string, string>("XERO_ACCOUNT", "XERO_ACCOUNT"),
                //list.Add(new Tuple<string, string>("ZONE", "ZONE"));
                new Tuple<string, string>("ZONEOUTLET", "ZONEOUTLET"),
                new Tuple<string, string>("TYPE", "TYPE"),
                new Tuple<string, string>("PromotionFrequency", "PromotionFrequency"),
                 new Tuple<string, string>("DEPT_MAPTYPE", "Department Map Type"),
                new Tuple<string, string>("OrderType", "Order Type"),
                new Tuple<string, string>("OrderStatus", "Order Status"),
                new Tuple<string, string>("OrderCreationType", "Order Creation Type"),
            };//code,name


            builder?.HasData(list.Select((x, i) =>
            new MasterList
            {
                Id = i + 1,
                Code = x.Item1,
                Name = x.Item2,
                Status = true,
                CreatedById = 1,
                UpdatedById = 1,
                CreatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                UpdatedAt = new DateTime(MigrationConstants.DateCreatedUpdatedYear, MigrationConstants.DateCreatedUpdatedMonth, MigrationConstants.DateCreatedUpdatedDay),
                IsDeleted = false,
            }));
        }
    }
}
