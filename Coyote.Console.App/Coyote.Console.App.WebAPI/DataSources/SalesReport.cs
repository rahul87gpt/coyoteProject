using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.ViewModels.RequestModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.WebAPI.Controllers;

namespace Coyote.Console.App.WebAPI.DataSources
{
   
    public class SalesReport
    {
        static ReportRequestModel _RRM;
        public SalesReport()
        {
             ReportRequestModel  dsds = GetModal();
             Filter = InitializeSalesreport(dsds);
        }
      public static void SetModal(ReportRequestModel reportRequestModel) {
            _RRM = reportRequestModel;
        }
        public static ReportRequestModel GetModal() {
            return _RRM;
        }
        public  FilterDataSet Filter { get; set; }
        public  FilterDataSet InitializeSalesreport(ReportRequestModel request)
        {
            FilterDataSet Filter = new FilterDataSet();
            

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
                new SqlParameter("@reportType", "outlet"),
                new SqlParameter("@IsMember", request?.IsMember),
                new SqlParameter("@Quantity", request?.Quantity),
                new SqlParameter("@Amount", request?.Amount),
                new SqlParameter("@GP", request?.GP),
                new SqlParameter("@Margin", request?.Margin)
            };



            DataSet ds = GetReportData("[dbo].[GetItemsSalesReport_New]", dbParams.ToArray());
            try
            {
                Filter = ds.Tables[1].AsEnumerable().Select(dr =>
                 new FilterDataSet
                 {
                     GrandTotalGPPer = dr.Field<double>("GrandTotalGPPer"),
                     startdate = dr.Field<string>("startdate"),
                     enddate = dr.Field<string>("enddate"),
                     selectedoutlet = dr.Field<string>("selectOutlets"),
                     reportName = dr.Field<string>("reportName"),
                     CurrentDate = dr.Field<string>("CurrentDate"),
                     variance= dr.Field<Boolean>("variance"),
                     wastage= dr.Field<Boolean>("wastage"),
                     drillDown=dr.Field<Boolean>("drillDown"),
                     contineous=dr.Field<Boolean>("contineous"),
                 }
                ).First();
                Filter.MainDataset = ds.Tables[0].AsEnumerable().Select(row => new SalesReportDataSet
                {
                    SUM_CODE = row.Field<string>("SUM_CODE"),
                    SUM_AMT = row.Field<double>("SUM_AMT"),
                    SUM_QTY = row.Field<double>("SUM_QTY"),
                    SUM_COST = row.Field<double>("SUM_COST"),
                    SUM_PROM_SALES = row.Field<double>("SUM_PROM_SALES"),
                    SUM_PROM_SALES_GST = row.Field<double>("SUM_PROM_SALES_GST"),
                    SUM_DISCOUNT = row.Field<double>("SUM_DISCOUNT"),
                    TRX_PRODUCT = row.Field<double>("TRX_PRODUCT"),
                    PROD_DESC = row.Field<string>("PROD_DESC"),
                    CODE_DESC = row.Field<string>("CODE_DESC"),
                    TotalGP_Percentage = row.Field<double>("TotalGP_Percentage"),
                    GrandTotalGP_Per = row.Field<double>("GrandTotalGP_Per"),
                    ITEMCOUNT = row.Field<int>("ITEMCOUNT"),
                    SUM_MARGIN = row.Field<double>("SUM_MARGIN"),
                    GP_PCNT = row.Field<double>("GP_PCNT"),
                    Wastage_SUM_MOVEMENT_UNITS= row.Field<double?>("Wastage_SUM_MOVEMENT_UNITS"),
                    Variance_EXCOST = row.Field<double?>("Variance_EXCOST"),
                    Variance_MARGIN = row.Field<double?>("Variance_MARGIN"),
                    Variance_GP = row.Field<double?>("Variance_GP"),
                     Variance_SUM_MOVEMENT_UNITS = row.Field<double?>("Variance_SUM_MOVEMENT_UNITS"),
                    Wastage_EXCOST = row.Field<double?>("Wastage_EXCOST"),
                    Wastage_MARGIN = row.Field<double?>("Wastage_MARGIN"),
                    Wastage_GP = row.Field<double?>("Wastage_GP"),



                }).ToList();

            }
            catch (Exception ex)
            {

            }
            return Filter;
        }

        public  DataSet GetReportData(string commandText, SqlParameter[] sqlParameters)
        {

            DataSet dataSet = new DataSet();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection("Password=fRatlTgoca4ye;Persist Security Info=True;User ID=coyote;Initial Catalog=coyote;Data Source=10.10.10.16\\MSSQL2016;"))
                {
                    using (SqlCommand sqlCmd = new SqlCommand(commandText, sqlConn))
                    {
                        sqlCmd.CommandTimeout = 380;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddRange(sqlParameters);
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dataSet);
                            sqlCmd.Parameters.Clear();
                            sqlCmd.Dispose();
                            if (dataSet.Tables.Count <= 0)
                            {
                                throw new Exception("message");
                            }
                            dataSet.Tables[0].TableName = "DataSet";
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

                return dataSet;
            }
        }
    }

    public class SalesReportDataSet
    {
        // main table 
        public string SUM_CODE { get; set; }
        public double? SUM_AMT { get; set; }
        public double? SUM_QTY { get; set; }
        public double? SUM_COST { get; set; }
        public double? SUM_PROM_SALES { get; set; }
        public double? SUM_PROM_SALES_GST { get; set; }
        public double? SUM_DISCOUNT { get; set; }
        public double? TRX_PRODUCT { get; set; }
        public string PROD_DESC { get; set; }
        public string CODE_DESC { get; set; }
        public double? TotalGP_Percentage { get; set; }
        public double? GrandTotalGP_Per { get; set; }
        public int? ITEMCOUNT { get; set; }
        public double? SUM_MARGIN { get; set; }
        public double? GP_PCNT { get; set; }
        public double? Variance_EXCOST { get; set; }
        public double? Variance_MARGIN { get; set; }
        public double? Variance_GP { get; set; }
        public double? Variance_SUM_MOVEMENT_UNITS { get; set; }
        public double? Wastage_EXCOST { get; set; }
        public double? Wastage_MARGIN { get; set; }
        public double? Wastage_GP { get; set; }
        public double? Wastage_SUM_MOVEMENT_UNITS { get; set; }

    }

    public class FilterDataSet
    {
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string selectedoutlet { get; set; }
        public double productStartId { get; set; }
        public double productEndId { get; set; }
        public string till { get; set; }
        public bool contineous { get; set; }
        public bool drillDown { get; set; }
        public bool summary { get; set; }
        public bool promoSales { get; set; }
        public string promoCode { get; set; }
        public bool merge { get; set; }
        public bool variance { get; set; }
        public bool wastage { get; set; }

        public string reportName { get; set; }
        public string CurrentDate { get; set; }

        public string Shrinkage { get; set; }
        public double GrandTotalGPPer { get; set; }

        public bool IsMember { get; set; }

        public List<SalesReportDataSet> MainDataset { get; set; }

    }
}
