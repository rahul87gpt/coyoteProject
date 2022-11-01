using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class AddAllTransAndJNLHdrIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_DepartmentId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_ProductId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_JournalHeader_OutletId",
                table: "JournalHeader");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 6, 3, 11, 23, 42, 890, DateTimeKind.Utc).AddTicks(3968), new DateTime(2022, 6, 18, 11, 23, 42, 890, DateTimeKind.Utc).AddTicks(4520) });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Type",
                table: "Transaction",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_OutletRanking",
                table: "Transaction",
                columns: new[] { "DepartmentId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Trans_Tp_Date",
                table: "Transaction",
                columns: new[] { "Type", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_product_Date_outlet",
                table: "Transaction",
                columns: new[] { "ProductId", "Date", "OutletId" });

            migrationBuilder.CreateIndex(
                name: "IX_Trans_Tp_Del_Dt",
                table: "Transaction",
                columns: new[] { "Type", "IsDeleted", "Date" });

            migrationBuilder.CreateIndex(
                name: "ix_Transaction_type_outlet_Date",
                table: "Transaction",
                columns: new[] { "Type", "OutletId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDateProductTypeTillSequence",
                table: "Transaction",
                columns: new[] { "Date", "ProductId", "Type", "OutletId", "TillId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader",
                table: "JournalHeader",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "JHeade_Trading_Date",
                table: "JournalHeader",
                columns: new[] { "Date", "OutletId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_Date_Outlet_id",
                table: "JournalHeader",
                columns: new[] { "Id", "OutletId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_Outlet_Date_id",
                table: "JournalHeader",
                columns: new[] { "OutletId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Jhead_Status_TradeDate",
                table: "JournalHeader",
                columns: new[] { "Status", "TradingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_JHead_Tp_TrsDate",
                table: "JournalHeader",
                columns: new[] { "Type", "TradingDate" });

            migrationBuilder.CreateIndex(
                name: "<IX_JournalHeader_Olet_IsDel_HDate>",
                table: "JournalHeader",
                columns: new[] { "OutletId", "IsDeleted", "HeaderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_JHead_Olet_Tp_TradDate",
                table: "JournalHeader",
                columns: new[] { "OutletId", "Type", "TradingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_JHead_Tp_Sta_TradDate",
                table: "JournalHeader",
                columns: new[] { "Type", "Status", "TradingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_JHead_Status_Del_OletId_TradeDate",
                table: "JournalHeader",
                columns: new[] { "Status", "IsDeleted", "OutletId", "TradingDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_Type",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_transaction_OutletRanking",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Trans_Tp_Date",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_product_Date_outlet",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Trans_Tp_Del_Dt",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "ix_Transaction_type_outlet_Date",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDateProductTypeTillSequence",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_JournalHeader",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "JHeade_Trading_Date",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "IX_JournalHeader_Date_Outlet_id",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "IX_JournalHeader_Outlet_Date_id",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "IX_Jhead_Status_TradeDate",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "IX_JHead_Tp_TrsDate",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "<IX_JournalHeader_Olet_IsDel_HDate>",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "IX_JHead_Olet_Tp_TradDate",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "IX_JHead_Tp_Sta_TradDate",
                table: "JournalHeader");

            migrationBuilder.DropIndex(
                name: "IX_JHead_Status_Del_OletId_TradeDate",
                table: "JournalHeader");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 6, 3, 10, 25, 57, 613, DateTimeKind.Utc).AddTicks(3998), new DateTime(2022, 6, 18, 10, 25, 57, 613, DateTimeKind.Utc).AddTicks(4439) });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DepartmentId",
                table: "Transaction",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ProductId",
                table: "Transaction",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_OutletId",
                table: "JournalHeader",
                column: "OutletId");
        }
    }
}
