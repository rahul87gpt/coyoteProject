using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class RemoveHeaderTBLCodeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashierNumber",
                table: "JournalHeader");

            migrationBuilder.DropColumn(
                name: "OutletCode",
                table: "JournalHeader");

            migrationBuilder.DropColumn(
                name: "TillCode",
                table: "JournalHeader");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 6, 14, 10, 29, 59, 421, DateTimeKind.Utc).AddTicks(7595), new DateTime(2022, 6, 29, 10, 29, 59, 421, DateTimeKind.Utc).AddTicks(8050) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CashierNumber",
                table: "JournalHeader",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutletCode",
                table: "JournalHeader",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TillCode",
                table: "JournalHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 6, 3, 11, 23, 42, 890, DateTimeKind.Utc).AddTicks(3968), new DateTime(2022, 6, 18, 11, 23, 42, 890, DateTimeKind.Utc).AddTicks(4520) });
        }
    }
}
