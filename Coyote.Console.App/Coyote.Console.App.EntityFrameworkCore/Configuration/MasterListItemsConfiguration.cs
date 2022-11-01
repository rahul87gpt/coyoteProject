using Coyote.Console.App.EntityFrameworkCore.Constants;
using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class MasterListItemsConfiguration : IEntityTypeConfiguration<MasterListItems>
    {
        public void Configure(EntityTypeBuilder<MasterListItems> builder)
        {
            builder?.HasOne(o => o.CreatedBy).WithMany(c => c.MasterListItemsCreated)
                .HasForeignKey(c => c.CreatedById).HasConstraintName("FK_User_MasterList_Items_Created_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.UpdatedBy).WithMany(c => c.MasterListItemsUpdated)
                .HasForeignKey(c => c.UpdatedById).HasConstraintName("FK_User_MasterList_Items_Updated_By").OnDelete(DeleteBehavior.NoAction);
            builder?.HasOne(o => o.MasterList).WithMany(c => c.Items)
                .HasForeignKey(c => c.ListId).HasConstraintName("FK_MasterList_MasterListItems_Id").OnDelete(DeleteBehavior.NoAction);


            builder?.HasIndex(p => new { p.ListId, p.Code,p.IsDeleted }).IsUnique().HasFilter("Code IS NOT NULL AND IsDeleted = 0").HasName("IX_List_Id_Item_Deleted_Code");

           // builder?.HasIndex(p => new { p.ListId, p.Code }).IsUnique().HasName("IX_List_Id_Item_Code");


            int index = 1;
            int listindex = 1;

            var list = new List<Tuple<int, int, string, string>>
            {
                new Tuple<int, int, string, string>(index++, listindex++, "ZONE", "ZONE"),
                new Tuple<int, int, string, string>(index++, listindex++, "CATEGORY", "CATEGORY"),
                new Tuple<int, int, string, string>(index++, listindex++, "MANUFACTURER", "MANUFACTURER"),
                new Tuple<int, int, string, string>(index++, listindex++, "GROUP", "GROUP"),
                new Tuple<int, int, string, string>(index++, listindex++, "NATIONALRANGE", "NATIONAL RANGE"),
                new Tuple<int, int, string, string>(index++, listindex++, "UNITOFMEASURE", "UNIT OF MEASURE"),
                new Tuple<int, int, string, string>(index++, listindex++, "WAREHOUSEHOSTFORMAT", "WAREHOUSE HOST FORMAT"),

                new Tuple<int, int, string, string>(index++, listindex, "APN", "APN"),
                new Tuple<int, int, string, string>(index++, listindex, "Commodity", "Commodity"),
                new Tuple<int, int, string, string>(index++, listindex, "Department", "Department"),
                new Tuple<int, int, string, string>(index++, listindex, "Login", "Login"),
                new Tuple<int, int, string, string>(index++, listindex, "MasterList", "MasterList"),
                new Tuple<int, int, string, string>(index++, listindex, "MasterListItem", "MasterListItem"),
                new Tuple<int, int, string, string>(index++, listindex, "Product", "Product"),
                new Tuple<int, int, string, string>(index++, listindex, "Role", "Role"),
                new Tuple<int, int, string, string>(index++, listindex, "Store", "Store"),
                new Tuple<int, int, string, string>(index++, listindex, "StoreGroup", "StoreGroup"),
                new Tuple<int, int, string, string>(index++, listindex, "Supplier", "Supplier"),
                new Tuple<int, int, string, string>(index++, listindex, "SupplierProduct", "SupplierProduct"),
                new Tuple<int, int, string, string>(index++, listindex, "Tax", "Tax"),
                new Tuple<int, int, string, string>(index++, listindex, "User", "User"),
                new Tuple<int, int, string, string>(index++, listindex, "UserRole", "UserRole"),
                new Tuple<int, int, string, string>(index++, listindex, "Warehouse", "Warehouse"),
                new Tuple<int, int, string, string>(index++, listindex++, "ZoneOutl", "ZoneOutlet"),

                new Tuple<int, int, string, string>(index++, listindex, "Get", "Get"),
                new Tuple<int, int, string, string>(index++, listindex, "Add", "Post"),
                new Tuple<int, int, string, string>(index++, listindex, "Update", "Put"),
                new Tuple<int, int, string, string>(index++, listindex++, "Delete", "Delete"),

                new Tuple<int, int, string, string>(index++, listindex, "OFFER", "OFFER"),
                new Tuple<int, int, string, string>(index++, listindex, "BUYING", "BUYING"),
                new Tuple<int, int, string, string>(index++, listindex, "SELLING", "SELLING"),
                new Tuple<int, int, string, string>(index++, listindex, "COMPETITION", "COMPITITION"),
                new Tuple<int, int, string, string>(index++, listindex, "MEMBEROFFER", "MEMBEROFFER"),
                new Tuple<int, int, string, string>(index++, listindex++, "MIXMATCH", "MIXMATCH"),


                new Tuple<int, int, string, string>(index++, listindex++, "ACCESSDEPT", "ACCESSDEPT"),
                new Tuple<int, int, string, string>(index++, listindex++, "ACCESSGROUP", "ACCESSGROUP"),
                new Tuple<int, int, string, string>(index++, listindex++, "ACCESSMENU", "ACCESSMENU"),
                new Tuple<int, int, string, string>(index++, listindex++, "ADJUSTCODE", "ADJUSTCODE"),
                new Tuple<int, int, string, string>(index++, listindex++, "ADJUSTMENT", "ADJUSTMENT"),
                new Tuple<int, int, string, string>(index++, listindex++, "ADJUSTMENTI", "ADJUSTMENTI"),
                new Tuple<int, int, string, string>(index++, listindex++, "CASHIERTYPE", "CASHIERTYPE"),
                new Tuple<int, int, string, string>(index++, listindex++, "COMMODITY", "COMMODITY"),
                new Tuple<int, int, string, string>(index++, listindex++, "COSTZONE", "COSTZONE"),
                new Tuple<int, int, string, string>(index++, listindex++, "CRUSERFLAGS", "CRUSERFLAGS"),
                new Tuple<int, int, string, string>(index++, listindex++, "DEBTORTERM", "DEBTORTERM"),
                new Tuple<int, int, string, string>(index++, listindex++, "DEBTORTERMS", "DEBTORTERMS"),
                new Tuple<int, int, string, string>(index++, listindex++, "DEPARTMENT", "DEPARTMENT"),
                new Tuple<int, int, string, string>(index++, listindex++, "ENTITY_EDIT", "ENTITY_EDIT"),
                new Tuple<int, int, string, string>(index++, listindex++, "HOST", "HOST"),
                new Tuple<int, int, string, string>(index++, listindex++, "HOSTUPD", "HOSTUPD"),
                new Tuple<int, int, string, string>(index++, listindex++, "HOSTUPD_CHG", "HOSTUPD_CHG"),
                new Tuple<int, int, string, string>(index++, listindex++, "JNL_POSTING_ON", "JNL_POSTING_ON"),
                new Tuple<int, int, string, string>(index++, listindex++, "KEYPAD", "KEYPAD"),
                new Tuple<int, int, string, string>(index++, listindex++, "KEYPAD_BTNS", "KEYPAD_BTNS"),
                new Tuple<int, int, string, string>(index++, listindex++, "KEYPAD_LEVS", "KEYPAD_LEVS"),
                new Tuple<int, int, string, string>(index++, listindex++, "LABELTYPE", "LABELTYPE"),
                new Tuple<int, int, string, string>(index++, listindex++, "MANUALSALE", "MANUALSALE"),
                new Tuple<int, int, string, string>(index++, listindex++, "MANUALSALEI", "MANUALSALEI"),
                new Tuple<int, int, string, string>(index++, listindex++, "MEMBERCLASS", "MEMBERCLASS"),
                new Tuple<int, int, string, string>(index++, listindex++, "MEMBEROFFER", "MEMBEROFFER"),
                new Tuple<int, int, string, string>(index++, listindex++, "MIXMATCH", "MIXMATCH"),
                new Tuple<int, int, string, string>(index++, listindex++, "MM_OLD", "MM_OLD"),
                new Tuple<int, int, string, string>(index++, listindex++, "OCCUPATION", "OCCUPATION"),
                new Tuple<int, int, string, string>(index++, listindex++, "OFFER", "OFFER"),
                new Tuple<int, int, string, string>(index++, listindex++, "OUTLET_BUDGETS", "OUTLET_BUDGETS"),
                new Tuple<int, int, string, string>(index++, listindex++, "OUTLET_FIFO", "OUTLET_FIFO"),
                new Tuple<int, int, string, string>(index++, listindex++, "OUTLET_SUPPLIER", "OUTLET_SUPPLIER"),
                new Tuple<int, int, string, string>(index++, listindex++, "PATH", "PATH"),
                new Tuple<int, int, string, string>(index++, listindex++, "PDELOAD", "PDELOAD"),
                new Tuple<int, int, string, string>(index++, listindex++, "PRICEZONE", "PRICEZONE"),
                new Tuple<int, int, string, string>(index++, listindex++, "PROMOTION", "PROMOTION"),
                new Tuple<int, int, string, string>(index++, listindex++, "ROYALTY_SCALES", "ROYALTY_SCALES"),
                new Tuple<int, int, string, string>(index++, listindex++, "SAV_AUTO_ORDER", "SAV_AUTO_ORDER"),
                new Tuple<int, int, string, string>(index++, listindex++, "STOCKTAKE", "STOCKTAKE"),
                new Tuple<int, int, string, string>(index++, listindex++, "STOCKTAKEI", "STOCKTAKEI"),
                new Tuple<int, int, string, string>(index++, listindex++, "STOREGROUP", "STOREGROUP"),
                new Tuple<int, int, string, string>(index++, listindex++, "SUBRANGE", "SUBRANGE"),
                new Tuple<int, int, string, string>(index++, listindex++, "SUPPLIER", "SUPPLIER"),
                new Tuple<int, int, string, string>(index++, listindex++, "SYNCTILL", "SYNCTILL"),
                new Tuple<int, int, string, string>(index++, listindex++, "SYSCONTROLS", "SYSCONTROLS"),
                new Tuple<int, int, string, string>(index++, listindex++, "TAXCODE", "TAXCODE"),
                new Tuple<int, int, string, string>(index++, listindex++, "USERLOG", "USERLOG"),
                new Tuple<int, int, string, string>(index++, listindex++, "USERTYPE", "USERTYPE"),
                new Tuple<int, int, string, string>(index++, listindex++, "WAREHOUSE", "WAREHOUSE"),
                new Tuple<int, int, string, string>(index++, listindex++, "XERO_ACCOUNT", "XERO_ACCOUNT"),
                new Tuple<int, int, string, string>(index++, listindex++, "ZONEOUTLET", "ZONEOUTLET"),
                new Tuple<int, int, string, string>(index++, listindex++, "TYPE", "TYPE"),

                new Tuple<int, int, string, string>(index++, listindex, "Hourly", "Active Minutes"),
                new Tuple<int, int, string, string>(index++, listindex, "Daily", "Active Hours"),
                new Tuple<int, int, string, string>(index++, listindex, "Weekly", "Active Week Days"),
                new Tuple<int, int, string, string>(index++, listindex++, "Monthly", "Active Date"),

                new Tuple<int, int, string, string>(index++, listindex, "MAP_TYPE-1", "MAP_TYPE-1"),
                new Tuple<int, int, string, string>(index++, listindex++, "MAP-TYPE-2", "MAP-TYPE-2"),
               //OrderType : ORDER, DELIVERY, INVOICE, TRANSFER
                new Tuple<int, int, string, string>(index++, listindex, "ORDER", "ORDER"),
                new Tuple<int, int, string, string>(index++, listindex, "DELIVERY", "DELIVERY"),
                new Tuple<int, int, string, string>(index++, listindex, "INVOICE", "INVOICE"),
                new Tuple<int, int, string, string>(index++, listindex++, "TRANSFER", "TRANSFER"),
                //OrderStatus: NEW, ORDER, DELIVERY, INVOICE, TRANSFER
                new Tuple<int, int, string, string>(index++, listindex, "NEW", "NEW"),
                new Tuple<int, int, string, string>(index++, listindex, "ORDER", "ORDER"),
                new Tuple<int, int, string, string>(index++, listindex, "DELIVERY", "DELIVERY"),
                new Tuple<int, int, string, string>(index++, listindex, "INVOICE", "INVOICE"),
                new Tuple<int, int, string, string>(index++, listindex++, "TRANSFER", "TRANSFER"),
                //OrderCreationType : 1,2,3,4,5,6
                new Tuple<int, int, string, string>(index++, listindex, "1", "1"),
                new Tuple<int, int, string, string>(index++, listindex, "2", "2"),
                new Tuple<int, int, string, string>(index++, listindex, "3", "3"),
                new Tuple<int, int, string, string>(index++, listindex, "4", "4"),
                new Tuple<int, int, string, string>(index++, listindex, "5", "5"),
                new Tuple<int, int, string, string>(index++, listindex++, "6", "6"),

            };//code,name


            builder?.HasData(list.Select((x, i) =>
            new MasterListItems
            {
                Id = x.Item1,
                ListId = x.Item2,
                Code = x.Item3.ToUpper(CultureInfo.CurrentCulture),
                Name = x.Item4.ToUpper(CultureInfo.CurrentCulture),
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
