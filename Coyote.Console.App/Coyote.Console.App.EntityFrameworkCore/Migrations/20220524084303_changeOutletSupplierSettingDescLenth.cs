using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class changeOutletSupplierSettingDescLenth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "OutletSupplierSetting",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerNumber",
                table: "OutletSupplierSetting",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "GLAccount",
                keyColumn: "Id",
                keyValue: 1,
                column: "AccountSystem",
                value: "XERO");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 14, 8, 42, 58, 276, DateTimeKind.Utc).AddTicks(3960), new DateTime(2022, 5, 29, 8, 42, 58, 276, DateTimeKind.Utc).AddTicks(4678) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "OutletSupplierSetting",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerNumber",
                table: "OutletSupplierSetting",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "GLAccount",
                keyColumn: "Id",
                keyValue: 1,
                column: "AccountSystem",
                value: "Xero");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 22, 12, 29, 49, 945, DateTimeKind.Utc).AddTicks(9135), new DateTime(2022, 4, 6, 12, 29, 49, 945, DateTimeKind.Utc).AddTicks(9793) });
        }
    }
}
