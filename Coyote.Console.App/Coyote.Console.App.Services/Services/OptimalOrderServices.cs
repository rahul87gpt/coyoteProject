using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;

namespace Coyote.Console.App.Services.Services
{
    public class OptimalOrderServices : IOptimalOrderServices
    {
        private ILoggerManager _iLoggerManager = null;
        public OptimalOrderServices(ILoggerManager iLoggerManager)
        {
            _iLoggerManager = iLoggerManager;
        }

        public PagedOutputModel<List<OptimalOrderBatchResponseModel>> GetOptimalBatch(int? OutletId, int? OrderNo, DateTime? OrderDate)
        {
            try
            {
                if (OutletId <= 0)
                {
                    throw new BadRequestException(ErrorMessages.OutletId.ToString(CultureInfo.CurrentCulture));
                }

                List<OptimalOrderBatchResponseModel> optimalOrderBatches = new List<OptimalOrderBatchResponseModel>();

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                //builder.ConnectionString = connectionStrings.DBConnectionLive;
                builder.ConnectionString = "Password=Gmc+^hJQD*gi=2;Persist Security Info=True;User ID=cdnro;Initial Catalog=AASystem;Data Source=db01.coyotepos.com.au;";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT ORDH_OUTLET as Outlet,ORDH_ORDER_NO as OrderNo, ORDH_SUPPLIER as Supplier,ORDH_SUPPLIER_NAME as SupplierName,");

                    sb.Append(" ORDH_ORDER_DATE as OrderDate,ORDH_DOCUMENT_TYPE as OrderType,ORDH_DOCUMENT_STATUS as");
                    sb.Append("OrderStatus,ORDL_LINE_NO as LineNum,");
                    sb.Append(" ORDL_PRODUCT as Product,ORDL_DESC as ProducDesc, ORDL_SUGG_UNITS_INVBUY as InvBuy,");
                    sb.Append(" ORDL_PromoBuy as PromoBuy , ORDL_NonPromoBuy as NormalBuy, ORDL_CoverDays as NomalCoverDays, ");
                    sb.Append(" ORDL_CoverDaysUsed as CoverDaysUsed, ORDL_MinReorderQty as MinReorderQty,");
                    sb.Append(" ORDL_SUGG_UNITS_AVGDAILY as NonPromoSalesRateDaily,");
                    sb.Append(" ORDL_SUGG_UNITS_PROMO_AVGDAILY as PromoSalesRateDaily,ORDL_Min_OnHand as MinOnHand,");
                    sb.Append(" ORDL_Promo_Units as PromoUnits,ORDL_SUGG_UNITS_ONHAND as OnHand,");
                    sb.Append(" ORDL_SUGG_UNITS_ONORDER as OnOrder, ORDL_CARTON_QTY as CartonQty,");
                    sb.Append(" ORDL_CARTONS as Cartons,ORDL_UNITS as Units, ORDL_Cover_Days as TradingCoverDays,");
                    sb.Append(" ORDL_CARTON_COST as NormalCartonCost, ORDL_FINAL_CARTON_COST as FinalCartonCostUsed,");
                    sb.Append(" ORDL_FINAL_LINE_TOTAL as LineTotal,ORDL_Sale_Promo_Code as SalePromoCode,");
                    sb.Append(" ORDL_Sale_Promo_End_Date as SalePromoEndDate,ORDL_Buy_Promo_Code as BuyPromoCode,");
                    sb.Append(" ORDL_PromoDisc as BuyPromoDisc, ORDL_PromoEndDate as BuyPromoEndDate,");
                    sb.Append(" ORDL_CheckSupplier as CheckSupplier, ORDL_CheaperSupplier as CheaperSupplier, ");
                    sb.Append(" ORDL_Perishable as Perishable,ORDL_NonPromoSales56Days as NonPromoSales56Days,");
                    sb.Append(" ORDL_PromoSales56Days as PromoSales56Days");
                    sb.Append(" FROM ORDHTBL_Temp");
                    sb.Append(" inner join ORDLTBL_Temp on");
                    sb.Append(" ordh_outlet=ordl_outlet and ");
                    sb.Append(" ordh_order_no=ordl_order_no ");
                    sb.Append(" and ORDH_ORDER_NO = ORDL_ORDER_NO where ");
                    sb.Append("  ORDH_ORDER_DATE = ORDL_ORDER_DATE and ");
                    sb.Append($"  ORDH_Creatation_Type = 3 ");
                    if (OutletId > 0)
                    {
                        sb.Append($" and ordh_outlet ={OutletId}  ");
                    }
                    if (OrderDate != null && OrderDate!=DateTime.MinValue)
                    {
                        var dateString = Convert.ToDateTime(OrderDate).ToString("yyyy-MM-dd");
                        sb.Append($" and  ORDH_ORDER_DATE = '{dateString}'  ");
                    }

                    if (OrderNo > 0)
                    {
                        sb.Append($" and ORDH_ORDER_NO ={OrderNo} ");
                    }

                    sb.Append(" ORDER by ordl_outlet,ORDH_SUPPLIER,ordl_product ");




                    String sql = sb.ToString();

                    if (!string.IsNullOrEmpty(sql))
                    {
                        using (SqlCommand command = new SqlCommand(sql, connection))

                        {
                            OptimalOrderBatchResponseModel responseModel = new OptimalOrderBatchResponseModel();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var response = MappingHelpers.ConvertToObject<OptimalOrderBatchResponseModel>(reader);

                                    optimalOrderBatches.Add(response);

                                }
                            }
                        }
                    }
                    connection.Close();

                }
                return new PagedOutputModel<List<OptimalOrderBatchResponseModel>>(optimalOrderBatches, optimalOrderBatches.Count);
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

    }


}

