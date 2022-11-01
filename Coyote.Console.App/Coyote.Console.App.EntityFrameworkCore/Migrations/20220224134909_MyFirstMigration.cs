using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coyote.Console.App.EntityFrameworkCore.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BulkPrintLabelFromTabletTbl",
                columns: table => new
                {
                    BPLT_Outlet = table.Column<int>(nullable: false),
                    BPLT_Product_Number = table.Column<float>(nullable: false),
                    BPLT_Print_Batch = table.Column<DateTime>(type: "datetime", nullable: false),
                    BPLT_Last_Import = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "PrintLabelFromTabletTbl",
                columns: table => new
                {
                    PLT_Outlet = table.Column<int>(nullable: false),
                    PLT_Product_Number = table.Column<float>(nullable: false),
                    PLT_Print_Batch = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 20, nullable: false),
                    MiddleName = table.Column<string>(maxLength: 20, nullable: true),
                    LastName = table.Column<string>(maxLength: 20, nullable: false),
                    Password = table.Column<string>(nullable: true),
                    MobileNo = table.Column<string>(maxLength: 15, nullable: true),
                    PhoneNo = table.Column<string>(maxLength: 15, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: false),
                    Address1 = table.Column<string>(maxLength: 30, nullable: true),
                    Address2 = table.Column<string>(maxLength: 30, nullable: true),
                    Address3 = table.Column<string>(maxLength: 30, nullable: true),
                    PostCode = table.Column<string>(maxLength: 4, nullable: true),
                    Gender = table.Column<string>(maxLength: 13, nullable: true),
                    Status = table.Column<bool>(maxLength: 8, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: true),
                    ZoneIds = table.Column<string>(nullable: true),
                    StoreIds = table.Column<string>(nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    PromoPrefix = table.Column<string>(nullable: true),
                    KeypadPrefix = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsResetPassword = table.Column<bool>(nullable: false),
                    TemporaryPassword = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    TempPasswordCreatedAt = table.Column<DateTime>(nullable: true),
                    PlainPassword = table.Column<string>(nullable: true),
                    AddUnlockProduct = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_User_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_User_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountLoyalty",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<int>(maxLength: 10, nullable: false),
                    Comp = table.Column<string>(maxLength: 15, nullable: false),
                    CycleStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    CycleEnd = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(maxLength: 15, nullable: true),
                    LastTransactionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TriggerType = table.Column<int>(nullable: true),
                    TriggCompliance = table.Column<float>(maxLength: 5000, nullable: true),
                    RewardType = table.Column<int>(nullable: true),
                    RewardInfo = table.Column<string>(maxLength: 15, nullable: true),
                    RewardDate = table.Column<DateTime>(nullable: true),
                    LoyalityPoints = table.Column<float>(nullable: true),
                    RedeemInfo = table.Column<string>(maxLength: 15, nullable: true),
                    RedeemDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RedeemExp = table.Column<DateTime>(type: "datetime", nullable: true),
                    RedeemCount = table.Column<int>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLoyalty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_AccountLoyalty_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_AccountLoyalty_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CSCPERIOD",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Heading = table.Column<string>(maxLength: 25, nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSCPERIOD", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CSCPeriod_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CSCPeriod_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: true),
                    Subject = table.Column<string>(maxLength: 100, nullable: true),
                    Body = table.Column<string>(maxLength: 1000, nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_EmailTemplate_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_EmailTemplate_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HOSTUPD",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<long>(nullable: false),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    Description = table.Column<string>(maxLength: 80, nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Posted = table.Column<bool>(nullable: true),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOSTUPD", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HostProcessing_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostProcessing_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MasterList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    AccessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_MasterList_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_MasterList_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PrintLabelType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    Desc = table.Column<string>(maxLength: 60, nullable: false),
                    LablesPerPage = table.Column<int>(nullable: false),
                    PrintBarCodeType = table.Column<string>(maxLength: 30, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintLabelType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PrintLabelType_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PrintLabelType_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    PermissionSet = table.Column<string>(nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PermissionDeptSet = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_role_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_role_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolesDefaultPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(maxLength: 100, nullable: false),
                    HttpVerb = table.Column<string>(maxLength: 6, nullable: false),
                    DefaultRolePermissionss = table.Column<string>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesDefaultPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_Default_Permission_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Role_Default_Permission_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SendEmail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromAddress = table.Column<string>(maxLength: 100, nullable: true),
                    ToAddress = table.Column<string>(maxLength: 100, nullable: true),
                    CC = table.Column<string>(maxLength: 100, nullable: true),
                    BCC = table.Column<string>(maxLength: 100, nullable: true),
                    MsgBody = table.Column<string>(maxLength: 1000, nullable: true),
                    TemplateId = table.Column<int>(nullable: false),
                    TemplateName = table.Column<string>(maxLength: 100, nullable: true),
                    TemplateContent = table.Column<string>(maxLength: 1000, nullable: true),
                    EmailSubject = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsSendEmail = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_SendEmail_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_SendEmail_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StoreGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Store_Group_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Store_Group_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    Desc = table.Column<string>(maxLength: 80, nullable: false),
                    Address1 = table.Column<string>(maxLength: 80, nullable: true),
                    Address2 = table.Column<string>(maxLength: 80, nullable: true),
                    Address3 = table.Column<string>(maxLength: 80, nullable: true),
                    Address4 = table.Column<string>(maxLength: 80, nullable: true),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Fax = table.Column<string>(maxLength: 20, nullable: true),
                    Email = table.Column<string>(maxLength: 30, nullable: true),
                    ABN = table.Column<string>(maxLength: 15, nullable: true),
                    UpdateCost = table.Column<string>(maxLength: 3, nullable: true),
                    PromoSupplier = table.Column<string>(maxLength: 30, nullable: true),
                    Contact = table.Column<string>(maxLength: 30, nullable: true),
                    CostZone = table.Column<string>(maxLength: 30, nullable: true),
                    GSTFreeItemCode = table.Column<string>(maxLength: 30, nullable: true),
                    GSTFreeItemDesc = table.Column<string>(maxLength: 80, nullable: true),
                    GSTInclItemCode = table.Column<string>(maxLength: 30, nullable: true),
                    GSTInclItemDesc = table.Column<string>(maxLength: 80, nullable: true),
                    XeroName = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Supplier_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Supplier_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemControls",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    SerialNo = table.Column<string>(maxLength: 30, nullable: false),
                    LicenceKey = table.Column<string>(maxLength: 30, nullable: false),
                    MaxStores = table.Column<int>(nullable: true),
                    TillJournal = table.Column<int>(nullable: false),
                    AllocateGroups = table.Column<bool>(nullable: true),
                    MassPriceUpdate = table.Column<string>(maxLength: 30, nullable: false),
                    AllowALM = table.Column<bool>(nullable: true),
                    DatabaseUsage = table.Column<string>(nullable: true),
                    GrowthFactor = table.Column<float>(nullable: false),
                    AllowFIFO = table.Column<bool>(nullable: false),
                    Color = table.Column<string>(maxLength: 30, nullable: false),
                    TransactionRef = table.Column<string>(maxLength: 30, nullable: true),
                    WastageRef = table.Column<string>(maxLength: 30, nullable: true),
                    TransferRef = table.Column<string>(maxLength: 30, nullable: true),
                    NumberFactor = table.Column<long>(nullable: false),
                    HostUpdatePricing = table.Column<bool>(nullable: false),
                    InvoicePostPricing = table.Column<bool>(nullable: false),
                    PriceRounding = table.Column<int>(nullable: false),
                    DefaultItemPricing = table.Column<int>(nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemControls", x => x.ID);
                    table.ForeignKey(
                        name: "FK_User_SysControl_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_SysControl_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tax",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Desc = table.Column<string>(maxLength: 30, nullable: false),
                    Factor = table.Column<float>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Tax_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Tax_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activity = table.Column<string>(maxLength: 50, nullable: true),
                    ActivityType = table.Column<string>(maxLength: 50, nullable: true),
                    Module = table.Column<string>(nullable: true),
                    Table = table.Column<string>(nullable: true),
                    TableId = table.Column<long>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    DataLog = table.Column<string>(nullable: true),
                    ActionBy = table.Column<int>(nullable: false),
                    UserRole = table.Column<string>(nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    RoleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserLog_Created_By",
                        column: x => x.ActionBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MasterListItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Col1 = table.Column<string>(maxLength: 50, nullable: true),
                    Col2 = table.Column<string>(maxLength: 50, nullable: true),
                    Col3 = table.Column<string>(maxLength: 50, nullable: true),
                    Col4 = table.Column<string>(maxLength: 50, nullable: true),
                    Col5 = table.Column<string>(maxLength: 50, nullable: true),
                    Num1 = table.Column<double>(nullable: true),
                    Num2 = table.Column<double>(nullable: true),
                    Num3 = table.Column<double>(nullable: true),
                    Num4 = table.Column<double>(nullable: true),
                    Num5 = table.Column<double>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    AccessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_MasterList_Items_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterList_MasterListItems_Id",
                        column: x => x.ListId,
                        principalTable: "MasterList",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_MasterList_Items_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    Desc = table.Column<string>(maxLength: 80, nullable: false),
                    MapTypeId = table.Column<int>(nullable: true),
                    BudgetGroethFactor = table.Column<double>(nullable: true),
                    RoyaltyDisc = table.Column<double>(nullable: true),
                    AdvertisingDisc = table.Column<double>(nullable: true),
                    AllowSaleDisc = table.Column<bool>(nullable: true),
                    ExcludeWastageOptimalOrdering = table.Column<bool>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Department_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_Department_Dept__Map_Type_Id",
                        column: x => x.MapTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Department_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ManualSale",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Desc = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualSale", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ManualSale_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Type_ManualSale_Type",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_ManualSale_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceId = table.Column<string>(maxLength: 20, nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    ReferenceType = table.Column<string>(maxLength: 20, nullable: false),
                    ReferenceOverrideType = table.Column<string>(maxLength: 20, nullable: true),
                    DisplayType = table.Column<string>(maxLength: 20, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    POSMessage = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    DayParts = table.Column<string>(maxLength: 24, nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Image = table.Column<byte[]>(nullable: true),
                    ImageType = table.Column<string>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_POSMsg_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_POSMsg_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Zone_POSMsg_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ModuleActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Action = table.Column<string>(nullable: false),
                    ActionTypeId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ControllerActionsAction_Type",
                        column: x => x.ActionTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_ControllerActionsCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_ControllerActionsModule_Id",
                        column: x => x.ModuleId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_ControllerActionsUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 25, nullable: false),
                    PromotionTypeId = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    RptGroup = table.Column<string>(maxLength: 15, nullable: true),
                    FrequencyId = table.Column<int>(nullable: true),
                    Availibility = table.Column<string>(maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionFrequency_MasterListItem",
                        column: x => x.FrequencyId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionType_MasterListItem",
                        column: x => x.PromotionTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionSource_MasterListItem",
                        column: x => x.SourceId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionZone_MasterListItem",
                        column: x => x.ZoneId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RebateHeader",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    ManufacturerId = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RebateHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ReabateHeader_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rebate_Manufacturer",
                        column: x => x.ManufacturerId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_RebateHeader_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rebate_Zone",
                        column: x => x.ZoneId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportScheduler",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilterName = table.Column<string>(nullable: true),
                    ReportId = table.Column<int>(nullable: false),
                    InceptionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    InceptionTime = table.Column<string>(maxLength: 10, nullable: true),
                    IntervalInd = table.Column<int>(nullable: false),
                    IntervalBracket = table.Column<int>(nullable: false),
                    LastRun = table.Column<DateTime>(nullable: true),
                    ExcelExport = table.Column<bool>(nullable: true),
                    PdfExport = table.Column<bool>(nullable: true),
                    CsvExport = table.Column<bool>(nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StoreIds = table.Column<string>(nullable: true),
                    ProductStartId = table.Column<long>(nullable: true),
                    ProductEndId = table.Column<long>(nullable: true),
                    TillId = table.Column<string>(nullable: true),
                    CashierId = table.Column<long>(nullable: true),
                    ProductIds = table.Column<string>(nullable: true),
                    CommodityIds = table.Column<string>(nullable: true),
                    DepartmentIds = table.Column<string>(nullable: true),
                    CategoryIds = table.Column<string>(nullable: true),
                    GroupIds = table.Column<string>(nullable: true),
                    SupplierIds = table.Column<string>(nullable: true),
                    ManufacturerIds = table.Column<string>(nullable: true),
                    TransactionTypes = table.Column<string>(nullable: true),
                    ZoneIds = table.Column<string>(nullable: true),
                    DayRange = table.Column<string>(nullable: true),
                    IsPromoSale = table.Column<bool>(nullable: true),
                    PromoCode = table.Column<string>(nullable: true),
                    MemberIds = table.Column<string>(nullable: true),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false),
                    NillTransactionInterval = table.Column<int>(nullable: true),
                    Continuous = table.Column<bool>(nullable: false),
                    DrillDown = table.Column<bool>(nullable: false),
                    Summary = table.Column<bool>(nullable: false),
                    StoreId = table.Column<long>(nullable: false),
                    SupplierId = table.Column<string>(nullable: true),
                    InvoiceDateFrom = table.Column<DateTime>(nullable: false),
                    InvoiceDateTo = table.Column<DateTime>(nullable: false),
                    IsHostCost = table.Column<bool>(nullable: false),
                    IsNormalCost = table.Column<bool>(nullable: false),
                    IsSupplierBatchCost = table.Column<bool>(nullable: false),
                    SupplierBatch = table.Column<string>(nullable: true),
                    stockNationalRange = table.Column<string>(nullable: true),
                    SalesAMT = table.Column<int>(nullable: false),
                    salesAMTRange = table.Column<float>(nullable: false),
                    PromotionIds = table.Column<string>(nullable: true),
                    PeriodicReportType = table.Column<string>(nullable: true),
                    PromoIds = table.Column<string>(nullable: true),
                    TillIds = table.Column<string>(nullable: true),
                    Chart = table.Column<bool>(nullable: false),
                    OutLetId = table.Column<int>(nullable: false),
                    Inline = table.Column<bool>(nullable: false),
                    WithVar = table.Column<bool>(nullable: false),
                    stockNegativeOH = table.Column<bool>(nullable: false),
                    stockSOHLevel = table.Column<bool>(nullable: false),
                    stockSOHButNoSales = table.Column<bool>(nullable: false),
                    stockLowWarn = table.Column<bool>(nullable: false),
                    stockNoOfDaysWarn = table.Column<int>(nullable: true),
                    SplitOverOutlet = table.Column<bool>(nullable: false),
                    OrderByAmt = table.Column<bool>(nullable: false),
                    OrderByQty = table.Column<bool>(nullable: false),
                    OrderByGP = table.Column<bool>(nullable: false),
                    OrderByMargin = table.Column<bool>(nullable: false),
                    OrderBySOH = table.Column<bool>(nullable: false),
                    OrderByAlp = table.Column<bool>(nullable: false),
                    SalesSOH = table.Column<int>(nullable: true),
                    salesSOHRange = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportScheduler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportScheduler_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportScheduler_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_Report",
                        column: x => x.ReportId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    HostFormatId = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Warehouse_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Warehouse_MasterList_Items",
                        column: x => x.HostFormatId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_Warehouse",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Warehouse_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessDepartment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(nullable: false),
                    RolesId = table.Column<int>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessDepartment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_AccessDepartment_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccessDepartment_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessDepartment_Role_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_AccessDepartment_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Commodity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    Desc = table.Column<string>(maxLength: 80, nullable: false),
                    CoverDays = table.Column<int>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: false),
                    GPPcntLevel1 = table.Column<double>(nullable: true),
                    GPPcntLevel2 = table.Column<double>(nullable: true),
                    GPPcntLevel3 = table.Column<double>(nullable: true),
                    GPPcntLevel4 = table.Column<double>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commodity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Commodity_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commodity_Department_Dept_Id",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Commodity_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompetitionDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    ResetCycleId = table.Column<int>(nullable: false),
                    LoyaltyFactor = table.Column<float>(nullable: false),
                    ComplDiscount = table.Column<float>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true),
                    PosReceiptPrint = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(maxLength: 100, nullable: true),
                    RewardTypeId = table.Column<int>(nullable: false),
                    Discount = table.Column<float>(nullable: true),
                    RewardExpiration = table.Column<int>(nullable: true),
                    ResetCycle = table.Column<bool>(nullable: false),
                    ForcePrint = table.Column<bool>(nullable: false),
                    TriggerTypeId = table.Column<int>(nullable: false),
                    ActivationPoints = table.Column<float>(nullable: true),
                    RewardThreshold = table.Column<float>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_CompetitionDetailsCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetitionDetails_Promotion",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetitionDetailsReset_MasterListItem",
                        column: x => x.ResetCycleId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetitionDetailsRewardType_MasterListItem",
                        column: x => x.RewardTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetitionDetailsTriggerType_MasterListItem",
                        column: x => x.TriggerTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetitionDetailsType_MasterListItem",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_CompetitionDetailsUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetitionDetailsZone_MasterListItem",
                        column: x => x.ZoneId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoMixmatch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionId = table.Column<int>(nullable: false),
                    Qty1 = table.Column<float>(nullable: true),
                    Amt1 = table.Column<float>(nullable: true),
                    DiscPcnt1 = table.Column<float>(nullable: true),
                    PriceLevel1 = table.Column<int>(nullable: true),
                    Qty2 = table.Column<float>(nullable: true),
                    Amt2 = table.Column<float>(nullable: true),
                    DiscPcnt2 = table.Column<float>(nullable: true),
                    PriceLevel2 = table.Column<int>(nullable: true),
                    CumulativeOffer = table.Column<bool>(nullable: false),
                    Group = table.Column<short>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoMixmatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionMixmatchCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_PromotionMixmatch",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionMixmatchUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoOffer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionId = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    TotalQty = table.Column<float>(nullable: true),
                    TotalPrice = table.Column<float>(nullable: true),
                    Group1Qty = table.Column<float>(nullable: true),
                    Group2Qty = table.Column<float>(nullable: true),
                    Group3Qty = table.Column<float>(nullable: true),
                    Group1Price = table.Column<string>(maxLength: 30, nullable: true),
                    Group2Price = table.Column<string>(maxLength: 30, nullable: true),
                    Group3Price = table.Column<string>(maxLength: 30, nullable: true),
                    Group = table.Column<short>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionOfferCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_PromotionOffer",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionOfferUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportSchedulerLog",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    SchedulerId = table.Column<long>(nullable: false),
                    IsEmailSent = table.Column<bool>(nullable: true),
                    IsReported = table.Column<bool>(nullable: true),
                    EmailTryCount = table.Column<int>(nullable: true),
                    IsReportGenerated = table.Column<bool>(nullable: false),
                    ReportTryCount = table.Column<int>(nullable: true),
                    EmailTemplate = table.Column<string>(nullable: true),
                    ReportGenerated = table.Column<byte[]>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSchedulerLog", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SchedulerLog_Scheduler",
                        column: x => x.SchedulerId,
                        principalTable: "ReportScheduler",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SchedulerUserLog_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SchedulerUser",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    SchedulerId = table.Column<long>(nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SchedulerUser_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchedulerUser_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchedulerUser_Scheduler",
                        column: x => x.SchedulerId,
                        principalTable: "ReportScheduler",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SchedulerUser_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JournalDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalHeaderId = table.Column<long>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 10, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    ProductId = table.Column<long>(nullable: true),
                    ProductNumber = table.Column<long>(nullable: true),
                    Quantity = table.Column<float>(maxLength: 50, nullable: false),
                    Amount = table.Column<float>(nullable: false),
                    DiscountAmount = table.Column<float>(nullable: false),
                    GSTAmount = table.Column<float>(nullable: false),
                    Cost = table.Column<float>(nullable: false),
                    GSTCost = table.Column<float>(nullable: false),
                    CashierId = table.Column<int>(nullable: false),
                    CashierNumber = table.Column<long>(nullable: true),
                    PriceLevel = table.Column<int>(nullable: false),
                    PromoSellId = table.Column<int>(nullable: true),
                    PromoSellCode = table.Column<string>(maxLength: 25, nullable: true),
                    PromoMixMatchId = table.Column<int>(nullable: true),
                    PromoMixCode = table.Column<string>(maxLength: 25, nullable: true),
                    PromoOfferId = table.Column<int>(nullable: true),
                    PromoOfferCode = table.Column<string>(maxLength: 25, nullable: true),
                    PromoCompId = table.Column<int>(nullable: true),
                    PromoCompCode = table.Column<string>(maxLength: 25, nullable: true),
                    PromoMemeberOfferId = table.Column<int>(nullable: true),
                    PromoMemberOfferCode = table.Column<string>(maxLength: 25, nullable: true),
                    PostStatus = table.Column<bool>(nullable: false),
                    APNNumber = table.Column<long>(nullable: true),
                    APNSold = table.Column<long>(nullable: true),
                    LoyaltyInfo = table.Column<string>(nullable: true),
                    ReferenceType = table.Column<string>(maxLength: 25, nullable: true),
                    Reference = table.Column<string>(maxLength: 64, nullable: true),
                    TermsRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    TermsRebate = table.Column<float>(nullable: true),
                    PromoScanRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    PromoScanRebate = table.Column<float>(nullable: true),
                    PromoPurchaseRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    PromoPurchaseRebate = table.Column<float>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    AccountNumber = table.Column<int>(nullable: true),
                    JournalDetailDate = table.Column<int>(nullable: true),
                    JournalDetailTime = table.Column<int>(nullable: true),
                    OutletCode = table.Column<string>(maxLength: 30, nullable: true),
                    TillCode = table.Column<string>(maxLength: 15, nullable: true),
                    TransactionNo = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Journal_Detail_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_Journal_Detail_Promotion_comp",
                        column: x => x.PromoCompId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_Journal_Detail_Promotion_MemberOffer",
                        column: x => x.PromoMemeberOfferId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_Journal_Detail_Promotion_MixMatch",
                        column: x => x.PromoMixMatchId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_Journal_Detail_Promotion_Offer",
                        column: x => x.PromoOfferId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_Journal_Detail_Promotion_Sell",
                        column: x => x.PromoSellId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Journal_Detail_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JournalHeader",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeaderDate = table.Column<int>(nullable: false),
                    HeaderTime = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    OutletCode = table.Column<string>(maxLength: 30, nullable: true),
                    TillId = table.Column<int>(nullable: false),
                    TillCode = table.Column<string>(nullable: true),
                    TransactionNo = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 10, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    TransactionAmount = table.Column<float>(nullable: false),
                    TransactionGST = table.Column<float>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    TradingDate = table.Column<DateTime>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    CashierId = table.Column<int>(nullable: false),
                    CashierNumber = table.Column<long>(nullable: true),
                    PostStatus = table.Column<bool>(nullable: false),
                    TransactionTimeStamp = table.Column<DateTime>(nullable: true),
                    ProcessStatus = table.Column<bool>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Journal_Header_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Journal_Header_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(maxLength: 10, nullable: false),
                    ProductId = table.Column<long>(nullable: true),
                    ProductNumber = table.Column<long>(nullable: true),
                    OutletId = table.Column<int>(nullable: true),
                    OutletCode = table.Column<string>(nullable: true),
                    TillId = table.Column<int>(nullable: true),
                    TillCode = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: true),
                    SupplierCode = table.Column<string>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    ManufacturerCode = table.Column<string>(nullable: true),
                    Group = table.Column<int>(nullable: true),
                    GroupCode = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    CommodityId = table.Column<int>(nullable: true),
                    CommodityCode = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    CategoryCode = table.Column<string>(nullable: true),
                    SubRange = table.Column<int>(nullable: true),
                    SubRangeCode = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(maxLength: 10, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Qty = table.Column<float>(nullable: false),
                    Amt = table.Column<float>(nullable: false),
                    ExGSTAmt = table.Column<float>(nullable: false),
                    Cost = table.Column<float>(nullable: false),
                    ExGSTCost = table.Column<float>(nullable: false),
                    Discount = table.Column<float>(nullable: false),
                    Price = table.Column<float>(nullable: true),
                    PromoBuyId = table.Column<int>(nullable: true),
                    PromoBuyCode = table.Column<string>(nullable: true),
                    PromoSellId = table.Column<int>(nullable: true),
                    PromoSellCode = table.Column<string>(nullable: true),
                    Weekend = table.Column<DateTime>(nullable: false),
                    Day = table.Column<string>(maxLength: 3, nullable: false),
                    NewOnHand = table.Column<float>(nullable: true),
                    Member = table.Column<long>(nullable: true),
                    Points = table.Column<float>(nullable: true),
                    CartonQty = table.Column<float>(nullable: true),
                    UnitQty = table.Column<float>(nullable: true),
                    Parent = table.Column<float>(nullable: true),
                    StockMovement = table.Column<float>(nullable: true),
                    Tender = table.Column<string>(maxLength: 15, nullable: true),
                    ManualInd = table.Column<bool>(nullable: true),
                    GLAccount = table.Column<string>(maxLength: 15, nullable: true),
                    GLPostedInd = table.Column<bool>(nullable: true),
                    PromoSales = table.Column<float>(nullable: false),
                    PromoSalesGST = table.Column<float>(nullable: false),
                    Debtor = table.Column<float>(nullable: false),
                    Flags = table.Column<string>(maxLength: 400, nullable: true),
                    ReferenceType = table.Column<string>(maxLength: 25, nullable: true),
                    ReferenceNumber = table.Column<string>(maxLength: 64, nullable: true),
                    Comp = table.Column<string>(maxLength: 64, nullable: true),
                    TriggCompliance = table.Column<float>(nullable: true),
                    RedeemInfo = table.Column<string>(maxLength: 64, nullable: true),
                    RedeemDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RewardInfo = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: true),
                    TransactionNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterListItem_AccountTransaction_Category_Id",
                        column: x => x.CategoryId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commodity_AccountTransaction_Commodity_Id",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_AccountTransaction_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Department_AccountTransaction_Department_Id",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_AccountTransaction_Manufacturer_Id",
                        column: x => x.ManufacturerId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_AccountTransaction_Promotion_Buy",
                        column: x => x.PromoBuyId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_AccountTransaction_Promotion_Sell",
                        column: x => x.PromoSellId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_AccountTransaction_Supplier_Id",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_AccountTransaction_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_AccountTransaction_User_Id",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    PosDesc = table.Column<string>(maxLength: 20, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CartonQty = table.Column<int>(nullable: false),
                    UnitQty = table.Column<float>(nullable: false),
                    CartonCost = table.Column<float>(nullable: false),
                    DepartmentId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    CommodityId = table.Column<int>(nullable: false),
                    TaxId = table.Column<long>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    ManufacturerId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    NationalRangeId = table.Column<int>(nullable: false),
                    UnitMeasureId = table.Column<int>(nullable: true),
                    AccessOutletIds = table.Column<string>(nullable: true),
                    ScaleInd = table.Column<bool>(nullable: true),
                    GmFlagInd = table.Column<bool>(nullable: true),
                    SlowMovingInd = table.Column<bool>(nullable: true),
                    WarehouseFrozenInd = table.Column<bool>(nullable: true),
                    StoreFrozenInd = table.Column<bool>(nullable: true),
                    AustMadeInd = table.Column<bool>(nullable: true),
                    AustOwnedInd = table.Column<bool>(nullable: true),
                    OrganicInd = table.Column<bool>(nullable: true),
                    HeartSmartInd = table.Column<bool>(nullable: true),
                    GenericInd = table.Column<bool>(nullable: true),
                    SeasonalInd = table.Column<bool>(nullable: true),
                    Parent = table.Column<long>(nullable: true),
                    TareWeight = table.Column<float>(nullable: true),
                    LabelQty = table.Column<float>(nullable: true),
                    Replicate = table.Column<string>(maxLength: 15, nullable: true),
                    Freight = table.Column<string>(maxLength: 15, nullable: true),
                    Size = table.Column<string>(maxLength: 8, nullable: true),
                    Info = table.Column<string>(maxLength: 50, nullable: true),
                    Litres = table.Column<float>(nullable: true),
                    VarietyInd = table.Column<bool>(nullable: true),
                    HostNumber = table.Column<string>(maxLength: 15, nullable: true),
                    HostNumber2 = table.Column<string>(maxLength: 15, nullable: true),
                    HostNumber3 = table.Column<string>(maxLength: 15, nullable: true),
                    HostNumber4 = table.Column<string>(maxLength: 15, nullable: true),
                    HostItemType = table.Column<string>(maxLength: 1, nullable: true),
                    HostItemType2 = table.Column<string>(maxLength: 1, nullable: true),
                    HostItemType3 = table.Column<string>(maxLength: 1, nullable: true),
                    HostItemType4 = table.Column<string>(maxLength: 1, nullable: true),
                    HostItemType5 = table.Column<string>(maxLength: 1, nullable: true),
                    LastApnSold = table.Column<long>(nullable: true),
                    Rrp = table.Column<float>(nullable: true),
                    AltSupplier = table.Column<bool>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategory_MasterList_Items",
                        column: x => x.CategoryId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commodity_Product",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Product_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Department_Product",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductGroup_MasterList_Items",
                        column: x => x.GroupId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductManufacturer_MasterList_Items",
                        column: x => x.ManufacturerId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductNationalRange_MasterList_Items",
                        column: x => x.NationalRangeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_Product",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tax_Product",
                        column: x => x.TaxId,
                        principalTable: "Tax",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductType_MasterList_Items",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductUnitMeasure_MasterList_Items",
                        column: x => x.UnitMeasureId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Product_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "APN",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    SoldDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Desc = table.Column<string>(maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_APN_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_APN_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_APN_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EPAY",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Item = table.Column<string>(maxLength: 64, nullable: false),
                    ProductID = table.Column<long>(nullable: true),
                    TIMESTAMP = table.Column<DateTime>(nullable: false),
                    ProductTypeCode = table.Column<string>(maxLength: 5, nullable: true),
                    ProductTypeTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ProductCategoryCode = table.Column<string>(maxLength: 50, nullable: true),
                    ProductCategoryTitle = table.Column<string>(maxLength: 50, nullable: true),
                    SupplierAccCode = table.Column<string>(maxLength: 50, nullable: true),
                    SupplierAccTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ProductGroupCode = table.Column<string>(maxLength: 50, nullable: true),
                    ProductGroupTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ProductCode = table.Column<string>(maxLength: 15, nullable: true),
                    ProductTitle = table.Column<string>(maxLength: 50, nullable: true),
                    Value = table.Column<string>(maxLength: 50, nullable: true),
                    ETUTrack1IINFieldNo = table.Column<string>(maxLength: 50, nullable: true),
                    ETUTrack1IINPosition = table.Column<string>(maxLength: 50, nullable: true),
                    ETUTrack1IIN = table.Column<string>(maxLength: 50, nullable: true),
                    ETUTrack2IIN = table.Column<string>(maxLength: 50, nullable: true),
                    ETUPANTrack = table.Column<string>(maxLength: 50, nullable: true),
                    ETUPANTrackFieldNo = table.Column<string>(maxLength: 50, nullable: true),
                    ETUPANTrackPosition = table.Column<string>(maxLength: 50, nullable: true),
                    ETUPANTrackSize = table.Column<string>(maxLength: 50, nullable: true),
                    PINPAN = table.Column<string>(maxLength: 50, nullable: true),
                    CustomField = table.Column<string>(maxLength: 50, nullable: true),
                    BarCode = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiptTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiptPINTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiptSerialTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiptTopupTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiptTopupReferenceTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ReceiptLogo1 = table.Column<string>(maxLength: 15, nullable: true),
                    ReceiptLogo2 = table.Column<string>(maxLength: 15, nullable: true),
                    ReceiptText = table.Column<string>(maxLength: 50, nullable: true),
                    Margin = table.Column<string>(maxLength: 15, nullable: true),
                    CostExGST = table.Column<string>(maxLength: 15, nullable: true),
                    GST = table.Column<string>(maxLength: 15, nullable: true),
                    IMPORTSEQ = table.Column<float>(nullable: false),
                    VariableValue = table.Column<string>(maxLength: 5, nullable: true),
                    MinValue = table.Column<string>(maxLength: 50, nullable: true),
                    MaxValue = table.Column<string>(maxLength: 50, nullable: true),
                    DivValue = table.Column<string>(maxLength: 50, nullable: true),
                    MenuTemplateItemTitle = table.Column<string>(maxLength: 50, nullable: true),
                    DataVersion = table.Column<int>(maxLength: 50, nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EPAY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EPAY_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EPAY_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EPAY_Product",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoBuying",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Action = table.Column<string>(maxLength: 4, nullable: true),
                    HostPromoType = table.Column<string>(maxLength: 5, nullable: true),
                    AmtOffNorm1 = table.Column<float>(nullable: true),
                    PromoUnits = table.Column<float>(nullable: true),
                    CostStart = table.Column<DateTime>(nullable: true),
                    CostEnd = table.Column<DateTime>(nullable: true),
                    CostIsPromInd = table.Column<bool>(nullable: true),
                    CartonCost = table.Column<float>(nullable: false),
                    CartonQty = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoBuying", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionBuyingCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_PromotionBuying",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_PromotionBuying",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_PromotionBuying",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionBuyingUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoMemberOffer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Action = table.Column<string>(maxLength: 4, nullable: true),
                    HostPromoType = table.Column<string>(maxLength: 5, nullable: true),
                    AmtOffNorm1 = table.Column<float>(nullable: true),
                    PromoUnits = table.Column<float>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    Price1 = table.Column<float>(nullable: true),
                    Price2 = table.Column<float>(nullable: true),
                    Price3 = table.Column<float>(nullable: true),
                    Price4 = table.Column<float>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoMemberOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionMemberOfferCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_PromotionMemberOffer",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_PromotionMemberOffer",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionMemberOfferUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoMixmatchProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionMixmatchId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Action = table.Column<string>(maxLength: 4, nullable: true),
                    HostPromoType = table.Column<string>(maxLength: 5, nullable: true),
                    AmtOffNorm1 = table.Column<float>(nullable: true),
                    PromoUnits = table.Column<float>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoMixmatchProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionMixmatchProductCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_PromotionMixmatchProduct",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionMixmatch_PromotionMixmatchProduct",
                        column: x => x.PromotionMixmatchId,
                        principalTable: "PromoMixmatch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionMixmatchProductUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoOfferProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionOfferId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Action = table.Column<string>(maxLength: 4, nullable: true),
                    HostPromoType = table.Column<string>(maxLength: 5, nullable: true),
                    AmtOffNorm1 = table.Column<float>(nullable: true),
                    PromoUnits = table.Column<float>(nullable: true),
                    OfferGroup = table.Column<byte>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoOfferProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionOfferProductCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_PromotionOfferProduct",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionOffer_PromotionOfferProduct",
                        column: x => x.PromotionOfferId,
                        principalTable: "PromoOffer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionOfferProductUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromoSelling",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Action = table.Column<string>(maxLength: 4, nullable: true),
                    HostPromoType = table.Column<string>(maxLength: 5, nullable: true),
                    AmtOffNorm1 = table.Column<float>(nullable: true),
                    PromoUnits = table.Column<float>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    Price1 = table.Column<float>(nullable: true),
                    Price2 = table.Column<float>(nullable: true),
                    Price3 = table.Column<float>(nullable: true),
                    Price4 = table.Column<float>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoSelling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PromotionSellingCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_PromotionSelling",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_PromotionSelling",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionSellingUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotionCompetition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetitionId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionCompetition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitionDetail_PromotionCompetition",
                        column: x => x.CompetitionId,
                        principalTable: "CompetitionDetail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionCompetitionCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_PromotionCompetition",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PromotionCompetitionUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RebateDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RebateHeaderId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Amount = table.Column<float>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RebateDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ReabateDetail_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rebate_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rebate_Detail_Header",
                        column: x => x.RebateHeaderId,
                        principalTable: "RebateHeader",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_RebateDetail_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(nullable: false),
                    SupplierItem = table.Column<string>(maxLength: 15, nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CartonCost = table.Column<float>(nullable: false),
                    MinReorderQty = table.Column<int>(nullable: true),
                    BestBuy = table.Column<bool>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_SupplierProduct_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_ProductSupplier",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_ProductSupplier",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_SupplierProduct_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompetitionReward",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompPromoId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionCompetition_CompetitionReward",
                        column: x => x.CompPromoId,
                        principalTable: "PromotionCompetition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_CompetitionRewardCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_CompetitionRewardUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompetitionTrigger",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompPromoId = table.Column<int>(nullable: false),
                    TriggerProductGroupID = table.Column<int>(nullable: false),
                    Share = table.Column<bool>(nullable: false),
                    LoyaltyFactor = table.Column<float>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionTrigger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionCompetition_CompetitionTrigger",
                        column: x => x.CompPromoId,
                        principalTable: "PromotionCompetition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_CompetitionTriggerCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetitionTriggerGroup_MasterListItem",
                        column: x => x.TriggerProductGroupID,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_CompetitionTriggerUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(maxLength: 10, nullable: false),
                    ProductId = table.Column<long>(nullable: true),
                    ProductNumber = table.Column<long>(nullable: true),
                    OutletId = table.Column<int>(nullable: false),
                    OutletCode = table.Column<string>(maxLength: 30, nullable: true),
                    TillId = table.Column<int>(nullable: true),
                    TillCode = table.Column<string>(maxLength: 15, nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 30, nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    ManufaturerCode = table.Column<string>(maxLength: 50, nullable: true),
                    Group = table.Column<int>(nullable: true),
                    GrpCode = table.Column<string>(maxLength: 50, nullable: true),
                    DepartmentId = table.Column<int>(nullable: true),
                    DepartmentCode = table.Column<string>(maxLength: 30, nullable: true),
                    CommodityId = table.Column<int>(nullable: true),
                    CommodityCode = table.Column<string>(maxLength: 30, nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    CategoryCode = table.Column<string>(maxLength: 50, nullable: true),
                    SubRange = table.Column<int>(nullable: true),
                    SubRangeCode = table.Column<int>(maxLength: 50, nullable: true),
                    Reference = table.Column<string>(maxLength: 10, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Qty = table.Column<float>(nullable: false),
                    Amt = table.Column<float>(nullable: false),
                    ExGSTAmt = table.Column<float>(nullable: false),
                    Cost = table.Column<float>(nullable: false),
                    ExGSTCost = table.Column<float>(nullable: false),
                    Discount = table.Column<float>(nullable: false),
                    Discount1 = table.Column<double>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    PromoBuyId = table.Column<int>(nullable: true),
                    PromoBuyCode = table.Column<string>(maxLength: 25, nullable: true),
                    PromoSellId = table.Column<int>(nullable: true),
                    PromoSellCode = table.Column<string>(maxLength: 25, nullable: true),
                    Weekend = table.Column<DateTime>(nullable: false),
                    Day = table.Column<string>(maxLength: 3, nullable: false),
                    NewOnHand = table.Column<float>(nullable: true),
                    Member = table.Column<long>(nullable: true),
                    Points = table.Column<float>(nullable: true),
                    CartonQty = table.Column<float>(nullable: true),
                    UnitQty = table.Column<float>(nullable: true),
                    Parent = table.Column<float>(nullable: true),
                    StockMovement = table.Column<float>(nullable: true),
                    Tender = table.Column<string>(maxLength: 15, nullable: true),
                    ManualInd = table.Column<bool>(nullable: true),
                    GLAccount = table.Column<string>(maxLength: 15, nullable: true),
                    GLPostedInd = table.Column<bool>(nullable: true),
                    PromoSales = table.Column<float>(nullable: false),
                    PromoSalesGST = table.Column<float>(nullable: false),
                    Debtor = table.Column<float>(nullable: false),
                    Flags = table.Column<string>(maxLength: 400, nullable: true),
                    ReferenceType = table.Column<string>(maxLength: 25, nullable: true),
                    ReferenceNumber = table.Column<string>(maxLength: 64, nullable: true),
                    TermsRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    TermsRebate = table.Column<float>(nullable: true),
                    ScanRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    ScanRebate = table.Column<float>(nullable: true),
                    PurchaseRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    PurchaseRebate = table.Column<float>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterListItem_Transaction_Category_Id",
                        column: x => x.CategoryId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commodity_Transaction_Commodity_Id",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Transaction_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Department_Transaction_Department_Id",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_Transaction_Group_Id",
                        column: x => x.Group,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_Transaction_Manufacturer_Id",
                        column: x => x.ManufacturerId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_Transaction_Product_Id",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_Transaction_Promotion_Buy",
                        column: x => x.PromoBuyId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_Transaction_Promotion_Sell",
                        column: x => x.PromoSellId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_Transaction_Supplier_Id",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Transaction_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Transaction_User_Id",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransactionReport",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(maxLength: 10, nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    TillId = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    Group = table.Column<int>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: false),
                    CommodityId = table.Column<int>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false),
                    SubRange = table.Column<int>(nullable: true),
                    Reference = table.Column<string>(maxLength: 10, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Qty = table.Column<float>(nullable: true),
                    Amt = table.Column<float>(nullable: true),
                    ExGSTAmt = table.Column<float>(nullable: true),
                    Cost = table.Column<float>(nullable: true),
                    ExGSTCost = table.Column<float>(nullable: true),
                    Discount = table.Column<float>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    PromoBuyId = table.Column<int>(nullable: true),
                    PromoSellId = table.Column<int>(nullable: true),
                    Weekend = table.Column<DateTime>(nullable: false),
                    Day = table.Column<string>(maxLength: 3, nullable: false),
                    NewOnHand = table.Column<float>(nullable: true),
                    Member = table.Column<long>(nullable: true),
                    Points = table.Column<float>(nullable: true),
                    CartonQty = table.Column<float>(nullable: true),
                    UnitQty = table.Column<float>(nullable: true),
                    Parent = table.Column<float>(nullable: true),
                    StockMovement = table.Column<float>(nullable: true),
                    Tender = table.Column<string>(maxLength: 15, nullable: true),
                    ManualInd = table.Column<bool>(nullable: true),
                    GLAccount = table.Column<string>(maxLength: 15, nullable: true),
                    GLPostedInd = table.Column<bool>(nullable: true),
                    PromoSales = table.Column<float>(nullable: true),
                    PromoSalesGST = table.Column<float>(nullable: true),
                    Debtor = table.Column<float>(nullable: true),
                    Flags = table.Column<string>(maxLength: 400, nullable: true),
                    ReferenceType = table.Column<string>(maxLength: 25, nullable: true),
                    ReferenceNumber = table.Column<string>(maxLength: 64, nullable: true),
                    TermsRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    TermsRebate = table.Column<float>(nullable: true),
                    ScanRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    ScanRebate = table.Column<float>(nullable: true),
                    PurchaseRebateCode = table.Column<string>(maxLength: 64, nullable: true),
                    PurchaseRebate = table.Column<float>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterListItem_TransactionReport_Category_Id",
                        column: x => x.CategoryId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commodity_TransactionReport_Commodity_Id",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_TransactionReport_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Department_TransactionReport_Department_Id",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_TransactionReport_Manufacturer_Id",
                        column: x => x.ManufacturerId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_TransactionReport_Product_Id",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_TransactionReport_Promotion_Buy",
                        column: x => x.PromoBuyId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promotion_TransactionReport_Promotion_Sell",
                        column: x => x.PromoSellId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_TransactionReport_Supplier_Id",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_TransactionReport_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_TransactionReport_User_Id",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutletProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    SupplierId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Till = table.Column<bool>(nullable: true),
                    OpenPrice = table.Column<bool>(nullable: true),
                    NormalPrice1 = table.Column<float>(nullable: false),
                    NormalPrice2 = table.Column<float>(nullable: true),
                    NormalPrice3 = table.Column<float>(nullable: true),
                    NormalPrice4 = table.Column<float>(nullable: true),
                    NormalPrice5 = table.Column<float>(nullable: true),
                    CartonCost = table.Column<float>(nullable: false),
                    CartonCostHost = table.Column<float>(nullable: true),
                    CartonCostInv = table.Column<float>(nullable: true),
                    CartonCostAvg = table.Column<float>(nullable: true),
                    QtyOnHand = table.Column<float>(nullable: false),
                    MinOnHand = table.Column<float>(nullable: false),
                    MaxOnHand = table.Column<float>(nullable: true),
                    MinReorderQty = table.Column<float>(nullable: true),
                    PickingBinNo = table.Column<int>(nullable: true),
                    ChangeLabelInd = table.Column<bool>(nullable: true),
                    ChangeTillInd = table.Column<bool>(nullable: true),
                    HoldNorm = table.Column<string>(nullable: true),
                    ChangeLabelPrinted = table.Column<DateTime>(type: "datetime", nullable: true),
                    LabelQty = table.Column<float>(nullable: true),
                    ShortLabelInd = table.Column<bool>(nullable: true),
                    SkipReorder = table.Column<bool>(nullable: false),
                    SpecPrice = table.Column<float>(nullable: true),
                    SpecCode = table.Column<string>(nullable: true),
                    SpecFrom = table.Column<DateTime>(type: "datetime", nullable: true),
                    SpecTo = table.Column<DateTime>(type: "datetime", nullable: true),
                    GenCode = table.Column<string>(nullable: true),
                    SpecCartonCost = table.Column<float>(nullable: true),
                    ScalePlu = table.Column<float>(nullable: true),
                    FifoStock = table.Column<bool>(nullable: true),
                    Mrp = table.Column<float>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PromoPrice1 = table.Column<float>(nullable: true),
                    PromoPrice2 = table.Column<float>(nullable: true),
                    PromoPrice3 = table.Column<float>(nullable: true),
                    PromoPrice4 = table.Column<float>(nullable: true),
                    PromoPrice5 = table.Column<float>(nullable: true),
                    PromoCartonCost = table.Column<float>(nullable: true),
                    PromoMixMatch1Id = table.Column<int>(nullable: true),
                    PromoMixMatch2Id = table.Column<int>(nullable: true),
                    PromoOffer1Id = table.Column<int>(nullable: true),
                    PromoOffer2Id = table.Column<int>(nullable: true),
                    PromoOffer3Id = table.Column<int>(nullable: true),
                    PromoOffer4Id = table.Column<int>(nullable: true),
                    PromoCompId = table.Column<long>(nullable: true),
                    PromoSellId = table.Column<int>(nullable: true),
                    PromoBuyId = table.Column<int>(nullable: true),
                    PromoMemeberOfferId = table.Column<int>(nullable: true),
                    PromoMemOffPrice1 = table.Column<float>(nullable: true),
                    PromoMemOffPrice2 = table.Column<float>(nullable: true),
                    PromoMemOffPrice3 = table.Column<float>(nullable: true),
                    PromoMemOffPrice4 = table.Column<float>(nullable: true),
                    PromoMemOffPrice5 = table.Column<float>(nullable: true),
                    PromoMemOff = table.Column<bool>(nullable: true),
                    AllMember = table.Column<bool>(nullable: true),
                    PromoBuy = table.Column<bool>(nullable: true),
                    PromoSell = table.Column<bool>(nullable: true),
                    PromoMix1 = table.Column<bool>(nullable: true),
                    PromoMix2 = table.Column<bool>(nullable: true),
                    PromoOffer1 = table.Column<bool>(nullable: true),
                    PromoOffer2 = table.Column<bool>(nullable: true),
                    PromoOffer3 = table.Column<bool>(nullable: true),
                    PromoOffer4 = table.Column<bool>(nullable: true),
                    PromoComp = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_OutletProduct_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_OutletProduct",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Buy_OutletProduct",
                        column: x => x.PromoBuyId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Comp_OutletProduct",
                        column: x => x.PromoCompId,
                        principalTable: "CompetitionDetail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_MemberOffer_OutletProduct",
                        column: x => x.PromoMemeberOfferId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_MixMatch_1_OutletProduct",
                        column: x => x.PromoMixMatch1Id,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_MixMatch_2_OutletProduct",
                        column: x => x.PromoMixMatch2Id,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Offer_1_OutletProduct",
                        column: x => x.PromoOffer1Id,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Offer_2_OutletProduct",
                        column: x => x.PromoOffer2Id,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Offer_3_OutletProduct",
                        column: x => x.PromoOffer3Id,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Offer_4_OutletProduct",
                        column: x => x.PromoOffer4Id,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Sell_OutletProduct",
                        column: x => x.PromoSellId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_OutletProduct",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_OutletProduct_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    GroupId = table.Column<int>(nullable: true),
                    Address1 = table.Column<string>(maxLength: 30, nullable: true),
                    Address2 = table.Column<string>(maxLength: 30, nullable: true),
                    Address3 = table.Column<string>(maxLength: 30, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 40, nullable: true),
                    Fax = table.Column<string>(maxLength: 40, nullable: true),
                    PostCode = table.Column<string>(maxLength: 4, nullable: true),
                    Status = table.Column<bool>(maxLength: 10, nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    PriceZoneId = table.Column<int>(nullable: true),
                    CostZoneId = table.Column<int>(nullable: true),
                    SellingInd = table.Column<bool>(nullable: false),
                    StockInd = table.Column<bool>(nullable: false),
                    DelName = table.Column<string>(maxLength: 30, nullable: true),
                    DelAddr1 = table.Column<string>(maxLength: 30, nullable: true),
                    DelAddr2 = table.Column<string>(maxLength: 30, nullable: true),
                    DelAddr3 = table.Column<string>(maxLength: 30, nullable: true),
                    DelPostCode = table.Column<string>(maxLength: 4, nullable: true),
                    CostType = table.Column<string>(maxLength: 10, nullable: true),
                    CostInd = table.Column<bool>(nullable: true),
                    Abn = table.Column<string>(maxLength: 30, nullable: true),
                    BudgetGrowthFact = table.Column<double>(nullable: true),
                    EntityNumber = table.Column<string>(maxLength: 3, nullable: true),
                    WarehouseId = table.Column<int>(nullable: true),
                    OutletPriceFromOutletId = table.Column<int>(nullable: true),
                    PriceFromLevel = table.Column<int>(nullable: true),
                    PriceLevelDesc1 = table.Column<string>(maxLength: 30, nullable: true),
                    PriceLevelDesc2 = table.Column<string>(maxLength: 30, nullable: true),
                    PriceLevelDesc3 = table.Column<string>(maxLength: 30, nullable: true),
                    PriceLevelDesc4 = table.Column<string>(maxLength: 30, nullable: true),
                    PriceLevelDesc5 = table.Column<string>(maxLength: 30, nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    OpenHours = table.Column<string>(maxLength: 256, nullable: true),
                    FuelSite = table.Column<bool>(nullable: true),
                    NameOnApp = table.Column<string>(maxLength: 64, nullable: true),
                    AddressOnApp = table.Column<string>(maxLength: 64, nullable: true),
                    DisplayOnApp = table.Column<bool>(nullable: true),
                    AppOrders = table.Column<bool>(nullable: true),
                    LabelTypeShelfId = table.Column<int>(nullable: true),
                    LabelTypePromoId = table.Column<int>(nullable: true),
                    LabelTypeShortId = table.Column<int>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Store_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_store_group_Store_Group_Id",
                        column: x => x.GroupId,
                        principalTable: "StoreGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_LabelTypePromo_Id",
                        column: x => x.LabelTypePromoId,
                        principalTable: "PrintLabelType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_LabelTypeShelf_Id",
                        column: x => x.LabelTypeShelfId,
                        principalTable: "PrintLabelType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_LabelTypeShort_Id",
                        column: x => x.LabelTypeShortId,
                        principalTable: "PrintLabelType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_OutletPriceFromOutlet",
                        column: x => x.OutletPriceFromOutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Store_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_Warehouse_Id",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AutoOrderSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    HistoryDays = table.Column<int>(nullable: false),
                    CoverDays = table.Column<float>(nullable: false),
                    InvestmentBuyDays = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoOrderSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_AutoOrderSettings_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_AutoOrderSettings",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_AutoOrderSettings",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_AutoOrderSettings_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BulkOrderFromTablet",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletId = table.Column<int>(nullable: false),
                    ProductNumber = table.Column<long>(nullable: false),
                    OrderBatch = table.Column<DateTime>(type: "datetime", nullable: false),
                    Qty = table.Column<float>(nullable: false),
                    LastImport = table.Column<DateTime>(type: "datetime", nullable: false),
                    Type = table.Column<string>(maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkOrderFromTablet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_BulkOrderFromTablet_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_BulkOrderFromTablet_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_BulkOrderFromTablet_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cashier",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<long>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 20, nullable: false),
                    Surname = table.Column<string>(maxLength: 30, nullable: false),
                    Addr1 = table.Column<string>(maxLength: 30, nullable: true),
                    Addr2 = table.Column<string>(maxLength: 30, nullable: true),
                    Addr3 = table.Column<string>(maxLength: 30, nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    Postcode = table.Column<string>(maxLength: 4, nullable: true),
                    Phone = table.Column<string>(maxLength: 15, nullable: true),
                    Mobile = table.Column<string>(maxLength: 15, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    Gender = table.Column<string>(maxLength: 6, nullable: true),
                    TypeId = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    StoreGroupId = table.Column<int>(nullable: true),
                    OutletId = table.Column<int>(nullable: true),
                    ZoneId = table.Column<int>(nullable: true),
                    Password = table.Column<string>(maxLength: 10, nullable: true),
                    AccessLevelId = table.Column<int>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    WristBandInd = table.Column<string>(maxLength: 3, nullable: true),
                    Dispname = table.Column<string>(maxLength: 20, nullable: true),
                    LeftHandTillInd = table.Column<string>(maxLength: 3, nullable: true),
                    FuelUser = table.Column<string>(maxLength: 10, nullable: true),
                    FuelPass = table.Column<string>(maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DepartmentsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cashier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterListItem_CashierAccesLevel",
                        column: x => x.AccessLevelId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Cashier_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cashier_Department_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Store_CashierStore",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoreGroup_CashierStoreGroup",
                        column: x => x.StoreGroupId,
                        principalTable: "StoreGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_CashierType",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Cashier_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_CashierZone",
                        column: x => x.ZoneId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Desc = table.Column<string>(maxLength: 40, nullable: false),
                    AccountSystem = table.Column<string>(maxLength: 10, nullable: false),
                    AccountNumber = table.Column<string>(maxLength: 15, nullable: false),
                    StoreId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Company = table.Column<int>(nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_GLAccount_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_GLAccountStore",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_GLAccountSupplier",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_AccountType",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_GLAccount_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Keypad",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    KeyPadButtonJSONData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keypad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Keypad_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Keypad_Updated_By",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Keypad_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ManualSaleItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManualSaleId = table.Column<long>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    Qty = table.Column<float>(nullable: false),
                    Price = table.Column<float>(nullable: false),
                    Amount = table.Column<float>(nullable: false),
                    Cost = table.Column<float>(nullable: false),
                    PriceLevel = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualSaleItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ManualSaleItem_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManualSale_ManualSaleItem_ManualSale",
                        column: x => x.ManualSaleId,
                        principalTable: "ManualSale",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_ManualSaleItem_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_ManualSaleItem_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Type_ManualSaleItem_Type",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_ManualSaleItem_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderAudit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletId = table.Column<int>(nullable: true),
                    OrderNo = table.Column<long>(nullable: false),
                    SupplierId = table.Column<int>(nullable: true),
                    InvoiceNo = table.Column<string>(maxLength: 15, nullable: true),
                    TypeId = table.Column<int>(nullable: true),
                    StatusId = table.Column<int>(nullable: true),
                    NewStatusId = table.Column<int>(nullable: true),
                    Action = table.Column<string>(maxLength: 150, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAudit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Order_Audit_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_Order_Audit_NewStatus",
                        column: x => x.NewStatusId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_Order_Audit_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_Order_Audit_Status",
                        column: x => x.StatusId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_Order_Audit_Supplier",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_Order_Audit_Type",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Order_Audit_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderHeader",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletId = table.Column<int>(nullable: false),
                    StoreCode = table.Column<string>(maxLength: 30, nullable: true),
                    OrderNo = table.Column<long>(nullable: false),
                    SupplierId = table.Column<int>(nullable: true),
                    SupplierCode = table.Column<string>(maxLength: 30, nullable: true),
                    StoreIdAsSupplier = table.Column<int>(nullable: true),
                    CreationTypeId = table.Column<int>(nullable: false),
                    CreationTypeCode = table.Column<string>(maxLength: 50, nullable: true),
                    TypeId = table.Column<int>(nullable: false),
                    TypeCode = table.Column<string>(maxLength: 50, nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    StatusCode = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Reference = table.Column<string>(maxLength: 15, nullable: true),
                    DeliveryNo = table.Column<string>(maxLength: 15, nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvoiceNo = table.Column<string>(maxLength: 15, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvoiceTotal = table.Column<float>(nullable: true),
                    SubTotalFreight = table.Column<float>(nullable: true),
                    SubTotalAdmin = table.Column<float>(nullable: true),
                    SubTotalSubsidy = table.Column<float>(nullable: true),
                    SubTotalDisc = table.Column<float>(nullable: true),
                    SubTotalTax = table.Column<float>(nullable: true),
                    Posted = table.Column<DateTime>(type: "datetime", nullable: true),
                    GstAmt = table.Column<float>(nullable: true),
                    CoverDays = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Order_Header_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_OrderCreationType",
                        column: x => x.CreationTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_Order_Header_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_OrderStatus",
                        column: x => x.StatusId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_Order_Header_StoreIdAsSupplier",
                        column: x => x.StoreIdAsSupplier,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_Order_Header_Supplier",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_OrderType",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Order_Header_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutletBudget",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    Budget = table.Column<float>(nullable: true),
                    EmployHours = table.Column<float>(nullable: true),
                    MDROver = table.Column<float>(nullable: true),
                    StockOnHand = table.Column<float>(nullable: true),
                    StockPurchase = table.Column<float>(nullable: true),
                    StockAdjust = table.Column<float>(nullable: true),
                    Wastage = table.Column<float>(nullable: true),
                    Voids = table.Column<float>(nullable: true),
                    Refunds = table.Column<float>(nullable: true),
                    Markdown = table.Column<float>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_OutletBudget_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutletBudget_Store_Id",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_OutletBudget_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutletRoyaltyScales",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ScalesFrom = table.Column<float>(nullable: false),
                    ScalesTo = table.Column<float>(nullable: false),
                    Percent = table.Column<float>(nullable: false),
                    IncGST = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletRoyaltyScales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_OutletRoyaltyScales_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_OutletRoyaltyScales_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_OutletRoyaltyScales_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutletSupplierSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    DOWGenerateOrder = table.Column<int>(nullable: false),
                    SendOrderOffset = table.Column<int>(nullable: false),
                    ReceiveOrderOffset = table.Column<int>(nullable: false),
                    LastRun = table.Column<DateTime>(nullable: true),
                    InvoiceOrderOffset = table.Column<int>(nullable: false),
                    DiscountThresholdOne = table.Column<float>(nullable: false),
                    DiscountThresholdTwo = table.Column<float>(nullable: false),
                    DiscountThresholdThree = table.Column<float>(nullable: false),
                    CoverDaysDiscountThreshold1 = table.Column<int>(nullable: false),
                    CoverDaysDiscountThreshold2 = table.Column<int>(nullable: false),
                    CoverDaysDiscountThreshold3 = table.Column<int>(nullable: false),
                    CoverDays = table.Column<int>(nullable: false),
                    MultipleOrdersInAWeek = table.Column<bool>(nullable: false),
                    OrderNonDefaultSupplier = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UserCreatedById = table.Column<int>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletSupplierSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_SupplierOrdering_Updated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierOrder_Store",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierOrder_Supplier",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutletSupplierSchedule_User_UserCreatedById",
                        column: x => x.UserCreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutletSupplierSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Desc = table.Column<string>(maxLength: 30, nullable: false),
                    CustomerNumber = table.Column<string>(maxLength: 30, nullable: true),
                    StateId = table.Column<int>(nullable: true),
                    DivisionId = table.Column<int>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    QtyDefault = table.Column<string>(nullable: true),
                    BuyCartoon = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletSupplierSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_OutletSupplierSetting_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItemDivision_OutletSupplierSetting",
                        column: x => x.DivisionId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItemState_OutletSupplierSetting",
                        column: x => x.StateId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_OutletSupplierSetting",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Supplier_OutletSupplierSetting",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_OutletSupplierSetting_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutletTradingHours",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OuteltId = table.Column<int>(nullable: false),
                    MonOpenTime = table.Column<string>(maxLength: 10, nullable: true),
                    MonCloseTime = table.Column<string>(maxLength: 10, nullable: true),
                    TueOpenTime = table.Column<string>(maxLength: 10, nullable: true),
                    TueCloseTime = table.Column<string>(maxLength: 10, nullable: true),
                    WedOpenTime = table.Column<string>(maxLength: 10, nullable: true),
                    WedCloseTime = table.Column<string>(maxLength: 10, nullable: true),
                    ThuOpenTime = table.Column<string>(maxLength: 10, nullable: true),
                    ThuCloseTime = table.Column<string>(maxLength: 10, nullable: true),
                    FriOpenTime = table.Column<string>(maxLength: 10, nullable: true),
                    FriCloseTime = table.Column<string>(maxLength: 10, nullable: true),
                    SatOpenTime = table.Column<string>(maxLength: 10, nullable: true),
                    SatCloseTime = table.Column<string>(maxLength: 10, nullable: true),
                    SunOpenTime = table.Column<string>(maxLength: 10, nullable: true),
                    SunCloseTime = table.Column<string>(maxLength: 10, nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletTradingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_OutTradingHours_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_OutTradingHours",
                        column: x => x.OuteltId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_OutTradingHours_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Paths",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PathType = table.Column<int>(nullable: false),
                    OutletID = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Path = table.Column<string>(maxLength: 200, nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paths", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Paths_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Paths_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Paths_Outlet",
                        column: x => x.OutletID,
                        principalTable: "Store",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RebateOutlets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RebateHeaderId = table.Column<long>(nullable: false),
                    StoreId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RebateOutlets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ReabateOutlet_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rebate_Outlet_Header",
                        column: x => x.RebateHeaderId,
                        principalTable: "RebateHeader",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rebate_Outlet",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_RebateOutlet_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<long>(nullable: false),
                    OutletID = table.Column<int>(nullable: true),
                    IngredientProductID = table.Column<long>(nullable: true),
                    RecipeTimeStamp = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<float>(nullable: false),
                    IsParents = table.Column<int>(nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Recipe_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecipeIngredient_Product",
                        column: x => x.IngredientProductID,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recipe_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recipe_Outlet",
                        column: x => x.OutletID,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recipe_Product",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustHeader",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletId = table.Column<int>(nullable: false),
                    PostToDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Reference = table.Column<string>(maxLength: 10, nullable: true),
                    Total = table.Column<float>(nullable: false),
                    Description = table.Column<string>(maxLength: 30, nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Stock_Adjust_Header_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_Stock_Adjust_Header_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Stock_Adjust_Header_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTakeHeader",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletId = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(maxLength: 40, nullable: true),
                    PostToDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Total = table.Column<float>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakeHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Stock_Take_Header_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_Stock_Take_Header_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Stock_Take_Header_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_User_Roles_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Roles_Roles_Id",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRole_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_User_Roles_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_User_Roles_User_Id",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "XeroAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Desc = table.Column<string>(maxLength: 30, nullable: true),
                    StoreId = table.Column<int>(nullable: false),
                    FinAccSummary = table.Column<string>(maxLength: 30, nullable: true),
                    GSTProdSale = table.Column<long>(nullable: true),
                    GSTProdSaleDesc = table.Column<string>(maxLength: 30, nullable: true),
                    NonGSTProdSale = table.Column<long>(nullable: true),
                    NonGSTProdSaleDesc = table.Column<string>(maxLength: 30, nullable: true),
                    LessUberEats = table.Column<long>(nullable: true),
                    LessUberEatsDesc = table.Column<string>(maxLength: 30, nullable: true),
                    Anex = table.Column<long>(nullable: true),
                    AnexDesc = table.Column<string>(maxLength: 30, nullable: true),
                    CashEFTPOS = table.Column<long>(nullable: true),
                    CashEFTPOSDesc = table.Column<string>(maxLength: 30, nullable: true),
                    UnderOver = table.Column<long>(nullable: true),
                    UnderOverDesc = table.Column<string>(maxLength: 30, nullable: true),
                    FuelCard = table.Column<long>(nullable: true),
                    FuelCardDesc = table.Column<string>(maxLength: 30, nullable: true),
                    FleetCard = table.Column<long>(nullable: true),
                    FleetCardDesc = table.Column<string>(maxLength: 30, nullable: true),
                    MotorPass = table.Column<long>(nullable: true),
                    MotorPassDesc = table.Column<string>(maxLength: 30, nullable: true),
                    MotorCharge = table.Column<long>(nullable: true),
                    MotorChargeDesc = table.Column<string>(maxLength: 30, nullable: true),
                    Other = table.Column<long>(nullable: true),
                    OtherDesc = table.Column<string>(maxLength: 30, nullable: true),
                    StockAccSummary = table.Column<string>(maxLength: 30, nullable: true),
                    BalanceSheet = table.Column<long>(nullable: true),
                    BalanceSheetDesc = table.Column<string>(maxLength: 30, nullable: true),
                    ProfitLoss = table.Column<long>(nullable: true),
                    ProfitLossDesc = table.Column<string>(maxLength: 30, nullable: true),
                    XeroSecretKey = table.Column<string>(maxLength: 50, nullable: true),
                    XeroConsumerKey = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    UserCreatedById = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XeroAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_XeroAccounting_Updated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_XeroAccounting_Store",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_XeroAccount_User_UserCreatedById",
                        column: x => x.UserCreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZoneOutlet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneOutlet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ZoneOut_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_ZoneOut_Store_Id",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_ZoneOut_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItems_ZoneOut_Zone_Id",
                        column: x => x.ZoneId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KeypadLevel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeypadId = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    LevelIndex = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeypadLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_KeypadLevel_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Keypad_KeypadLevel",
                        column: x => x.KeypadId,
                        principalTable: "Keypad",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_KeypadLevel_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Till",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    KeypadId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    SerialNo = table.Column<string>(maxLength: 15, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Till", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Till_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Till_Keypad",
                        column: x => x.KeypadId,
                        principalTable: "Keypad",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Till_Outlet",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_TillType",
                        column: x => x.TypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Till_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderHeaderId = table.Column<long>(nullable: false),
                    OrderTypeId = table.Column<int>(nullable: false),
                    TypeCode = table.Column<string>(maxLength: 50, nullable: true),
                    LineNo = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    ProductNumber = table.Column<long>(nullable: true),
                    SupplierProductId = table.Column<long>(nullable: true),
                    SupplierItem = table.Column<string>(maxLength: 15, nullable: true),
                    CartonCost = table.Column<float>(nullable: true),
                    Cartons = table.Column<float>(nullable: false),
                    Units = table.Column<float>(nullable: false),
                    TotalUnits = table.Column<float>(nullable: false),
                    DeliverCartons = table.Column<float>(nullable: true),
                    DeliverUnits = table.Column<float>(nullable: true),
                    DeliverTotalUnits = table.Column<float>(nullable: true),
                    LineTotal = table.Column<float>(nullable: false),
                    CartonQty = table.Column<float>(nullable: false),
                    BonusInd = table.Column<bool>(nullable: true),
                    PostedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    FinalLineTotal = table.Column<float>(nullable: true),
                    FinalCartonCost = table.Column<float>(nullable: true),
                    OnHand = table.Column<float>(nullable: true),
                    OnOrder = table.Column<float>(nullable: true),
                    UnitsSold = table.Column<float>(nullable: true),
                    CoverUnits = table.Column<float>(nullable: true),
                    NonPromoMinOnHand = table.Column<float>(nullable: true),
                    PromoMinOnHand = table.Column<float>(nullable: true),
                    NonPromoAvgDaily = table.Column<float>(nullable: true),
                    PromoAvgDaily = table.Column<float>(nullable: true),
                    NormalCoverDays = table.Column<int>(nullable: true),
                    CoverDaysUsed = table.Column<int>(nullable: true),
                    MinReorderQty = table.Column<int>(nullable: true),
                    Perishable = table.Column<bool>(nullable: true),
                    NonPromoSales56Days = table.Column<float>(nullable: true),
                    PromoSales56Days = table.Column<float>(nullable: true),
                    BuyPromoCode = table.Column<string>(maxLength: 15, nullable: true),
                    BuyPromoEndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    BuyPromoDisc = table.Column<float>(nullable: true),
                    SalePromoCode = table.Column<string>(maxLength: 15, nullable: true),
                    SalePromoEndDate = table.Column<DateTime>(nullable: true),
                    CheckSuuplier = table.Column<bool>(nullable: true),
                    CheaperSupplierId = table.Column<int>(maxLength: 15, nullable: true),
                    SupplierCode = table.Column<string>(maxLength: 30, nullable: true),
                    NewProduct = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PromoBuy = table.Column<bool>(nullable: true),
                    NonPromoBuy = table.Column<bool>(nullable: true),
                    InvestBuy = table.Column<bool>(nullable: true),
                    StoreCode = table.Column<string>(nullable: true),
                    CheaperSupplierCode = table.Column<string>(nullable: true),
                    OrderCreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    OrderNo = table.Column<long>(nullable: true),
                    TradingCoverDays = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supplier_OrderDetail",
                        column: x => x.CheaperSupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Order_Detail_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Order_Header_Order_Detail",
                        column: x => x.OrderHeaderId,
                        principalTable: "OrderHeader",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_OrderDetail_Type",
                        column: x => x.OrderTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_Order_Detail",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierProduct_Order_Detail",
                        column: x => x.SupplierProductId,
                        principalTable: "SupplierProduct",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Order_Detail_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HostSettings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Description = table.Column<string>(maxLength: 80, nullable: false),
                    InitialLoadFileWeekly = table.Column<string>(maxLength: 30, nullable: false),
                    WeeklyFile = table.Column<string>(maxLength: 30, nullable: false),
                    FilePathID = table.Column<int>(nullable: false),
                    NumberFactor = table.Column<float>(nullable: false),
                    SupplierID = table.Column<int>(nullable: true),
                    WareHouseID = table.Column<int>(nullable: false),
                    HostFormatID = table.Column<int>(nullable: false),
                    BuyPromoPrefix = table.Column<string>(maxLength: 30, nullable: false),
                    SellPromoPrefix = table.Column<string>(maxLength: 30, nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostSettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HostSettings_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostSettings_FilePath",
                        column: x => x.FilePathID,
                        principalTable: "Paths",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_HostSettings_MasterListItem",
                        column: x => x.HostFormatID,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostSettings_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostSettings_Supplier",
                        column: x => x.SupplierID,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostSettings_Warehouse",
                        column: x => x.WareHouseID,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockAdjustHeaderId = table.Column<long>(nullable: false),
                    LineNo = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    OutletProductId = table.Column<long>(nullable: false),
                    Quantity = table.Column<float>(nullable: false),
                    LineTotal = table.Column<float>(nullable: false),
                    ItemCost = table.Column<float>(nullable: false),
                    ReasonId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Stock_Adjust_Detail_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_Product_Stock_Adjust_Detail",
                        column: x => x.OutletProductId,
                        principalTable: "OutletProduct",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_Stock_Adjust_Detail",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterListItem_StockAdjustDetail_Type",
                        column: x => x.ReasonId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Stock_Adjust_Header_Stock_Adjust_Detail",
                        column: x => x.StockAdjustHeaderId,
                        principalTable: "StockAdjustHeader",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Stock_Adjust_Detail_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTakeDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockTakeHeaderId = table.Column<long>(nullable: false),
                    OutletProductId = table.Column<long>(nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    OnHandUnits = table.Column<int>(nullable: false),
                    Quantity = table.Column<float>(nullable: false),
                    ItemCost = table.Column<float>(nullable: false),
                    LineCost = table.Column<float>(nullable: false),
                    LineTotal = table.Column<float>(nullable: false),
                    ItemCount = table.Column<int>(nullable: false),
                    VarQty = table.Column<float>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ProductId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakeDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Stock_Take_Detail_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Outlet_Product_Stock_Take_Detail",
                        column: x => x.OutletProductId,
                        principalTable: "OutletProduct",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakeDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stock_Take_Header_Stock_Take_Detail",
                        column: x => x.StockTakeHeaderId,
                        principalTable: "StockTakeHeader",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Stock_Take_Detail_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KeypadButton",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeypadId = table.Column<int>(nullable: false),
                    LevelId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(maxLength: 30, nullable: false),
                    ShortDesc = table.Column<string>(maxLength: 50, nullable: false),
                    Desc = table.Column<string>(maxLength: 50, nullable: true),
                    Color = table.Column<string>(maxLength: 10, nullable: false),
                    Password = table.Column<string>(maxLength: 4, nullable: true),
                    Size = table.Column<int>(nullable: false),
                    CashierLevel = table.Column<int>(nullable: false),
                    PriceLevel = table.Column<int>(nullable: true),
                    ProductId = table.Column<long>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    SalesDiscountPerc = table.Column<float>(nullable: true),
                    BtnKeypadLevelId = table.Column<int>(nullable: true),
                    ButtonIndex = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    AttributesDetails = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeypadButton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeypadButton_ButtonTypeLevel",
                        column: x => x.BtnKeypadLevelId,
                        principalTable: "KeypadLevel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KeypadButton_ButtonTypeCategory",
                        column: x => x.CategoryId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_KeypadButton_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Keypad_KeypadButton",
                        column: x => x.KeypadId,
                        principalTable: "Keypad",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KeypadButton_KeypadLevel",
                        column: x => x.LevelId,
                        principalTable: "KeypadLevel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KeypadButton_ButtonTypeProduct",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KeypadButton_ButtonType",
                        column: x => x.Type,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_KeypadButton_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TillSync",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TillId = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: false),
                    Product = table.Column<int>(nullable: true),
                    Cashier = table.Column<int>(nullable: true),
                    Account = table.Column<int>(nullable: true),
                    Keypad = table.Column<int>(nullable: true),
                    TillActivity = table.Column<DateTime>(nullable: true),
                    ClientVersion = table.Column<string>(maxLength: 20, nullable: true),
                    PosVersion = table.Column<string>(maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TillSync", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_TillSyncCreated_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Store_TillsSync",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Till_TillsSync",
                        column: x => x.TillId,
                        principalTable: "Till",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_TillSyncUpdated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CostPriceZones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 80, nullable: false),
                    HostSettingID = table.Column<int>(nullable: false),
                    Factor1 = table.Column<float>(nullable: true),
                    Factor2 = table.Column<float>(nullable: true),
                    Factor3 = table.Column<float>(nullable: true),
                    SuspUpdOutlet = table.Column<bool>(nullable: false),
                    IsActive = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostPriceZones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CostPriceZones_Created_By",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CostPriceZones_HostSettings",
                        column: x => x.HostSettingID,
                        principalTable: "HostSettings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CostPriceZones_Updated_By",
                        column: x => x.ModifiedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HostUpdChange",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostUpdId = table.Column<int>(nullable: false),
                    HostId = table.Column<int>(nullable: false),
                    ChangeTypeId = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: true),
                    PromotionId = table.Column<int>(nullable: true),
                    OutletId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CtnCostBefore = table.Column<float>(nullable: false),
                    CtnCostAfter = table.Column<float>(nullable: false),
                    Price1Before = table.Column<float>(nullable: false),
                    CtnCostSuggested = table.Column<float>(nullable: false),
                    CtnQtyBefore = table.Column<float>(nullable: false),
                    CtnQtyAfter = table.Column<float>(nullable: false),
                    HostUpdTimeStamp = table.Column<long>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostUpdChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_ChangeTypeId",
                        column: x => x.ChangeTypeId,
                        principalTable: "MasterListItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_HostId",
                        column: x => x.HostId,
                        principalTable: "HostSettings",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_HostUpdId",
                        column: x => x.HostUpdId,
                        principalTable: "HOSTUPD",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Store",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_HostUpdChange_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ZonePricing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(nullable: false),
                    PriceZoneId = table.Column<int>(nullable: false),
                    CtnCost = table.Column<float>(nullable: true),
                    CtnCostDate = table.Column<DateTime>(nullable: true),
                    CtnCostSV1 = table.Column<float>(nullable: true),
                    CtnCostDateSV1 = table.Column<DateTime>(nullable: true),
                    CtnCostSV2 = table.Column<float>(nullable: true),
                    CtnCostDateSV2 = table.Column<DateTime>(nullable: true),
                    CtnCostStd = table.Column<float>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    PriceSV1 = table.Column<float>(nullable: true),
                    PriceSV2 = table.Column<float>(nullable: true),
                    PriceDate = table.Column<DateTime>(nullable: true),
                    PriceDateSV1 = table.Column<DateTime>(nullable: true),
                    PriceDateSV2 = table.Column<DateTime>(nullable: true),
                    MinReorderQty = table.Column<float>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonePricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZonePricing_Created_By",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CostZone_PriceZone",
                        column: x => x.PriceZoneId,
                        principalTable: "CostPriceZones",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Product_PriceZone",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ZonePricing_Updated_By",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "AddUnlockProduct", "Address1", "Address2", "Address3", "CreatedAt", "CreatedById", "DateOfBirth", "Email", "FirstName", "Gender", "ImagePath", "IsDeleted", "IsResetPassword", "KeypadPrefix", "LastLogin", "LastName", "MiddleName", "MobileNo", "Password", "PhoneNo", "PlainPassword", "PostCode", "PromoPrefix", "RefreshToken", "Status", "StoreIds", "TempPasswordCreatedAt", "TemporaryPassword", "Type", "UpdatedAt", "UpdatedById", "UserName", "ZoneIds" },
                values: new object[,]
                {
                    { 1, false, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "SuperAdmin@coyote.com", "SuperAdmin", null, null, false, false, null, null, "SuperAdmin", null, null, "8kx7T0Uf3VYimsY1g/7aaA==", null, null, null, null, null, true, "7", null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SuperAdmin", "657" },
                    { 2, false, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "user1@cdnsol.com", "USER1", null, null, false, false, null, null, "W", null, null, "8kx7T0Uf3VYimsY1g/7aaA==", null, null, null, null, null, true, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "User1", null },
                    { 3, false, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "coyote@mailinator.com", "COYOTE", null, null, false, false, null, null, "test", null, null, "8kx7T0Uf3VYimsY1g/7aaA==", null, null, null, null, null, true, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COYOTE_Test", null },
                    { 4, false, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "user@example.com", "swagger", null, null, false, false, null, null, "test", null, null, "QIWtwazDYZU8/oVq2Xufwg==", null, null, null, null, null, true, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Swagger", null }
                });

            migrationBuilder.InsertData(
                table: "Department",
                columns: new[] { "Id", "AdvertisingDisc", "AllowSaleDisc", "BudgetGroethFactor", "Code", "CreatedAt", "CreatedById", "Desc", "ExcludeWastageOptimalOrdering", "IsDefault", "IsDeleted", "MapTypeId", "RoyaltyDisc", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, null, null, null, "Department1", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Department Description", null, null, false, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "EmailTemplate",
                columns: new[] { "Id", "Body", "CreatedAt", "CreatedById", "DisplayName", "IsDeleted", "Name", "Status", "Subject", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, @"Dear %UserFirstName% %UserLastName% <br /> <br />

                                             Username: %Username% <br />
                			                 You will be required to change your password.  Coyote Application system. <br />
                			                 Please click the following URL to change your password: %TemporaryPasswordURL%  <br />
                			                 Please do not reply to this email as this message will be undeliverable <br /> <br />
                			 

                                              Coyote Application Team", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Forgot Password", false, "Forgot Password", false, "Forgot Password", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, @"Dear %UserFirstName% %UserLastName% <br /> <br />

                                         Username: %Username%  <br />
                			             You will be required to change your password the first time you log into the Coyote Application system.<br />
                			             Coyote Application system. <br />
                			             Please click the following URL to change your password: %TemporaryPasswordURL% <br />
                			             Please do not reply to this email as this message will be undeliverable <br />
                			             <br />

                                          Coyote Application Team", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "New User Creation", false, "New User Creation", false, "New User Creation", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "MasterList",
                columns: new[] { "Id", "AccessId", "Code", "CreatedAt", "CreatedById", "IsDeleted", "Name", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 50, 1, "STOCKTAKE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "STOCKTAKE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 49, 1, "SAV_AUTO_ORDER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "SAV_AUTO_ORDER", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 48, 1, "ROYALTY_SCALES", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ROYALTY_SCALES", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 47, 1, "PROMOTION", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "PROMOTION", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 46, 1, "PRICEZONE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "PRICEZONE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 45, 1, "PDELOAD", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "PDELOAD", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 43, 1, "OUTLET_SUPPLIER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "OUTLET_SUPPLIER", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 51, 1, "STOCKTAKEI", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "STOCKTAKEI", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 42, 1, "OUTLET_FIFO", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "OUTLET_FIFO", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 41, 1, "OUTLET_BUDGETS", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "OUTLET_BUDGETS", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 40, 1, "OFFER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "OFFER", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 39, 1, "OCCUPATION", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "OCCUPATION", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 38, 1, "MM_OLD", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MM_OLD", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 44, 1, "PATH", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "PATH", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 52, 1, "STOREGROUP", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "STOREGROUP", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 54, 1, "SUPPLIER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "SUPPLIER", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 37, 1, "MIXMATCH", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MIXMATCH", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 68, 1, "OrderCreationType", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "Order Creation Type", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 67, 1, "OrderStatus", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "Order Status", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 66, 1, "OrderType", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "Order Type", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 65, 1, "DEPT_MAPTYPE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "Department Map Type", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 64, 1, "PromotionFrequency", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "PromotionFrequency", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 63, 1, "TYPE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "TYPE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 53, 1, "SUBRANGE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "SUBRANGE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 62, 1, "ZONEOUTLET", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ZONEOUTLET", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 60, 1, "WAREHOUSE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "WAREHOUSE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 59, 1, "USERTYPE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "USERTYPE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 58, 1, "USERLOG", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "USERLOG", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 57, 1, "TAXCODE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "TAXCODE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 56, 1, "SYSCONTROLS", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "SYSCONTROLS", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 55, 1, "SYNCTILL", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "SYNCTILL", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 61, 1, "XERO_ACCOUNT", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "XERO_ACCOUNT", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 35, 1, "MEMBERCLASS", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MEMBERCLASS", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 36, 1, "MEMBEROFFER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MEMBEROFFER", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 33, 1, "MANUALSALE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MANUALSALE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 34, 1, "MANUALSALEI", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MANUALSALEI", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 13, 1, "ACCESSMENU", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ACCESSMENU", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 12, 1, "ACCESSGROUP", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ACCESSGROUP", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 11, 1, "ACCESSDEPT", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ACCESSDEPT", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 10, 1, "PROMOTYPE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "PROMO TYPE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 9, 1, "ACTION", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "PERMISSION", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 15, 1, "ADJUSTMENT", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ADJUSTMENT", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 8, 1, "CONTROLLER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MODULE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 6, 1, "UNITOFMEASURE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "UNIT OF MEASURE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 5, 1, "NATIONALRANGE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "NATIONAL RANGE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, 1, "GROUP", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "GROUP", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 1, "MANUFACTURER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "MANUFACTURER", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 1, "CATEGORY", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "CATEGORY", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 1, 1, "ZONE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ZONE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 7, 1, "WAREHOUSEHOSTFORMAT", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "WAREHOUSE HOST FORMAT", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 16, 1, "ADJUSTMENTI", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ADJUSTMENTI", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 14, 1, "ADJUSTCODE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ADJUSTCODE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 18, 1, "COMMODITY", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "COMMODITY", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 32, 1, "LABELTYPE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "LABELTYPE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 31, 1, "KEYPAD_LEVS", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "KEYPAD_LEVS", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 30, 1, "KEYPAD_BTNS", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "KEYPAD_BTNS", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 17, 1, "CASHIERTYPE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "CASHIERTYPE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 28, 1, "JNL_POSTING_ON", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "JNL_POSTING_ON", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 27, 1, "HOSTUPD_CHG", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "HOSTUPD_CHG", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 26, 1, "HOSTUPD", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "HOSTUPD", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 29, 1, "KEYPAD", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "KEYPAD", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 24, 1, "ENTITY_EDIT", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "ENTITY_EDIT", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 23, 1, "DEPARTMENT", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "DEPARTMENT", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 22, 1, "DEBTORTERMS", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "DEBTORTERMS", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 21, 1, "DEBTORTERM", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "DEBTORTERM", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 20, 1, "CRUSERFLAGS", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "CRUSERFLAGS", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 19, 1, "COSTZONE", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "COSTZONE", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 25, 1, "HOST", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "HOST", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedById", "IsDeleted", "Name", "PermissionDeptSet", "PermissionSet", "Status", "Type", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, "SuperAdmin", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "SuperAdmin", "", "*", true, "SuperAdmin", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Admin", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "Admin", "", "*", true, "Admin", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, "Super", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "Super", "", "*", true, "Super", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "StoreGroup",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedById", "IsDeleted", "Name", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, "Super Admin Store Group", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, "Super Admin Store Group", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "Supplier",
                columns: new[] { "Id", "ABN", "Address1", "Address2", "Address3", "Address4", "Code", "Contact", "CostZone", "CreatedAt", "CreatedById", "Desc", "Email", "Fax", "GSTFreeItemCode", "GSTFreeItemDesc", "GSTInclItemCode", "GSTInclItemDesc", "IsDeleted", "Phone", "PromoSupplier", "UpdateCost", "UpdatedAt", "UpdatedById", "XeroName" },
                values: new object[] { 1, null, null, null, null, null, "Supp1", null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Supplier1", null, null, null, null, null, null, false, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null });

            migrationBuilder.InsertData(
                table: "SystemControls",
                columns: new[] { "ID", "AllocateGroups", "AllowALM", "AllowFIFO", "Color", "CreatedBy", "CreatedDate", "DatabaseUsage", "DefaultItemPricing", "ExpiryDate", "GrowthFactor", "HostUpdatePricing", "InvoicePostPricing", "IsActive", "LicenceKey", "MassPriceUpdate", "MaxStores", "ModifiedBy", "ModifiedDate", "Name", "NumberFactor", "PriceRounding", "SerialNo", "TillJournal", "TransactionRef", "TransferRef", "WastageRef" },
                values: new object[] { 1, false, false, false, "Blue(Default)", 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0f, false, false, 1, "123", "REAL", 20, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Test Controll", 1000000L, 0, "12345", 0, null, null, null });

            migrationBuilder.InsertData(
                table: "Tax",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedById", "Desc", "Factor", "IsDeleted", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1L, "codTax1", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "codTax1", 1f, false, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "AccessDepartment",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DepartmentId", "IsDeleted", "RoleId", "RolesId", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, false, 1, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "Commodity",
                columns: new[] { "Id", "Code", "CoverDays", "CreatedAt", "CreatedById", "DepartmentId", "Desc", "GPPcntLevel1", "GPPcntLevel2", "GPPcntLevel3", "GPPcntLevel4", "IsDeleted", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, "Comodity1", null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Comodity Description", null, null, null, null, false, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "MasterListItems",
                columns: new[] { "Id", "AccessId", "Code", "Col1", "Col2", "Col3", "Col4", "Col5", "CreatedAt", "CreatedById", "IsDeleted", "ListId", "Name", "Num1", "Num2", "Num3", "Num4", "Num5", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 80, 1, "SYSCONTROLS", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 56, "SYSCONTROLS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 79, 1, "SYNCTILL", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 55, "SYNCTILL", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 78, 1, "SUPPLIER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 54, "SUPPLIER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 77, 1, "SUBRANGE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 53, "SUBRANGE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 76, 1, "STOREGROUP", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 52, "STOREGROUP", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 75, 1, "STOCKTAKEI", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 51, "STOCKTAKEI", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 74, 1, "STOCKTAKE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 50, "STOCKTAKE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 73, 1, "SAV_AUTO_ORDER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 49, "SAV_AUTO_ORDER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 72, 1, "ROYALTY_SCALES", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 48, "ROYALTY_SCALES", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 71, 1, "PROMOTION", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 47, "PROMOTION", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 70, 1, "PRICEZONE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 46, "PRICEZONE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 69, 1, "PDELOAD", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 45, "PDELOAD", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 68, 1, "PATH", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 44, "PATH", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 67, 1, "OUTLET_SUPPLIER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 43, "OUTLET_SUPPLIER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 66, 1, "OUTLET_FIFO", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 42, "OUTLET_FIFO", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 65, 1, "OUTLET_BUDGETS", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 41, "OUTLET_BUDGETS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 64, 1, "OFFER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 40, "OFFER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 63, 1, "OCCUPATION", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 39, "OCCUPATION", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 62, 1, "MM_OLD", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 38, "MM_OLD", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 61, 1, "MIXMATCH", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 37, "MIXMATCH", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 60, 1, "MEMBEROFFER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 36, "MEMBEROFFER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 59, 1, "MEMBERCLASS", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 35, "MEMBERCLASS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 58, 1, "MANUALSALEI", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 34, "MANUALSALEI", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 81, 1, "TAXCODE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 57, "TAXCODE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 82, 1, "USERLOG", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 58, "USERLOG", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 83, 1, "USERTYPE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 59, "USERTYPE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 84, 1, "WAREHOUSE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 60, "WAREHOUSE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 108, 1, "6", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "6", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 107, 1, "5", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "5", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 106, 1, "4", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "4", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 105, 1, "3", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "3", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 104, 1, "2", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "2", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 103, 1, "1", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 68, "1", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 102, 1, "TRANSFER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 67, "TRANSFER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 101, 1, "INVOICE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 67, "INVOICE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 100, 1, "DELIVERY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 67, "DELIVERY", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 99, 1, "ORDER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 67, "ORDER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 98, 1, "NEW", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 67, "NEW", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 57, 1, "MANUALSALE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 33, "MANUALSALE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 97, 1, "TRANSFER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 66, "TRANSFER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 95, 1, "DELIVERY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 66, "DELIVERY", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 94, 1, "ORDER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 66, "ORDER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 93, 1, "MAP-TYPE-2", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 65, "MAP-TYPE-2", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 92, 1, "MAP_TYPE-1", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 65, "MAP_TYPE-1", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 91, 1, "MONTHLY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 64, "ACTIVE DATE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 90, 1, "WEEKLY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 64, "ACTIVE WEEK DAYS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 89, 1, "DAILY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 64, "ACTIVE HOURS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 88, 1, "HOURLY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 64, "ACTIVE MINUTES", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 87, 1, "TYPE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 63, "TYPE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 86, 1, "ZONEOUTLET", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 62, "ZONEOUTLET", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 85, 1, "XERO_ACCOUNT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 61, "XERO_ACCOUNT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 96, 1, "INVOICE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 66, "INVOICE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 55, 1, "KEYPAD_LEVS", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 31, "KEYPAD_LEVS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 56, 1, "LABELTYPE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 32, "LABELTYPE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 53, 1, "KEYPAD", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 29, "KEYPAD", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 24, 1, "ZONEOUTL", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "ZONEOUTLET", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 23, 1, "WAREHOUSE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "WAREHOUSE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 22, 1, "USERROLE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "USERROLE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 21, 1, "USER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "USER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 20, 1, "TAX", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "TAX", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 19, 1, "SUPPLIERPRODUCT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "SUPPLIERPRODUCT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 18, 1, "SUPPLIER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "SUPPLIER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 17, 1, "STOREGROUP", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "STOREGROUP", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 16, 1, "STORE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "STORE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 15, 1, "ROLE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "ROLE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 14, 1, "PRODUCT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "PRODUCT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 54, 1, "KEYPAD_BTNS", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 30, "KEYPAD_BTNS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 13, 1, "MASTERLISTITEM", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "MASTERLISTITEM", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 11, 1, "LOGIN", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "LOGIN", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 10, 1, "DEPARTMENT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "DEPARTMENT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 9, 1, "COMMODITY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "COMMODITY", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 8, 1, "APN", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "APN", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 7, 1, "WAREHOUSEHOSTFORMAT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 7, "WAREHOUSE HOST FORMAT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 6, 1, "UNITOFMEASURE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 6, "UNIT OF MEASURE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 5, 1, "NATIONALRANGE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 5, "NATIONAL RANGE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, 1, "GROUP", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 4, "GROUP", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 1, "MANUFACTURER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 3, "MANUFACTURER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 1, "CATEGORY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 2, "CATEGORY", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 1, 1, "ZONE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 1, "ZONE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 12, 1, "MASTERLIST", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 8, "MASTERLIST", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 26, 1, "ADD", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 9, "POST", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 25, 1, "GET", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 9, "GET", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 28, 1, "DELETE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 9, "DELETE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 52, 1, "JNL_POSTING_ON", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 28, "JNL_POSTING_ON", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 51, 1, "HOSTUPD_CHG", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 27, "HOSTUPD_CHG", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 50, 1, "HOSTUPD", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 26, "HOSTUPD", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 49, 1, "HOST", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 25, "HOST", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 48, 1, "ENTITY_EDIT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 24, "ENTITY_EDIT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 47, 1, "DEPARTMENT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 23, "DEPARTMENT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 46, 1, "DEBTORTERMS", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 22, "DEBTORTERMS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 45, 1, "DEBTORTERM", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 21, "DEBTORTERM", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 44, 1, "CRUSERFLAGS", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 20, "CRUSERFLAGS", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 27, 1, "UPDATE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 9, "PUT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 42, 1, "COMMODITY", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 18, "COMMODITY", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 41, 1, "CASHIERTYPE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 17, "CASHIERTYPE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 43, 1, "COSTZONE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 19, "COSTZONE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 39, 1, "ADJUSTMENT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 15, "ADJUSTMENT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 29, 1, "OFFER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 10, "OFFER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "MasterListItems",
                columns: new[] { "Id", "AccessId", "Code", "Col1", "Col2", "Col3", "Col4", "Col5", "CreatedAt", "CreatedById", "IsDeleted", "ListId", "Name", "Num1", "Num2", "Num3", "Num4", "Num5", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 30, 1, "BUYING", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 10, "BUYING", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 40, 1, "ADJUSTMENTI", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 16, "ADJUSTMENTI", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 32, 1, "COMPITITION", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 10, "COMPITITION", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 33, 1, "MEMBEROFFER", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 10, "MEMBEROFFER", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 31, 1, "SELLING", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 10, "SELLING", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 35, 1, "ACCESSDEPT", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 11, "ACCESSDEPT", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 36, 1, "ACCESSGROUP", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 12, "ACCESSGROUP", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 37, 1, "ACCESSMENU", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 13, "ACCESSMENU", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 38, 1, "ADJUSTCODE", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 14, "ADJUSTCODE", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 34, 1, "MIXMATCH", null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 10, "MIXMATCH", null, null, null, null, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "Store",
                columns: new[] { "Id", "Abn", "Address1", "Address2", "Address3", "AddressOnApp", "AppOrders", "BudgetGrowthFact", "Code", "CostInd", "CostType", "CostZoneId", "CreatedAt", "CreatedById", "DelAddr1", "DelAddr2", "DelAddr3", "DelName", "DelPostCode", "Desc", "DisplayOnApp", "Email", "EntityNumber", "Fax", "FuelSite", "GroupId", "IsDeleted", "LabelTypePromoId", "LabelTypeShelfId", "LabelTypeShortId", "Latitude", "Longitude", "NameOnApp", "OpenHours", "OutletPriceFromOutletId", "PhoneNumber", "PostCode", "PriceFromLevel", "PriceLevelDesc1", "PriceLevelDesc2", "PriceLevelDesc3", "PriceLevelDesc4", "PriceLevelDesc5", "PriceZoneId", "SellingInd", "Status", "StockInd", "UpdatedAt", "UpdatedById", "WarehouseId" },
                values: new object[] { 1, null, null, null, null, null, null, null, "SuperAdminStore", null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, null, null, null, "Super Admin Store", null, null, null, null, null, 1, false, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "IsDefault", "IsDeleted", "RoleId", "StoreId", "UpdatedAt", "UpdatedById", "UserId" },
                values: new object[,]
                {
                    { 4, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, false, 3, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4 },
                    { 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, false, 1, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, false, 2, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 3, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, false, 3, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3 }
                });

            migrationBuilder.InsertData(
                table: "Cashier",
                columns: new[] { "Id", "AccessLevelId", "Addr1", "Addr2", "Addr3", "CreatedAt", "CreatedById", "DateOfBirth", "DepartmentsId", "Dispname", "Email", "FirstName", "FuelPass", "FuelUser", "Gender", "ImagePath", "IsDeleted", "LeftHandTillInd", "Mobile", "Number", "OutletId", "Password", "Phone", "Postcode", "Status", "StoreGroupId", "Surname", "TypeId", "UpdatedAt", "UpdatedById", "WristBandInd", "ZoneId" },
                values: new object[] { 1, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, null, "cashier@coyote.com", "cashier1", null, null, null, null, false, null, null, 1L, 1, "123456654", null, null, true, 1, "cash sur", 41, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1 });

            migrationBuilder.InsertData(
                table: "GLAccount",
                columns: new[] { "Id", "AccountNumber", "AccountSystem", "Company", "CreatedAt", "CreatedById", "Desc", "IsDeleted", "StoreId", "SupplierId", "TypeId", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, "123", "Xero", 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "GLAccount", false, 1, 1, 41, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "Keypad",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedById", "Desc", "IsDeleted", "KeyPadButtonJSONData", "OutletId", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, "792_Keypad", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "792 Keypad", false, null, 1, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DateFrom", "DateTo", "DayParts", "Desc", "DisplayType", "Image", "ImageType", "IsDeleted", "POSMessage", "Priority", "ReferenceId", "ReferenceOverrideType", "ReferenceType", "Status", "UpdatedAt", "UpdatedById", "ZoneId" },
                values: new object[] { 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2022, 2, 14, 13, 49, 5, 358, DateTimeKind.Utc).AddTicks(4902), new DateTime(2022, 3, 1, 13, 49, 5, 358, DateTimeKind.Utc).AddTicks(5487), "YYYYYYYYYYYYYYYYYYYYYYY", "Prod/Promo Desc", "1", null, null, false, "Message for POS", 1, "Prod_1", null, "Product", true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 });

            migrationBuilder.InsertData(
                table: "ModuleActions",
                columns: new[] { "Id", "Action", "ActionTypeId", "CreatedAt", "CreatedById", "Desc", "IsDeleted", "ModuleId", "Name", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, "Get", null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, false, 8, "Get", false, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Post", null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, false, 8, "Post", false, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, "Put", null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, false, 8, "Put", false, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, "Delete", null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, false, 8, "Delete", false, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "OutletSupplierSchedule",
                columns: new[] { "Id", "CoverDays", "CoverDaysDiscountThreshold1", "CoverDaysDiscountThreshold2", "CoverDaysDiscountThreshold3", "CreatedAt", "CreatedById", "DOWGenerateOrder", "DiscountThresholdOne", "DiscountThresholdThree", "DiscountThresholdTwo", "InvoiceOrderOffset", "IsDeleted", "LastRun", "MultipleOrdersInAWeek", "OrderNonDefaultSupplier", "ReceiveOrderOffset", "SendOrderOffset", "StoreId", "SupplierId", "UpdatedAt", "UpdatedById", "UserCreatedById" },
                values: new object[] { 1, 0, 1, 2, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1f, 1f, 1f, 0, false, null, true, false, 0, 0, 1, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null });

            migrationBuilder.InsertData(
                table: "OutletSupplierSetting",
                columns: new[] { "Id", "BuyCartoon", "CreatedAt", "CreatedById", "CustomerNumber", "Desc", "DivisionId", "IsDeleted", "Password", "PhoneNumber", "QtyDefault", "StateId", "Status", "StoreId", "SupplierId", "UpdatedAt", "UpdatedById", "UserId" },
                values: new object[] { 1, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "123654", "First OutletSupplierSetting", 1, false, "testPassword", "1234567890", "1", 1, true, 1, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "5462Qt61" });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "AccessOutletIds", "AltSupplier", "AustMadeInd", "AustOwnedInd", "CartonCost", "CartonQty", "CategoryId", "CommodityId", "CreatedAt", "CreatedById", "DeactivatedAt", "DeletedAt", "DepartmentId", "Desc", "Freight", "GenericInd", "GmFlagInd", "GroupId", "HeartSmartInd", "HostItemType", "HostItemType2", "HostItemType3", "HostItemType4", "HostItemType5", "HostNumber", "HostNumber2", "HostNumber3", "HostNumber4", "ImagePath", "Info", "IsDeleted", "LabelQty", "LastApnSold", "Litres", "ManufacturerId", "NationalRangeId", "Number", "OrganicInd", "Parent", "PosDesc", "Replicate", "Rrp", "ScaleInd", "SeasonalInd", "Size", "SlowMovingInd", "Status", "StoreFrozenInd", "StoreId", "SupplierId", "TareWeight", "TaxId", "TypeId", "UnitMeasureId", "UnitQty", "UpdatedAt", "UpdatedById", "VarietyInd", "WarehouseFrozenInd" },
                values: new object[] { 1L, "1", null, null, null, 12f, 12, 2, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, 1, "First prod1", null, null, null, 4, null, null, null, null, null, null, null, null, null, null, null, null, false, null, null, null, 3, 5, 1L, null, null, null, null, null, null, null, null, null, true, null, null, 1, null, 1L, 63, 6, 0f, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null });

            migrationBuilder.InsertData(
                table: "Promotion",
                columns: new[] { "Id", "Availibility", "Code", "CreatedAt", "CreatedById", "Desc", "End", "FrequencyId", "ImagePath", "IsDeleted", "PromotionTypeId", "RptGroup", "SourceId", "Start", "Status", "UpdatedAt", "UpdatedById", "ZoneId" },
                values: new object[,]
                {
                    { 6, "YYYYYYY", "MEMBEROFFER", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "MEMBEROFFER 1", new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 90, null, false, 33, null, 33, new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 3, "YYYYYYY", "BUYING", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "BUYING 1", new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 90, null, false, 30, null, 30, new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 4, "YYYYYYY", "SELLING", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "SELLING 1", new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 90, null, false, 31, null, 31, new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, "YYYYYYY", "Offer", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "off 1", new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 90, null, false, 29, null, 29, new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 1, "YYYYYYY", "Mixmatch1", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Mix 1", new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 90, null, false, 34, null, 34, new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 5, "YYYYYYY", "COMPITITION", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "COMPITITION 1", new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 90, null, false, 32, null, 32, new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Warehouse",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedById", "Desc", "HostFormatId", "IsDeleted", "Status", "SupplierId", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, "WAREHOUSE1", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "WAREHOUSE1", 7, false, false, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "XeroAccount",
                columns: new[] { "Id", "Anex", "AnexDesc", "BalanceSheet", "BalanceSheetDesc", "CashEFTPOS", "CashEFTPOSDesc", "CreatedAt", "CreatedById", "Desc", "FinAccSummary", "FleetCard", "FleetCardDesc", "FuelCard", "FuelCardDesc", "GSTProdSale", "GSTProdSaleDesc", "IsDeleted", "LessUberEats", "LessUberEatsDesc", "MotorCharge", "MotorChargeDesc", "MotorPass", "MotorPassDesc", "NonGSTProdSale", "NonGSTProdSaleDesc", "Other", "OtherDesc", "ProfitLoss", "ProfitLossDesc", "StockAccSummary", "StoreId", "UnderOver", "UnderOverDesc", "UpdatedAt", "UpdatedById", "UserCreatedById", "XeroConsumerKey", "XeroSecretKey" },
                values: new object[] { 1, null, null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "First prod1", null, null, null, null, null, null, null, false, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, null });

            migrationBuilder.InsertData(
                table: "ZoneOutlet",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "IsDeleted", "StoreId", "UpdatedAt", "UpdatedById", "ZoneId" },
                values: new object[] { 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 7 });

            migrationBuilder.InsertData(
                table: "APN",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Desc", "IsDeleted", "Number", "ProductId", "SoldDate", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1L, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Test", false, 1L, 1L, null, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "OutletProduct",
                columns: new[] { "Id", "AllMember", "CartonCost", "CartonCostAvg", "CartonCostHost", "CartonCostInv", "ChangeLabelInd", "ChangeLabelPrinted", "ChangeTillInd", "CreatedAt", "CreatedById", "FifoStock", "GenCode", "HoldNorm", "IsDeleted", "LabelQty", "MaxOnHand", "MinOnHand", "MinReorderQty", "Mrp", "NormalPrice1", "NormalPrice2", "NormalPrice3", "NormalPrice4", "NormalPrice5", "OpenPrice", "PickingBinNo", "ProductId", "PromoBuy", "PromoBuyId", "PromoCartonCost", "PromoComp", "PromoCompId", "PromoMemOff", "PromoMemOffPrice1", "PromoMemOffPrice2", "PromoMemOffPrice3", "PromoMemOffPrice4", "PromoMemOffPrice5", "PromoMemeberOfferId", "PromoMix1", "PromoMix2", "PromoMixMatch1Id", "PromoMixMatch2Id", "PromoOffer1", "PromoOffer1Id", "PromoOffer2", "PromoOffer2Id", "PromoOffer3", "PromoOffer3Id", "PromoOffer4", "PromoOffer4Id", "PromoPrice1", "PromoPrice2", "PromoPrice3", "PromoPrice4", "PromoPrice5", "PromoSell", "PromoSellId", "QtyOnHand", "ScalePlu", "ShortLabelInd", "SkipReorder", "SpecCartonCost", "SpecCode", "SpecFrom", "SpecPrice", "SpecTo", "Status", "StoreId", "SupplierId", "Till", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1L, null, 0f, null, null, null, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, null, false, null, null, 0f, null, null, 1f, null, null, null, null, null, 0, 1L, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0f, null, null, false, null, null, null, null, null, true, 1, 1, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "PromoBuying",
                columns: new[] { "Id", "Action", "AmtOffNorm1", "CartonCost", "CartonQty", "CostEnd", "CostIsPromInd", "CostStart", "CreatedAt", "CreatedById", "Desc", "HostPromoType", "IsDeleted", "ProductId", "PromoUnits", "PromotionId", "Status", "SupplierId", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, null, null, 12f, 12, null, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, false, 1L, null, 3, true, 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "PromoMemberOffer",
                columns: new[] { "Id", "Action", "AmtOffNorm1", "CreatedAt", "CreatedById", "Desc", "HostPromoType", "IsDeleted", "Price", "Price1", "Price2", "Price3", "Price4", "ProductId", "PromoUnits", "PromotionId", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, false, 1f, null, null, null, null, 1L, null, 6, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "PromoMixmatch",
                columns: new[] { "Id", "Amt1", "Amt2", "CreatedAt", "CreatedById", "CumulativeOffer", "DiscPcnt1", "DiscPcnt2", "Group", "IsDeleted", "PriceLevel1", "PriceLevel2", "PromotionId", "Qty1", "Qty2", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, 1f, 1f, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 10f, 20f, (short)0, false, null, null, 1, 2f, 2f, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "PromoOffer",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Group", "Group1Price", "Group1Qty", "Group2Price", "Group2Qty", "Group3Price", "Group3Qty", "IsDeleted", "PromotionId", "Status", "TotalPrice", "TotalQty", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, (short)1, "Free", 1f, "1.0", 2f, null, null, false, 1, false, 20f, 3f, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "PromoSelling",
                columns: new[] { "Id", "Action", "AmtOffNorm1", "CreatedAt", "CreatedById", "Desc", "HostPromoType", "IsDeleted", "Price", "Price1", "Price2", "Price3", "Price4", "ProductId", "PromoUnits", "PromotionId", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, false, 1f, null, null, null, null, 1L, null, 4, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "SupplierProduct",
                columns: new[] { "Id", "BestBuy", "CartonCost", "CreatedAt", "CreatedById", "Desc", "IsDeleted", "MinReorderQty", "ProductId", "Status", "SupplierId", "SupplierItem", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1L, null, 10f, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, false, null, 1L, true, 1, "Test", new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "PromoMixmatchProduct",
                columns: new[] { "Id", "Action", "AmtOffNorm1", "CreatedAt", "CreatedById", "Desc", "HostPromoType", "IsDeleted", "ProductId", "PromoUnits", "PromotionMixmatchId", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1L, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, false, 1L, null, 1, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "PromoOfferProduct",
                columns: new[] { "Id", "Action", "AmtOffNorm1", "CreatedAt", "CreatedById", "Desc", "HostPromoType", "IsDeleted", "OfferGroup", "ProductId", "PromoUnits", "PromotionOfferId", "Status", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, null, null, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, false, (byte)1, 1L, null, 1, true, new DateTime(2020, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AccessDepartment_CreatedById",
                table: "AccessDepartment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccessDepartment_DepartmentId",
                table: "AccessDepartment",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessDepartment_RolesId",
                table: "AccessDepartment",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessDepartment_UpdatedById",
                table: "AccessDepartment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLoyalty_CreatedById",
                table: "AccountLoyalty",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLoyalty_UpdatedById",
                table: "AccountLoyalty",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_CategoryId",
                table: "AccountTransaction",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_CommodityId",
                table: "AccountTransaction",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_CreatedById",
                table: "AccountTransaction",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_DepartmentId",
                table: "AccountTransaction",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_ManufacturerId",
                table: "AccountTransaction",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_OutletId",
                table: "AccountTransaction",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_ProductId",
                table: "AccountTransaction",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_PromoBuyId",
                table: "AccountTransaction",
                column: "PromoBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_PromoSellId",
                table: "AccountTransaction",
                column: "PromoSellId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_SupplierId",
                table: "AccountTransaction",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_TillId",
                table: "AccountTransaction",
                column: "TillId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_UpdatedById",
                table: "AccountTransaction",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_UserId",
                table: "AccountTransaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_APN_CreatedById",
                table: "APN",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_APNNumber_Unique",
                table: "APN",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APN_ProductId",
                table: "APN",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_APN_UpdatedById",
                table: "APN",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOrderSettings_CreatedById",
                table: "AutoOrderSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOrderSettings_StoreId",
                table: "AutoOrderSettings",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOrderSettings_SupplierId",
                table: "AutoOrderSettings",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOrderSettings_UpdatedById",
                table: "AutoOrderSettings",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOrderFromTablet_CreatedById",
                table: "BulkOrderFromTablet",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOrderFromTablet_OutletId",
                table: "BulkOrderFromTablet",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOrderFromTablet_UpdatedById",
                table: "BulkOrderFromTablet",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_AccessLevelId",
                table: "Cashier",
                column: "AccessLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_CreatedById",
                table: "Cashier",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_DepartmentsId",
                table: "Cashier",
                column: "DepartmentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_OutletId",
                table: "Cashier",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_StoreGroupId",
                table: "Cashier",
                column: "StoreGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_TypeId",
                table: "Cashier",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_UpdatedById",
                table: "Cashier",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_ZoneId",
                table: "Cashier",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Commodity_CreatedById",
                table: "Commodity",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Commodity_DepartmentId",
                table: "Commodity",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Commodity_UpdatedById",
                table: "Commodity",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Commodity_Delete_Unique",
                table: "Commodity",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_CreatedById",
                table: "CompetitionDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_PromotionId",
                table: "CompetitionDetail",
                column: "PromotionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_ResetCycleId",
                table: "CompetitionDetail",
                column: "ResetCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_RewardTypeId",
                table: "CompetitionDetail",
                column: "RewardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_TriggerTypeId",
                table: "CompetitionDetail",
                column: "TriggerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_TypeId",
                table: "CompetitionDetail",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_UpdatedById",
                table: "CompetitionDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_ZoneId",
                table: "CompetitionDetail",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionDetail_Code_IsDeleted_Unique",
                table: "CompetitionDetail",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionReward_CompPromoId",
                table: "CompetitionReward",
                column: "CompPromoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionReward_CreatedById",
                table: "CompetitionReward",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionReward_UpdatedById",
                table: "CompetitionReward",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionTrigger_CompPromoId",
                table: "CompetitionTrigger",
                column: "CompPromoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionTrigger_CreatedById",
                table: "CompetitionTrigger",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionTrigger_TriggerProductGroupID",
                table: "CompetitionTrigger",
                column: "TriggerProductGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionTrigger_UpdatedById",
                table: "CompetitionTrigger",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CostPriceZones_CreatedBy",
                table: "CostPriceZones",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CostPriceZones_HostSettingID",
                table: "CostPriceZones",
                column: "HostSettingID");

            migrationBuilder.CreateIndex(
                name: "IX_CostPriceZones_ModifiedBy",
                table: "CostPriceZones",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CSCPERIOD_CreatedBy",
                table: "CSCPERIOD",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CSCPERIOD_ModifiedBy",
                table: "CSCPERIOD",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreatedById",
                table: "Department",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Department_MapTypeId",
                table: "Department",
                column: "MapTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_UpdatedById",
                table: "Department",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Department_Delete_Unique",
                table: "Department",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_CreatedById",
                table: "EmailTemplate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_Name",
                table: "EmailTemplate",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_UpdatedById",
                table: "EmailTemplate",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EPAY_CreatedBy",
                table: "EPAY",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EPAY_ModifiedBy",
                table: "EPAY",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EPAY_ProductID",
                table: "EPAY",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccount_CreatedById",
                table: "GLAccount",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccount_StoreId",
                table: "GLAccount",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccount_SupplierId",
                table: "GLAccount",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccount_TypeId",
                table: "GLAccount",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccount_UpdatedById",
                table: "GLAccount",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HostSettings_CreatedBy",
                table: "HostSettings",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HostSettings_FilePathID",
                table: "HostSettings",
                column: "FilePathID");

            migrationBuilder.CreateIndex(
                name: "IX_HostSettings_HostFormatID",
                table: "HostSettings",
                column: "HostFormatID");

            migrationBuilder.CreateIndex(
                name: "IX_HostSettings_ModifiedBy",
                table: "HostSettings",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HostSettings_SupplierID",
                table: "HostSettings",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_HostSettings_WareHouseID",
                table: "HostSettings",
                column: "WareHouseID");

            migrationBuilder.CreateIndex(
                name: "IX_HOSTUPD_CreatedBy",
                table: "HOSTUPD",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HOSTUPD_ModifiedBy",
                table: "HOSTUPD",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_ChangeTypeId",
                table: "HostUpdChange",
                column: "ChangeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_CreatedById",
                table: "HostUpdChange",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_HostId",
                table: "HostUpdChange",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_HostUpdId",
                table: "HostUpdChange",
                column: "HostUpdId");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_OutletId",
                table: "HostUpdChange",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_ProductId",
                table: "HostUpdChange",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_PromotionId",
                table: "HostUpdChange",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_HostUpdChange_UpdatedById",
                table: "HostUpdChange",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_APNSold",
                table: "JournalDetail",
                column: "APNSold");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_CashierId",
                table: "JournalDetail",
                column: "CashierId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_CreatedById",
                table: "JournalDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_JournalHeaderId",
                table: "JournalDetail",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_ProductId",
                table: "JournalDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_PromoCompId",
                table: "JournalDetail",
                column: "PromoCompId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_PromoMemeberOfferId",
                table: "JournalDetail",
                column: "PromoMemeberOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_PromoMixMatchId",
                table: "JournalDetail",
                column: "PromoMixMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_PromoOfferId",
                table: "JournalDetail",
                column: "PromoOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_PromoSellId",
                table: "JournalDetail",
                column: "PromoSellId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetail_UpdatedById",
                table: "JournalDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_CashierId",
                table: "JournalHeader",
                column: "CashierId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_CreatedById",
                table: "JournalHeader",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_OutletId",
                table: "JournalHeader",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_TillId",
                table: "JournalHeader",
                column: "TillId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_UpdatedById",
                table: "JournalHeader",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeader_HeaderDate_HeaderTime_Outlet_Till_TransactionNo_Unique",
                table: "JournalHeader",
                columns: new[] { "HeaderDate", "HeaderTime", "OutletId", "TillId", "TransactionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keypad_CreatedById",
                table: "Keypad",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Keypad_OutletId",
                table: "Keypad",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Keypad_UpdatedById",
                table: "Keypad",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Keypad_Delete_Unique",
                table: "Keypad",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_BtnKeypadLevelId",
                table: "KeypadButton",
                column: "BtnKeypadLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_CategoryId",
                table: "KeypadButton",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_CreatedById",
                table: "KeypadButton",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_KeypadId",
                table: "KeypadButton",
                column: "KeypadId");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_LevelId",
                table: "KeypadButton",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_ProductId",
                table: "KeypadButton",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_Type",
                table: "KeypadButton",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadButton_UpdatedById",
                table: "KeypadButton",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadLevel_CreatedById",
                table: "KeypadLevel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadLevel_KeypadId",
                table: "KeypadLevel",
                column: "KeypadId");

            migrationBuilder.CreateIndex(
                name: "IX_KeypadLevel_UpdatedById",
                table: "KeypadLevel",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSale_CreatedById",
                table: "ManualSale",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSale_TypeId",
                table: "ManualSale",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSale_UpdatedById",
                table: "ManualSale",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSaleItem_CreatedById",
                table: "ManualSaleItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSaleItem_ManualSaleId",
                table: "ManualSaleItem",
                column: "ManualSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSaleItem_OutletId",
                table: "ManualSaleItem",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSaleItem_ProductId",
                table: "ManualSaleItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSaleItem_TypeId",
                table: "ManualSaleItem",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualSaleItem_UpdatedById",
                table: "ManualSaleItem",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MasterList_CreatedById",
                table: "MasterList",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MasterList_UpdatedById",
                table: "MasterList",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MasterList_Delete_Unique",
                table: "MasterList",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_MasterListItems_CreatedById",
                table: "MasterListItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MasterListItems_UpdatedById",
                table: "MasterListItems",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_List_Id_Item_Deleted_Code",
                table: "MasterListItems",
                columns: new[] { "ListId", "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedById",
                table: "Messages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedById",
                table: "Messages",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ZoneId",
                table: "Messages",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActions_ActionTypeId",
                table: "ModuleActions",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActions_CreatedById",
                table: "ModuleActions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActions_ModuleId",
                table: "ModuleActions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActions_UpdatedById",
                table: "ModuleActions",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudit_CreatedById",
                table: "OrderAudit",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudit_NewStatusId",
                table: "OrderAudit",
                column: "NewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudit_OutletId",
                table: "OrderAudit",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudit_StatusId",
                table: "OrderAudit",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudit_SupplierId",
                table: "OrderAudit",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudit_TypeId",
                table: "OrderAudit",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudit_UpdatedById",
                table: "OrderAudit",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_CheaperSupplierId",
                table: "OrderDetail",
                column: "CheaperSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_CreatedById",
                table: "OrderDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderTypeId",
                table: "OrderDetail",
                column: "OrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_SupplierProductId",
                table: "OrderDetail",
                column: "SupplierProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_UpdatedById",
                table: "OrderDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderHeaderId_LineNo_Time_Unique",
                table: "OrderDetail",
                columns: new[] { "OrderHeaderId", "LineNo", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_CreatedById",
                table: "OrderHeader",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_CreationTypeId",
                table: "OrderHeader",
                column: "CreationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_OutletId",
                table: "OrderHeader",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_StatusId",
                table: "OrderHeader",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_StoreIdAsSupplier",
                table: "OrderHeader",
                column: "StoreIdAsSupplier");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_SupplierId",
                table: "OrderHeader",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_TypeId",
                table: "OrderHeader",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_UpdatedById",
                table: "OrderHeader",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletBudget_CreatedById",
                table: "OutletBudget",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletBudget_StoreId",
                table: "OutletBudget",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletBudget_UpdatedById",
                table: "OutletBudget",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_CreatedById",
                table: "OutletProduct",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoBuyId",
                table: "OutletProduct",
                column: "PromoBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoCompId",
                table: "OutletProduct",
                column: "PromoCompId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoMemeberOfferId",
                table: "OutletProduct",
                column: "PromoMemeberOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoMixMatch1Id",
                table: "OutletProduct",
                column: "PromoMixMatch1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoMixMatch2Id",
                table: "OutletProduct",
                column: "PromoMixMatch2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoOffer1Id",
                table: "OutletProduct",
                column: "PromoOffer1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoOffer2Id",
                table: "OutletProduct",
                column: "PromoOffer2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoOffer3Id",
                table: "OutletProduct",
                column: "PromoOffer3Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoOffer4Id",
                table: "OutletProduct",
                column: "PromoOffer4Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_PromoSellId",
                table: "OutletProduct",
                column: "PromoSellId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_StoreId",
                table: "OutletProduct",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_SupplierId",
                table: "OutletProduct",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProduct_UpdatedById",
                table: "OutletProduct",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStore_Unique",
                table: "OutletProduct",
                columns: new[] { "ProductId", "StoreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutletRoyaltyScales_CreatedById",
                table: "OutletRoyaltyScales",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletRoyaltyScales_OutletId",
                table: "OutletRoyaltyScales",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletRoyaltyScales_UpdatedById",
                table: "OutletRoyaltyScales",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSchedule_CreatedById",
                table: "OutletSupplierSchedule",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSchedule_StoreId",
                table: "OutletSupplierSchedule",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSchedule_SupplierId",
                table: "OutletSupplierSchedule",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSchedule_UserCreatedById",
                table: "OutletSupplierSchedule",
                column: "UserCreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSetting_CreatedById",
                table: "OutletSupplierSetting",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSetting_DivisionId",
                table: "OutletSupplierSetting",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSetting_StateId",
                table: "OutletSupplierSetting",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSetting_StoreId",
                table: "OutletSupplierSetting",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSetting_SupplierId",
                table: "OutletSupplierSetting",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletSupplierSetting_UpdatedById",
                table: "OutletSupplierSetting",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTradingHours_CreatedById",
                table: "OutletTradingHours",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTradingHours_OuteltId",
                table: "OutletTradingHours",
                column: "OuteltId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTradingHours_UpdatedById",
                table: "OutletTradingHours",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Paths_CreatedBy",
                table: "Paths",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Paths_ModifiedBy",
                table: "Paths",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Paths_OutletID",
                table: "Paths",
                column: "OutletID");

            migrationBuilder.CreateIndex(
                name: "IX_PrintLabelType_CreatedById",
                table: "PrintLabelType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintLabelType_UpdatedById",
                table: "PrintLabelType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintLabelType_Delete_Unique",
                table: "PrintLabelType",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CommodityId",
                table: "Product",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CreatedById",
                table: "Product",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Product_DepartmentId",
                table: "Product",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_GroupId",
                table: "Product",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ManufacturerId",
                table: "Product",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_NationalRangeId",
                table: "Product",
                column: "NationalRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductNumber_Unique",
                table: "Product",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_StoreId",
                table: "Product",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SupplierId",
                table: "Product",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_TaxId",
                table: "Product",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_TypeId",
                table: "Product",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_UnitMeasureId",
                table: "Product",
                column: "UnitMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_UpdatedById",
                table: "Product",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoBuying_CreatedById",
                table: "PromoBuying",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoBuying_ProductId",
                table: "PromoBuying",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoBuying_PromotionId",
                table: "PromoBuying",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoBuying_SupplierId",
                table: "PromoBuying",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoBuying_UpdatedById",
                table: "PromoBuying",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMemberOffer_CreatedById",
                table: "PromoMemberOffer",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMemberOffer_ProductId",
                table: "PromoMemberOffer",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMemberOffer_PromotionId",
                table: "PromoMemberOffer",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMemberOffer_UpdatedById",
                table: "PromoMemberOffer",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMixmatch_CreatedById",
                table: "PromoMixmatch",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMixmatch_PromotionId",
                table: "PromoMixmatch",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMixmatch_UpdatedById",
                table: "PromoMixmatch",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMixmatchProduct_CreatedById",
                table: "PromoMixmatchProduct",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMixmatchProduct_ProductId",
                table: "PromoMixmatchProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMixmatchProduct_PromotionMixmatchId",
                table: "PromoMixmatchProduct",
                column: "PromotionMixmatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoMixmatchProduct_UpdatedById",
                table: "PromoMixmatchProduct",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoOffer_CreatedById",
                table: "PromoOffer",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoOffer_PromotionId",
                table: "PromoOffer",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoOffer_UpdatedById",
                table: "PromoOffer",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoOfferProduct_CreatedById",
                table: "PromoOfferProduct",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoOfferProduct_ProductId",
                table: "PromoOfferProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoOfferProduct_PromotionOfferId",
                table: "PromoOfferProduct",
                column: "PromotionOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoOfferProduct_UpdatedById",
                table: "PromoOfferProduct",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoSelling_CreatedById",
                table: "PromoSelling",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromoSelling_ProductId",
                table: "PromoSelling",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoSelling_PromotionId",
                table: "PromoSelling",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoSelling_UpdatedById",
                table: "PromoSelling",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_CreatedById",
                table: "Promotion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_FrequencyId",
                table: "Promotion",
                column: "FrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_PromotionTypeId",
                table: "Promotion",
                column: "PromotionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_SourceId",
                table: "Promotion",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_UpdatedById",
                table: "Promotion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_ZoneId",
                table: "Promotion",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_Delete_Unique",
                table: "Promotion",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCompetition_CompetitionId",
                table: "PromotionCompetition",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCompetition_CreatedById",
                table: "PromotionCompetition",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCompetition_ProductId",
                table: "PromotionCompetition",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCompetition_UpdatedById",
                table: "PromotionCompetition",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RebateDetails_CreatedById",
                table: "RebateDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RebateDetails_ProductId",
                table: "RebateDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RebateDetails_RebateHeaderId",
                table: "RebateDetails",
                column: "RebateHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_RebateDetails_UpdatedById",
                table: "RebateDetails",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RebateHeader_CreatedById",
                table: "RebateHeader",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RebateHeader_ManufacturerId",
                table: "RebateHeader",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_RebateHeader_UpdatedById",
                table: "RebateHeader",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RebateHeader_ZoneId",
                table: "RebateHeader",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_RebateOutlets_CreatedById",
                table: "RebateOutlets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RebateOutlets_RebateHeaderId",
                table: "RebateOutlets",
                column: "RebateHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_RebateOutlets_StoreId",
                table: "RebateOutlets",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_RebateOutlets_UpdatedById",
                table: "RebateOutlets",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_CreatedBy",
                table: "Recipe",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_IngredientProductID",
                table: "Recipe",
                column: "IngredientProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_ModifiedBy",
                table: "Recipe",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_OutletID",
                table: "Recipe",
                column: "OutletID");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_ProductID",
                table: "Recipe",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportScheduler_CreatedBy",
                table: "ReportScheduler",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportScheduler_ModifiedBy",
                table: "ReportScheduler",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportScheduler_ReportId",
                table: "ReportScheduler",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSchedulerLog_SchedulerId",
                table: "ReportSchedulerLog",
                column: "SchedulerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSchedulerLog_UserId",
                table: "ReportSchedulerLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatedById",
                table: "Role",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UpdatedById",
                table: "Role",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Delete_Unique",
                table: "Role",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RolesDefaultPermissions_CreatedById",
                table: "RolesDefaultPermissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RolesDefaultPermissions_UpdatedById",
                table: "RolesDefaultPermissions",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerUser_CreatedBy",
                table: "SchedulerUser",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerUser_ModifiedBy",
                table: "SchedulerUser",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerUser_SchedulerId",
                table: "SchedulerUser",
                column: "SchedulerId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerUser_UserId",
                table: "SchedulerUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SendEmail_CreatedById",
                table: "SendEmail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SendEmail_UpdatedById",
                table: "SendEmail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustDetail_CreatedById",
                table: "StockAdjustDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustDetail_OutletProductId",
                table: "StockAdjustDetail",
                column: "OutletProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustDetail_ProductId",
                table: "StockAdjustDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustDetail_ReasonId",
                table: "StockAdjustDetail",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustDetail_UpdatedById",
                table: "StockAdjustDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustDetail_StockAdjustHeaderId_ProductId_Created_Unique",
                table: "StockAdjustDetail",
                columns: new[] { "StockAdjustHeaderId", "ProductId", "LineNo", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustHeader_CreatedById",
                table: "StockAdjustHeader",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustHeader_OutletId",
                table: "StockAdjustHeader",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustHeader_UpdatedById",
                table: "StockAdjustHeader",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetail_CreatedById",
                table: "StockTakeDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetail_OutletProductId",
                table: "StockTakeDetail",
                column: "OutletProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetail_ProductId",
                table: "StockTakeDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetail_StockTakeHeaderId",
                table: "StockTakeDetail",
                column: "StockTakeHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeDetail_UpdatedById",
                table: "StockTakeDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeHeader_CreatedById",
                table: "StockTakeHeader",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeHeader_OutletId",
                table: "StockTakeHeader",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeHeader_UpdatedById",
                table: "StockTakeHeader",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Store_CostZoneId",
                table: "Store",
                column: "CostZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_CreatedById",
                table: "Store",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Store_GroupId",
                table: "Store",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_LabelTypePromoId",
                table: "Store",
                column: "LabelTypePromoId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_LabelTypeShelfId",
                table: "Store",
                column: "LabelTypeShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_LabelTypeShortId",
                table: "Store",
                column: "LabelTypeShortId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_OutletPriceFromOutletId",
                table: "Store",
                column: "OutletPriceFromOutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_PriceZoneId",
                table: "Store",
                column: "PriceZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_UpdatedById",
                table: "Store",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Store_WarehouseId",
                table: "Store",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCode_Delete_Unique",
                table: "Store",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_StoreGroup_CreatedById",
                table: "StoreGroup",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoreGroup_UpdatedById",
                table: "StoreGroup",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoreGroupCode_Delete_Unique",
                table: "StoreGroup",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_CreatedById",
                table: "Supplier",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_UpdatedById",
                table: "Supplier",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Delete_Unique",
                table: "Supplier",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProduct_CreatedById",
                table: "SupplierProduct",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProduct_SupplierId",
                table: "SupplierProduct",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProduct_UpdatedById",
                table: "SupplierProduct",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProduct_ProductId_SupplierId_SupplierItem_Unique",
                table: "SupplierProduct",
                columns: new[] { "ProductId", "SupplierId", "SupplierItem" },
                unique: true,
                filter: "[SupplierItem] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SystemControls_CreatedBy",
                table: "SystemControls",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SystemControls_ModifiedBy",
                table: "SystemControls",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_CreatedById",
                table: "Tax",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_UpdatedById",
                table: "Tax",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_Delete_Unique",
                table: "Tax",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Till_CreatedById",
                table: "Till",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Till_KeypadId",
                table: "Till",
                column: "KeypadId");

            migrationBuilder.CreateIndex(
                name: "IX_Till_OutletId",
                table: "Till",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Till_TypeId",
                table: "Till",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Till_UpdatedById",
                table: "Till",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Till_Delete_Unique",
                table: "Till",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TillSync_CreatedById",
                table: "TillSync",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TillSync_StoreId",
                table: "TillSync",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_TillSync_TillId",
                table: "TillSync",
                column: "TillId");

            migrationBuilder.CreateIndex(
                name: "IX_TillSync_UpdatedById",
                table: "TillSync",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CategoryId",
                table: "Transaction",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CommodityId",
                table: "Transaction",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CreatedById",
                table: "Transaction",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DepartmentId",
                table: "Transaction",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Group",
                table: "Transaction",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ManufacturerId",
                table: "Transaction",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_OutletId",
                table: "Transaction",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ProductId",
                table: "Transaction",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PromoBuyId",
                table: "Transaction",
                column: "PromoBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PromoSellId",
                table: "Transaction",
                column: "PromoSellId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_SupplierId",
                table: "Transaction",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TillId",
                table: "Transaction",
                column: "TillId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_UpdatedById",
                table: "Transaction",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_UserId",
                table: "Transaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_CategoryId",
                table: "TransactionReport",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_CommodityId",
                table: "TransactionReport",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_CreatedById",
                table: "TransactionReport",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_DepartmentId",
                table: "TransactionReport",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_ManufacturerId",
                table: "TransactionReport",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_OutletId",
                table: "TransactionReport",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_ProductId",
                table: "TransactionReport",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_PromoBuyId",
                table: "TransactionReport",
                column: "PromoBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_PromoSellId",
                table: "TransactionReport",
                column: "PromoSellId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_SupplierId",
                table: "TransactionReport",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_TillId",
                table: "TransactionReport",
                column: "TillId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_UpdatedById",
                table: "TransactionReport",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReport_UserId",
                table: "TransactionReport",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedById",
                table: "User",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_UpdatedById",
                table: "User",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserName_Delete_Unique",
                table: "User",
                columns: new[] { "UserName", "IsDeleted" },
                unique: true,
                filter: "UserName IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserLog_ActionBy",
                table: "UserLog",
                column: "ActionBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_CreatedById",
                table: "UserRole",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_StoreId",
                table: "UserRole",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UpdatedById",
                table: "UserRole",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_CreatedById",
                table: "Warehouse",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_HostFormatId",
                table: "Warehouse",
                column: "HostFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_SupplierId",
                table: "Warehouse",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_UpdatedById",
                table: "Warehouse",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseCode_Delete_Unique",
                table: "Warehouse",
                columns: new[] { "Code", "IsDeleted" },
                unique: true,
                filter: "Code IS NOT NULL AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_XeroAccount_CreatedById",
                table: "XeroAccount",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_XeroAccount_StoreId",
                table: "XeroAccount",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_XeroAccount_UserCreatedById",
                table: "XeroAccount",
                column: "UserCreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneOutlet_CreatedById",
                table: "ZoneOutlet",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneOutlet_StoreId",
                table: "ZoneOutlet",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneOutlet_UpdatedById",
                table: "ZoneOutlet",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneOutlet_ZoneId",
                table: "ZoneOutlet",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ZonePricing_CreatedById",
                table: "ZonePricing",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ZonePricing_PriceZoneId",
                table: "ZonePricing",
                column: "PriceZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ZonePricing_ProductId",
                table: "ZonePricing",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ZonePricing_UpdatedById",
                table: "ZonePricing",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Journal_Detail_Product",
                table: "JournalDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_APN_Journal_Detail_APN",
                table: "JournalDetail",
                column: "APNSold",
                principalTable: "APN",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cashier_Journal_Detail_Cashier",
                table: "JournalDetail",
                column: "CashierId",
                principalTable: "Cashier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Journal_Header_Journal_Detail_Journal_Header",
                table: "JournalDetail",
                column: "JournalHeaderId",
                principalTable: "JournalHeader",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Outlet_Journal_Header_Outlet",
                table: "JournalHeader",
                column: "OutletId",
                principalTable: "Store",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Till_Journal_Header_Till",
                table: "JournalHeader",
                column: "TillId",
                principalTable: "Till",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cashier_Journal_Header_Cashier",
                table: "JournalHeader",
                column: "CashierId",
                principalTable: "Cashier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_AccountTransaction_Outlet_Id",
                table: "AccountTransaction",
                column: "OutletId",
                principalTable: "Store",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AccountTransaction_Product_Id",
                table: "AccountTransaction",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Till_AccountTransaction_Till_Id",
                table: "AccountTransaction",
                column: "TillId",
                principalTable: "Till",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Store_StoreId",
                table: "Product",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Transaction_Store_Id",
                table: "Transaction",
                column: "OutletId",
                principalTable: "Store",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Till_Transaction_Till_Id",
                table: "Transaction",
                column: "TillId",
                principalTable: "Till",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_TransactionReport_Outlet_Id",
                table: "TransactionReport",
                column: "OutletId",
                principalTable: "Store",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Till_TransactionReport_Till_Id",
                table: "TransactionReport",
                column: "TillId",
                principalTable: "Till",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_OutletProduct",
                table: "OutletProduct",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CostZones_Cost_Store_Id",
                table: "Store",
                column: "CostZoneId",
                principalTable: "CostPriceZones",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Price_PriceZones_Id",
                table: "Store",
                column: "PriceZoneId",
                principalTable: "CostPriceZones",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostPriceZones_Created_By",
                table: "CostPriceZones");

            migrationBuilder.DropForeignKey(
                name: "FK_CostPriceZones_Updated_By",
                table: "CostPriceZones");

            migrationBuilder.DropForeignKey(
                name: "FK_HostSettings_Created_By",
                table: "HostSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_HostSettings_Updated_By",
                table: "HostSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_User_MasterList_Created_By",
                table: "MasterList");

            migrationBuilder.DropForeignKey(
                name: "FK_User_MasterList_Updated_By",
                table: "MasterList");

            migrationBuilder.DropForeignKey(
                name: "FK_User_MasterList_Items_Created_By",
                table: "MasterListItems");

            migrationBuilder.DropForeignKey(
                name: "FK_User_MasterList_Items_Updated_By",
                table: "MasterListItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Paths_Created_By",
                table: "Paths");

            migrationBuilder.DropForeignKey(
                name: "FK_Paths_Updated_By",
                table: "Paths");

            migrationBuilder.DropForeignKey(
                name: "FK_User_PrintLabelType_Created_By",
                table: "PrintLabelType");

            migrationBuilder.DropForeignKey(
                name: "FK_User_PrintLabelType_Updated_By",
                table: "PrintLabelType");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Store_Created_By",
                table: "Store");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Store_Updated_By",
                table: "Store");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Store_Group_Created_By",
                table: "StoreGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Store_Group_Updated_By",
                table: "StoreGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Supplier_Created_By",
                table: "Supplier");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Supplier_Updated_By",
                table: "Supplier");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Warehouse_Created_By",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Warehouse_Updated_By",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_HostSettings_MasterListItem",
                table: "HostSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_MasterList_Items",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_Paths_Outlet",
                table: "Paths");

            migrationBuilder.DropTable(
                name: "AccessDepartment");

            migrationBuilder.DropTable(
                name: "AccountLoyalty");

            migrationBuilder.DropTable(
                name: "AccountTransaction");

            migrationBuilder.DropTable(
                name: "AutoOrderSettings");

            migrationBuilder.DropTable(
                name: "BulkOrderFromTablet");

            migrationBuilder.DropTable(
                name: "BulkPrintLabelFromTabletTbl");

            migrationBuilder.DropTable(
                name: "CompetitionReward");

            migrationBuilder.DropTable(
                name: "CompetitionTrigger");

            migrationBuilder.DropTable(
                name: "CSCPERIOD");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "EPAY");

            migrationBuilder.DropTable(
                name: "GLAccount");

            migrationBuilder.DropTable(
                name: "HostUpdChange");

            migrationBuilder.DropTable(
                name: "JournalDetail");

            migrationBuilder.DropTable(
                name: "KeypadButton");

            migrationBuilder.DropTable(
                name: "ManualSaleItem");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ModuleActions");

            migrationBuilder.DropTable(
                name: "OrderAudit");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "OutletBudget");

            migrationBuilder.DropTable(
                name: "OutletRoyaltyScales");

            migrationBuilder.DropTable(
                name: "OutletSupplierSchedule");

            migrationBuilder.DropTable(
                name: "OutletSupplierSetting");

            migrationBuilder.DropTable(
                name: "OutletTradingHours");

            migrationBuilder.DropTable(
                name: "PrintLabelFromTabletTbl");

            migrationBuilder.DropTable(
                name: "PromoBuying");

            migrationBuilder.DropTable(
                name: "PromoMemberOffer");

            migrationBuilder.DropTable(
                name: "PromoMixmatchProduct");

            migrationBuilder.DropTable(
                name: "PromoOfferProduct");

            migrationBuilder.DropTable(
                name: "PromoSelling");

            migrationBuilder.DropTable(
                name: "RebateDetails");

            migrationBuilder.DropTable(
                name: "RebateOutlets");

            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.DropTable(
                name: "ReportSchedulerLog");

            migrationBuilder.DropTable(
                name: "RolesDefaultPermissions");

            migrationBuilder.DropTable(
                name: "SchedulerUser");

            migrationBuilder.DropTable(
                name: "SendEmail");

            migrationBuilder.DropTable(
                name: "StockAdjustDetail");

            migrationBuilder.DropTable(
                name: "StockTakeDetail");

            migrationBuilder.DropTable(
                name: "SystemControls");

            migrationBuilder.DropTable(
                name: "TillSync");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "TransactionReport");

            migrationBuilder.DropTable(
                name: "UserLog");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "XeroAccount");

            migrationBuilder.DropTable(
                name: "ZoneOutlet");

            migrationBuilder.DropTable(
                name: "ZonePricing");

            migrationBuilder.DropTable(
                name: "PromotionCompetition");

            migrationBuilder.DropTable(
                name: "HOSTUPD");

            migrationBuilder.DropTable(
                name: "APN");

            migrationBuilder.DropTable(
                name: "JournalHeader");

            migrationBuilder.DropTable(
                name: "KeypadLevel");

            migrationBuilder.DropTable(
                name: "ManualSale");

            migrationBuilder.DropTable(
                name: "OrderHeader");

            migrationBuilder.DropTable(
                name: "SupplierProduct");

            migrationBuilder.DropTable(
                name: "PromoMixmatch");

            migrationBuilder.DropTable(
                name: "PromoOffer");

            migrationBuilder.DropTable(
                name: "RebateHeader");

            migrationBuilder.DropTable(
                name: "ReportScheduler");

            migrationBuilder.DropTable(
                name: "StockAdjustHeader");

            migrationBuilder.DropTable(
                name: "OutletProduct");

            migrationBuilder.DropTable(
                name: "StockTakeHeader");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Cashier");

            migrationBuilder.DropTable(
                name: "Till");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "CompetitionDetail");

            migrationBuilder.DropTable(
                name: "Keypad");

            migrationBuilder.DropTable(
                name: "Commodity");

            migrationBuilder.DropTable(
                name: "Tax");

            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "MasterListItems");

            migrationBuilder.DropTable(
                name: "MasterList");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "CostPriceZones");

            migrationBuilder.DropTable(
                name: "StoreGroup");

            migrationBuilder.DropTable(
                name: "PrintLabelType");

            migrationBuilder.DropTable(
                name: "HostSettings");

            migrationBuilder.DropTable(
                name: "Paths");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
