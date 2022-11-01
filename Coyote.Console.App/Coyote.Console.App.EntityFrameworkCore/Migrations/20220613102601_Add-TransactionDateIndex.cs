using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class AddTransactionDateIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 6, 3, 10, 25, 57, 613, DateTimeKind.Utc).AddTicks(3998), new DateTime(2022, 6, 18, 10, 25, 57, 613, DateTimeKind.Utc).AddTicks(4439) });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Date",
                table: "Transaction",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Trans_date_PromoSales",
                table: "Transaction",
                columns: new[] { "Type", "PromoSales" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_Date",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Trans_date_PromoSales",
                table: "Transaction");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 22, 11, 0, 44, 727, DateTimeKind.Utc).AddTicks(1233), new DateTime(2022, 6, 6, 11, 0, 44, 727, DateTimeKind.Utc).AddTicks(1673) });
        }
    }
}
