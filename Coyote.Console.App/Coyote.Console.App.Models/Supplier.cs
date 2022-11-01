using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Supplier")]
    public class Supplier : IKeyIdentifier<int>, IAuditable<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(MaxLengthConstants.SuppCodeLength, ErrorMessage = ErrorMessages.SuppCode)]
        [Required]
        public string Code { get; set; }
        [MaxLength(MaxLengthConstants.SuppDescLength, ErrorMessage = ErrorMessages.SuppDesc)]
        [Required]
        public string Desc { get; set; }
        [MaxLength(MaxLengthConstants.SuppAddress1Length, ErrorMessage = ErrorMessages.SuppAddress1)]
        public string Address1 { get; set; }
        [MaxLength(MaxLengthConstants.SuppAddress2Length, ErrorMessage = ErrorMessages.SuppAddress2)]
        public string Address2 { get; set; }
        [MaxLength(MaxLengthConstants.SuppAddress3Length, ErrorMessage = ErrorMessages.SuppAddress3)]
        public string Address3 { get; set; }

        [MaxLength(MaxLengthConstants.SuppAddress3Length, ErrorMessage = ErrorMessages.SuppAddress3)]
        public string Address4 { get; set; }
        [MaxLength(MaxLengthConstants.SuppPhoneLength, ErrorMessage = ErrorMessages.SuppPhone)]
        public string Phone { get; set; }
        [MaxLength(MaxLengthConstants.SuppFaxLength, ErrorMessage = ErrorMessages.SuppFax)]
        public string Fax { get; set; }
        [MaxLength(MaxLengthConstants.SuppEmailLength, ErrorMessage = ErrorMessages.SuppEmail)]
        public string Email { get; set; }
        [MaxLength(MaxLengthConstants.SuppABNLength, ErrorMessage = ErrorMessages.SuppABN)]
        public string ABN { get; set; }
        [MaxLength(MaxLengthConstants.SuppUpdateCostLength, ErrorMessage = ErrorMessages.SuppUpdateCost)]
        public string UpdateCost { get; set; }
        //[MaxLength(MaxLengthConstants.SuppPromoCodeLength, ErrorMessage = ErrorMessages.SuppPromoCode)]
        //public string PromoCode { get; set; }

        [MaxLength(MaxLengthConstants.SuppPromoCodeLength, ErrorMessage = ErrorMessages.SuppPromoCode)]
        public string PromoSupplier { get; set; }
        //[MaxLength(MaxLengthConstants.SuppContactNameLength, ErrorMessage = ErrorMessages.SuppContactName)]
        //public string ContactName { get; set; }

        [MaxLength(MaxLengthConstants.SuppContactNameLength, ErrorMessage = ErrorMessages.SuppContactName)]
        public string Contact { get; set; }
        [MaxLength(MaxLengthConstants.SuppCostZoneLength, ErrorMessage = ErrorMessages.SuppCostZone)]
        public string CostZone { get; set; }
        [MaxLength(MaxLengthConstants.SuppGSTFreeItemCodeLength, ErrorMessage = ErrorMessages.SuppGSTFreeItemCode)]
        public string GSTFreeItemCode { get; set; }
        [MaxLength(MaxLengthConstants.SuppGSTFreeItemDescLength, ErrorMessage = ErrorMessages.SuppGSTFreeItemDesc)]
        public string GSTFreeItemDesc { get; set; }
        [MaxLength(MaxLengthConstants.SuppGSTInclItemCodeLength, ErrorMessage = ErrorMessages.SuppGSTInclItemCode)]
        public string GSTInclItemCode { get; set; }
        [MaxLength(MaxLengthConstants.SuppGSTInclItemDescLength, ErrorMessage = ErrorMessages.SuppGSTInclItemDesc)]
        public string GSTInclItemDesc { get; set; }
        [MaxLength(MaxLengthConstants.SuppXeroNameLength, ErrorMessage = ErrorMessages.SuppXeroName)]
        public string XeroName { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public System.DateTime UpdatedAt { get; set; }
        [Required]
        public int CreatedById { get; set; }
        [Required]
        public int UpdatedById { get; set; }

        public virtual Users CreatedBy { get; set; }
        public virtual Users UpdatedBy { get; set; }

        public virtual ICollection<Warehouse> WarehousesSupplier { get; }
        public virtual ICollection<Product> ProductsSupplier { get; }
        public virtual ICollection<SupplierProduct> SupplierProductSupplier { get; }
        public virtual ICollection<OutletProduct> OutletProductSupplier { get; }
        public virtual ICollection<PromotionBuying> PromotionBuying { get; }
        public virtual ICollection<OrderHeader> OrderHeaderSupplier { get; }
        public virtual ICollection<OrderAudit> OrderAuditSupplier { get; }
        public virtual ICollection<OrderDetail> OrderDetailCheaperSupplier { get; }
        public virtual ICollection<OutletSupplierSetting> OutletSupplierSettings { get; }
        public virtual ICollection<GLAccount> GLAccountSupplier { get; }
        public virtual ICollection<Transaction> TransactionSuppliers { get; }
        public virtual ICollection<TransactionReport> TransactionReportSuppliers { get; }
        public virtual ICollection<SupplierOrderingSchedule> SupplierOrderingScheduleSupplier { get; }
        public virtual ICollection<HostSettings> HostSettingsSupplier  { get; }
        public virtual ICollection<AccountTransaction> AccountTransactionSuppliers { get; }
        public virtual ICollection<AutoOrderSettings> AutoOrderSettings { get; }
    }
}
