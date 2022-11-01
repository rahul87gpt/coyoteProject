using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("User")]
    public class Users : IKeyIdentifier<int>, IAuditable<int?>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(MaxLengthConstants.UserNameLength, ErrorMessage = ErrorMessages.UserFirstName)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(MaxLengthConstants.UserNameLength, ErrorMessage = ErrorMessages.UserLastName)]

        public string MiddleName { get; set; }
        [MaxLength(MaxLengthConstants.UserNameLength, ErrorMessage = ErrorMessages.UserMiddleName)]
        [Required]
        public string LastName { get; set; }

        public string Password { get; set; }

        [MaxLength(MaxLengthConstants.UserPhoneLength, ErrorMessage = ErrorMessages.UserMobileNo)]
        public string MobileNo { get; set; }
        [MaxLength(MaxLengthConstants.UserPhoneLength, ErrorMessage = ErrorMessages.UserPhoneNo)]
        public string PhoneNo { get; set; }

        [MaxLength(MaxLengthConstants.MaxUserLength, ErrorMessage = ErrorMessages.UserEmail)]
        [Required]
        public string Email { get; set; }

        [Required]
        [MaxLength(MaxLengthConstants.MaxUserNameLength, ErrorMessage = ErrorMessages.UsernameNameLength)]
        public string UserName { get; set; }

        [MaxLength(MaxLengthConstants.UserAddressLength, ErrorMessage = ErrorMessages.UserAddress)]
        public string Address1 { get; set; }
        [MaxLength(MaxLengthConstants.UserAddressLength, ErrorMessage = ErrorMessages.UserAddress)]
        public string Address2 { get; set; }
        [MaxLength(MaxLengthConstants.UserAddressLength, ErrorMessage = ErrorMessages.UserAddress)]
        public string Address3 { get; set; }
        [MaxLength(MaxLengthConstants.MinUserLength, ErrorMessage = ErrorMessages.UserPostal)]
        public string PostCode { get; set; }
        [MaxLength(MaxLengthConstants.UserGenderLength, ErrorMessage = ErrorMessages.UserGender)]
        public string Gender { get; set; }
        [MaxLength(MaxLengthConstants.UserStatusLength, ErrorMessage = ErrorMessages.UserStatus)]
        [Required]
        public bool Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateOfBirth { get; set; }
        public string ZoneIds { get; set; }
        public string StoreIds { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastLogin { get; set; }
        public string PromoPrefix { get; set; }
        public string KeypadPrefix { get; set; }
        public int? Type { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public int? CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsResetPassword { get; set; }
        public string TemporaryPassword { get; set; }

        public string RefreshToken { get; set; }

        public string ImagePath { get; set; }

        public DateTime? TempPasswordCreatedAt { get; set; }

        public string PlainPassword { get; set; }
        public bool AddUnlockProduct { get; set; } = false;

        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
        public virtual ICollection<Roles> RolesCreated { get; }
        public virtual ICollection<Roles> RolesUpdated { get; }

        public virtual ICollection<StoreGroup> StoreGroupCreated { get; }

        public virtual ICollection<StoreGroup> StoreGroupUpdated { get; }

        public virtual ICollection<MasterListItems> MasterListItemsCreated { get; }

        public virtual ICollection<MasterListItems> MasterListItemsUpdated { get; }

        public virtual ICollection<MasterList> MasterListCreated { get; }

        public virtual ICollection<MasterList> MasterListUpdated { get; }

        public virtual ICollection<Store> StoresCreated { get; }
        public virtual ICollection<Store> StoresUpdated { get; }
        public virtual ICollection<UserRoles> UserRolesCreated { get; }

        public virtual ICollection<UserRoles> UserRolesUpdated { get; }

        public virtual ICollection<UserRoles> RolesList { get; }

        public virtual ICollection<Users> Updated { get; }

        public virtual ICollection<Users> Created { get; }

        public virtual ICollection<EmailTemplate> EmailTemplateUpdated { get; }

        public virtual ICollection<EmailTemplate> EmailTemplateCreated { get; }

        public virtual ICollection<SendEmail> SendEmailUpdated { get; }

        public virtual ICollection<SendEmail> SendEmailCreated { get; }

        public virtual ICollection<Department> DepartmentCreated { get; }

        public virtual ICollection<Department> DepartmentUpdated { get; }

        public virtual ICollection<Commodity> CommodityCreated { get; }

        public virtual ICollection<Commodity> CommodityUpdated { get; }

        public virtual ICollection<Supplier> SupplierCreated { get; }

        public virtual ICollection<Supplier> SupplierUpdated { get; }

        public virtual ICollection<ZoneOutlet> ZoneOutLetCreated { get; }

        public virtual ICollection<ZoneOutlet> ZoneOutLetUpdated { get; }
        public virtual ICollection<Tax> TaxCreated { get; }
        public virtual ICollection<Tax> TaxUpdated { get; }

        public virtual ICollection<Warehouse> WarehouseCreated { get; }
        public virtual ICollection<Warehouse> WarehouseUpdated { get; }
        public virtual ICollection<Product> ProductCreated { get; }
        public virtual ICollection<Product> ProductUpdated { get; }
        public virtual ICollection<APN> APNCreated { get; }
        public virtual ICollection<APN> APNUpdated { get; }
        public virtual ICollection<SupplierProduct> SupplierProductCreated { get; }
        public virtual ICollection<SupplierProduct> SupplierProductUpdated { get; }
        public virtual ICollection<OutletProduct> OutletProductCreated { get; }
        public virtual ICollection<OutletProduct> OutletProductUpdated { get; }

        public virtual ICollection<Promotion> PromotionCreated { get; }
        public virtual ICollection<Promotion> PromotionUpdated { get; }
        public virtual ICollection<PromotionBuying> PromotionBuyingCreated { get; }
        public virtual ICollection<PromotionBuying> PromotionBuyingUpdated { get; }
        public virtual ICollection<PromotionMemberOffer> PromotionMemberOfferCreated { get; }
        public virtual ICollection<PromotionMemberOffer> PromotionMemberOfferUpdated { get; }
        public virtual ICollection<PromotionMixmatch> PromotionMixmatchCreated { get; }
        public virtual ICollection<PromotionMixmatch> PromotionMixmatchUpdated { get; }

        public virtual ICollection<PromotionMixmatchProduct> PromotionMixmatchProductCreated { get; }
        public virtual ICollection<PromotionMixmatchProduct> PromotionMixmatchProductUpdated { get; }

        public virtual ICollection<PromotionOffer> PromotionOfferCreated { get; }
        public virtual ICollection<PromotionOffer> PromotionOfferUpdated { get; }

        public virtual ICollection<PromotionOfferProduct> PromotionOfferProductCreated { get; }
        public virtual ICollection<PromotionOfferProduct> PromotionOfferProductUpdated { get; }

        public virtual ICollection<PromotionSelling> PromotionSellingCreated { get; }
        public virtual ICollection<PromotionSelling> PromotionSellingUpdated { get; }

        public virtual ICollection<ModuleActions> ControllerActionsCreated { get; }
        public virtual ICollection<ModuleActions> ControllerActionsUpdated { get; }

        public virtual ICollection<Cashier> CashierCreated { get; }
        public virtual ICollection<Cashier> CashierUpdated { get; }

        public virtual ICollection<Keypad> KeypadCreated { get; }
        public virtual ICollection<Keypad> KeypadUpdated { get; }

        public virtual ICollection<Till> TillCreated { get; }
        public virtual ICollection<Till> TillUpdated { get; }
        public virtual ICollection<OrderHeader> OrderHeaderCreated { get; }
        public virtual ICollection<OrderHeader> OrderHeaderUpdated { get; }
        public virtual ICollection<OrderDetail> OrderDetailCreated { get; }
        public virtual ICollection<OrderDetail> OrderDetailUpdated { get; }
        public virtual ICollection<OrderAudit> OrderAuditCreated { get; }
        public virtual ICollection<OrderAudit> OrderAuditUpdated { get; }
        public virtual ICollection<BulkOrderFromTablet> BulkOrderFromTabletCreated { get; }
        public virtual ICollection<BulkOrderFromTablet> BulkOrderFromTabletUpdated { get; }
        public virtual ICollection<ManualSale> ManualSaleCreated { get; }
        public virtual ICollection<ManualSale> ManualSaleUpdated { get; }
        public virtual ICollection<ManualSaleItem> ManualSaleItemCreated { get; }
        public virtual ICollection<ManualSaleItem> ManualSaleItemUpdated { get; }
        public virtual ICollection<PrintLabelType> PrintLabelTypeCreated { get; }
        public virtual ICollection<PrintLabelType> PrintLabelTypeUpdated { get; }
        public virtual ICollection<KeypadLevel> KeypadLevelCreated { get; }
        public virtual ICollection<KeypadLevel> KeypadLevelUpdated { get; }
        public virtual ICollection<KeypadButton> KeypadButtonCreated { get; }
        public virtual ICollection<KeypadButton> KeypadButtonUpdated { get; }

        public virtual ICollection<StockAdjustHeader> StockAdjustHeaderCreated { get; }
        public virtual ICollection<StockAdjustHeader> StockAdjustHeaderUpdated { get; }
        public virtual ICollection<StockAdjustDetail> StockAdjustDetailCreated { get; }
        public virtual ICollection<StockAdjustDetail> StockAdjustDetailUpdated { get; }

        public virtual ICollection<StockTakeHeader> StockTakeHeaderCreated { get; }
        public virtual ICollection<StockTakeHeader> StockTakeHeaderUpdated { get; }
        public virtual ICollection<StockTakeDetail> StockTakeDetailCreated { get; }
        public virtual ICollection<StockTakeDetail> StockTakeDetailUpdated { get; }
        public virtual ICollection<CompetitionDetail> CompetitionDetailsCreated { get; }
        public virtual ICollection<CompetitionDetail> CompetitionDetailsUpdated { get; }
        public virtual ICollection<CompetitionTrigger> CompetitionTriggerCreated { get; }
        public virtual ICollection<CompetitionTrigger> CompetitionTriggerUpdated { get; }
        public virtual ICollection<CompetitionReward> CompetitionRewardCreated { get; }
        public virtual ICollection<CompetitionReward> CompetitionRewardUpdated { get; }
        public virtual ICollection<PromotionCompetition> PromotionCompetitionCreated { get; }
        public virtual ICollection<PromotionCompetition> PromotionCompetitionUpdated { get; }
        public virtual ICollection<OutletSupplierSetting> OutletSupplierSettingCreated { get; }
        public virtual ICollection<OutletSupplierSetting> OutletSupplierSettingUpdated { get; }
        public virtual ICollection<JournalHeader> JournalHeaderCreated { get; }
        public virtual ICollection<JournalHeader> JournalHeaderUpdated { get; }
        public virtual ICollection<GLAccount> GLAccountCreated { get; }
        public virtual ICollection<GLAccount> GLAccountUpdated { get; }
        public virtual ICollection<XeroAccount> XeroAccountCreated { get; }
        public virtual ICollection<XeroAccount> XeroAccountUpdated { get; }

        public virtual ICollection<JournalDetail> JournalDetailsCreated { get; }
        public virtual ICollection<JournalDetail> JournalDetailsUpdated { get; }
        public virtual ICollection<Transaction> TransactionUsers { get; }
        public virtual ICollection<Transaction> TransactionCreatedBy { get; }
        public virtual ICollection<Transaction> TransactionUpdatedBy { get; }
        public virtual ICollection<TransactionReport> TransactionReportUsers { get; }
        public virtual ICollection<TransactionReport> TransactionReportCreatedBy { get; }
        public virtual ICollection<TransactionReport> TransactionReportUpdatedBy { get; }
        public virtual ICollection<SupplierOrderingSchedule> SupplierOrderingCreatedBy { get; }
        public virtual ICollection<SupplierOrderingSchedule> SupplierOrderingUpdatedBy { get; }

        public virtual ICollection<POSMessages> POSMessageCreatedBy { get; }
        public virtual ICollection<POSMessages> POSMessageUpdatedBy { get; }
        public virtual ICollection<RolesDefaultPermissions> RolesDefaultPermissionsCreatedBy { get; }
        public virtual ICollection<RolesDefaultPermissions> RolesDefaultPermissionsUpdatedBy { get; }
        public virtual ICollection<OutletTradingHours> OutletTradingHoursCreatedBy { get; }
        public virtual ICollection<OutletTradingHours> OutletTradingHoursUpdatedBy { get; }

        public virtual ICollection<ZonePricing> ZonePricingCreatedBy { get; }
        public virtual ICollection<UserLog> LogCreatedBy { get; }
        public virtual ICollection<ZonePricing> ZonePricingUpdatedBy { get; }
        public virtual ICollection<SystemControls> SystemControlCreatedBy { get; }
        public virtual ICollection<SystemControls> SystemControlUpdatedBy { get; }
        public virtual ICollection<OutletRoyaltyScales> OutletRoyaltyScalesCreatedBy { get; }
        public virtual ICollection<OutletRoyaltyScales> OutletRoyaltyScalesUpdatedBy { get; }
        public virtual ICollection<TillSync> TillSyncUpdatedBy { get; }
        public virtual ICollection<TillSync> TillSyncCreatedBy { get; }

        public virtual ICollection<OutletBudget> OutletBudgetUpdatedBy { get; }
        public virtual ICollection<OutletBudget> OutletBudgetCreatedBy { get; }
        public virtual ICollection<Recipe> RecipeCreated { get; }
        public virtual ICollection<Recipe> RecipeUpdated { get; }
        public virtual ICollection<Paths> PathsCreated { get; }
        public virtual ICollection<Paths> PathsUpdated { get; }
        public virtual ICollection<RebateHeader> RebateHeaderCreatedBy { get; }
        public virtual ICollection<RebateHeader> RebateHeaderUpdatedBy { get; }
        public virtual ICollection<RebateOutlets> RebateOutletCreatedBy { get; }
        public virtual ICollection<RebateOutlets> RebateOutletUpdatedBy { get; }
        public virtual ICollection<RebateDetails> RebateDetailsCreatedBy { get; }
        public virtual ICollection<RebateDetails> RebateDetailsUpdatedBy { get; }
        public virtual ICollection<EPay> EPAYCreated { get; }
        public virtual ICollection<EPay> EPAYUpdated { get; }
        public virtual ICollection<CSCPeriod> CSCPeriodCreated { get; }
        public virtual ICollection<CSCPeriod> CSCPeriodUpdated { get; }
        public virtual ICollection<HostSettings> HostSettingsCreated { get; }
        public virtual ICollection<HostSettings> HostSettingsUpdated { get; }
        public virtual ICollection<CostPriceZones> CostPriceZonesCreated { get; }
        public virtual ICollection<CostPriceZones> CostPriceZonesUpdated { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionUsers { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionCreatedBy { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionUpdatedBy { get; }
        public virtual ICollection<AccountLoyalty> AccountLoyaltyCreatedBy { get; }
        public virtual ICollection<AccountLoyalty> AccountLoyaltyUpdatedBy { get; }
        public virtual ICollection<HostProcessing> HostProcessingCreated { get; }
        public virtual ICollection<HostProcessing> HostProcessingUpdated { get; }
        public virtual ICollection<ReportScheduler> ReportSchedulerCreatedBy { get; }
        public virtual ICollection<ReportScheduler> ReportSchedulerUpdatedBy { get; }
        public virtual ICollection<SchedulerUser> SchedulerUserCreatedBy { get; }
        public virtual ICollection<SchedulerUser> SchedulerUserUpdatedBy { get; }
        public virtual ICollection<SchedulerUser> SchedulerUser { get; }
        public virtual ICollection<ReportSchedulerLog> SchedulerUserLog { get; }
        public virtual ICollection<HostUpdChange> HostUpdChangeCreatedBy { get; }
        public virtual ICollection<HostUpdChange> HostUpdChangeUpdatedBy { get; }
        public virtual ICollection<AutoOrderSettings> AutoOrderSettingsCreatedBy { get; }
        public virtual ICollection<AutoOrderSettings> AutoOrderSettingsUpdatedBy { get; }

        public virtual ICollection<AccessDepartment> AccessDepartmentUpdatedBy { get; }
        public virtual ICollection<AccessDepartment> AccessDepartmentCreatedBy { get; }
    }
}
