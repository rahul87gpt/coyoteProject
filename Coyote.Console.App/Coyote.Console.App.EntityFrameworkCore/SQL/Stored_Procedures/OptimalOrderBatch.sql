USE [AASystem]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--execute [dbo].[OptimalOrderBatch] 5308,'20200801',1,12
Create Procedure [dbo].[OptimalOrderBatch]

@OrderNo int,
@OrderDate Date,
@OrderCreationType int,
@OutletId int

As

Begin
SET NOCOUNT ON;

SELECT ORDH_OUTLET as Outlet,ORDH_ORDER_NO as OrderNo, ORDH_SUPPLIER as Supplier,ORDH_SUPPLIER_NAME as SupplierName,ORDH_ORDER_DATE as OrderDate,
ORDH_DOCUMENT_TYPE as OrderType,ORDH_DOCUMENT_STATUS as OrderStatus,ORDL_LINE_NO as LineNum,ORDL_PRODUCT as Product,ORDL_DESC as ProducDesc, ORDL_SUGG_UNITS_INVBUY as InvBuy,
ORDL_PromoBuy as PromoBuy , ORDL_NonPromoBuy as NormalBuy, ORDL_CoverDays as NomalCoverDays, ORDL_CoverDaysUsed as CoverDaysUsed, ORDL_MinReorderQty as MinReorderQty,
ORDL_SUGG_UNITS_AVGDAILY as NonPromoSalesRateDaily,
ORDL_SUGG_UNITS_PROMO_AVGDAILY as PromoSalesRateDaily,ORDL_Min_OnHand as MinOnHand,ORDL_Promo_Units as PromoUnits,ORDL_SUGG_UNITS_ONHAND as OnHand,
ORDL_SUGG_UNITS_ONORDER as OnOrder, ORDL_CARTON_QTY as CartonQty,ORDL_CARTONS as Cartons,ORDL_UNITS as Units, ORDL_Cover_Days as TradingCoverDays,
ORDL_CARTON_COST as NormalCartonCost, ORDL_FINAL_CARTON_COST as FinalCartonCostUsed,ORDL_FINAL_LINE_TOTAL as LineTotal,ORDL_Sale_Promo_Code as SalePromoCode,
ORDL_Sale_Promo_End_Date as SalePromoEndDate,ORDL_Buy_Promo_Code as BuyPromoCode,ORDL_PromoDisc as BuyPromoDisc, ORDL_PromoEndDate as BuyPromoEndDate,
ORDL_CheckSupplier as CheckSupplier, ORDL_CheaperSupplier as CheaperSupplier, ORDL_Perishable as Perishable,ORDL_NonPromoSales56Days as NonPromoSales56Days
,ORDL_PromoSales56Days as PromoSales56Days
FROM ORDHTBL_Temp
inner join ORDLTBL_Temp on
ordh_outlet=ordl_outlet and
ordh_order_no=ordl_order_no
and ORDH_ORDER_NO = ORDL_ORDER_NO

 where ordh_outlet = 700
-- and ordh_supplier ='1' and
and ORDH_ORDER_DATE = ORDL_ORDER_DATE
and ORDH_ORDER_DATE = '20200807' 
and ORDH_Creatation_Type=3
and ORDH_ORDER_NO = 5308

ORDER by ordl_outlet,ORDH_SUPPLIER,ordl_product

End