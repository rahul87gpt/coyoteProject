using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using FastReport;
using FastReport.Export.Html;
using FastReport.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Coyote.Console.App.Services.Services
{
    public class ReportService : IReportService
    {
        private IConfiguration _configuration;
        //private IUnitOfWork _unitOfwork;
        public ReportService(IConfiguration configuration)
        {
            _configuration = configuration;
            //   _unitOfwork = unitOfWork;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetItemSales(ReportRequestModel request, string type, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALESReport.frx";
                if (type == ReportType.NoSales.ToString())
                    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEM_WITH_NOSALES.frx";
                //if (type == ReportType.Supplier.ToString())
                //    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALES.frx";
                //if (type == ReportType.Outlet.ToString())
                //    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALES_Outlet.frx";

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@productIds", request?.ProductIds),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@reportType", type),
                new SqlParameter("@IsMember", request?.IsMember),
                new SqlParameter("@Quantity", request?.Quantity),
                new SqlParameter("@Amount", request?.Amount),
                new SqlParameter("@GP", request?.GP),
                new SqlParameter("@Margin", request?.Margin)
            };

                var dataSet = GetReportData(StoredProcedures.GetItemsSalesReportNew, dbParams.ToArray());
                if (dataSet?.Tables[0]?.Rows?.Count > 0)
                    return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public DataSet GetItemSales2(ReportRequestModel request, string type, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALESReport.frx";
                if (type == ReportType.NoSales.ToString())
                    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEM_WITH_NOSALES.frx";
                //if (type == ReportType.Supplier.ToString())
                //    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALES.frx";
                //if (type == ReportType.Outlet.ToString())
                //    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALES_Outlet.frx";

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@productIds", request?.ProductIds),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@reportType", type),
                new SqlParameter("@IsMember", request?.IsMember),
                new SqlParameter("@Quantity", request?.Quantity),
                new SqlParameter("@Amount", request?.Amount),
                new SqlParameter("@GP", request?.GP),
                new SqlParameter("@Margin", request?.Margin)
            };

               return GetReportData(StoredProcedures.GetItemsSalesReportNew, dbParams.ToArray());
                
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetStockPurchase(StockPurchaseReportRequest request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_STOCK_PURCHASE.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@invoiceStartDate", request.OrderInvoiceStartDate),
                new SqlParameter("@invoiceEndDate", request.OrderInvoiceEndDate),
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@productIds", request?.ProductIds),
                new SqlParameter("@useInvoiceDates", request.UseInvoiceDates),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@includeRebate", request?.IsRebates)
            };
                var dataSet = GetReportData(StoredProcedures.GetStockPurchaseReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetStockVariance(ReportRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_STOCK_VARIANCE.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@productIds", request?.ProductIds),
                new SqlParameter("@memberIds", request?.MemberIds)
            };

                var dataSet = GetReportData(StoredProcedures.GetStockVarianceReport, dbParams.ToArray());

                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetStockOnHand(ReportRequestModel request, string mime, int userId)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_STOCK_OnHand.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@productIds", request?.ProductIds),
                new SqlParameter("@memberIds",request?.MemberIds),
                new SqlParameter("@isNegativeOnHandZero",request?.IsNegativeOnHandZero),
                new SqlParameter("@userId",userId)


            };

                var dataSet = GetReportData(StoredProcedures.GetStockOnHandReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] AutomaticOrderPrint(OrderPrintRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_AutoOrderPrint.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@StoreId", request?.StoreId),
                new SqlParameter("@orderNo", request?.OrderNo),
                new SqlParameter("@SupplierId", request?.SupplierId)
            };

                var dataSet = GetReportData(StoredProcedures.GetAutoOrderReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] NormalOrderPrint(OrderPrintRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_NormalOrderPrint.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@StoreId", request?.StoreId),
                new SqlParameter("@orderNo", request?.OrderNo),
                new SqlParameter("@SupplierId", request?.SupplierId)
            };

                var dataSet = GetReportData(StoredProcedures.GetNormalOrderReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetStockWastageProductWise(ReportRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_WASTAGE_DEP_PRODUCT_WISE.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@productIds", request?.ProductIds),
                new SqlParameter("@memberIds", request?.MemberIds)
            };

                var dataSet = GetReportData(StoredProcedures.GetStockWastageProductWise, dbParams.ToArray());

                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetStockAdjustment(ReportRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_STOCK_ADJUST.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@productIds", request?.ProductIds),
                new SqlParameter("@memberIds", request?.MemberIds)
            };

                var dataSet = GetReportData(StoredProcedures.GetStockAdjustmentReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] CostVarience(CostVarianceFilters request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_COST_VARIENCE.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@storeId", request?.StoreId),
                new SqlParameter("@SupplierId", request?.SupplierId),
                new SqlParameter("@InvoiceDateFrom", request?.InvoiceDateFrom),
                new SqlParameter("@InvoiceDateTo", request?.InvoiceDateTo),
                new SqlParameter("@IsHostCost", request.IsHostCost),
                new SqlParameter("@IsNormalCost", request.IsNormalCost),
                new SqlParameter("@IsSupplierBatchCost", request.IsSupplierBatchCost),
                new SqlParameter("@SupplierBatch", request.SupplierBatch??"")
                };
                var dataSet = GetReportData(StoredProcedures.GetCostVarienceReport, dbParams.ToArray());

                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetJournalSalesFinancialSummary(JournalSalesRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            if (request.StoreIds == null && request.ZoneIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.JournalSalesReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Financial Summary"));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_JOURNALSALES_FINANCIAL_SUMMARY.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@cashierIds ", request?.CashierIds)
            };

                var dataSet = GetReportData(StoredProcedures.GetJournalSalesFinancialSummaryReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetJournalSalesRoyaltyAndAdvertisingSummary(JournalSalesRequestModel request, string type, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (request.StoreIds == null && request.ZoneIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), type, "outlet or zone."));

            //if (request.DepartmentIds == null)
            //    throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), type, "department."));

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_JOURNALSALES_ROYALTY&ADVERTISING_SUMMARY.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@startDate", request?.StartDate),
                    new SqlParameter("@endDate", request?.EndDate),
                    new SqlParameter("@tillId", request?.TillId),
                    new SqlParameter("@storeIds", request?.StoreIds),
                    new SqlParameter("@zoneIds", request?.ZoneIds),
                    new SqlParameter("@departmentIds", request?.DepartmentIds),
                    new SqlParameter("@cashierIds ", request?.CashierIds),
                    new SqlParameter("@reportType", type),
                };
                var dataSet = GetReportData(StoredProcedures.GetJournalSalesRoyaltyAndAdvertisingSummaryReport, dbParams.ToArray());

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    var royaltyAdvertising = MappingHelpers.ConvertDataTable<SalesRoyaltyAdvertisingResponseModel>(dataSet.Tables[0]);
                    var outletList = royaltyAdvertising.Select(x => x.OutletId).GroupBy(x => x);
                    var royaltyAdvertisingList = new List<SalesRoyaltyAdvertisingResponseModel>();

                    foreach (var outlet in outletList)
                    {
                        double departmentTotal = 0;
                        double outletTotal = 0;

                        var salesRoyaltyAdvertising = new SalesRoyaltyAdvertisingResponseModel();
                        salesRoyaltyAdvertising.OutletId = outlet.Key;
                        salesRoyaltyAdvertising.Number = royaltyAdvertising.FirstOrDefault(x => x.OutletId == outlet.Key).Number;
                        salesRoyaltyAdvertising.Description = royaltyAdvertising.FirstOrDefault(x => x.OutletId == outlet.Key).Description;
                        salesRoyaltyAdvertising.SalesAmt = royaltyAdvertising.Where(x => x.OutletId == outlet.Key)?.Sum(x => x.SalesAmt) ?? 0;
                        salesRoyaltyAdvertising.SalesAmtExGST = royaltyAdvertising.Where(x => x.OutletId == outlet.Key)?.Sum(x => x.SalesAmtExGST) ?? 0;
                        salesRoyaltyAdvertising.DepartmentRate = royaltyAdvertising.Where(x => x.OutletId == outlet.Key)?.Sum(x => x.DepartmentRate) ?? 0;
                        salesRoyaltyAdvertising.GSTAmount = royaltyAdvertising.Where(x => x.OutletId == outlet.Key)?.Sum(x => x.GSTAmount) ?? 0;
                        salesRoyaltyAdvertising.Values = 0;

                        if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
                        {
                            var outletRoyaltyList = MappingHelpers.ConvertDataTable<OutletRoyaltyScalesResponseModel>(dataSet.Tables[2]);
                            var listData = outletRoyaltyList.Where(x => x.OutletId == outlet.Key).ToList();
                            if (listData.Count > 0)
                            {
                                if (listData[0].IncGST == true)
                                {
                                    departmentTotal = royaltyAdvertising.Where(x => x.OutletId == outlet.Key && x.DepartmentRate != 0)?.Sum(x => x.SalesAmt) ?? 0;
                                    outletTotal = royaltyAdvertising.Where(x => x.OutletId == outlet.Key && x.DepartmentRate == 0)?.Sum(x => x.SalesAmt) ?? 0;
                                }
                                else
                                {
                                    departmentTotal = royaltyAdvertising.Where(x => x.OutletId == outlet.Key && x.DepartmentRate != 0)?.Sum(x => x.GSTAmount) ?? 0;
                                    outletTotal = royaltyAdvertising.Where(x => x.OutletId == outlet.Key && x.DepartmentRate == 0)?.Sum(x => x.GSTAmount) ?? 0;
                                }

                                if (departmentTotal != 0 && salesRoyaltyAdvertising.DepartmentRate != 0)
                                    departmentTotal = departmentTotal * salesRoyaltyAdvertising.DepartmentRate / 100;

                                if (listData.Count == 1)
                                {
                                    if (outletTotal < listData[0].ScalesFrom)
                                        salesRoyaltyAdvertising.Values = 0;
                                    else if (outletTotal < listData[0].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((outletTotal - listData[0].ScalesFrom) * listData[0].Percent) / 100;
                                }
                                else if (listData.Count == 2)
                                {
                                    if (outletTotal < listData[0].ScalesFrom)
                                        salesRoyaltyAdvertising.Values = 0;
                                    else if (outletTotal < listData[0].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((outletTotal - listData[0].ScalesFrom) * listData[0].Percent) / 100;
                                    else if (outletTotal < listData[1].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                         + ((outletTotal - listData[1].ScalesFrom) * listData[1].Percent) / 100;
                                }
                                else if (listData.Count == 3)
                                {
                                    if (outletTotal < listData[0].ScalesFrom)
                                        salesRoyaltyAdvertising.Values = 0;
                                    else if (outletTotal < listData[0].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((outletTotal - listData[0].ScalesFrom) * listData[0].Percent) / 100;
                                    else if (outletTotal < listData[1].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((outletTotal - listData[1].ScalesFrom) * listData[1].Percent) / 100;
                                    else if (outletTotal < listData[2].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((listData[1].ScalesTo - listData[1].ScalesFrom) * listData[1].Percent) / 100
                                                + ((outletTotal - listData[2].ScalesFrom) * listData[2].Percent) / 100;
                                }
                                else if (listData.Count == 4)
                                {
                                    if (outletTotal < listData[0].ScalesFrom)
                                        salesRoyaltyAdvertising.Values = 0;
                                    else if (outletTotal < listData[0].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((outletTotal - listData[0].ScalesFrom) * listData[0].Percent) / 100;
                                    else if (outletTotal < listData[1].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((outletTotal - listData[1].ScalesFrom) * listData[1].Percent) / 100;
                                    else if (outletTotal < listData[2].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((listData[1].ScalesTo - listData[1].ScalesFrom) * listData[1].Percent) / 100
                                                + ((outletTotal - listData[2].ScalesFrom) * listData[2].Percent) / 100;
                                    else if (outletTotal < listData[3].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((listData[1].ScalesTo - listData[1].ScalesFrom) * listData[1].Percent) / 100
                                                + ((listData[2].ScalesTo - listData[2].ScalesFrom) * listData[2].Percent) / 100
                                                + ((outletTotal - listData[3].ScalesFrom) * listData[3].Percent) / 100;
                                }
                                else if (listData.Count == 5)
                                {
                                    if (outletTotal < listData[0].ScalesFrom)
                                        salesRoyaltyAdvertising.Values = 0;
                                    else if (outletTotal < listData[0].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((outletTotal - listData[0].ScalesFrom) * listData[0].Percent) / 100;
                                    else if (outletTotal < listData[1].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((outletTotal - listData[1].ScalesFrom) * listData[1].Percent) / 100;
                                    else if (outletTotal < listData[2].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((listData[1].ScalesTo - listData[1].ScalesFrom) * listData[1].Percent) / 100
                                                + ((outletTotal - listData[2].ScalesFrom) * listData[2].Percent) / 100;
                                    else if (outletTotal < listData[3].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((listData[1].ScalesTo - listData[1].ScalesFrom) * listData[1].Percent) / 100
                                                + ((listData[2].ScalesTo - listData[2].ScalesFrom) * listData[2].Percent) / 100
                                                + ((outletTotal - listData[3].ScalesFrom) * listData[3].Percent) / 100;
                                    else if (outletTotal < listData[4].ScalesTo)
                                        salesRoyaltyAdvertising.Values = ((listData[0].ScalesTo - listData[0].ScalesFrom) * listData[0].Percent) / 100
                                                + ((listData[1].ScalesTo - listData[1].ScalesFrom) * listData[1].Percent) / 100
                                                + ((listData[2].ScalesTo - listData[2].ScalesFrom) * listData[2].Percent) / 100
                                                + ((listData[3].ScalesTo - listData[3].ScalesFrom) * listData[3].Percent) / 100
                                                + ((outletTotal - listData[4].ScalesFrom) * listData[4].Percent) / 100;
                                }
                                salesRoyaltyAdvertising.Values = salesRoyaltyAdvertising.Values + departmentTotal;
                                royaltyAdvertisingList.Add(salesRoyaltyAdvertising);
                            }
                        }
                    }
                    var dsRAScales = royaltyAdvertisingList.ConvertToDataSet("DataSet");
                    dataSet.Tables.Remove("DataSet");
                    dataSet.Tables.Add(dsRAScales.Tables["DataSet"].Copy());
                }
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public DataSet GetSalesAndStockTrxSheet(StockTrxSheetRequestModel request, SecurityViewModel securityViewModel, string type, PagedInputModel filter = null)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                bool StoreIds = false;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                    StoreIds = true;

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.zoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@reportType", type),
                new SqlParameter("@globalFilter", filter?.GlobalFilter),
                new SqlParameter("@skipCount", filter?.SkipCount),
                new SqlParameter("@maxResultCount", filter?.MaxResultCount),
                new SqlParameter("@accessOutletIds", (StoreIds == true)?securityViewModel?.StoreIds:null),
                new SqlParameter("@sortColumn", filter?.Sorting),
                new SqlParameter("@sortDirection", filter?.Direction),
            };
                var dataSet = GetReportData(StoredProcedures.GetSalesAndStockTrxSheet, dbParams.ToArray());
                dataSet.Tables[0].TableName = "data";
                return dataSet;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public DataSet SalesChart(ReportRequestModel request, string reportFor)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request.StartDate),
                new SqlParameter("@endDate", request.EndDate),
                new SqlParameter("@storeIds", request.StoreIds),
                new SqlParameter("@productStartId", request.ProductStartId),
                new SqlParameter("@productEndId", request.ProductEndId),
                new SqlParameter("@tillId", request.TillId),
                new SqlParameter("@commodityIds", request.CommodityIds),
                new SqlParameter("@departmentIds", request.DepartmentIds),
                new SqlParameter("@categoryIds", request.CategoryIds),
                new SqlParameter("@groupIds", request.GroupIds),
                new SqlParameter("@suppliers", request.SupplierIds),
                new SqlParameter("@manufacturerIds", request.ManufacturerIds),
                new SqlParameter("@isPromoSales", request.IsPromoSale),
                new SqlParameter("@promoCode", request.PromoCode),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@ChartReportFor", reportFor),
                new SqlParameter("@dayRange", request?.DayRange)

            };
                return GetReportData(StoredProcedures.GetSalesChartReport, dbParams.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public DataSet SalesChartById(ReportRequestModel request, string reportFor)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request.StartDate),
                new SqlParameter("@endDate", request.EndDate),
                new SqlParameter("@storeIds", request.StoreIds),
                new SqlParameter("@productStartId", request.ProductStartId),
                new SqlParameter("@productEndId", request.ProductEndId),
                new SqlParameter("@tillId", request.TillId),
                new SqlParameter("@commodityIds", request.CommodityIds),
                new SqlParameter("@departmentIds", request.DepartmentIds),
                new SqlParameter("@categoryIds", request.CategoryIds),
                new SqlParameter("@groupIds", request.GroupIds),
                new SqlParameter("@suppliers", request.SupplierIds),
                new SqlParameter("@manufacturerIds", request.ManufacturerIds),
                new SqlParameter("@isPromoSales", request.IsPromoSale),
                new SqlParameter("@promoCode", request.PromoCode),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@ChartReportFor", reportFor),
                new SqlParameter("@dayRange", request?.DayRange)
            };
                return GetReportData(StoredProcedures.GetSalesChartReportByID, dbParams.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public DataSet SalesChartDetailed(ReportRequestModel request, string reportFor)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request.StartDate),
                new SqlParameter("@endDate", request.EndDate),
                new SqlParameter("@storeIds", request.StoreIds),
                new SqlParameter("@productStartId", request.ProductStartId),
                new SqlParameter("@productEndId", request.ProductEndId),
                new SqlParameter("@tillId", request.TillId),
                new SqlParameter("@commodityIds", request.CommodityIds),
                new SqlParameter("@departmentIds", request.DepartmentIds),
                new SqlParameter("@categoryIds", request.CategoryIds),
                new SqlParameter("@groupIds", request.GroupIds),
                new SqlParameter("@suppliers", request.SupplierIds),
                new SqlParameter("@manufacturerIds", request.ManufacturerIds),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@isPromoSales", request.IsPromoSale),
                new SqlParameter("@promoCode", request.PromoCode),
                new SqlParameter("@ChartReportFor", reportFor),
                new SqlParameter("@dayRange", request.DayRange)
            };
                return GetReportData(StoredProcedures.GetSalesChartDetailedReport, dbParams.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetItemSalesSummary(SalesSummaryRequestModel request, string type, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALES_SUMMARY.frx";
                if (type == ReportType.ItemWithSlowMovingStock.ToString())
                    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMSALES_WITH_SLOW_MOVING_SUMMARY.frx";

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@chart",request.Chart),
                new SqlParameter("@splitOverOutlet",request.SplitOverOutlet),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@orderByQty", request?.OrderByQty),
                new SqlParameter("@orderByAmt", request?.OrderByAmt),
                new SqlParameter("@orderByGP",request.OrderByGP),
                new SqlParameter("@orderByMargin",request.OrderByMargin),
                new SqlParameter("@orderBySOH",request.OrderBySOH),
                new SqlParameter("@orderByAlph",request.OrderByAlp),
                new SqlParameter("@stockNegativeOH",request?.stockNegativeOH),
                new SqlParameter("@stockSOHLevel", request?.stockSOHLevel),
                new SqlParameter("@stockSOHButNoSales",request?.stockSOHButNoSales),
                new SqlParameter("@stockLowWarn",request?.stockLowWarn),
                new SqlParameter("@stockNoOfDaysWarn",request?.stockNoOfDaysWarn),
                new SqlParameter("@stockNationalRange",request?.stockNationalRange),
                new SqlParameter("@salesAMT",(int)request?.SalesAMT),
                new SqlParameter("@salesSOH",(int)request?.SalesSOH),
                new SqlParameter("@salesAMTRange", request?.salesAMTRange),
                new SqlParameter("@salesSOHRange", request?.salesSOHRange),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@promoSales", request?.IsPromoSale),
                new SqlParameter("@promoCode", request?.PromoCode),
                new SqlParameter("@promoIds", request?.PromotionIds),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@reportType", type),
            };
                var dataSet = GetReportData(StoredProcedures.GetItemsSalesSummaryReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetItemWithNoSalesProduct(NoSalesSummaryRequestModel request, string type, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMWITHNOSALES_SUMMARY.frx";
                if (type == ReportType.ItemNoSales.ToString())
                    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMWITHNOSALES_SUMMARY_NO_STOCKOPTIONS.frx";
                //string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMWITHNOSALES_SUMMARY.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@orderByAmt", true),
                new SqlParameter("@stockSOHButNoSales",true),
                new SqlParameter("@salesSOH",0),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@promoSales", request?.IsPromoSale),
                new SqlParameter("@promoCode", request?.PromoCode),
                new SqlParameter("@promoIds", request?.PromotionIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@reportType", type),
            };
                var dataSet = GetReportData(StoredProcedures.GetItemsSalesSummaryReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get Labels to Print
        /// </summary>
        /// <param name="request"></param>
        /// <param name="mime"></param>
        /// <returns></returns>
        public byte[] GetPrintLabel(PrintLabelRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (string.IsNullOrEmpty(request.LabelType))
                throw new BadRequestException(ErrorMessages.LabelTypeRequired.ToString(CultureInfo.CurrentCulture));

            if (string.IsNullOrEmpty(request.Format) || request.Format.ToLower() != "pdf")
                throw new BadRequestException(ErrorMessages.FormatRequired.ToString(CultureInfo.CurrentCulture));

            if (string.IsNullOrEmpty(request.PrintType))
                throw new BadRequestException(ErrorMessages.PrintTypeRequired.ToString(CultureInfo.CurrentCulture));

            if (request.PriceLevel <= 0 || request.PriceLevel > 5)
                throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));

            switch (request.PrintType.ToLower())
            {
                case "change":
                    if (request.StoreId <= 0)
                        throw new BadRequestException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
                    break;
                case "reprintchange":
                    if (request.ReprintDateTime == null)
                        throw new BadRequestException(ErrorMessages.InvalidReprintDate.ToString(CultureInfo.CurrentCulture));
                    break;
                case "specprice":
                    if (request.SpecDateTo == null || request.SpecDateFrom == null)
                        throw new BadRequestException(ErrorMessages.InvalidSpecialDate.ToString(CultureInfo.CurrentCulture));
                    break;
                case "selected":
                    if (request.ProductRangeFrom <= 0 || request.ProductRangeTo <= 0)
                        throw new BadRequestException(ErrorMessages.InvalidProductRange.ToString(CultureInfo.CurrentCulture));
                    break;
                case "promotion":
                    if (request.PromoId == null)
                        throw new BadRequestException(ErrorMessages.InvalidPromo.ToString(CultureInfo.CurrentCulture));
                    break;
                case "tablet":
                    if (request.ReprintDateTime == null)
                        throw new BadRequestException(ErrorMessages.InvalidReprintDate.ToString(CultureInfo.CurrentCulture));
                    break;
                case "pdeload":
                    if (request.BatchPrintDate == null)
                        throw new BadRequestException(ErrorMessages.InvalidBatchDate.ToString(CultureInfo.CurrentCulture));
                    break;
                default:
                    throw new BadRequestException(ErrorMessages.PrintTypeRequired.ToString(CultureInfo.CurrentCulture));
            }

            if (request.StoreId <= 0)
            {
                throw new BadRequestException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
            }
            try
            {

                string reportPath = _configuration["FastReportSettings:ReportPath"];
                string reportType = "";
                switch (request.LabelType?.ToUpper())
                {
                    case PrintLabelTypes.SHELF1UP: reportType = PrintLabelPaths.SHELF1UP; break;
                    case PrintLabelTypes.SLADEPOINT33: reportType = PrintLabelPaths.SLADEPOINT33; break;
                    case PrintLabelTypes.Shelf24: reportType = PrintLabelPaths.Shelf24; break;
                    case PrintLabelTypes.Potrait21: reportType = PrintLabelPaths.Potrait21; break;
                    case PrintLabelTypes.BurleighAPN: reportType = PrintLabelPaths.BurleighAPN; break;
                    case PrintLabelTypes.Fresh9UP: reportType = PrintLabelPaths.Fresh9UP; break;
                    case PrintLabelTypes.Helensvale24: reportType = PrintLabelPaths.Helensvale24; break;
                    case PrintLabelTypes.Offer9UP: reportType = PrintLabelPaths.Offer9UP; break;
                    case PrintLabelTypes.OfferA4OWL: reportType = PrintLabelPaths.OfferA4OWL; break;
                    case PrintLabelTypes.OwlAPN: reportType = PrintLabelPaths.OwlAPN; break;
                    case PrintLabelTypes.OwlMSI: reportType = PrintLabelPaths.OwlMSI; break;
                    case PrintLabelTypes.OWL24: reportType = PrintLabelPaths.OWL24; break;
                    case PrintLabelTypes.Promo9OWL: reportType = PrintLabelPaths.Promo9OWL; break;
                    case PrintLabelTypes.Promo9UP: reportType = PrintLabelPaths.Promo9UP; break;
                    case PrintLabelTypes.Promo4UP: reportType = PrintLabelPaths.Promo4UP; break;
                    case PrintLabelTypes.PromoA4OWL: reportType = PrintLabelPaths.PromoA4OWL; break;
                    case PrintLabelTypes.SHORT1: reportType = PrintLabelPaths.SHORT1; break;
                    case PrintLabelTypes.STARLINE: reportType = PrintLabelPaths.STARLINE; break;
                    case PrintLabelTypes.StarlineAPN: reportType = PrintLabelPaths.StarlineAPN; break;
                    case PrintLabelTypes.Std4OWL: reportType = PrintLabelPaths.Std4OWL; break;
                    case PrintLabelTypes.Std9BGOLD: reportType = PrintLabelPaths.Std9BGOLD; break;
                    case PrintLabelTypes.Std9FBLS: reportType = PrintLabelPaths.Std9FBLS; break;
                    case PrintLabelTypes.Std9OWL: reportType = PrintLabelPaths.Std9OWL; break;
                    case PrintLabelTypes.StdA4OWL: reportType = PrintLabelPaths.StdA4OWL; break;
                    case PrintLabelTypes.StdAPN: reportType = PrintLabelPaths.StdAPN; break;
                    default: reportType = ""; break;
                }

                if (string.IsNullOrEmpty(reportType))
                    throw new BadRequestException(ErrorMessages.LabelTypeUnavailable.ToString(CultureInfo.CurrentCulture));

                reportPath = reportPath + reportType;

                var APNLow = "999999999999999";
                if (request?.BarCodeType?.ToLower() == "apn")
                {
                    APNLow = "9999999";
                }

                var dataSet = new DataSet();


                List<SqlParameter> dbParams = new List<SqlParameter>
                   {
                   new SqlParameter("@Outlet", request?.StoreId),
                   new SqlParameter("@PromoId", request.PromoId.HasValue?request?.PromoId : null),
                   new SqlParameter("@PrintType", request?.PrintType.ToLower()),
                   new SqlParameter("@APNLow", APNLow),
                   new SqlParameter("@PriceLevel", request?.PriceLevel),
                   new SqlParameter("@SpecDateFrom", request?.SpecDateFrom),
                   new SqlParameter("@SpecDateTo", request?.SpecDateTo),
                   new SqlParameter("@ReprintDateTime", request?.ReprintDateTime),
                   new SqlParameter("@CommodityIds", request?.CommodityIds),
                   new SqlParameter("@DepartmentIds", request?.DepartmentIds),
                   new SqlParameter("@GroupIds", request?.GroupIds),
                   new SqlParameter("@SupplierIds", request?.SupplierIds),
                   new SqlParameter("@ManufacturerIds ", request?.ManufacturerIds),
                   new SqlParameter("@CategoryIds", request?.CategoryIds),
                   new SqlParameter("@ProductRangeFrom", request?.ProductRangeFrom),
                   new SqlParameter("@ProductRangeTo", request?.ProductRangeTo),
                   new SqlParameter("@MemberIds",request?.MemberIds)
                   };

                dataSet = GetReportData(StoredProcedures.GetPrintChangeLabels, dbParams.ToArray());


                if (dataSet == null || dataSet.Tables.Count == 0)
                    throw new BadRequestException(ErrorMessages.NoLabelAvailable.ToString(CultureInfo.CurrentCulture));

                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message, ex);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message, ex);
                }
                if (ex is NotFoundException)
                {
                    throw new BadRequestException(ex.Message, ex);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }



        /// <summary>
        /// Get Labels to Print
        /// </summary>
        /// <param name="request"></param>
        /// <param name="mime"></param>
        /// <returns></returns>
        public DataSet GetPrintLabelDataSet(PrintLabelRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (string.IsNullOrEmpty(request.LabelType))
                throw new BadRequestException(ErrorMessages.LabelTypeRequired.ToString(CultureInfo.CurrentCulture));

            if (string.IsNullOrEmpty(request.Format) || request.Format.ToLower() != "pdf")
                throw new BadRequestException(ErrorMessages.FormatRequired.ToString(CultureInfo.CurrentCulture));

            if (string.IsNullOrEmpty(request.PrintType))
                throw new BadRequestException(ErrorMessages.PrintTypeRequired.ToString(CultureInfo.CurrentCulture));

            if (request.PriceLevel <= 0 || request.PriceLevel > 5)
                throw new BadRequestException(ErrorMessages.PriceLevelReq.ToString(CultureInfo.CurrentCulture));

            switch (request.PrintType.ToLower())
            {
                case "change":
                    if (request.StoreId <= 0)
                        throw new BadRequestException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
                    break;
                case "reprintchange":
                    if (request.ReprintDateTime == null)
                        throw new BadRequestException(ErrorMessages.InvalidReprintDate.ToString(CultureInfo.CurrentCulture));
                    break;
                case "specprice":
                    if (request.SpecDateTo == null || request.SpecDateFrom == null)
                        throw new BadRequestException(ErrorMessages.InvalidSpecialDate.ToString(CultureInfo.CurrentCulture));
                    break;
                case "selected":
                    if (request.ProductRangeFrom <= 0 || request.ProductRangeTo <= 0)
                        throw new BadRequestException(ErrorMessages.InvalidProductRange.ToString(CultureInfo.CurrentCulture));
                    break;
                case "promotion":
                    if (request.PromoId == null)
                        throw new BadRequestException(ErrorMessages.InvalidPromo.ToString(CultureInfo.CurrentCulture));
                    break;
                case "tablet":
                    if (request.ReprintDateTime == null)
                        throw new BadRequestException(ErrorMessages.InvalidReprintDate.ToString(CultureInfo.CurrentCulture));
                    break;
                case "pdeload":
                    if (request.BatchPrintDate == null)
                        throw new BadRequestException(ErrorMessages.InvalidBatchDate.ToString(CultureInfo.CurrentCulture));
                    break;
                default:
                    throw new BadRequestException(ErrorMessages.PrintTypeRequired.ToString(CultureInfo.CurrentCulture));
            }

            if (request.StoreId <= 0)
            {
                throw new BadRequestException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
            }
            try
            {

                string reportPath = _configuration["FastReportSettings:ReportPath"];
                string reportType = "";
                switch (request.LabelType?.ToUpper())
                {
                    case PrintLabelTypes.SHELF1UP: reportType = PrintLabelPaths.SHELF1UP; break;
                    case PrintLabelTypes.SLADEPOINT33: reportType = PrintLabelPaths.SLADEPOINT33; break;
                    case PrintLabelTypes.Shelf24: reportType = PrintLabelPaths.Shelf24; break;
                    case PrintLabelTypes.Potrait21: reportType = PrintLabelPaths.Potrait21; break;
                    case PrintLabelTypes.BurleighAPN: reportType = PrintLabelPaths.BurleighAPN; break;
                    case PrintLabelTypes.Fresh9UP: reportType = PrintLabelPaths.Fresh9UP; break;
                    case PrintLabelTypes.Helensvale24: reportType = PrintLabelPaths.Helensvale24; break;
                    case PrintLabelTypes.Offer9UP: reportType = PrintLabelPaths.Offer9UP; break;
                    case PrintLabelTypes.OfferA4OWL: reportType = PrintLabelPaths.OfferA4OWL; break;
                    case PrintLabelTypes.OwlAPN: reportType = PrintLabelPaths.OwlAPN; break;
                    case PrintLabelTypes.OwlMSI: reportType = PrintLabelPaths.OwlMSI; break;
                    case PrintLabelTypes.OWL24: reportType = PrintLabelPaths.OWL24; break;
                    case PrintLabelTypes.Promo9OWL: reportType = PrintLabelPaths.Promo9OWL; break;
                    case PrintLabelTypes.Promo9UP: reportType = PrintLabelPaths.Promo9UP; break;
                    case PrintLabelTypes.Promo4UP: reportType = PrintLabelPaths.Promo4UP; break;
                    case PrintLabelTypes.PromoA4OWL: reportType = PrintLabelPaths.PromoA4OWL; break;
                    case PrintLabelTypes.SHORT1: reportType = PrintLabelPaths.SHORT1; break;
                    case PrintLabelTypes.STARLINE: reportType = PrintLabelPaths.STARLINE; break;
                    case PrintLabelTypes.StarlineAPN: reportType = PrintLabelPaths.StarlineAPN; break;
                    case PrintLabelTypes.Std4OWL: reportType = PrintLabelPaths.Std4OWL; break;
                    case PrintLabelTypes.Std9BGOLD: reportType = PrintLabelPaths.Std9BGOLD; break;
                    case PrintLabelTypes.Std9FBLS: reportType = PrintLabelPaths.Std9FBLS; break;
                    case PrintLabelTypes.Std9OWL: reportType = PrintLabelPaths.Std9OWL; break;
                    case PrintLabelTypes.StdA4OWL: reportType = PrintLabelPaths.StdA4OWL; break;
                    case PrintLabelTypes.StdAPN: reportType = PrintLabelPaths.StdAPN; break;
                    default: reportType = ""; break;
                }

                if (string.IsNullOrEmpty(reportType))
                    throw new BadRequestException(ErrorMessages.LabelTypeUnavailable.ToString(CultureInfo.CurrentCulture));

                reportPath = reportPath + reportType;

                var APNLow = "999999999999999";
                if (request?.BarCodeType?.ToLower() == "apn")
                {
                    APNLow = "9999999";
                }

#pragma warning disable CA2000 // Dispose objects before losing scope
                var dataSet = new DataSet();
#pragma warning restore CA2000 // Dispose objects before losing scope


                List<SqlParameter> dbParams = new List<SqlParameter>
                   {
                   new SqlParameter("@Outlet", request?.StoreId),
                   new SqlParameter("@PromoId", request.PromoId.HasValue?request?.PromoId : null),
                   new SqlParameter("@PrintType", request?.PrintType.ToLower()),
                   new SqlParameter("@APNLow", APNLow),
                   new SqlParameter("@PriceLevel", request?.PriceLevel),
                   new SqlParameter("@SpecDateFrom", request?.SpecDateFrom),
                   new SqlParameter("@SpecDateTo", request?.SpecDateTo),
                   new SqlParameter("@ReprintDateTime", request?.ReprintDateTime),
                   new SqlParameter("@CommodityIds", request?.CommodityIds),
                   new SqlParameter("@DepartmentIds", request?.DepartmentIds),
                   new SqlParameter("@GroupIds", request?.GroupIds),
                   new SqlParameter("@SupplierIds", request?.SupplierIds),
                   new SqlParameter("@ManufacturerIds ", request?.ManufacturerIds),
                   new SqlParameter("@CategoryIds", request?.CategoryIds),
                   new SqlParameter("@ProductRangeFrom", request?.ProductRangeFrom),
                   new SqlParameter("@ProductRangeTo", request?.ProductRangeTo),
                   new SqlParameter("@MemberIds",request?.MemberIds)
                   };

#pragma warning disable CA2000 // Dispose objects before losing scope
                dataSet = GetReportData(StoredProcedures.GetPrintChangeLabels, dbParams.ToArray());
#pragma warning restore CA2000 // Dispose objects before losing scope


                if (dataSet == null || dataSet.Tables.Count == 0)
                    throw new BadRequestException(ErrorMessages.NoLabelAvailable.ToString(CultureInfo.CurrentCulture));

                return dataSet;
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message, ex);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message, ex);
                }
                if (ex is NotFoundException)
                {
                    throw new BadRequestException(ex.Message, ex);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public async Task<ReportFilterationResponseModel> ReportFilterationDropdownList(SecurityViewModel securityViewModel, PagedInputModel filter = null)
        {
            bool StoreIds = false;
            if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                StoreIds = true;
            try
            {
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@SkipCount", filter?.SkipCount),
                new SqlParameter("@MaxResultCount", filter?.MaxResultCount),
                new SqlParameter("@StoreIds", (StoreIds == true)?securityViewModel?.StoreIds:null),
                new SqlParameter("@SortColumn", filter?.Sorting),
                new SqlParameter("@SortDirection", filter?.Direction),
                };

                var dset = await BindReportDropdownListItems(StoredProcedures.GetReportFilteationDropDownListItems, dbParams.ToArray()).ConfigureAwait(false);
                ReportFilterationResponseModel reportFilterationResponse = new ReportFilterationResponseModel();
                reportFilterationResponse.Categories.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[0])?.ToList());
                reportFilterationResponse.Groups.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[1])?.ToList());
                reportFilterationResponse.Manufacturers.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[2])?.ToList());
                reportFilterationResponse.Zones.AddRange(MappingHelpers.ConvertDataTable<MasterListItemResponseViewModel>(dset.Tables[3])?.ToList());
                reportFilterationResponse.Commodities.AddRange(MappingHelpers.ConvertDataTable<CommodityResponseModel>(dset.Tables[4])?.ToList());
                reportFilterationResponse.Departments.AddRange(MappingHelpers.ConvertDataTable<DepartmentResponseModel>(dset.Tables[5])?.ToList());
                reportFilterationResponse.Suppliers.AddRange(MappingHelpers.ConvertDataTable<SupplierResponseViewModel>(dset.Tables[6])?.ToList());
                reportFilterationResponse.Stores.AddRange(MappingHelpers.ConvertDataTable<StoreResponseModel>(dset.Tables[7])?.ToList());
                reportFilterationResponse.Tills.AddRange(MappingHelpers.ConvertDataTable<TillResponseModel>(dset.Tables[8])?.ToList());
                reportFilterationResponse.Cashiers.AddRange(MappingHelpers.ConvertDataTable<CashierResponseModel>(dset.Tables[9])?.ToList());

                return reportFilterationResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetRankingByOutlet(RankingOutletRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_RANKING_BY_OUTLET.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@promoIds", request?.PromoIds),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds)
            };

                var dataSet = GetReportData(StoredProcedures.GetRankingByOutlet, dbParams.ToArray());
                if (dataSet?.Tables[0]?.Rows?.Count > 0)
                    return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetItemPurchaseSummary(SalesSummaryRequestModel request, string type, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ITEMPURCHASE_SUMMARY.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@orderByAlph",request.OrderByAlp),
                new SqlParameter("@orderByQty", request?.OrderByQty),
                new SqlParameter("@orderByAmt", request?.OrderByAmt),
                new SqlParameter("@orderByGP",request.OrderByGP),
                new SqlParameter("@orderByMargin",request.OrderByMargin),
                new SqlParameter("@orderBySOH",request.OrderBySOH),
                new SqlParameter("@stockNegativeOH",request?.stockNegativeOH),
                new SqlParameter("@stockSOHLevel", request?.stockSOHLevel),
                new SqlParameter("@stockSOHButNoSales",request?.stockSOHButNoSales),
                new SqlParameter("@stockLowWarn",request?.stockLowWarn),
                new SqlParameter("@stockNoOfDaysWarn",request?.stockNoOfDaysWarn),
                new SqlParameter("@stockNationalRange",request?.stockNationalRange),
                new SqlParameter("@salesAMT",(int)request?.SalesAMT),
                new SqlParameter("@salesSOH",(int)request?.SalesSOH),
                new SqlParameter("@salesAMTRange", request?.salesAMTRange),
                new SqlParameter("@salesSOHRange", request?.salesSOHRange),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@promoSales", request?.IsPromoSale),
                new SqlParameter("@promoCode", request?.PromoCode),
                new SqlParameter("@promoIds", request?.PromotionIds),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@reportType", type),
            };
                var dataSet = GetReportData(StoredProcedures.GetItemsPurchaseSummaryReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetRanging(RangingRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (request.StoreIds == null || request.DepartmentIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Ranging", "outlet or department."));

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_RANGING_PRODUCT.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@stockNationalRange",request?.stockNationalRange),
                new SqlParameter("@salesAMT",(int)request?.SalesAMT),
                new SqlParameter("@salesAMTRange", request?.salesAMTRange),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds)
            };
                var dataSet = GetReportData(StoredProcedures.GetRangingProductReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetNationalRanging(NationalRangingRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (request.StoreIds == null || request.DepartmentIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Ranging", "outlet or department."));

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_NATIONAL_RANGING_PRODUCT.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@stockNationalRange",request?.stockNationalRange),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds)
            };
                var dataSet = GetReportData(StoredProcedures.GetNationalRangingProductReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetProductPriceDeviation(NationalRangingRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            if (request.DepartmentIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Ranging", "department."));
            try
            {
                //****Globle parameter getting from Report Global Setting: Ticket No - CC-1316
                int deviationPercCutOff = 1;
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_PRODUCT_PRICE_DEVIATION.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@stockNationalRange",request?.stockNationalRange),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds)
                };

                var dataSet = GetReportData(StoredProcedures.GetProductPriceDeviationReport, dbParams.ToArray());
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    int outletId = 0;
                    int outletCount = 0;
                    if (!string.IsNullOrEmpty(request.StoreIds))
                    {
                        string[] split = request.StoreIds.Split(',');
                        if (split.Length > 0)
                            outletId = Convert.ToInt32(split[0]);
                        else
                            outletId = Convert.ToInt32(request.StoreIds);

                        outletCount = split.Length;
                    }
                    else
                    {
                        outletCount = dataSet.Tables[2].Rows.Count;
                        outletId = Convert.ToInt32(dataSet.Tables[2].Rows[0]["Store"]);
                    }

                    var productPriceDeviationList = new List<ProductPriceDeviationResponseModel>();
                    var productPriceDeviation = MappingHelpers.ConvertDataTable<ProductPriceDeviationResponseModel>(dataSet.Tables[0]);
                    var groupProductList = productPriceDeviation.Select(x => x.Number).GroupBy(x => x);
                    foreach (var prod in groupProductList)
                    {
                        // double groupSelling = 0;
                        var productList = productPriceDeviation.Where(x => x.Number == prod.Key).OrderBy(x => x.Selling).ToList();
                        var outletIndex = productList?.FindIndex(x => x.OutletId == outletId) ?? -1;
                        if (outletIndex == -1)
                            continue;
                        var iMean = productList.Average(a => a.Selling);
                        //if (productList?.Count > 1)
                        //{
                        //    var productIndex = (productList.Count / 2) - 1;
                        //    groupSelling = Math.Round(productList[productIndex].Selling, 2);
                        //}
                        //else
                        //{
                        //    groupSelling = Math.Round(productList[0].Selling, 2);
                        //}
                        if (iMean != 0)
                        {
                            foreach (var obj in productList)
                            {
                                obj.GroupSelling = iMean;
                                if (iMean == 0)
                                    obj.Deviation = 0;
                                else
                                    obj.Deviation = (obj.Selling - obj.GroupSelling) / iMean * 100;
                                if (obj.Deviation < 0)
                                    obj.Deviation = obj.Deviation * -1;
                            }
                        }

                        int topCutoff = GetCutoff(productList, outletCount, true);
                        int bottomCutoff = GetCutoff(productList, outletCount, false);
                        if ((topCutoff > -1 && outletIndex <= topCutoff) || (bottomCutoff > -1 && outletIndex >= bottomCutoff))
                        {
                            var list = productList[outletIndex];
                            if (list.Selling != iMean && list.Deviation > deviationPercCutOff)
                                productPriceDeviationList.Add(list);
                        }
                    }
                    var result = productPriceDeviationList.OrderByDescending(x => x.Deviation).ThenBy(x => x.Description).ToList();
                    var dsProductPrice = result.ConvertToDataSet("DataSet");
                    dataSet.Tables.Remove("DataSet");
                    dataSet.Tables.Add(dsProductPrice.Tables["DataSet"].Copy());
                }
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetNationalLevelSalesSummary(NationalLevelRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (request.StoreIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "National Level", "outlet."));

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_NATIONAL_LEVEL_SALES_SUMMARY.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@stockNationalRange",request?.stockNationalRange),
                new SqlParameter("@storeIds", request?.StoreIds),
            };
                var dataSet = GetReportData(StoredProcedures.GetNationalLevelSalesSummaryReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetItemsWithHourlySales(ReportRequestModel request, string type, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            if (request.StoreIds == null && request.ZoneIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.JournalSalesReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Hourly Summary"));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_HOURLY_SALES.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@productStartId", request?.ProductStartId),
                new SqlParameter("@productEndId", request?.ProductEndId),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@cashierId", request?.CashierId),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@variance",request?.Variance),
                new SqlParameter("@wastage", request?.Wastage),
                new SqlParameter("@merge",request?.Merge),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@reportType", type)
            };

                var dataSet = GetReportData(StoredProcedures.GetItemsWithHourlySales, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetKPIRanking(KPIRankingRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_RANKING_BY_STORE_KPI.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@departmentIds", request?.DepartmentIds)
                };

                var dataSet = GetReportData(StoredProcedures.GetKPIRankingReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetSalesHistoryChart(SalesHistoryRequestModle request, string mime, string reportFor)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_SalesHistortChartReport.frx";
                if (request?.Chart == true)
                    reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_SalesHistortChartReport_Chart.frx";

                if (request?.StartDate != null && request?.EndDate != null)
                {
                    int startingDateYear = Convert.ToDateTime(request?.StartDate).Year;
                    int startingDateMonth = Convert.ToDateTime(request?.StartDate).Month;
                    int endingDateYear = Convert.ToDateTime(request?.EndDate).Year;
                    int endingDateMonth = Convert.ToDateTime(request?.EndDate).Month;
                    int noOfDaysInMonth = DateTime.DaysInMonth(startingDateYear, startingDateMonth);
                    int nofOfDaysInTwoDates = Convert.ToDateTime(request?.EndDate.ToShortDateString()).Subtract(Convert.ToDateTime(request?.StartDate.ToShortDateString()).AddDays(-1)).Days;
                    if (Convert.ToString(request?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Monthly).ToLower())
                    {
                        if (startingDateYear == endingDateYear && startingDateMonth == endingDateMonth)
                        {
                            if (nofOfDaysInTwoDates < noOfDaysInMonth)
                            {
                                return null;//throw new NotFoundException(ErrorMessages.ItemSalesDatesAreNotInMonthRange.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                    }
                    if (Convert.ToString(request?.PeriodicReportType).ToLower() == Convert.ToString(PeriodicReportType.Weekly).ToLower())
                    {
                        if (nofOfDaysInTwoDates < 7)
                        {
                            return null;//throw new NotFoundException(ErrorMessages.ItemSalesDatesAreNotInMonthRange.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                }

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@promoIds", request?.PromoIds),
                new SqlParameter("@tillIds", request?.TillIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@summary",request.Summary),
                new SqlParameter("@promoSales",request.IsPromoSale),
                new SqlParameter("@promoCode",request.PromoCode),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@PeriodicReportType", request?.PeriodicReportType),
                new SqlParameter("@IsChart", request?.Chart),
                };
                string commandText = null;
                switch ((ReportType)Enum.Parse(typeof(ReportType), reportFor))
                {
                    case ReportType.Department:
                        commandText = StoredProcedures.GetSalesHistoryChartByDepartment;
                        break;
                    case ReportType.Supplier:
                        commandText = StoredProcedures.GetSalesHistoryChartBySupplier;
                        break;
                    case ReportType.Commodity:
                        commandText = StoredProcedures.GetSalesHistoryChartByCommodity;
                        break;
                    case ReportType.Group:
                        commandText = StoredProcedures.GetSalesHistoryChartByGroup;
                        reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_SalesHistortChartReport_Group.frx";
                        if (request?.Chart == true)
                            reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_SalesHistortChartReport_Chart.frx";
                        break;
                    case ReportType.Outlet:
                        commandText = StoredProcedures.GetSalesHistoryChartByOutlet;
                        break;
                    case ReportType.Category:
                        commandText = StoredProcedures.GetSalesHistoryChartByCategory;
                        break;
                    default:
                        commandText = StoredProcedures.GetSalesHistoryChartByOutlet;
                        break;
                }
                var dataSet = GetReportData(commandText, dbParams.ToArray());
                if (dataSet.Tables.Count > 16)
                    dataSet.Tables[16].TableName = "ChartDataSet";
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetSalesNilTransaction(SalesNilTransaction request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_NIL_TRANS.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@NillTransactionInterval", request?.NilTransactionInterval),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@cashierId", request?.CashierId),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliersIds", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@memberIds", request?.MemberIds),
                new SqlParameter("@contineous",request?.Continuous),
                new SqlParameter("@drillDown", request?.DrillDown),
                new SqlParameter("@summary",request.Summary)
            };

                var dataSet = GetReportData(StoredProcedures.GetNILTransactionReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetFinancialSummary(FinancialSummaryRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (request.StoreIds == null && request.ZoneIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.JournalSalesReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Financial Summary"));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_SALES_FINANCIAL_SUMMARY.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request?.StartDate),
                new SqlParameter("@endDate", request?.EndDate),
                new SqlParameter("@storeIds", request?.StoreIds),
                new SqlParameter("@tillId", request?.TillId),
                new SqlParameter("@cashierId", request?.CashierId),
                new SqlParameter("@commodityIds", request?.CommodityIds),
                new SqlParameter("@departmentIds", request?.DepartmentIds),
                new SqlParameter("@categoryIds", request?.CategoryIds),
                new SqlParameter("@groupIds", request?.GroupIds),
                new SqlParameter("@suppliers", request?.SupplierIds),
                new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                new SqlParameter("@zoneIds", request?.ZoneIds),
                new SqlParameter("@dayRange", request?.DayRange),
                new SqlParameter("@memberIds", request?.MemberIds)
            };

                var dataSet = GetReportData(StoredProcedures.GetFinancialSummaryReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetTillJournal(TillJournalReportModel requestModel, string mime, SecurityViewModel securityViewModel = null)
        {
            if (requestModel == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            var AccessStores = String.Empty;
            if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
            {
                foreach (var storeId in securityViewModel.StoreIds)
                    AccessStores += storeId + ",";
            }

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_ShowTillJournalRange.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                 new SqlParameter("@DateFrom",requestModel?.StartDate.Date),
                 new SqlParameter("@DateTo",requestModel?.EndDate.Date),
                 new SqlParameter("@HourFrom",requestModel?.StartHour),
                 new SqlParameter("@HourTo",requestModel?.EndHour),
                 new SqlParameter("@TransactionType",requestModel?.TransactionType),
                 new SqlParameter("@ExceptionsOnly",requestModel?.ShowException),
                 new SqlParameter("@DocketNo",requestModel?.DocketNo),
                 new SqlParameter("@TillId",requestModel?.TillId),
                 new SqlParameter("@CashierId",requestModel?.CashierId),
                 new SqlParameter("@OutletIds",requestModel?.StoreIds),
                 new SqlParameter("@PromoSales",requestModel?.IsPromoSale),
                 new SqlParameter("@PromoId",requestModel?.PromoId),
                 new SqlParameter("@CommodityIds",requestModel?.CommodityIds),
                 new SqlParameter("@DepartmentIds",requestModel?.DepartmentIds),
                 new SqlParameter("@CategoryIds",requestModel?.CategoryIds),
                 new SqlParameter("@ManufacturerIds",requestModel?.ManufacturerIds),
                 new SqlParameter("@GroupIds",requestModel?.GroupIds),
                 new SqlParameter("@SupplierIds",requestModel?.SupplierIds),
                 new SqlParameter("@AccessOutletIds",AccessStores),
                 new SqlParameter("@JournalRange",requestModel?.JournalType.ToUpper(CultureInfo.CurrentCulture)),
                 new SqlParameter("@SkipCount",null),
                 new SqlParameter("@MaxResultCount",null),
                 new SqlParameter("@MemberIds",requestModel?.MemberIds),
                 new SqlParameter("@ProductStartId",requestModel?.ProductStartId),
                 new SqlParameter("@ProductEndId",requestModel?.ProductEndId),
                 new SqlParameter("@ZoneIds",requestModel?.ZoneIds),
                 new SqlParameter("@TenderTypeName",requestModel?.TenderTypeName),
                 new SqlParameter("@LineTypeName",requestModel?.LineTypeName)
            };
                var dataSet = GetReportData(StoredProcedures.GetTillJournalReport, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", requestModel.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetBasketIncidentReport(BasketIncidentFilters requestModel, string mime, SecurityViewModel securityViewModel = null)
        {
            if (requestModel == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            var AccessStores = String.Empty;
            if (requestModel.StoreIds == null && requestModel.ZoneIds == null)
                throw new NullReferenceException(String.Format(ErrorMessages.JournalSalesReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Incidents Report"));
            if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
            {
                foreach (var storeId in securityViewModel.StoreIds)
                    AccessStores += storeId + ",";
            }

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_Incident.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                 new SqlParameter("@StartDate",requestModel?.StartDate),
                 new SqlParameter("@EndDate",requestModel?.EndDate),
                 new SqlParameter("@StoreIds",requestModel?.StoreIds),
                 new SqlParameter("@ReplicateCode",requestModel?.ReplicateCode),
                 new SqlParameter("@DepartmentIds",requestModel?.DepartmentIds),
                 new SqlParameter("@CommodityIds",requestModel?.CommodityIds),
                 new SqlParameter("@SupplierIds",requestModel?.SupplierIds),
                 new SqlParameter("@ManufacturerIds",requestModel?.ManufacturerIds),
                 new SqlParameter("@ProductStart",requestModel?.ProductStartId),
                 new SqlParameter("@ProductEnd",requestModel?.ProductEndId),
                 new SqlParameter("@IsPromoSale",requestModel?.IsPromoSale),
                 new SqlParameter("@PromoCode",requestModel?.PromoCode),
                 new SqlParameter("@CategoryIds",requestModel?.CategoryIds),
                 new SqlParameter("@GroupIds",requestModel?.GroupIds),
                 new SqlParameter("@TillId",requestModel?.TillId),
                 new SqlParameter("@CashierId",requestModel?.CashierId),
                 new SqlParameter("@zoneIds",requestModel?.ZoneIds),
                 new SqlParameter("@MemberIds",requestModel?.MemberIds),
                 new SqlParameter("@dayRange",requestModel?.DayRange)
            };
                var dataSet = GetReportData(StoredProcedures.GetBasketIncident, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", requestModel.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetStockTakePrint(StockTakePrintRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (request.OutLetId == 0)
                throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_STOCK_TAKE_PRINT.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@outletId", request.OutLetId),
                        new SqlParameter("@productStartId", request?.ProductStartId),
                        new SqlParameter("@productEndId", request?.ProductEndId),
                        new SqlParameter("@departmentIds", request?.DepartmentIds),
                        new SqlParameter("@commodityIds", request?.CommodityIds),
                        new SqlParameter("@categoryIds", request?.CategoryIds),
                        new SqlParameter("@groupIds", request?.GroupIds),
                        new SqlParameter("@suppliersIds", request?.SupplierIds),
                        new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                        new SqlParameter("@productIds", request?.ProductIds),
                        new SqlParameter("@var", request?.WithVar),
                        new SqlParameter("@Print", "PRINT")
                    };
                var dataSet = GetReportData(StoredProcedures.GetStockTakeLoadProductRange, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] GetStoreDashboard(StoreDashboardRequest request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_StoreDashBoard.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@StoreId", request.OutletId),
                new SqlParameter("@ThisYearDateFrom", request.DateRangeFrom),
                new SqlParameter("@ThisYearDateTo", request.DateRangeTo),
                new SqlParameter("@LastYearDateFrom", request.CompareRangeFrom),
                new SqlParameter("@LastYearDateTo", request.CompareRangeTo),
                new SqlParameter("@ZoneId", request.ZoneId),
                new SqlParameter("@DepDateFrom", request.DepartmentDateFrom),
                new SqlParameter("@DepDateTo", request.DepartmentDateTo),
                new SqlParameter("@LineChart", request?.LineChart),
                new SqlParameter("@BarChart", request?.BarChart)
            };
                var dataSet = GetReportData(StoredProcedures.GetStoreDashboardSales, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public string GetReporterStoreKPI(ReporterStoreKPIReport request)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
            //if (request.ModuleName == null)
            //    throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired, "Store KPI", "ModuleName"));
            try
            {
                string moduleName = request.ModuleName;
                //if (request.ModuleName == "DEPARTMENTS")
                //{
                //    moduleName = null;
                //}
                List <SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@startDate", request.StartDate),
                new SqlParameter("@endDate", request.EndDate),
                new SqlParameter("@OutletId", request.StoreIds),
                new SqlParameter("@DepartmentIds", request.DepartmentIds),
                new SqlParameter("@ZoneIds", request.ZoneIds),
                new SqlParameter("@DaysOfWeek", request.DayRange),
                new SqlParameter("@AutoLinkedOutlets", request.AutoLinkedOutlets),
                new SqlParameter("@AutoLinkedCommodities", request.AutoLinkedCommodities),
                new SqlParameter("@Module", moduleName)
            };
                var dset = GetReportData(StoredProcedures.GetReporterStoreKPI, dbParams.ToArray());
                if (dset.Tables != null && dset.Tables.Count > 0 && dset.Tables[0] != null && dset.Tables[0].Rows != null && dset.Tables[0].Rows.Count > 0)
                {
                    DataTable dtStoreKPITable = new DataTable();
                    dtStoreKPITable = dset.Tables[0].Copy();
                    dtStoreKPITable.Clear();

                    if (request.ModuleName == "SALES" || request.ModuleName == "DEPARTMENTS" || request.ModuleName == "SP-CUSTOMER")
                    {
                        foreach (DataRow item in dset.Tables[0]?.Rows)
                        {
                            dtStoreKPITable.Rows.Add(item.ItemArray);
                            string expression = "OUTL_OUTLET = " + item["OUTL_OUTLET"];
                            DataRow[] selectedRows = dset?.Tables[3]?.Select(expression);
                            DataTable dtStoreKPIYTDTable = new DataTable();
                            dtStoreKPIYTDTable = dset.Tables[3].Copy();
                            dtStoreKPIYTDTable.Clear();
                            dtStoreKPIYTDTable.Rows.Add(selectedRows[0]?.ItemArray);
                            if (selectedRows != null)
                            {
                                foreach (DataRow row in dtStoreKPIYTDTable.Rows)
                                {
                                    row["OUTL_OUTLET"] = "";
                                    row["OUTL_DESC"] = "YTD";
                                }
                                dtStoreKPITable.Rows.Add(dtStoreKPIYTDTable.Rows[0].ItemArray);
                            }
                        }
                        if (request.ModuleName == "SALES" || request.ModuleName == "SP-CUSTOMER")
                        {

                            DataRow selectedTYRows = dset.Tables[1].Rows[0];
                            if (selectedTYRows != null)
                                dtStoreKPITable.Rows.Add(selectedTYRows.ItemArray);
                            DataRow selectedYTDRows = dset.Tables[4].Rows[0];
                            if (selectedYTDRows != null)
                            {
                                dtStoreKPITable.Rows.Add(selectedYTDRows.ItemArray);
                            }

                            DataRow selectedAvgTYRows = dset.Tables[2].Rows[0];
                            if (selectedAvgTYRows != null)
                                dtStoreKPITable.Rows.Add(selectedAvgTYRows.ItemArray);
                            DataRow selectedAvgYTDRows = dset.Tables[5].Rows[0];
                            if (selectedAvgYTDRows != null)
                                dtStoreKPITable.Rows.Add(selectedAvgYTDRows.ItemArray);
                        }
                        string jsonString = MappingHelpers.DataTableToJsonObj(dtStoreKPITable, "StoreKPI");
                        string departmentJsonstring = null;
                        string dateRangeString = null;
                        if (request.ModuleName == "DEPARTMENTS")
                        {
                            departmentJsonstring = MappingHelpers.DataTableToJsonObj(dset.Tables[4], "Departments");
                            dateRangeString = MappingHelpers.DataTableToJsonObj(dset.Tables[5], "DateRangeString");
                        }
                        else
                        {
                            departmentJsonstring = MappingHelpers.DataTableToJsonObj(dset.Tables[6], "Departments");
                            dateRangeString = MappingHelpers.DataTableToJsonObj(dset.Tables[7], "DateRangeString");
                        }
                        jsonString = jsonString.Remove(jsonString.Length - 1, 1) + "," + departmentJsonstring.Substring(1, departmentJsonstring.Length - 2) + "," + dateRangeString.Substring(1);


                        var json_Obj = JsonConvert.DeserializeObject(jsonString);
                        jsonString = jsonString + departmentJsonstring + dateRangeString;

                        return JsonConvert.SerializeObject(json_Obj);

                    }
                    else if (request.ModuleName == "SALES-SUMMARY" || request.ModuleName == "SS-YID" || request.ModuleName == "SPC-SUMMARY" || request.ModuleName == "SPCS-YID")
                    {
                        string departmentJsonstring = MappingHelpers.DataTableToJsonObj(dset.Tables[1], "Departments");
                        string jsonString = MappingHelpers.DataTableToJsonObj(dset.Tables[0], "StoreKPI");
                        string dateRangeString = MappingHelpers.DataTableToJsonObj(dset.Tables[2], "DateRangeString"); ;
                        jsonString = jsonString.Remove(jsonString.Length - 1, 1) + "," + departmentJsonstring.Substring(1, departmentJsonstring.Length - 2) + "," + dateRangeString.Substring(1);


                        var json_Obj = JsonConvert.DeserializeObject(jsonString);
                        jsonString = jsonString + departmentJsonstring;
                        return Convert.ToString(json_Obj);
                    }
                    else if (request.ModuleName == "SC-OUTLET" || request.ModuleName == "SC-DEPARTMENT")
                    {
                        string jsonString = MappingHelpers.DataTableToJsonObj(dset.Tables[0], "StoreKPIChartData");
                        string dateRangeString = MappingHelpers.DataTableToJsonObj(dset.Tables[1], "DateRangeString"); ;

                        var json_Obj = JsonConvert.DeserializeObject(jsonString);
                        return Convert.ToString(json_Obj);
                    }
                    else
                        return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        public AutomaticOrderResponseModel AutomaticOrder(AutomaticOrderRequestModel viewModel, string mime, int userId)
        {
            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_AUTO_ORDER.frx";

                List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@StoreId", viewModel?.StoreId),
                    new SqlParameter("@SupplierId", viewModel?.SupplierId),
                    new SqlParameter("@AltDirectSupplierId", viewModel?.AltSupplierId),
                    new SqlParameter("@OrderType", viewModel?.OrderType),
                    new SqlParameter("@IgnoreStockLevel", viewModel?.IngnoreStockLevel),
                    new SqlParameter("@HistoryDays", viewModel?.DaysHistory),
                    new SqlParameter("@ExcludePromo", viewModel?.ExcludePromo),
                    new SqlParameter("@CoverDays", viewModel?.CoverDays),
                    new SqlParameter("@DiscountThreshold", viewModel?.DiscountThreshold),
                    new SqlParameter("@OrderNumber", viewModel?.ExistingOrderNo),
                    new SqlParameter("@NewOrderNumber", GetNewOrderNumber(viewModel.StoreId)),
                    new SqlParameter("@Normal", viewModel?.MetcashNormal),
                    new SqlParameter("@variety", viewModel?.MetcashVariety),
                    new SqlParameter("@SlowMoving", viewModel?.MetcashSlow),
                    new SqlParameter("@CompareDirectSupplier", viewModel?.CompareDirectSuppliers),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@InvestmentBuyDays", viewModel?.InvestmentBuyDays),
                    new SqlParameter("@DepartmentIds", viewModel?.DepartmentIds),
                    new SqlParameter("@ProductId",viewModel?.ProductId)
                };

                var autoOrderResponse = new AutomaticOrderResponseModel();

                //if (dataSet.Tables.Count > 0)
                //{
                //    dataSet.Tables[0].TableName = "HeaderDataSet";
                //}
                //if (dataSet.Tables.Count > 1)
                //{
                //    dataSet.Tables[1].TableName = "DetailDataSet";
                //
                //
                //}


#pragma warning disable CA2000 // Dispose objects before losing scope
                var dataSet = GetReportData(StoredProcedures.AutomaticOrder, dbParams.ToArray());
#pragma warning restore CA2000 // Dispose objects before losing scope
                var orderArray = GetByteArrayOfReport(reportPath, dataSet, "Data", "pdf", mime);

                autoOrderResponse.SuggestedOrder = orderArray;

                if (dataSet.Tables.Count > 2)
                {
                    List<InvestmentBuyProducts> IncludeProd = MappingHelpers.ConvertDataTable<InvestmentBuyProducts>(dataSet.Tables[2]);
                    autoOrderResponse.IncludedProducts.AddRange(IncludeProd);
                }
                if (dataSet.Tables.Count > 3)
                {
                    List<InvestmentBuyProducts> ExcludeProd = MappingHelpers.ConvertDataTable<InvestmentBuyProducts>(dataSet.Tables[3]);
                    autoOrderResponse.IncludedProducts.AddRange(ExcludeProd);
                }

                return autoOrderResponse;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] WeeklySalesWorkBook(WeeklySalesWorkBookRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            //if (request.StoreIds == null)
            //    throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_SALESWORKBOOK.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@StoreIds", string.IsNullOrEmpty(request?.StoreIds) ?null: request?.StoreIds ),
                        new SqlParameter("@TyDateFrom", request?.DateFrom),
                        new SqlParameter("@TyDateTo",request?.DateTo),
                        new SqlParameter("@LyDateFrom",request?.DateFrom.AddDays(-364)),
                        new SqlParameter("@LyDateTo",request?.DateTo.AddDays(-364))
                    };
                var dataSet = GetReportData(StoredProcedures.GetWeeklySalesWorkbook, dbParams.ToArray());

                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public byte[] StockAdjustmentPrint(StockAdjustPrintRequestModel request, string mime)
        {
            if (request == null)
                throw new NullReferenceException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));

            if (request.StoreId == null)
                throw new NullReferenceException(String.Format(ErrorMessages.ReportCompulsorySelectionsRequired.ToString(CultureInfo.CurrentCulture), "Stock Adjustment Print", "outlet."));

            try
            {
                string reportPath = _configuration["FastReportSettings:ReportPath"] + "RPT_StockAdjustmentPrint.frx";
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@outletId", request?.StoreId),
            };
                var dataSet = GetReportData(StoredProcedures.GetStockAdjustmentPrint, dbParams.ToArray());
                return GetByteArrayOfReport(reportPath, dataSet, "Data", request.Format, mime);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        #region private function
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public DataSet GetReportData(string commandText, SqlParameter[] sqlParameters)
        {
            try
            {
                DataSet dataSet = new DataSet();
                using (SqlConnection sqlConn = new SqlConnection(_configuration["ConnectionStrings:DBConnection"]))
                {
                    using (SqlCommand sqlCmd = new SqlCommand(commandText, sqlConn))
                    {
                        sqlCmd.CommandTimeout = 380;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddRange(sqlParameters);
                        //sqlConn.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dataSet);
                            sqlCmd.Parameters.Clear();
                            sqlCmd.Dispose();
                            if (dataSet.Tables.Count <= 0)
                            {
                                throw new NotFoundException(ErrorMessages.NoDataFound.ToString(CultureInfo.CurrentCulture));
                            }
                            dataSet.Tables[0].TableName = "DataSet";
                            //to avoid error on others
                            if (dataSet.Tables.Count > 1)
                                dataSet.Tables[1].TableName = "FilterDataSet";
                            if (dataSet.Tables.Count > 2)
                                dataSet.Tables[2].TableName = "StoreDataSet";
                            if (dataSet.Tables.Count > 3)
                                dataSet.Tables[3].TableName = "DeptDataSet";
                            if (dataSet.Tables.Count > 4)
                                dataSet.Tables[4].TableName = "CommodityDataSet";
                            if (dataSet.Tables.Count > 5)
                                dataSet.Tables[5].TableName = "CategoryDataSet";
                            if (dataSet.Tables.Count > 6)
                                dataSet.Tables[6].TableName = "GroupsDataSet";
                            if (dataSet.Tables.Count > 7)
                                dataSet.Tables[7].TableName = "SupplierDataSet";
                            if (dataSet.Tables.Count > 8)
                                dataSet.Tables[8].TableName = "ManufacturerDataSet";
                            if (dataSet.Tables.Count > 9)
                                dataSet.Tables[9].TableName = "CashierDataSet";
                            if (dataSet.Tables.Count > 10)
                                dataSet.Tables[10].TableName = "ZoneDataSet";
                            if (dataSet.Tables.Count > 11)
                                dataSet.Tables[11].TableName = "DaysDataSet";
                            if (dataSet.Tables.Count > 12)
                                dataSet.Tables[12].TableName = "PromotionDataSet";
                            if (dataSet.Tables.Count > 13)
                                dataSet.Tables[13].TableName = "NationalRangeDataSet";
                            if (dataSet.Tables.Count > 14)
                                dataSet.Tables[14].TableName = "MemberDataSet";
                            if (dataSet.Tables.Count > 15)
                                dataSet.Tables[15].TableName = "TillDataSet";
                            if (dataSet.Tables.Count > 16)
                                dataSet.Tables[16].TableName = "VarianceDataSet";
                            if (dataSet.Tables.Count > 17)
                                dataSet.Tables[17].TableName = "WastageDataSet";
                            if (dataSet.Tables.Count > 18)
                                dataSet.Tables[18].TableName = "TransactionInterval";

                            return dataSet;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message, ex);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message, ex);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message, ex);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }


        private static byte[] GetByteArrayOfReport(string reportPath, DataSet dataSet, string dataSetName, string format, string mimeType)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    // Turn on web mode FastReport
                    Config.WebMode = true;
                    using (Report report = new Report())
                    {
                        report.Load(reportPath);
                        report.RegisterData(dataSet, dataSetName);
                        report.Prepare();

                        // If pdf format is selected
                        if (format.ToLower() == "pdf")
                        {
                            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                            // Use the stream to store the report, so as not to create unnecessary files
                            report.Export(pdf, stream);
                            pdf.Dispose();
                        }
                        // If html report format is selected
                        else if (format.ToLower() == "html")
                        {
                            HTMLExport html = new HTMLExport
                            {
                                SinglePage = true,
                                Navigator = false,
                                EmbedPictures = true
                            };
                            report.Export(html, stream);
                            mimeType = "text/" + format; // Override mime for html
                            html.Dispose();
                        }

                    }
                    return stream.ToArray();
                }
                // Handle exceptions
                catch (Exception ex)
                {
                    throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
                }
                finally
                {
                    stream.Dispose();
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        private async Task<DataSet> BindReportDropdownListItems(string commandText, SqlParameter[] sqlParameters)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(_configuration["ConnectionStrings:DBConnection"]))
            {
                await sqlConn.OpenAsync().ConfigureAwait(false);
                using (SqlCommand sqlCmd = new SqlCommand(commandText, sqlConn))
                {
                    sqlCmd.CommandTimeout = 180;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddRange(sqlParameters);
                    //sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(dataSet);
                        dataSet.Tables[0].TableName = "Category";
                        //to avoid error on others
                        if (dataSet.Tables.Count > 1)
                            dataSet.Tables[1].TableName = "Group";
                        if (dataSet.Tables.Count > 2)
                            dataSet.Tables[2].TableName = "Manufacturer";
                        if (dataSet.Tables.Count > 3)
                            dataSet.Tables[3].TableName = "Zone";
                        if (dataSet.Tables.Count > 4)
                            dataSet.Tables[4].TableName = "Commodity";
                        if (dataSet.Tables.Count > 5)
                            dataSet.Tables[5].TableName = "Department";
                        if (dataSet.Tables.Count > 6)
                            dataSet.Tables[6].TableName = "Supplier";
                        if (dataSet.Tables.Count > 7)
                            dataSet.Tables[7].TableName = "Store";
                        if (dataSet.Tables.Count > 8)
                            dataSet.Tables[8].TableName = "Till";
                        if (dataSet.Tables.Count > 9)
                            dataSet.Tables[9].TableName = "Cashier";
                        return dataSet;
                    }
                }
            }
        }
        private static int GetCutoff(List<ProductPriceDeviationResponseModel> productList, int outletCount, bool pTop)
        {
            int retrunValue = -1;
            double prevValue = 0;
            int numDeviations = 0;
            //****Globle parameter getting from Report Global Setting: Ticket No - CC-1316
            int deviationOutletCount = 2;
            int deviationUpperLowerPerc = 2;

            if (outletCount <= 1)
                return -1;

            if (outletCount < deviationOutletCount)
            {
                if (pTop)
                    return 0;
                else
                    return outletCount - 1;
            }

            int cutoffCount = outletCount * deviationUpperLowerPerc / 100;
            if (cutoffCount <= 0)
                cutoffCount = 1;

            if (pTop)
            {
                for (int i = 0; i <= productList.Count; i++)
                {
                    if (productList[i].Selling != prevValue)
                    {
                        numDeviations++;
                        prevValue = productList[i].Selling;
                        if (numDeviations == cutoffCount)
                        {
                            retrunValue = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                int count = productList.Count - 1;
                for (int i = count; i >= 0; i--)
                {
                    if (productList[i].Selling != prevValue)
                    {
                        numDeviations++;
                        prevValue = productList[i].Selling;
                        if (numDeviations == cutoffCount)
                        {
                            retrunValue = i;
                            break;
                        }
                    }
                }
            }

            return retrunValue;
        }

        public long GetNewOrderNumber(int storeId)
        {
            long newOrderNo = 0;
            string newOrder = "";
            try
            {
                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@storeId", storeId)
            };
#pragma warning disable CA2000 // Dispose objects before losing scope
                var dataset = GetReportData(StoredProcedures.GetNewOrderNumber, dbParams.ToArray());
#pragma warning restore CA2000 // Dispose objects before losing scope

                var list = MappingHelpers.ConvertDataTable<NewOrderNumber>(dataset.Tables[0]);

                long startNo = Convert.ToInt64(storeId.ToString() + "1");

                if (list == null || list.Count <= 0)
                {
                    return startNo;
                }

                long[] orderNo = new long[list.Count];

                int i = 0;
                foreach (var no in list)
                {
                    var index = no.OrderNo.ToString().ElementAt(0);

                    if (index == storeId.ToString().ElementAt(0))
                    {
                        orderNo[i] = Convert.ToInt64(no.OrderNo.ToString().Substring(storeId.ToString().Length));
                        i++;
                    }
                }

                long missing = findFirstMissing(orderNo, 0, orderNo.Length - 1);

                //Check if missing

                long findFirstMissing(long[] array, int start, int end)
                {
                    if (start >= end + 1)
                        return end + 2;

                    if (start + 1 != array[start])
                        return start + 1;

                    int mid = (start + end) / 2;

                    // Left half has all elements
                    // from 0 to mid
                    if (array[mid] == mid + 1)
                        return findFirstMissing(array, mid + 1, end);

                    return findFirstMissing(array, start, mid);
                }

                if (missing != 0)
                {
                    newOrder = storeId.ToString() + missing.ToString();
                }
                else
                {
                    newOrder = storeId.ToString() + (orderNo.Max() + 1).ToString();
                }

                newOrderNo = Convert.ToInt64(newOrder);

                try
                {
                    var orderdataset = new DataSet();
                    var count = 0;
                    do
                    {
                        List<SqlParameter> dbParams_2 = new List<SqlParameter>{
                          new SqlParameter("@storeId", storeId),
                          new SqlParameter("@orderNo", newOrderNo)
                        };

                        orderdataset = GetReportData(StoredProcedures.GetNewOrderNumber, dbParams_2.ToArray());
                        count = Convert.ToInt32(orderdataset.Tables[1].Rows[0]["TotalRecordCount"]);

                        if (count > 0)
                        {
                            missing = missing + 1;
                            newOrder = storeId.ToString() + missing.ToString();
                            newOrderNo = Convert.ToInt64(newOrder);
                        }
                    }
                    while (count > 0);
                }
                catch (Exception ex)
                {
                    throw new ArithmeticException(ex.Message);
                }

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            return newOrderNo;
        }



        #endregion
    }
}
