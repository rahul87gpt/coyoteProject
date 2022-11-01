using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class ChangeTillSyncTablecolumnlength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PosVersion",
                table: "TillSync",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientVersion",
                table: "TillSync",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 32,
                column: "Code",
                value: "COMPETITION");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 15, 13, 31, 58, 470, DateTimeKind.Utc).AddTicks(5070), new DateTime(2022, 3, 30, 13, 31, 58, 470, DateTimeKind.Utc).AddTicks(5985) });

            migrationBuilder.UpdateData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "999999");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PosVersion",
                table: "TillSync",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientVersion",
                table: "TillSync",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "MasterListItems",
                keyColumn: "Id",
                keyValue: 32,
                column: "Code",
                value: "COMPITITION");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 13, 11, 32, 51, 291, DateTimeKind.Utc).AddTicks(7139), new DateTime(2022, 3, 28, 11, 32, 51, 291, DateTimeKind.Utc).AddTicks(7745) });

            migrationBuilder.UpdateData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 1,
                column: "Code",
                value: "SuperAdminStore");
        }
    }
}
