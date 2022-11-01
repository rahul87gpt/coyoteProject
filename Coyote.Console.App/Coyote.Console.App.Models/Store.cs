using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Store")]
    public class Store : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress1)]
        [Required]
        public string Code { get; set; }
        public int? GroupId { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress1)]
        public string Address1 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress2)]
        public string Address2 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAddress3)]
        public string Address3 { get; set; }
        [MaxLength(MaxLengthConstants.StoreDescLength, ErrorMessage = ErrorMessages.StorePhoneNumber)]
        public string PhoneNumber { get; set; }
        [MaxLength(MaxLengthConstants.StoreDescLength, ErrorMessage = ErrorMessages.StoreFax)]
        public string Fax { get; set; }
        [MaxLength(MaxLengthConstants.StorePostCodeLength, ErrorMessage = ErrorMessages.StorePostCode)]
        public string PostCode { get; set; }
        [MaxLength(MaxLengthConstants.StoreStatusLength, ErrorMessage = ErrorMessages.StoreStatus)]
        public bool Status { get; set; }
        [MaxLength(MaxLengthConstants.StoreDescLength, ErrorMessage = ErrorMessages.StoreDesc)]
        [Required]
        public string Desc { get; set; }
        [MaxLength(MaxLengthConstants.MaxStoreLength, ErrorMessage = ErrorMessages.StoreEmail)]
        public string Email { get; set; }
        public int? PriceZoneId { get; set; }
        public virtual CostPriceZones PriceZone { get; }
        public int? CostZoneId { get; set; }
        public virtual CostPriceZones CostZone { get; }
        public bool SellingInd { get; set; }
        public bool StockInd { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelName)]
        public string DelName { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelAddr1)]
        public string DelAddr1 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelAddr2)]
        public string DelAddr2 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreDelAddr3)]
        public string DelAddr3 { get; set; }
        [MaxLength(MaxLengthConstants.StorePostCodeLength, ErrorMessage = ErrorMessages.StoreDelPostCode)]
        public string DelPostCode { get; set; }
        [MaxLength(MaxLengthConstants.StoreStatusLength, ErrorMessage = ErrorMessages.StoreCostType)]
        public string CostType { get; set; }
        public bool? CostInd { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreAbn)]
        public string Abn { get; set; }

        public double? BudgetGrowthFact { get; set; }

        [MaxLength(MaxLengthConstants.MinStoreLength, ErrorMessage = ErrorMessages.StoreEntityNumber)]
        public string EntityNumber { get; set; }
        
        public int? WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; }
        public int? OutletPriceFromOutletId { get; set; }
        public virtual Store OutletPriceFromOutlet { get; set; }
        public int? PriceFromLevel { get; set; }

        [MaxLength(MaxLengthConstants.StoreCodeLength)]
        public string PriceLevelDesc1 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength)]
        public string PriceLevelDesc2 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength)]
        public string PriceLevelDesc3 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength)]
        public string PriceLevelDesc4 { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength)]
        public string PriceLevelDesc5 { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        [MaxLength(MaxLengthConstants.StoreOpenHoursLength, ErrorMessage = ErrorMessages.StoreOpenHours)]
        public string OpenHours { get; set; }
        public bool? FuelSite { get; set; }
        [MaxLength(MaxLengthConstants.StoreLatitudeLength, ErrorMessage = ErrorMessages.StoreNameOnApp)]
        public string NameOnApp { get; set; }
        [MaxLength(MaxLengthConstants.StoreLatitudeLength, ErrorMessage = ErrorMessages.StoreAddressOnApp)]
        public string AddressOnApp { get; set; }
        public bool? DisplayOnApp { get; set; }
        public bool? AppOrders { get; set; }
        
        public int? LabelTypeShelfId { get; set; }
        public virtual PrintLabelType LabelTypeShelf { get; set; }
        public int? LabelTypePromoId { get; set; }
        public virtual PrintLabelType LabelTypePromo { get; set; }
        public int? LabelTypeShortId { get; set; }
        public virtual PrintLabelType LabelTypeShort { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public virtual StoreGroup StoreGroups { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public virtual Users UserUpdatedBy { get; set; }

        public virtual ICollection<UserRoles> UserRolesStoreList { get; }
        public virtual ICollection<ZoneOutlet> ZoneOutlets { get; set; }
        public virtual ICollection<Product> ProductAccessOutlets { get; }
        public virtual ICollection<OutletProduct> OutletProducts { get; }
        public virtual ICollection<Cashier> CashierStores { get; }
        public virtual ICollection<Keypad> KeypadOutlet { get; }
        public virtual ICollection<Till> TillOutlet { get; }
        public virtual ICollection<OrderHeader> OrderHeaderOutlet { get; }
        public virtual ICollection<OrderAudit> OrderAuditOutlet { get; }
        public virtual ICollection<BulkOrderFromTablet> BulkOrderFromTabletOutlet { get; }
        public virtual ICollection<StockAdjustHeader> StockAdjustHeaderOutlet { get; }
        public virtual ICollection<StockTakeHeader> StockTakeHeaderOutlet { get; }
        public virtual ICollection<OutletSupplierSetting> OutletSupplierSettingStroes { get; }
        public virtual ICollection<JournalHeader> JournalHeaderOutlet { get; }
        public virtual ICollection<GLAccount> GLAccountStores { get; }
        public virtual ICollection<XeroAccount> XeroAccountingStores { get; }
        public virtual ICollection<Transaction> TransactionStores { get; }
        public virtual ICollection<TransactionReport> TransactionReportStores { get; }
        public virtual ICollection<OutletTradingHours> OutletTradingHours { get; }
        public virtual ICollection<SupplierOrderingSchedule> SupplierOrderingScheduleStore { get; set; }
        public virtual ICollection<ManualSaleItem> ManualSaleItemOutlet { get; }
        public virtual ICollection<OutletRoyaltyScales> OutletRoyaltyScalesOutlet { get; set; }
        public virtual ICollection<TillSync> OutletTillSync { get;}
        public virtual ICollection<OutletBudget> OutletBudgetTarget { get;}
        public virtual ICollection<Recipe> RecipeOutlet { get; }
        public virtual ICollection<Paths> PathsOutlet { get; }
        public virtual ICollection<RebateOutlets> RebateOutlets { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionStores { get; }
        public virtual ICollection<Store> OutletPriceFromOutletStore { get; }
        public virtual ICollection<HostUpdChange> HostUpdChangeOutlet { get; }

        public virtual ICollection<OrderHeader> OrderHeaderStoreAsSuppliers { get; }
        public virtual ICollection<AutoOrderSettings> AutoOrderSettings { get; }
    }
}
