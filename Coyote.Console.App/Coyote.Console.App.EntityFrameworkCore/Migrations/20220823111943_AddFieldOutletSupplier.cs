using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class AddFieldOutletSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostedOrder",
                table: "OutletSupplierSetting",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 8, 13, 11, 19, 38, 26, DateTimeKind.Utc).AddTicks(6549), new DateTime(2022, 8, 28, 11, 19, 38, 26, DateTimeKind.Utc).AddTicks(7224) });

            migrationBuilder.UpdateData(
                table: "StoreGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostedOrder",
                table: "OutletSupplierSetting");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 8, 8, 10, 17, 13, 734, DateTimeKind.Utc).AddTicks(1109), new DateTime(2022, 8, 23, 10, 17, 13, 734, DateTimeKind.Utc).AddTicks(1812) });

            migrationBuilder.UpdateData(
                table: "StoreGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "Super Admin Store Group");
        }
    }
}
