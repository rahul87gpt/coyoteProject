USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_GetSalesFromPromoPeriodInDateRange]    Script Date: 9/5/2020 1:48:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CDN_SP_GetSalesFromPromoPeriodInDateRange] @aPromoStartDateInRange DATE
	,@aPromoEndDateInRange DATE
	,@aProduct INT
	,@aProdParent INT
	,@aOutlet INT
	,@aSupplier NVARCHAR(30)
	,@aPromoType NVARCHAR(30)
	,@aPromoTotalSales FLOAT = 0 OUT
	,@aPromoDailySales FLOAT = 0 OUT
	,@aSalePromoCode NVARCHAR(30) = '' OUT
	,@FOUND_RESULT BIT = 0 OUT
	,@Debug INT = 0
AS
BEGIN TRY

	SELECT TOP 1 @aPromoTotalSales = Sum(TRX_QTY),@FOUND_RESULT = CASE WHEN  Sum(ISNULL(TRX_QTY,0))> 0 THEN 1 END
		,@aPromoDailySales = (Sum(TRX_QTY) / (DateDiff(dd, Cast(@aPromoStartDateInRange AS DATETIME), Cast(@aPromoEndDateInRange AS DATETIME)) +1)) -- added +1 to make 28 days diff as DailySales
	FROM (
		SELECT OUTP_PRODUCT
			,ISNULL(PROD_PARENT, 0) AS PROD_PARENT
			,OUTP_MIN_ONHAND
			,OUTP_CARTON_COST
			,PROD_CARTON_QTY
			,PROD_HOST_NUMBER
			,PROD_HOST_NUMBER_2
			,PROD_HOST_NUMBER_3
			,OUTP_MIN_REORDER_QTY
			,OUTP_MAX_ONHAND
			,PROD_UNIT_QTY
			,CASE 
				WHEN OUTP_QTY_ONHAND > 0
					THEN OUTP_QTY_ONHAND
				ELSE 0
				END AS OUTP_QTY_ONHAND
			,CASE 
				WHEN TRX_TYPE = 'ITEMSALE'
					THEN SUM(TRX_QTY)
				WHEN TRX_TYPE = 'CHILDSALE'
					THEN CASE 
							WHEN ISNULL(PROD_PARENT, 0) = 0
								THEN - 1 * SUM(TRX_STOCK_MOVEMENT)
							ELSE (- 1 * SUM(TRX_STOCK_MOVEMENT)) * Prod_unit_qty
							END
				WHEN TRX_TYPE = 'WASTAGE'
					THEN CASE 
							WHEN (
									TRX_DEPARTMENT IN (
										SELECT CODE_KEY_NUM
										FROM CodeTbl
										WHERE Code_Key_Type = 'DEPARTMENT'
											AND CODE_ALP_6 = 'True'
										)
									)
								THEN 0
							ELSE CASE 
									WHEN ISNULL(PROD_PARENT, 0) = 0
										THEN - 1 * SUM(TRX_QTY)
									ELSE (- 1 * SUM(TRX_QTY)) * Prod_unit_qty
									END
							END			
				END AS TRX_QTY
		FROM  dbo.OUTPTBL WITH (NOLOCK)
		JOIN  dbo.PRODTBL WITH (NOLOCK) ON (PROD_NUMBER = OUTP_PRODUCT) AND (OUTP_OUTLET = @aOutlet) AND (OUTP_STATUS = 'Active')
		LEFT JOIN  dbo.TRXTBL WITH (NOLOCK) ON TRX_PRODUCT = OUTP_PRODUCT
			AND TRX_OUTLET = OUTP_OUTLET		
		WHERE  TRX_DATE BETWEEN Cast(@aPromoStartDateInRange AS DATE) AND Cast(@aPromoEndDateInRange AS DATE)
			--AND TRX_TYPE IN ('WASTAGE','ITEMSALE','CHILDSALE')
			AND TRX_TYPE IN ('ITEMSALE')
			AND OUTP_PRODUCT = @aProduct
		GROUP BY OUTP_PRODUCT
			,TRX_TYPE
			,PROD_PARENT
			,OUTP_QTY_ONHAND
			,OUTP_MIN_ONHAND
			,Prod_unit_qty
			,OUTP_CARTON_COST
			,PROD_CARTON_QTY
			,PROD_HOST_NUMBER
			,PROD_HOST_NUMBER_2
			,PROD_HOST_NUMBER_3
			,OUTP_MIN_REORDER_QTY
			,OUTP_MAX_ONHAND
			,TRX_DEPARTMENT
			,TRX_TENDER
		) TheData
	GROUP BY OUTP_PRODUCT
		,PROD_PARENT
		,OUTP_QTY_ONHAND
		,OUTP_MIN_ONHAND
		,OUTP_CARTON_COST
		,PROD_CARTON_QTY
		,PROD_HOST_NUMBER
		,PROD_HOST_NUMBER_2
		,PROD_HOST_NUMBER_3
		,OUTP_MIN_REORDER_QTY
		,OUTP_MAX_ONHAND
		,PROD_UNIT_QTY
	ORDER BY PROD_PARENT
		,OUTP_PRODUCT

		IF @Debug = 2 SELECT '[CDN_SP_GetSalesFromPromoPeriodInDateRange]',@aPromoStartDateInRange,@aPromoEndDateInRange,(DateDiff(dd, Cast(@aPromoStartDateInRange AS DATETIME), Cast(@aPromoEndDateInRange AS DATETIME)))
			,CASE 
				WHEN OUTP_QTY_ONHAND > 0
					THEN OUTP_QTY_ONHAND
				ELSE 0
				END AS OUTP_QTY_ONHAND
			,CASE 
				WHEN TRX_TYPE = 'ITEMSALE'
					THEN (TRX_QTY)
				WHEN TRX_TYPE = 'CHILDSALE'
					THEN CASE 
							WHEN ISNULL(PROD_PARENT, 0) = 0
								THEN - 1 * (TRX_STOCK_MOVEMENT)
							ELSE (- 1 * (TRX_STOCK_MOVEMENT)) * Prod_unit_qty
							END
				WHEN TRX_TYPE = 'WASTAGE'
					THEN CASE 
							WHEN (
									TRX_DEPARTMENT IN (
										SELECT CODE_KEY_NUM
										FROM CodeTbl
										WHERE Code_Key_Type = 'DEPARTMENT'
											AND CODE_ALP_6 = 'True'
										)
									)
								THEN 0
							ELSE CASE 
									WHEN ISNULL(PROD_PARENT, 0) = 0
										THEN - 1 * (TRX_QTY)
									ELSE (- 1 * (TRX_QTY)) * Prod_unit_qty
									END
							END			
				END AS TRX_QTY
			,*			
		FROM  dbo.OUTPTBL WITH (NOLOCK)
		JOIN  dbo.PRODTBL WITH (NOLOCK) ON (PROD_NUMBER = OUTP_PRODUCT) AND (OUTP_OUTLET = @aOutlet) AND (OUTP_STATUS = 'Active')
		LEFT JOIN  dbo.TRXTBL WITH (NOLOCK) ON TRX_PRODUCT = OUTP_PRODUCT
			AND TRX_OUTLET = OUTP_OUTLET		
		WHERE  TRX_DATE BETWEEN Cast(@aPromoStartDateInRange AS DATE) AND Cast(@aPromoEndDateInRange AS DATE)
			--AND TRX_TYPE IN ('WASTAGE','ITEMSALE','CHILDSALE')
			AND TRX_TYPE IN ('ITEMSALE')
			AND OUTP_PRODUCT = @aProduct
		

END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @aOutlet,@aSupplier, @aProduct,'CDN_SP_GetSalesFromPromoPeriodInDateRange',(convert(VARCHAR, @aPromoStartDateInRange, 112) + convert(VARCHAR, @aPromoEndDateInRange, 112)));
END CATCH


