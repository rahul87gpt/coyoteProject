using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class AddProductId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Product",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 15, 8, 12, 4, 157, DateTimeKind.Utc).AddTicks(6799), new DateTime(2022, 5, 30, 8, 12, 4, 157, DateTimeKind.Utc).AddTicks(7537) });

            migrationBuilder.CreateIndex(
                name: "IX_Product_ParentId",
                table: "Product",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Product_ParentId",
                table: "Product",
                column: "ParentId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Product_ParentId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ParentId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Product");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 14, 8, 42, 58, 276, DateTimeKind.Utc).AddTicks(3960), new DateTime(2022, 5, 29, 8, 42, 58, 276, DateTimeKind.Utc).AddTicks(4678) });
        }
    }
}
