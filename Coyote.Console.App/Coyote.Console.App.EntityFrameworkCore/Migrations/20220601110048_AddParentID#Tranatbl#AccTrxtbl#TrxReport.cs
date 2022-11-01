using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class AddParentIDTranatblAccTrxtblTrxReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "TransactionReport",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "AccountTransaction",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 22, 11, 0, 44, 727, DateTimeKind.Utc).AddTicks(1233), new DateTime(2022, 6, 6, 11, 0, 44, 727, DateTimeKind.Utc).AddTicks(1673) });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_ParentId",
                table: "TransactionReport",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ParentId",
                table: "Transaction",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_ParentId",
                table: "AccountTransaction",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTransaction_AccountTransaction_ParentId",
                table: "AccountTransaction",
                column: "ParentId",
                principalTable: "AccountTransaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Transaction_ParentId",
                table: "Transaction",
                column: "ParentId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionReport_TransactionReport_ParentId",
                table: "TransactionReport",
                column: "ParentId",
                principalTable: "TransactionReport",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTransaction_AccountTransaction_ParentId",
                table: "AccountTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Transaction_ParentId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionReport_TransactionReport_ParentId",
                table: "TransactionReport");

            migrationBuilder.DropIndex(
                name: "IX_TransactionReport_ParentId",
                table: "TransactionReport");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_ParentId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_AccountTransaction_ParentId",
                table: "AccountTransaction");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "TransactionReport");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "AccountTransaction");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 5, 16, 12, 3, 48, 9, DateTimeKind.Utc).AddTicks(44), new DateTime(2022, 5, 31, 12, 3, 48, 9, DateTimeKind.Utc).AddTicks(781) });
        }
    }
}
