using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("MasterListItems")]
    public class MasterListItems : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        public int ListId { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength, ErrorMessage = ErrorMessages.ListItemCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.MaxListItemCodeLength, ErrorMessage = ErrorMessages.ListItemName)]
        [Required]
        public string Name { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col1 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col2 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col3 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col4 { get; set; }
        [MaxLength(MaxLengthConstants.MinListItemCodeLength)]
        public string Col5 { get; set; }
        public double? Num1 { get; set; }
        public double? Num2 { get; set; }
        public double? Num3 { get; set; }
        public double? Num4 { get; set; }
        public double? Num5 { get; set; }
        public bool Status { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedById { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public MasterListAccess AccessId { get; set; } = MasterListAccess.ReadWrite;
        public virtual MasterList MasterList { get; set; }
        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }
        public virtual ICollection<Department> DeptMapType { get; }
        public virtual ICollection<ZoneOutlet> ZoneOutlet { get; }
        public virtual ICollection<Warehouse> WarehousMasterListItem { get; }
        public virtual ICollection<Product> ProductGroupMasterListItem { get; }
        public virtual ICollection<Product> ProductCategoryMasterListItem { get; }
        public virtual ICollection<Product> ProductManufacturerMasterListItem { get; }
        public virtual ICollection<Product> ProductTypeMasterListItem { get; }
        public virtual ICollection<Product> ProductNationalRangeMasterListItem { get; }
        public virtual ICollection<Product> ProductUnitMeasureMasterListItem { get; }
        public virtual ICollection<Promotion> PromotionTypeMasterListItem { get; }
        public virtual ICollection<Promotion> PromotionSourceMasterListItem { get; }
        public virtual ICollection<Promotion> PromotionZoneMasterListItem { get; }
        public virtual ICollection<Promotion> PromotionFrequencyMasterListItem { get; }

        public virtual ICollection<KeypadButton> KeypadButtonsMasterListItem { get; }

        public virtual ICollection<ModuleActions> ModuleNameMasterListItem { get; }
        public virtual ICollection<ModuleActions> ActionTypeMasterListItem { get; }
        public virtual ICollection<Cashier> CashierTypeMasterListItem { get; }
        public virtual ICollection<Cashier> CashierZoneMasterListItem { get; }
        public virtual ICollection<Cashier> CashierAccessLevelMasterListItem { get; }
        public virtual ICollection<Till> TillTypeMasterListItem { get; }
        public virtual ICollection<OrderHeader> OrderTypeMasterListItem { get; }
        public virtual ICollection<OrderHeader> OrderStatusMasterListItem { get; }
        public virtual ICollection<OrderAudit> OrderAuditTypeMasterListItem { get; }
        public virtual ICollection<OrderAudit> OrderAuditStatusMasterListItem { get; }
        public virtual ICollection<OrderAudit> OrderAuditNewStatusMasterListItem { get; }
        public virtual ICollection<OrderHeader> OrderCreationTypeMasterListItem { get; }
        public virtual ICollection<OrderDetail> OrderDetailTypeMasterListItem { get; }
        public virtual ICollection<StockAdjustDetail> StockAdjustDetailListItem { get; }
        public virtual ICollection<CompetitionDetail> CompetitionDetailsZoneMasterListItem { get; }
        public virtual ICollection<CompetitionDetail> CompetitionDetailsTypeMasterLisItem { get; }
        public virtual ICollection<CompetitionDetail> CompetitionDetailsResetMasterListItem { get; }
        public virtual ICollection<CompetitionDetail> CompetitionDetailTriggerTypeMasterList { get; }
        public virtual ICollection<CompetitionTrigger> CompetitionTriggerGroupMasterList { get; }
        public virtual ICollection<CompetitionDetail> CompetitionRewardTypeMasterList { get; }

        public virtual ICollection<OutletSupplierSetting> OutletSupplierSettingStateList { get; }
        public virtual ICollection<OutletSupplierSetting> OutletSupplierSettingDivisionList { get; }


        public virtual ICollection<GLAccount> AccountTypeMasterListItem { get; }
        public virtual ICollection<Transaction> TransactionManufacturers { get; }
        public virtual ICollection<Transaction> TransactionCategory { get; }
        public virtual ICollection<Transaction> TransactionGroup { get; }
        public virtual ICollection<TransactionReport> TransactionReportManufacturers { get; }
        public virtual ICollection<TransactionReport> TransactionReportCategory { get; }
        public virtual ICollection<POSMessages> POSMessagesZone { get; }
        public virtual ICollection<ManualSale> ManualSaleMasterListItems { get; }
        public virtual ICollection<ManualSaleItem> ManualSaleItemMasterListItems { get; }
     
        public virtual ICollection<RebateHeader> RebateZone { get; }
        public virtual ICollection<RebateHeader> RebateManufacturer { get; }
        public virtual ICollection<HostSettings> HostSettingsMasterListItem { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionManufacturers { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionCategory { get; }
        public virtual ICollection<ReportScheduler> ReportSchedulerTypes { get; }
        public virtual ICollection<KeypadButton> ButtonTypeCategory { get; }
        public virtual ICollection<HostUpdChange> HostUpdChangeType { get; }

        public virtual ICollection<Commodity> CommodityTypeCategory { get; }



    }
}
