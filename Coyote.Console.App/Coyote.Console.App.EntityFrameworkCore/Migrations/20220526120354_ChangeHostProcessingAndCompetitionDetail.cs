using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class ChangeHostProcessingAndCompetitionDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TimeStamp",
                table: "HOSTUPD",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "TriggerCount",
                table: "CompetitionDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 16, 12, 3, 48, 9, DateTimeKind.Utc).AddTicks(44), new DateTime(2022, 5, 31, 12, 3, 48, 9, DateTimeKind.Utc).AddTicks(781) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TriggerCount",
                table: "CompetitionDetail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "HOSTUPD",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 15, 8, 12, 4, 157, DateTimeKind.Utc).AddTicks(6799), new DateTime(2022, 5, 30, 8, 12, 4, 157, DateTimeKind.Utc).AddTicks(7537) });
        }
    }
}
