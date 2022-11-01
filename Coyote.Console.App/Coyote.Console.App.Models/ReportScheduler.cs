using Coyote.Console.App.Models.EntityContracts;
using Coyote.Console.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coyote.Console.App.Models
{
    [Table("ReportScheduler")]
    public class ReportScheduler : IAuditableField<int>, IKeyIdentifierField<long>
    {
        [Key]
        public long ID { get; set; }
        public string FilterName { get; set; }
        public int ReportId { get; set; }
        public MasterListItems Report { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime InceptionDate { get; set; }

        [MaxLength(MaxLengthConstants.StoreOpenTimeLength)]
        public string InceptionTime { get; set; }
        public SchedulerInterval IntervalInd { get; set; }
        public int IntervalBracket { get; set; }
        public DateTime? LastRun { get; set; }
        public bool? ExcelExport { get; set; } = false;
        public bool? PdfExport { get; set; } = false;
        public bool? CsvExport { get; set; } = false;

        //Filter Params

        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        public string StoreIds { get; set; }
        public long? ProductStartId { get; set; }
        public long? ProductEndId { get; set; }
        public string TillId { get; set; }
        public long? CashierId { get; set; }
        public string ProductIds { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string TransactionTypes { get; set; }
        public string ZoneIds { get; set; }
        public string DayRange { get; set; }
        public bool? IsPromoSale { get; set; }
        public string PromoCode { get; set; }
        public string MemberIds { get; set; }
        public Status IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public virtual Users UserCreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public virtual Users UserModifiedBy { get; set; }


        //Sales NIL transaction
        public int? NillTransactionInterval { get; set; }
        public bool Continuous { get; set; }
        public bool DrillDown { get; set; }
        public bool Summary { get; set; }


        // Cost Variance
        public long StoreId { get; set; }
        public string SupplierId { get; set; }
        public DateTime InvoiceDateFrom { get; set; }
        public DateTime InvoiceDateTo { get; set; }
        public bool IsHostCost { get; set; }
        public bool IsNormalCost { get; set; }
        public bool IsSupplierBatchCost { get; set; }
        public string SupplierBatch { get; set; }

        //Ranging
        public string stockNationalRange { get; set; }
        public GeneralFieldFilter SalesAMT { get; set; }
        public float salesAMTRange { get; set; }

        //No Sale Request
        public string PromotionIds { get; set; }

        //SalesHistory
        public string PeriodicReportType { get; set; }
        public string PromoIds { get; set; }
        public string TillIds { get; set; }
        public bool Chart { get; set; }


        //Stock Print
        public int OutLetId { get; set; }
        public bool Inline { get; set; }
        public bool WithVar { get; set; }

        //Sales Summary
        public bool stockNegativeOH { get; set; } = false;
        public bool stockSOHLevel { get; set; } = false;
        public bool stockSOHButNoSales { get; set; } = false;
        public bool stockLowWarn { get; set; } = false;
        public int? stockNoOfDaysWarn { get; set; }
        public bool SplitOverOutlet { get; set; } = false;
        public bool OrderByAmt { get; set; } = false;
        public bool OrderByQty { get; set; } = false;
        public bool OrderByGP { get; set; } = false;
        public bool OrderByMargin { get; set; } = false;
        public bool OrderBySOH { get; set; } = false;
        public bool OrderByAlp { get; set; } = false;
        public int? SalesSOH { get; set; }
        public float? salesSOHRange { get; set; }

        public virtual ICollection<SchedulerUser> SchedulerUser { get; set; }
        public virtual ICollection<ReportSchedulerLog> ReportSchedulerLog { get; }
    }
}
