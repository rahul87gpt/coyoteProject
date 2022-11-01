using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Coyote.Console.App.EntityFrameworkCore
{
    public class CoyoteAppDBContext : DbContext
    {
        public CoyoteAppDBContext(DbContextOptions<CoyoteAppDBContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(5 * 60);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder?.ApplyConfigurationsFromAssembly(typeof(CoyoteAppDBContext).Assembly);

            #region Transaction Index

            modelBuilder.Entity<Transaction>()
                .HasIndex(e => new { e.Type, e.PromoSales }).IsUnique(false).HasName("IX_Trans_date_PromoSales");

            modelBuilder.Entity<Transaction>()
           .HasIndex(e => new { e.Date }).IsUnique(false).HasName("IX_Transaction_Date");

            modelBuilder.Entity<Transaction>()
          .HasIndex(e => new { e.Type, e.Date }).IsUnique(false).HasName("IX_Trans_Tp_Date");

            modelBuilder.Entity<Transaction>()
          .HasIndex(e => new { e.Type, e.IsDeleted, e.Date }).IsUnique(false).HasName("IX_Trans_Tp_Del_Dt");

            modelBuilder.Entity<Transaction>()
          .HasIndex(e => new { e.Type, e.OutletId, e.Date }).IsUnique(false).HasName("IX_Transaction_outlet_Date");

            modelBuilder.Entity<Transaction>()
          .HasIndex(e => new { e.DepartmentId, e.Date }).IsUnique(false).HasName("IX_transaction_OutletRanking");

            modelBuilder.Entity<Transaction>()
         .HasIndex(e => new { e.ProductId, e.Date, e.OutletId }).IsUnique(false).HasName("IX_Transaction_product_Date_outlet");

            modelBuilder.Entity<Transaction>()
         .HasIndex(e => new { e.Type }).IsUnique(false).HasName("IX_Transaction_Type");


            modelBuilder.Entity<Transaction>()
         .HasIndex(e => new { e.Type, e.OutletId, e.Date }).IsUnique(false).HasName("ix_Transaction_type_outlet_Date");


            modelBuilder.Entity<Transaction>()
         .HasIndex(e => new { e.Date, e.ProductId, e.Type, e.OutletId, e.TillId, e.Sequence }).IsUnique(false).HasName("IX_TransactionDateProductTypeTillSequence");

            #endregion

            #region JournalHeader
            modelBuilder.Entity<JournalHeader>()
                .HasIndex(e => new { e.OutletId, e.IsDeleted, e.HeaderDate }).IsUnique(false).HasName("<IX_JournalHeader_Olet_IsDel_HDate>");

            modelBuilder.Entity<JournalHeader>()
             .HasIndex(e => new { e.Date }).IsUnique(false).HasName("IX_Jhead_Date");

            modelBuilder.Entity<JournalHeader>()
             .HasIndex(e => new { e.OutletId, e.Type, e.TradingDate }).IsUnique(false).HasName("IX_JHead_Olet_Tp_TradDate");

            modelBuilder.Entity<JournalHeader>()
             .HasIndex(e => new { e.Status, e.IsDeleted, e.OutletId, e.TradingDate }).IsUnique(false).HasName("IX_JHead_Status_Del_OletId_TradeDate");

            modelBuilder.Entity<JournalHeader>()
             .HasIndex(e => new { e.Status, e.TradingDate }).IsUnique(false).HasName("IX_Jhead_Status_TradeDate");

            modelBuilder.Entity<JournalHeader>()
            .HasIndex(e => new { e.Type, e.Status, e.TradingDate }).IsUnique(false).HasName("IX_JHead_Tp_Sta_TradDate");

            modelBuilder.Entity<JournalHeader>()
            .HasIndex(e => new { e.Status, e.TradingDate }).IsUnique(false).HasName("IX_Jhead_Status_TradeDate");

            modelBuilder.Entity<JournalHeader>()
            .HasIndex(e => new { e.Type, e.TradingDate }).IsUnique(false).HasName("IX_JHead_Tp_TrsDate");

            modelBuilder.Entity<JournalHeader>()
            .HasIndex(e => new { e.Date }).IsUnique(false).HasName("IX_JournalHeader");

            modelBuilder.Entity<JournalHeader>()
            .HasIndex(e => new { e.Id, e.OutletId }).IsUnique(false).HasName("IX_JournalHeader_Date_Outlet_id");

            modelBuilder.Entity<JournalHeader>()
           .HasIndex(e => new { e.OutletId, e.Date }).IsUnique(false).HasName("IX_JournalHeader_Outlet_Date_id");

            modelBuilder.Entity<JournalHeader>()
           .HasIndex(e => new { e.Date, e.OutletId }).IsUnique(false).HasName("JHeade_Trading_Date");

            #endregion

        }
        /// <summary>
        /// Declare schema name
        /// </summary>
        public string SchemaName { get; set; }
        public DbSet<APN> APN { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<AccessDepartment> AccessDepartment { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreGroup> StoreGroups { get; set; }
        public DbSet<MasterList> MasterLists { get; set; }
        public DbSet<MasterListItems> MasterListItems { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<SendEmail> SendEmailLogs { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ZoneOutlet> ZoneOutlets { get; set; }
        public DbSet<SupplierProduct> SupplierProducts { get; set; }
        public DbSet<OutletProduct> OutletProducts { get; set; }
        public DbSet<StockTakeHeader> StockTakeHeader { get; set; }
        public DbSet<StockTakeDetail> StockTakeDetail { get; set; }
        public DbSet<StockAdjustHeader> StockAdjustHeader { get; set; }
        public DbSet<StockAdjustDetail> StockAdjustDetail { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionBuying> PromotionBuyings { get; set; }
        public DbSet<PromotionMemberOffer> PromotionMemberOffers { get; set; }
        public DbSet<PromotionMixmatch> PromotionMixmatches { get; set; }
        public DbSet<PromotionMixmatchProduct> PromotionMixmatchProducts { get; set; }
        public DbSet<PromotionOffer> PromotionOffers { get; set; }
        public DbSet<PromotionOfferProduct> PromotionOfferProducts { get; set; }
        public DbSet<PromotionSelling> PromotionSellings { get; set; }
        public DbSet<ModuleActions> ModuleActions { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Keypad> Keypads { get; set; }
        public DbSet<Till> Tills { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PrintLabelType> PrintLabelTypes { get; set; }
        public DbSet<KeypadLevel> KeypadLevels { get; set; }
        public DbSet<KeypadButton> KeypadButtons { get; set; }
        public DbSet<OutletSupplierSetting> OutletSuppliersSettings { get; set; }
        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<OrderAudit> OrderAudit { get; set; }
        public DbSet<BulkOrderFromTablet> BulkOrderFromTablet { get; set; }
        public DbSet<ManualSale> ManualSale { get; set; }
        public DbSet<ManualSaleItem> ManualSaleItem { get; set; }
        public DbSet<OutletRoyaltyScales> OutletRoyaltyScales { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Paths> Paths { get; set; }
        public DbSet<EPay> EPAY { get; set; }
        public DbSet<CSCPeriod> CSCPERIOD { get; set; }
        public DbSet<HostSettings> HostSettings { get; set; }
        public DbSet<CostPriceZones> CostPriceZones { get; set; }
        public DbSet<HostProcessing> HOSTUPD { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Members> Members { get; set; }
        public DbSet<HostUpdChange> HOSTUPDChange { get; set; }

    }
}
