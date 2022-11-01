using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class removeindexfromuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserName_Delete_Unique",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "User",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 22, 12, 29, 49, 945, DateTimeKind.Utc).AddTicks(9135), new DateTime(2022, 4, 6, 12, 29, 49, 945, DateTimeKind.Utc).AddTicks(9793) });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "PlainPassword",
                value: "cdn@12345");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Type",
                table: "User",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 20, 10, 54, 31, 982, DateTimeKind.Utc).AddTicks(4186), new DateTime(2022, 4, 4, 10, 54, 31, 982, DateTimeKind.Utc).AddTicks(4806) });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "PlainPassword",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_UserName_Delete_Unique",
                table: "User",
                columns: new[] { "UserName", "IsDeleted" },
                unique: true,
                filter: "UserName IS NOT NULL AND IsDeleted = 0");
        }
    }
}
