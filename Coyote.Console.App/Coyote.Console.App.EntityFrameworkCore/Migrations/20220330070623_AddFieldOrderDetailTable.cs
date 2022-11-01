using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class AddFieldOrderDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.AddColumn<float>(
                name: "FinalGSTAMT",
                table: "OrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GSTInd",
                table: "OrderDetail",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "Code", "Name" },
                values: new object[] { "ORDER", "ORDER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "Code", "Name" },
                values: new object[] { "DELIVERY", "DELIVERY" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "Code", "Name" },
                values: new object[] { "INVOICE", "INVOICE" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "Code", "Name" },
                values: new object[] { "TRANSFER", "TRANSFER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "NEW", 67, "NEW" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Code", "Name" },
                values: new object[] { "ORDER", "ORDER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Code", "Name" },
                values: new object[] { "DELIVERY", "DELIVERY" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "Code", "Name" },
                values: new object[] { "INVOICE", "INVOICE" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "Code", "Name" },
                values: new object[] { "TRANSFER", "TRANSFER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "1", 68, "1" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "2", 68, "2" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "3", 68, "3" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "Code", "Name" },
                values: new object[] { "4", "4" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "Code", "Name" },
                values: new object[] { "5", "5" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "Code", "Name" },
                values: new object[] { "6", "6" });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 20, 7, 6, 18, 107, DateTimeKind.Utc).AddTicks(9120), new DateTime(2022, 4, 4, 7, 6, 18, 107, DateTimeKind.Utc).AddTicks(9722) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalGSTAMT",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "GSTInd",
                table: "OrderDetail");

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "Code", "Name" },
                values: new object[] { "CREDIT", "CREDIT" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "Code", "Name" },
                values: new object[] { "ORDER", "ORDER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "Code", "Name" },
                values: new object[] { "DELIVERY", "DELIVERY" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "Code", "Name" },
                values: new object[] { "INVOICE", "INVOICE" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "TRANSFER", 66, "TRANSFER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Code", "Name" },
                values: new object[] { "99999", "NEW_UNALLOCATE" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Code", "Name" },
                values: new object[] { "CREDIT", "CREDIT" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "Code", "Name" },
                values: new object[] { "NEW", "NEW" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "Code", "Name" },
                values: new object[] { "ORDER", "ORDER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "DELIVERY", 67, "DELIVERY" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "INVOICE", 67, "INVOICE" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "Code", "ListId", "Name" },
                values: new object[] { "TRANSFER", 67, "TRANSFER" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "Code", "Name" },
                values: new object[] { "INVEST", "INVEST" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "Code", "Name" },
                values: new object[] { "AUTO", "AUTO" });

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "Code", "Name" },
                values: new object[] { "OPTIMAL", "OPTIMAL" });

            migrationBuilder.InsertData(
                table: "MasterListItems",
                columns: new[] { "Id", "AccessId", "Code", "Col1", "Col2", "Col3", "Col4", "Col5", "CreatedAt", "CreatedById", "IsDeleted", "ListId", "Name", "Num1", "Num2", "Num3", "Num4", "Num5", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 112, 1, "DEFAULTCREATIONTYPE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "DEFAULTCREATIONTYPE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 111, 1, "TRANSFER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "TRANSFER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 110, 1, "TABLET", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "TABLET", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 109, 1, "MANUAL", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "MANUAL", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 18, 11, 29, 48, 123, DateTimeKind.Utc).AddTicks(5639), new DateTime(2022, 4, 2, 11, 29, 48, 123, DateTimeKind.Utc).AddTicks(6549) });
        }
    }
}
