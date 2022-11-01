using System;
using System.ComponentModel.DataAnnotations;
using Coyote.Console.Common;
using Microsoft.AspNetCore.Http;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class ReportRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StoreIds { get; set; }
        public long? ProductStartId { get; set; }
        public long? ProductEndId { get; set; }
        public long? TillId { get; set; }
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
        public bool IsPromoSale { get; set; }
        public string PromoCode { get; set; }
        public bool Continuous { get; set; }
        public bool DrillDown { get; set; }
        public bool Summary { get; set; }
        public bool Variance { get; set; }
        public bool Wastage { get; set; }
        public bool Merge { get; set; }
        public string MemberIds { get; set; }
        public bool IsMember { get; set; }

        public bool Quantity { get; set; }
        public bool Amount { get; set; }
        public bool GP { get; set; }
        public bool Margin { get; set; }

        public bool IsNegativeOnHandZero { get; set; }
    }

    public class JournalSalesRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        public string StoreIds { get; set; }
        public string ZoneIds { get; set; }
        public string DepartmentIds { get; set; }
        public string TillId { get; set; }
        public string CashierIds { get; set; }

    }

    public class KPIReportInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StoreIds { get; set; }
        public string DepartmentIds { get; set; }
        public string ZoneIds { get; set; }
        public string DayRange { get; set; }
        public string CommodityIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public int TillId { get; set; }
        public string MemberIds { get; set; }
    }

    public class PrintLabelRequestModel
    {
        [Required]
        public string Format { get; set; }
        public bool Inline { get; set; }
        public string PrintType { get; set; }
        public string LabelType { get; set; }
        public string BarCodeType { get; set; }

        [Required(ErrorMessage = ErrorMessages.OutletId)]
        public int StoreId { get; set; }
        [MaxLength(MaxLengthConstants.StoreCodeLength, ErrorMessage = ErrorMessages.StoreCode)]
        public string StoreCode { get; set; }
        public int? PromoId { get; set; }
        [Required(ErrorMessage = ErrorMessages.PriceLevelReq)]
        public int PriceLevel { get; set; }
        public DateTime? SpecDateFrom { get; set; }
        public DateTime? SpecDateTo { get; set; }
        public DateTime? ReprintDateTime { get; set; }
        public DateTime? BatchPrintDate { get; set; }
        public long? ProductRangeFrom { get; set; }
        public long? ProductRangeTo { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string MemberIds { get; set; }
        public IFormFile PDEFile { get; set; }
        public string PDEFilePath { get; set; }
    }


    public class SalesSummaryRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DayRange { get; set; }
        public bool stockNegativeOH { get; set; }
        public bool stockSOHLevel { get; set; }
        public bool stockSOHButNoSales { get; set; }
        public bool stockLowWarn { get; set; }
        public int stockNoOfDaysWarn { get; set; }
        public string stockNationalRange { get; set; }
        public string TillId { get; set; }
        public string StoreIds { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string ZoneIds { get; set; }
        public bool IsPromoSale { get; set; }
        public string PromoCode { get; set; }
        public string PromotionIds { get; set; }
        public string MemberIds { get; set; }
        public bool Continuous { get; set; }
        public bool DrillDown { get; set; }
        public bool Summary { get; set; }
        public bool Chart { get; set; }
        public bool SplitOverOutlet { get; set; }
        public bool OrderByAmt { get; set; }
        public bool OrderByQty { get; set; }
        public bool OrderByGP { get; set; }
        public bool OrderByMargin { get; set; }
        public bool OrderBySOH { get; set; }
        public bool OrderByAlp { get; set; }
        public GeneralFieldFilter SalesAMT { get; set; }
        public float salesAMTRange { get; set; }
        public GeneralFieldFilter SalesSOH { get; set; }
        public float salesSOHRange { get; set; }
    }

    public class NoSalesSummaryRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Continuous { get; set; }
        public string StoreIds { get; set; }
        public string ZoneIds { get; set; }
        public string DepartmentIds { get; set; }
        public bool IsPromoSale { get; set; }
        public string PromoCode { get; set; }
        public string PromotionIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string SupplierIds { get; set; }
        public string GroupIds { get; set; }
        public string CategoryIds { get; set; }

    }
    public class RankingOutletRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DayRange { get; set; }
        public bool IsPromoSale { get; set; }
        public string PromoCode { get; set; }
        public string PromoIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CommodityIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }

    }

    public class RangingRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DayRange { get; set; }
        public string stockNationalRange { get; set; }
        public string StoreIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CommodityIds { get; set; }
        public string ZoneIds { get; set; }
        public GeneralFieldFilter SalesAMT { get; set; }
        public float salesAMTRange { get; set; }
    }

    public class NationalRangingRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        public string stockNationalRange { get; set; }
        public string StoreIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CommodityIds { get; set; }
        public string ZoneIds { get; set; }
    }
    public class NationalLevelRequestModel
    {
        [Required]
        public string Format { get; set; }
        public bool Inline { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StoreIds { get; set; }
        public string stockNationalRange { get; set; }
    }

    public class StockTrxSheetRequestModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StoreIds { get; set; }
        public string zoneIds { get; set; }
        public long? ProductStartId { get; set; }
        public long? ProductEndId { get; set; }
        public long? TillId { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string DayRange { get; set; }
        public bool IsPromoSale { get; set; }
        public string PromoCode { get; set; }
        public string MemberIds { get; set; }
    }

    public class KPIRankingRequestModel
    {
        [Required]
        public string Format { get; set; }
        public bool Inline { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DepartmentIds { get; set; }
    }
    public class SalesHistoryRequestModle
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        [Required]
        public string PeriodicReportType { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string StoreIds { get; set; }
        public string ZoneIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CommodityIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string DayRange { get; set; }
        public bool IsPromoSale { get; set; }
        public string PromoCode { get; set; }
        public string PromoIds { get; set; }
        public string TillIds { get; set; }
        public bool Continuous { get; set; }
        public bool DrillDown { get; set; }
        public bool Chart { get; set; }
        public bool Summary { get; set; }
    }
    public class SalesNilTransaction
    {
        public string Format { get; set; }
        public bool Inline { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public long? TillId { get; set; }
        public long? CashierId { get; set; }
        public int? NilTransactionInterval { get; set; }
        public string StoreIds { get; set; }
        public string ZoneIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CommodityIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string MemberIds { get; set; }
        public string DayRange { get; set; }
        public bool Continuous { get; set; }
        public bool DrillDown { get; set; }
        public bool Summary { get; set; }
    }
    public class FinancialSummaryRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        public bool Inline { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public long? TillId { get; set; }
        public long? CashierId { get; set; }
        public string StoreIds { get; set; }
        public string ZoneIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CommodityIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string MemberIds { get; set; }
        public string DayRange { get; set; }

    }

    public class StockTakePrintRequestModel
    {
        // Format of resulting report: png, pdf, html
        [Required]
        public string Format { get; set; }
        [Required]
        public int OutLetId { get; set; }
        public long? ProductStartId { get; set; }
        public long? ProductEndId { get; set; }
        public string ProductIds { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public bool Inline { get; set; }
        public bool WithVar { get; set; }

    }
    public class StoreDashboardRequest
    {
        [Required]
        public string Format { get; set; }
        public bool Inline { get; set; }
        public int OutletId { get; set; }
        public int ZoneId { get; set; }
        public DateTime DateRangeFrom { get; set; }
        public DateTime DateRangeTo { get; set; }
        public DateTime CompareRangeFrom { get; set; }
        public DateTime CompareRangeTo { get; set; }
        public DateTime DepartmentDateFrom { get; set; }
        public DateTime DepartmentDateTo { get; set; }
        public bool? LineChart { get; set; }
        public bool? BarChart { get; set; }
    }
    public class ReporterStoreKPIReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StoreIds { get; set; }
        public string DepartmentIds { get; set; }
        public string ZoneIds { get; set; }
        public string DayRange { get; set; }

        public bool AutoLinkedOutlets { get; set; }
        public bool AutoLinkedCommodities { get; set; }
        public string ModuleName { get; set; }
    }

    public class TillJournalReportModel
    {
        public string Format { get; set; }
        public bool Inline { get; set; }
        [Required]
        public string JournalType { get; set; }
        public bool ShowException { get; set; } = false;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //Jounal Filters
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public string CommodityIds { get; set; }
        public string DepartmentIds { get; set; }
        public string CategoryIds { get; set; }
        public string GroupIds { get; set; }
        public string SupplierIds { get; set; }
        public string ManufacturerIds { get; set; }
        public string TransactionType { get; set; }
        public long? TillId { get; set; }
        public long? CashierId { get; set; }
        public bool IsPromoSale { get; set; }
        public string PromoId { get; set; }
        public string StoreIds { get; set; }
        public string DocketNo { get; set; }
        public string MemberIds { get; set; }
        
        public long? ProductStartId { get; set; }
        public long? ProductEndId { get; set; }
        public string ZoneIds { get; set; }

        public string TenderTypeName { get; set; }

        public string LineTypeName { get; set; }
    }

    public class BasketIncidentFilters : ReportRequestModel
    {
        public string ReplicateCode { get; set; }

    }

    public class WeeklySalesWorkBookRequestModel
    {
        [Required]
        public string Format { get; set; }
        public bool Inline { get; set; }
        [Required]
        public DateTime DateFrom { get; set; }
        [Required]
        public DateTime DateTo { get; set; }
        public string StoreIds { get; set; }

    }
}