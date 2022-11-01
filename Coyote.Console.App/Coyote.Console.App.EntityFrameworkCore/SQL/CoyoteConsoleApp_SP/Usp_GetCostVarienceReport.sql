CREATE PROCEDURE Usp_GetCostVarienceReport 
@storeId BIGINT,
@SupplierId BIGINT,
@InvoiceDateFrom datetime ,
@InvoiceDateTo datetime,
@IsHostCost bit,
@IsNormalCost bit,
@IsSupplierBatchCost bit,
@SupplierBatch nvarchar(2000)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
	 stre.[Desc] ORDL_OUTLET
	,ordr_hedr.OrderNo ORDL_ORDER_NO
	,supler.Code ORDH_SUPPLIER
	,ordr_hedr.InvoiceNo ORDH_INVOICE_NO
	,ordr_dtl.ProductId ORDL_PRODUCT
	,ordr_dtl.FinalCartonCost ORDL_FINAL_CARTON_COST
	,ordr_dtl.TotalUnits ORDL_TOTAL_UNITS
	,prod.Number PROD_NUMBER
	,prod.[Desc] PROD_DESC
	,prod.CartonQty PROD_CARTON_QTY
	,ordr_dtl.FinalCartonCost - CASE WHEN @IsHostCost = 1 THEN olet_prod.CartonCostHost ELSE olet_prod.CartonCost END AS SUM_DIFF
	,olet_prod.CartonCostHost OUTP_CARTON_COST_HOST
	,supler.[Desc] AS SUPP_NAME
	FROM OrderHeader ordr_hedr
	INNER JOIN OrderDetail ordr_dtl ON ordr_dtl.OrderHeaderId=ordr_hedr.Id 
	INNER JOIN OutletProduct olet_prod ON ordr_hedr.OutletId = olet_prod.StoreId AND ordr_dtl.ProductId = olet_prod.ProductId
	INNER JOIN Product prod ON olet_prod.ProductId = prod.Id 
	INNER JOIN Store stre ON ordr_hedr.OutletId=stre.Id
	LEFT OUTER JOIN Supplier supler ON ordr_hedr.SupplierId=supler.Id
	INNER JOIN MasterListItems odr_tp ON ordr_hedr.TypeId = odr_tp.Id
	WHERE odr_tp.Code='INVOICE' AND ordr_dtl.TotalUnits > 0
		  AND  (@SupplierId IS NULL OR ordr_hedr.SupplierId IN (SELECT * FROM DBO.SplitString(ISNULL(@SupplierId, ''),',')))
		  AND ordr_hedr.InvoiceDate BETWEEN @InvoiceDateFrom AND @InvoiceDateTo
		  AND ((case when @IsHostCost=1 then olet_prod.CartonCostHost else olet_prod.CartonCost end) + 0.20) < ordr_dtl.FinalCartonCost  
		  AND (@storeId IS NULL OR ordr_hedr.OutletId IN (SELECT * FROM dbo.SplitString(ISNULL(@storeId, ''),','))) 
	ORDER BY stre.[Desc], supler.[Desc], prod.[Desc], ordr_hedr.InvoiceNo

	select stre.[Desc] store, case when @IsHostCost= 1 then 'Invoice Variance to Host Cost' else 'Invoice Variance to Normal Cost' end as costType,
	convert(varchar, @InvoiceDateFrom, 105) startDate, convert(varchar, @InvoiceDateTo, 105) endDate
	from Store stre where Id=@storeId
END
