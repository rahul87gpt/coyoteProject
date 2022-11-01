﻿namespace Coyote.Console.Common
{
    public static class StoredProcedures
    {
        public const string GetActiveProducts = "Usp_GetAllActiveProducts";
        public const string GetActiveProductsList = "Usp_GetAllActiveProducts_list";
        public const string GetActiveStores = "USP_GetAllActiveStores";
        public const string GetActiveOutletProducts = "Usp_GetAllActiveOutletProducts";
        public const string GetActiveOutletProductsByProdId = "Usp_GetAllActiveOutletProductsByProductId";
        public const string GetActiveSupplier = "Usp_Get_AllActiveSupplier";
        public const string GetProductById = "Usp_GetProductById";
        public const string GetNewProductNumber = "usp_GetProductNumber";
        public const string AutomaticOrder = "[dbo].[AutomaticOrder]";
        public const string KPIStoreReport = "[dbo].[KPI_StoreReport]";
        public const string GetSalesChartReport = "USP_GetSalesChartReport";
        public const string GetSalesChartReportByID = "USP_GetSalesChartReportByID";
        public const string GetSalesChartDetailedReport = "USP_GetSalesChartDetailedReport";
        public const string GetReportFilteationDropDownListItems = "USP_ReportFilteration";
        public const string GetItemsSalesReport = "[dbo].[GetItemsSalesReport]";
        public const string GetItemsSalesReportNew = "[dbo].[GetItemsSalesReport_New]";
        public const string GetStockPurchaseReport = "Usp_GetStockPurchaseReport";
        public const string GetStockVarianceReport = "USP_GetStockVarianceReport";
        public const string GetStockOnHandReport = "Usp_GetStockOnHandReport";
        public const string GetStockWastageProductWise = "[dbo].[Usp_GetStockWastageReportProductWise]";
        public const string GetStockAdjustmentReport = "[dbo].[Usp_GetStockAdjustmentReport]";
        public const string GetCostVarienceReport = "[dbo].[Usp_GetCostVarienceReport]";
        public const string GetPrintChangeLabels = "[dbo].[RPT_GetPrintChangeLabels]";
        public const string AddUpdateProduct = "USP_InsertUpdate_Product";
        public const string GetActiveMasterListItems = "Usp_GetListItemByCode";
        public const string GetActiveCommodities = "usp_GetActiveCommodities";
        public const string GetJournalSalesFinancialSummaryReport = "[dbo].[GetJournalSalesFinancialSummaryReport]";
        public const string GetJournalSalesRoyaltyAndAdvertisingSummaryReport = "[dbo].[GetJournalSalesRoyaltyAndAdvertisingSummaryReport]";
        public const string GetSalesAndStockTrxSheet = "[dbo].[GetSalesAndStockTrxSheet]";
        public const string GetItemsSalesSummaryReport = "[dbo].[GetItemsSalesSummaryReport]";
        public const string GetRankingByOutlet = "[dbo].[GetRankingByOutlet]";
        public const string GetItemsPurchaseSummaryReport = "[dbo].[GetItemsPurchaseSummaryReport]";
        public const string GetRangingProductReport = "[dbo].[GetRangingProductReport]";
        public const string GetNationalRangingProductReport = "[dbo].[GetNationalRangingProductReport]";
        public const string GetProductPriceDeviationReport = "[dbo].[GetProductPriceDeviationReport]";
        public const string GetTillJournal = "[dbo].[Get_TillJournal]";
        public const string GetWeeklySalesWorkbook = "[dbo].[Get_WeeklySalesWorkbook]";
        public const string GetUserLog = "USP_GetUserLog";
        public const string GetPurchaseHistory = "USP_ProductTabsHistory_Purchase";
        public const string GetWeeklySales = "USP_ProductTabsHistory_WeeklySales";
        public const string GetTransactionHistory = "USP_ProductTabsHistory_Transaction";
        public const string GetChilderns = "USP_ProductTabsHistory_Childern";
        public const string GetStockMovement = "USP_ProductTabsHistory_StockMovement";
        public const string GetZonePricing = "USP_ProductTabsHistory_ZonePricing";
        public const string GetActiveAPN = "USP_GetAPN";
        public const string GetActivePromotionBatches = "USP_GetPromotionBatches";
        public const string GetActiveCompetitionDetail = "USP_GetCompetitionDetail";
        public const string GetPrintLabelFromTablet = "[dbo].[CDN_RPT_GetPrintLabelsFromTablet]";  // Stored Procedure in DB99
        public const string GetItemsWithHourlySales = "[dbo].[USP_GetItemsWithHourlySales]";
        public const string GetStockAdjustHeader = "[dbo].[USP_GetStockAdjustHeader]";
        public const string GetNationalLevelSalesSummaryReport = "[dbo].[GetNationalLevelSalesSummaryReport]";
        public const string GetPrintLabelsFromTablet = "[dbo].[GetPrintLabelsFromTablet]";
        public const string GetPrintLabelsFromTabletPDE = "[dbo].[GetPrintLabelsFromTabletPDE]";
        public const string GetPDELoadHistory = "[dbo].[USp_GetPDEHistory]";
        public const string GetAllRecipe = "[dbo].[USP_GetAllRecipes]";
        public const string AddUpdateRecipe = "[dbo].[USP_Recipe_AddUpdate]";
        public const string GetStockTakeHeader = "[dbo].[USP_GetStockTakeHeader]";
        public const string GetKPIRankingReport = "[dbo].[GetKPIRankingReport]";
        public const string GetPosMessages = "[dbo].[USP_GetPosMessages]";
        public const string GetPosMessagesByID = "[dbo].[USP_GetPosMessageByID]";
        public const string GetUserActivity = "[dbo].[USP_GetUserActivity]";
        public const string DeactivateProductList = "[dbo].[USP_DeactivateProductList]";
        public const string GetExternalStockTake = "[dbo].[Get_ExternalStockTake]";
        public const string GetSalesHistoryChart = "[dbo].[USP_SalesHistory_Commodity]";
        public const string GetAllCashier = "[dbo].[USP_GetALLCashier]";
        public const string ADDUpdateCashier = "[dbo].[USP_Cashier_AddUpdate]";
        public const string GetAllOrderHeaders = "[dbo].[Get_AllOrder_InvoiceHistory]";
        public const string GetSalesHistoryChartByCommodity = "[dbo].[USP_SalesHistory_Commodity]";
        public const string GetSalesHistoryChartByDepartment = "[dbo].[USP_SalesHistory_Department]";
        public const string GetSalesHistoryChartByOutlet = "[dbo].[USP_SalesHistory_Outlet]";
        public const string GetSalesHistoryChartBySupplier = "[dbo].[USP_SalesHistory_Supplier]";
        public const string GetSalesHistoryChartByCategory = "[dbo].[USP_SalesHistory_Category]";
        public const string GetSalesHistoryChartByGroup = "[dbo].[USP_SalesHistory_Group]";
        public const string GetNILTransactionReport = "[dbo].[USP_NILTransaction_Report]";
        public const string GetALLActiveMembers = "USP_GetALLActiveMembers";
        public const string MassPriceUpdate = "[dbo].[MassPriceUpdate]";
        public const string GetAllRebate = "[dbo].[USP_GetAllRebateHeaders]";
        public const string GetAllEPAY = "[dbo].[USP_GetALLEPAY]";
        public const string GetStockTakeTabletLoad = "[dbo].[USP_GetStockTakeTabletLoad]";
        public const string GetFinancialSummaryReport = "[dbo].[USP_GetFinancialSummaryReport]";
        public const string GetStockTakeLoadProductRange = "[dbo].[Usp_GetStockTake_LoadProductRange]";
        public const string GetDataToExport = "[dbo].[GetExportDataSet]";
        public const string GetStoreDashboardSales = "[dbo].[StoreDashboardSales]";
        public const string GetAllTill = "[dbo].[USP_GetALLActiveTill]";
        public const string ImportStoreDataToTable = "[dbo].[ImportStoreTable]";
        public const string ImportStoreGroupDataToTable = "[dbo].[ImportStoreGroupTable]";
        public const string ImportSupplierToTable = "[dbo].[ImportSupplierTable]";
        public const string ImportSupplierProductToTable = "[dbo].[ImportSupplierProductTable]";
        public const string ImportTilltToTable = "[dbo].[ImportTillTable]";
        public const string ImportAPNtToTable = "[dbo].[ImportAPNTable]";
        public const string ImportOutletProductToTable = "[dbo].[ImportOutletProductTable]";
        public const string ImportUserToTable = "[dbo].[ImportUserTable]";
        public const string ImportMasterListItemsToTable = "[dbo].[ImportMasterListItemsTable]";
        public const string ImportProductToTable = "[dbo].[ImportProductTable]";
        public const string ImportCashierToTable = "[dbo].[ImportCashierTable]";
        public const string HostProcessingAddUpdateProduct = "[dbo].[USP_HostProcessing]";
        public const string GetAllReportScheduler = "[dbo].[USP_GetReportScheduler]";
        public const string GetUserByAccessStore = "[dbo].[GetUserByAccessStore]";
        public const string GetAllSupplierProducts = "[dbo].[USP_GetSupplierProducts]";
        public const string GetHOSTUPD = "[dbo].[USP_GetHOSTUPD]";
		public const string GetReporterStoreKPI = "[dbo].[USP_Reporter_StoreKPI]";
        public const string GetAllDepartments = "[dbo].[usp_GetActiveDepartments]";
        public const string GetAllReplicateProduct = "[dbo].[USP_GETAllReplicateProduct]";
        public const string GetTillJournalReport = "[dbo].[Get_TillJournalReport]";
        public const string GetReprintChangedLabel = "[dbo].[USP_GetReprintChangedLabels]";
        public const string GetSpecLabel = "[dbo].[USP_GetSpecPrintLabels]";
        public const string GetActiveOutletSupplier = "[dbo].[USP_GetAllOutletSuppliers]";
        public const string GetStoreById = "[dbo].[USP_GetStoreById]";
        public const string GetSchedulerIds = "[dbo].[USP_GetSchedulerIds]";
        public const string GetSchedulerEmails = "[dbo].[USP_GetUserEmails]";
        public const string GetBasketIncident = "[dbo].[Usp_GetBasketIncidentReport]";
        public const string GetPromoProductToInsert = "[dbo].[Usp_GetPromoProductImport]";
        public const string GetNewOrderNumber = "[dbo].[Usp_GetNewOrderNumber]";
        public const string GetAllZoneOutlet = "[dbo].[USP_GetALLZoneOutlet]";
        public const string GetActiveMasterListZoneOutlet = "[dbo].[Usp_GetListItemZoneOutlet]";
        public const string GetReportSchedulerById = "[dbo].[USP_GetReportSchedulerById]";
        public const string AddKeypadButtons = "[dbo].[USP_AddKeypadButtons]";
        public const string GetActiveStoresDig = "[dbo].[DigSin_GetActiveStores]";
        public const string TestScheduler = "[dbo].[SchedulerTesting]";
        public const string GetPrintChangedLabels = "[dbo].[USP_GetPrintChangedLabels]";
        public const string GetOrderNumberNew = "[dbo].[Get_NewOrderNumber]";
        public const string KPIStoreReportSalesHistory = "[dbo].[USP_KPIStoreReport_SalesHistory]";
        public const string GetAutoOrderReport = "[dbo].[usp_getAutoOrderReport]";
        public const string GetNormalOrderReport = "[dbo].[usp_getNormalOrderReport]";
        public const string CDNOptimalOrderCalc = "[dbo].[CDN_SP__DoOptimalOrder]";
        public const string GetStockAdjustmentPrint = "[dbo].[GetStockAdjustmentPrint]";
        public const string GetPOSMessageId = "[dbo].[USP_GetPOSMessageId]";
        public const string SaveEmails = "[dbo].[USP_Save_SendEmails]";
        public const string GetOutletProductByProductId = "[dbo].[USP_Get_OutletProductByProductId]";
        public const string DeleteOutletProductByProductId = "[dbo].[USP_Delete_OutletProductByProductId]";
        public const string AddUpdateStockTakeHeader = "[dbo].[usp_InsertUpdateStockTakeEntry]";
        public const string RefreshStockTakeDetails = "[dbo].[usp_RefreshStockTakeDetails]";
        public const string GetStockTakeById = "[dbo].[usp_GetStockTakeById]";
        public const string GetKeypadDetailsById = "[dbo].[USP_GetKeypadDetailsById]";
        public const string GetHostUpdChangeByHostUpdId = "[dbo].[USP_GetHostUpdChangeByHostUpdId]";
    }
}






