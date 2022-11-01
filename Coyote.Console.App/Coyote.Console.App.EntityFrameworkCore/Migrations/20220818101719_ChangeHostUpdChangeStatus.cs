using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class ChangeHostUpdChangeStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "HostUpdChange",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 8, 8, 10, 17, 13, 734, DateTimeKind.Utc).AddTicks(1109), new DateTime(2022, 8, 23, 10, 17, 13, 734, DateTimeKind.Utc).AddTicks(1812) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "HostUpdChange",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 7, 22, 12, 54, 25, 387, DateTimeKind.Utc).AddTicks(6463), new DateTime(2022, 8, 6, 12, 54, 25, 387, DateTimeKind.Utc).AddTicks(7199) });
        }
    }
}
