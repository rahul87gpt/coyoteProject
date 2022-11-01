using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class addPromoAndPromoDescInHostUpdChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromoDesc",
                table: "HostUpdChange",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Promocode",
                table: "HostUpdChange",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 7, 22, 12, 54, 25, 387, DateTimeKind.Utc).AddTicks(6463), new DateTime(2022, 8, 6, 12, 54, 25, 387, DateTimeKind.Utc).AddTicks(7199) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromoDesc",
                table: "HostUpdChange");

            migrationBuilder.DropColumn(
                name: "Promocode",
                table: "HostUpdChange");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 6, 14, 10, 29, 59, 421, DateTimeKind.Utc).AddTicks(7595), new DateTime(2022, 6, 29, 10, 29, 59, 421, DateTimeKind.Utc).AddTicks(8050) });
        }
    }
}
