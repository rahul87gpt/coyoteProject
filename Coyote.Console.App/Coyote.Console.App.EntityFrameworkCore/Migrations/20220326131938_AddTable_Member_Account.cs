using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class AddTable_Member_Account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountsACC_NUMBER",
                table: "JournalDetail",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    ACC_NUMBER = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACC_CARD_NUMBER = table.Column<string>(nullable: true),
                    ACC_ADD_ID_1 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_2 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_3 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_4 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_5 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_6 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_7 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_8 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_9 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_10 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_11 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_12 = table.Column<string>(nullable: true),
                    ACC_ADD_ID_TYPE_1 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_2 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_3 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_4 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_5 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_6 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_7 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_8 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_9 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_10 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_11 = table.Column<int>(nullable: true),
                    ACC_ADD_ID_TYPE_12 = table.Column<int>(nullable: true),
                    ACC_MASTER_ACC = table.Column<int>(nullable: true),
                    ACC_TITLE = table.Column<string>(nullable: true),
                    ACC_FIRST_NAME = table.Column<string>(nullable: true),
                    ACC_SURNAME = table.Column<string>(nullable: true),
                    ACC_ADDR_1 = table.Column<string>(nullable: true),
                    ACC_ADDR_2 = table.Column<string>(nullable: true),
                    ACC_ADDR_3 = table.Column<string>(nullable: true),
                    ACC_POST_CODE = table.Column<string>(nullable: true),
                    ACC_ACCOUNT_NAME = table.Column<string>(nullable: true),
                    ACC_CONTACT = table.Column<string>(nullable: true),
                    ACC_PHONE = table.Column<string>(nullable: true),
                    ACC_MOBILE = table.Column<string>(nullable: true),
                    ACC_FAX = table.Column<string>(nullable: true),
                    ACC_EMAIL = table.Column<string>(nullable: true),
                    ACC_GENDER = table.Column<int>(nullable: true),
                    ACC_STATUS = table.Column<int>(nullable: true),
                    ACC_DATE_OF_BIRTH = table.Column<string>(nullable: true),
                    ACC_OCCUPATION = table.Column<string>(nullable: true),
                    ACC_DATE_ADDED = table.Column<string>(nullable: true),
                    ACC_DATE_CHANGE = table.Column<string>(nullable: true),
                    ACC_TILL_IND = table.Column<string>(nullable: true),
                    ACC_OUTLET = table.Column<string>(nullable: true),
                    ACC_ZONE = table.Column<string>(nullable: true),
                    ACC_PASSWORD = table.Column<string>(nullable: true),
                    ACC_EMPLOYEE_IND = table.Column<string>(nullable: true),
                    ACC_STATEMENT_TYPE = table.Column<string>(nullable: true),
                    ACC_PRICE_LEVEL = table.Column<string>(nullable: true),
                    ACC_DEL_NAME = table.Column<string>(nullable: true),
                    ACC_DEL_ADDR_1 = table.Column<string>(nullable: true),
                    ACC_DEL_ADDR_2 = table.Column<string>(nullable: true),
                    ACC_DEL_ADDR_3 = table.Column<string>(nullable: true),
                    ACC_DEL_POST_CODE = table.Column<string>(nullable: true),
                    ACC_ACCOUNT_CLASS = table.Column<string>(nullable: true),
                    ACC_DATE_JOINED = table.Column<DateTime>(nullable: true),
                    ACC_FINANCIAL_UNTIL = table.Column<DateTime>(nullable: true),
                    ACC_SUSPEND_UNTIL = table.Column<DateTime>(nullable: true),
                    ACC_SUSPEND_REASON = table.Column<string>(nullable: true),
                    ACC_NOTES_LINE_1 = table.Column<string>(nullable: true),
                    ACC_NOTES_LINE_2 = table.Column<string>(nullable: true),
                    ACC_NOTES_LINE_3 = table.Column<string>(nullable: true),
                    ACC_NOTES_LINE_4 = table.Column<string>(nullable: true),
                    ACC_NOTES_LINE_5 = table.Column<string>(nullable: true),
                    ACC_NOTES_LINE_6 = table.Column<string>(nullable: true),
                    ACC_SEND_PROMO_INFO_IND = table.Column<string>(nullable: true),
                    ACC_FLAGS = table.Column<string>(nullable: true),
                    ACC_Last_Modified_Date = table.Column<string>(nullable: true),
                    ACC_OWL_CLUB_USER = table.Column<byte>(nullable: false),
                    ACC_ADDR_UNIT_NUMBER = table.Column<string>(nullable: true),
                    ACC_ADDR_STREET_NUMBER = table.Column<string>(nullable: true),
                    ACC_ADDR_CITY = table.Column<string>(nullable: true),
                    ACC_ADDR_STATE = table.Column<string>(nullable: true),
                    ACC_DELETED = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.ACC_NUMBER);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MEMB_NUMBER = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MEMB_LOYALTY_TYPE = table.Column<float>(nullable: true),
                    MEMB_ACCUM_POINTS_IND = table.Column<string>(nullable: true),
                    MEMB_POINTS_BALANCE = table.Column<float>(nullable: true),
                    MEMB_FLAGS = table.Column<string>(nullable: true),
                    MEMB_Last_Modified_Date = table.Column<DateTime>(nullable: true),
                    MEMB_Exclude_From_Competitions = table.Column<bool>(nullable: false),
                    Memb_Home_Store = table.Column<int>(nullable: true),
                    MEMB_OUTLET = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MEMB_NUMBER);
                });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 16, 13, 19, 32, 475, DateTimeKind.Utc).AddTicks(2943), new DateTime(2022, 3, 31, 13, 19, 32, 475, DateTimeKind.Utc).AddTicks(3611) });

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_AccountsACC_NUMBER",
                table: "JournalDetail",
                column: "AccountsACC_NUMBER");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalDetail_Account_AccountsACC_NUMBER",
                table: "JournalDetail",
                column: "AccountsACC_NUMBER",
                principalTable: "Account",
                principalColumn: "ACC_NUMBER",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalDetail_Account_AccountsACC_NUMBER",
                table: "JournalDetail");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropIndex(
                name: "IX_JournalDetail_AccountsACC_NUMBER",
                table: "JournalDetail");

            migrationBuilder.DropColumn(
                name: "AccountsACC_NUMBER",
                table: "JournalDetail");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateFrom", "DateTo" },
                values: new object[] { new DateTime(2022, 3, 15, 13, 31, 58, 470, DateTimeKind.Utc).AddTicks(5070), new DateTime(2022, 3, 30, 13, 31, 58, 470, DateTimeKind.Utc).AddTicks(5985) });
        }
    }
}
