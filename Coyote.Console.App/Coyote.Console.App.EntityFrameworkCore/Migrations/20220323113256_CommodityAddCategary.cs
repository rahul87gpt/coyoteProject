using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class CommodityAddCategary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Commodity",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 13, 11, 32, 51, 291, DateTimeKind.Utc).AddTicks(7139), new DateTime(2022, 3, 28, 11, 32, 51, 291, DateTimeKind.Utc).AddTicks(7745) });

            migrationBuilder.CreateIndex(
                name: "IX_Commodity_CategoryId",
                table: "Commodity",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commodity_MasterListItems_Id",
                table: "Commodity",
                column: "CategoryId",
                principalTable: "MasterListItems",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commodity_MasterListItems_Id",
                table: "Commodity");

            migrationBuilder.DropIndex(
                name: "IX_Commodity_CategoryId",
                table: "Commodity");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Commodity");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 2, 14, 13, 49, 5, 358, DateTimeKind.Utc).AddTicks(4902), new DateTime(2022, 3, 1, 13, 49, 5, 358, DateTimeKind.Utc).AddTicks(5487) });
        }
    }
}
