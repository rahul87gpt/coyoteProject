USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_GetNonPromoSalesCalculation]    Script Date: 9/5/2020 1:44:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--SELECT dbo.CDN_fn_GetNonPromoSalesCalculation(1013011,0,792,'COKE',GETDATE(),56)

ALTER PROCEDURE [dbo].[CDN_SP_GetNonPromoSalesCalculation](
@Product int,
@ProductParent int,
@OutletNo int,
@Supplier varchar(100),
@RunDate  Date,
--@DaysSpan int=56,
@DaysSpan int,
@NonPromoDailySales FLOAT OUT,
@Debug BIT = 0,
@IsANewProd BIT
)

AS

BEGIN
	DECLARE @PromoStartDate  Date,@PromoEndDate  Date, @SaleEndDate Date
	DECLARE @ProductScanDays INT = 180
	DECLARE @FirstTrxDate DATE 

	SELECT @FirstTrxDate = Min(TRX_DATE)
	FROM dbo.TrxTbl WITH (NOLOCK)
	WHERE TRX_OUTLET = @OutletNo
	AND TRX_TYPE = 'ITEMSALE'
	AND TRX_PRODUCT = @Product

	--SET @RunDate = ISNULL(@RunDate,GETDATE())

	select @PromoStartDate  = Max(PRMH_START) 
	from PrmhTbl 	
	join PRMPTBL on PRMH_PROM_CODE = PRMP_PROM_CODE 
	join OutpTbl on PRMP_PRODUCT = Outp_Product AND OUTP_OUTLET = @OutletNo  AND OUTP_STATUS = 'Active'
	
	LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
	BB.Outlet = @OutletNo AND BB.supplier = @Supplier AND BB.BEST_BUY = 1 AND Product = @Product

	where PRMH_OUTLET_ZONE in (select CODE_KEY_ALP FROM CODETBL where CODE_KEY_TYPE = 'ZONEOUTLET' and CODE_KEY_NUM = @OutletNo) 
	and (PRMH_START <= @RunDate) and PRMH_PROMOTION_TYPE in ('OFFER', 'SELLING', 'MIXMATCH')  
	and (OUTP_SUPPLIER = @Supplier OR BB.BEST_BUY=1)
	and (PRMP_PRODUCT = @Product or PRMP_PRODUCT = @ProductParent)
	and DATEDIFF(DD, PRMH_START, @RunDate) <= @ProductScanDays

	IF @Debug = 1 OR @Debug = 2
		select 'CDN_SP_GetNonPromoSalesCalculation 'CDN_SP_GetNonPromoSalesCalculation,@PromoStartDate PromoStartDateFound,* 
	from PrmhTbl 	
	join PRMPTBL on PRMH_PROM_CODE = PRMP_PROM_CODE 
	join OutpTbl on PRMP_PRODUCT = Outp_Product AND OUTP_OUTLET = @OutletNo  AND OUTP_STATUS = 'Active'

	LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
	BB.Outlet = @OutletNo AND BB.supplier = @Supplier AND BB.BEST_BUY = 1 AND Product = @Product

	where PRMH_OUTLET_ZONE in (select CODE_KEY_ALP FROM CODETBL where CODE_KEY_TYPE = 'ZONEOUTLET' and CODE_KEY_NUM = @OutletNo) 
	and (PRMH_START <= @RunDate) and PRMH_PROMOTION_TYPE in ('OFFER', 'SELLING', 'MIXMATCH')
	and (OUTP_SUPPLIER = @Supplier OR BB.BEST_BUY = 1) 
	and (PRMP_PRODUCT = @Product or PRMP_PRODUCT = @ProductParent)
	and DATEDIFF(DD, PRMH_START, @RunDate) <= @ProductScanDays
	order by PRMH_START desc

	---- No Sale Promo date is foind then retutn from here and use last 28 days sales rate
	--IF @PromoStartDate IS NULL 
	--BEGIN
	--	RETURN
	--END
	
	SET @PromoStartDate = ISNULL(@PromoStartDate ,@RunDate)

	SET @PromoEndDate = DATEADD(DD,-1,@PromoStartDate)

	SET @PromoStartDate = DATEADD(DD,-@DaysSpan+1,@PromoEndDate)
	
	IF @Debug in (1,2) SELECT 'CDN_SP_GetNonPromoSalesCalculation' CDN_SP_GetNonPromoSalesCalculation, @PromoStartDate PromoStartDate,@PromoEndDate PromoEndDate,@PromoStartDate PromoStartDate,DATEDIFF(dd,@PromoStartDate,@PromoEndDate) diff



	SELECT	 @NonPromoDailySales=(CASE 
				WHEN TRX_TYPE = 'ITEMSALE' THEN SUM(TRX_QTY) 
				WHEN TRX_TYPE = 'CHILDSALE' THEN 
					CASE 
						WHEN ISNULL(PROD_PARENT, 0) = 0 THEN -1 * SUM(TRX_STOCK_MOVEMENT) 
						ELSE (-1 * SUM(TRX_STOCK_MOVEMENT)) * PROD_UNIT_QTY 
					END 
				WHEN TRX_TYPE = 'WASTAGE' THEN
						CASE 
							WHEN (TRX_DEPARTMENT IN (SELECT CODE_KEY_NUM FROM CODETBL WHERE CODE_KEY_TYPE = 'DEPARTMENT' AND CODE_ALP_6 = 'TRUE')) THEN 0 
					ELSE 
						CASE 
							WHEN ISNULL(PROD_PARENT, 0) = 0 THEN -1 * SUM(TRX_QTY) 
							ELSE (-1 * SUM(TRX_QTY)) * PROD_UNIT_QTY 
						END 
					END 
				END) 
			FROM OUTPTBL 
			INNER JOIN PRODTBL ON (PROD_NUMBER = OUTP_PRODUCT) 
			INNER JOIN TRXTBL ON TRX_PRODUCT = OUTP_PRODUCT AND TRX_OUTLET = OUTP_OUTLET AND TRX_DATE BETWEEN @PromoStartDate AND @PromoEndDate 
			WHERE TRX_DATE BETWEEN @PromoStartDate AND @PromoEndDate  AND (OUTP_STATUS = 'ACTIVE') 
			--AND TRX_TYPE IN ('WASTAGE', 'ITEMSALE', 'CHILDSALE')
			AND TRX_TYPE IN ('ITEMSALE')
			AND OUTP_PRODUCT = @Product and OUTP_OUTLET = @OutletNo
			GROUP BY OUTP_PRODUCT, PROD_DEPARTMENT, TRX_TYPE, PROD_PARENT,TRX_DEPARTMENT, TRX_TENDER,OUTP_SUPPLIER,OUTP_OUTLET,PROD_UNIT_QTY

			IF @Debug in (1,2)
			 SELECT 'CDN_SP_GetNonPromoSalesCalculation sdefefde' CDN_SP_GetNonPromoSalesCalculation, (CASE 
				WHEN TRX_TYPE = 'ITEMSALE' THEN (TRX_QTY) 
				WHEN TRX_TYPE = 'CHILDSALE' THEN 
					CASE 
						WHEN ISNULL(PROD_PARENT, 0) = 0 THEN -1 * (TRX_STOCK_MOVEMENT) 
						ELSE (-1 * (TRX_STOCK_MOVEMENT)) * PROD_UNIT_QTY 
					END 
				WHEN TRX_TYPE = 'WASTAGE' THEN
						CASE 
							WHEN (TRX_DEPARTMENT IN (SELECT CODE_KEY_NUM FROM CODETBL WHERE CODE_KEY_TYPE = 'DEPARTMENT' AND CODE_ALP_6 = 'TRUE')) THEN 0 
					ELSE 
						CASE 
							WHEN ISNULL(PROD_PARENT, 0) = 0 THEN -1 * (TRX_QTY) 
							ELSE (-1 * (TRX_QTY)) * PROD_UNIT_QTY 
						END 
					END 
				END) NonPromoDailySales,*
			FROM OUTPTBL 
			INNER JOIN PRODTBL ON (PROD_NUMBER = OUTP_PRODUCT) 
			INNER JOIN TRXTBL ON TRX_PRODUCT = OUTP_PRODUCT AND TRX_OUTLET = OUTP_OUTLET AND TRX_DATE BETWEEN @PromoStartDate AND @PromoEndDate 
			WHERE TRX_DATE BETWEEN @PromoStartDate AND @PromoEndDate  AND (OUTP_STATUS = 'ACTIVE')  AND TRX_TYPE IN ('WASTAGE', 'ITEMSALE', 'CHILDSALE')
			AND OUTP_PRODUCT = @Product and OUTP_OUTLET = @OutletNo
			--GROUP BY OUTP_PRODUCT, PROD_DEPARTMENT, TRX_TYPE, PROD_PARENT,TRX_DEPARTMENT, TRX_TENDER,OUTP_SUPPLIER,OUTP_OUTLET,PROD_UNIT_QTY


	
	
	-- SET @NonPromoDailySales =(@NonPromoDailySales/@DaysSpan)

	SET @SaleEndDate = DATEADD(DD,-1,@RunDate)

	IF @Debug in (1,2) SELECT 'CDN_SP_GetNonPromoSalesCalculation' CDN_SP_GetNonPromoSalesCalculation, @NonPromoDailySales NonPromoDailySales, @DaysSpan DaysSpan, @SaleEndDate SaleEndDate,
	@IsANewProd IsANewProd, @FirstTrxDate FirstTrxDate, @PromoEndDate SaleEndDateBeforePromoStartDate

	IF @IsANewProd = 1 
	BEGIN
		IF DATEDIFF(DD, @FirstTrxDate, @SaleEndDate) >= 7
		BEGIN
			IF DATEDIFF(DD, @FirstTrxDate, @PromoEndDate) >= @DaysSpan - 1 --- Diff is 55 Days
			BEGIN
				SET @NonPromoDailySales = @NonPromoDailySales / @DaysSpan
			END
			ELSE
			BEGIN
				SET @NonPromoDailySales = @NonPromoDailySales / (DATEDIFF(DD, @FirstTrxDate, @PromoEndDate) +1)
			END
		END
		ELSE
		BEGIN
			SET @NonPromoDailySales = @NonPromoDailySales/ 7
		END
	END
	ELSE -- OLD Product
	BEGIN
		SET @NonPromoDailySales = @NonPromoDailySales/@DaysSpan
	END
	
	
    
	

	
	
	
	IF @Debug in (1,2) SELECT 'CDN_SP_GetNonPromoSalesCalculation' CDN_SP_GetNonPromoSalesCalculation, @NonPromoDailySales NonPromoDailySales

END