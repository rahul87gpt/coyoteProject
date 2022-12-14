USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_GetSalePromoCodeStartingInDateRange]    Script Date: 9/5/2020 1:47:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CDN_SP_GetSalePromoCodeStartingInDateRange] @StartDate DATE
	,@EndDate DATE
	,@OUTP_PRODUCT INT
	,@PROD_PARENT INT
	,@OSS_Outlet INT
	,@OSS_Supplier VARCHAR(100)
	,@GetSaleTypeCase INT = 1
	,@aPromoType VARCHAR(100) = NULL OUT
	,@aPromoUnits FLOAT = 0 OUT
	,@aPromoStartDate DATETIME = NULL OUT
	,@aPromoEndDate DATETIME = NULL OUT
	,@aSalePromoCode VARCHAR(100) = '' OUT
	,@aPromoTotalSales FLOAT = 0 OUT
	,@aPromoDailySales FLOAT = 0 OUT
	,@Result BIT = 0 OUT
	,@Debug INT = 0
AS
/*

DECLARE 
@StartDate DATE,
@EndDate DATE,
@OUTP_PRODUCT INT=1012007,
@PROD_PARENT INT,
@OSS_Outlet INT=700,
@OSS_Supplier VARCHAR(100)='1',@SlotDays int =28
DECLARE @aRunDate date =GETDATE()
SET @EndDate = DATEADD(DD,-1,@aRunDate)
SET @StartDate = DATEADD(DD,-((@SlotDays)+1),@EndDate)
EXEC CDN_SP_GetSalePromoCodeStartingInDateRange @StartDate,@EndDate,@OUTP_PRODUCT,@PROD_PARENT,@OSS_Outlet,@OSS_Supplier 

*/
BEGIN TRY

	DECLARE @ProductPromoList TABLE (
		ID INT IDENTITY(1, 1)
		,PRMH_PROM_CODE NVARCHAR(30)
		,PRMP_Promo_Units FLOAT
		,PRMH_START DATETIME
		,PRMH_PROMOTION_TYPE NVARCHAR(30)
		,PRMH_END DATETIME
		,Is_Processed BIT
		)
	DECLARE @ID INT
	 SET NOCOUNT ON
	INSERT INTO @ProductPromoList
	SELECT DISTINCT PRMH_PROM_CODE
		,PRMP_Promo_Units
		,PRMH_START
		,PRMH_PROMOTION_TYPE
		,PRMH_END
		,0
	FROM  dbo.PrmhTbl WITH(NOLOCK) 
	JOIN  dbo.PRMPTBL WITH(NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE AND PRMP_PRODUCT= @OUTP_PRODUCT
	JOIN  dbo.OutpTbl WITH(NOLOCK) ON PRMP_PRODUCT = @OUTP_PRODUCT AND OUTP_STATUS = 'Active' AND OUTP_PRODUCT=@OUTP_PRODUCT AND  OUTP_OUTLET = @OSS_Outlet
	
	LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
	BB.Outlet = @OUTP_PRODUCT AND BB.supplier = @OSS_Supplier AND BB.BEST_BUY = 1 AND Product = @OUTP_PRODUCT
		

	WHERE (OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)  AND PRMH_OUTLET_ZONE IN (
			SELECT CODE_KEY_ALP
			FROM CODETBL
			WHERE CODE_KEY_TYPE = 'ZONEOUTLET'
				AND CODE_KEY_NUM = @OSS_Outlet
			)
		
		AND DATEDIFF(DD,PRMH_START ,PRMH_END)>=21 
		AND (
		(@StartDate BETWEEN PRMH_START AND PRMH_END AND DATEADD(DD,21,@StartDate) <= PRMH_END)
		OR
		(PRMH_START BETWEEN @StartDate AND @EndDate AND  DATEADD(DD,21,PRMH_START) <= @EndDate))
		AND PRMH_PROMOTION_TYPE IN (
			'OFFER'
			,'SELLING'
			,'MIXMATCH'
			)
		AND (
			PRMP_PRODUCT = @OUTP_PRODUCT
			OR PRMP_PRODUCT = @PROD_PARENT
			)
				
	ORDER BY PRMP_Promo_Units DESC

	DECLARE @PromoDailySalesTotal FLOAT = 0
		,@PromoTotalSalesTotal FLOAT = 0
		,@PromoTotalUnits FLOAT = 0
		,@PromoCodeCount INT = 0
		,@cnt INT = 1
		,@totalcnt INT

	SELECT @totalcnt = count(*)
	FROM @ProductPromoList
	
	IF @Debug = 2 SELECT DISTINCT 'CDN_SP_GetSalePromoCodeStartingInDateRange kysah'  CDN_SP_GetSalePromoCodeStartingInDateRange, PRMH_PROM_CODE
		,PRMP_Promo_Units
		,PRMH_START
		,PRMH_PROMOTION_TYPE
		,PRMH_END
		,0
	FROM  dbo.PrmhTbl WITH(NOLOCK) 
	JOIN  dbo.PRMPTBL WITH(NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE AND PRMP_PRODUCT= @OUTP_PRODUCT
	JOIN  dbo.OutpTbl WITH(NOLOCK) ON PRMP_PRODUCT = @OUTP_PRODUCT AND OUTP_STATUS = 'Active' AND OUTP_PRODUCT=@OUTP_PRODUCT AND  OUTP_OUTLET = @OSS_Outlet
	
	LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
	BB.Outlet = @OUTP_PRODUCT AND BB.supplier = @OSS_Supplier AND BB.BEST_BUY = 1 AND Product = @OUTP_PRODUCT
	
	WHERE (OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)  AND PRMH_OUTLET_ZONE IN (
			SELECT CODE_KEY_ALP
			FROM CODETBL
			WHERE CODE_KEY_TYPE = 'ZONEOUTLET'
				AND CODE_KEY_NUM = @OSS_Outlet
			)
		AND DATEDIFF(DD,PRMH_START ,PRMH_END)>=21 
		AND (
		(@StartDate BETWEEN PRMH_START AND PRMH_END AND DATEADD(DD,21,@StartDate) <= PRMH_END)
		OR
		(PRMH_START BETWEEN @StartDate AND @EndDate AND  DATEADD(DD,21,PRMH_START) <= @EndDate))
		AND PRMH_PROMOTION_TYPE IN (
			'OFFER'
			,'SELLING'
			,'MIXMATCH'
			)
		AND (
			PRMP_PRODUCT = @OUTP_PRODUCT
			OR PRMP_PRODUCT = @PROD_PARENT
			)				
	ORDER BY PRMP_Promo_Units DESC

	DECLARE @Dictionary AS TABLE (DATES NVARCHAR(20))

	WHILE @totalcnt >= @cnt
	BEGIN
		SELECT @aSalePromoCode = PRMH_PROM_CODE
			,@aPromoUnits = PRMP_Promo_Units
			,@aPromoStartDate = PRMH_START
			,@aPromoType = PRMH_PROMOTION_TYPE
			,@aPromoEndDate = PRMH_END
			,@ID = ID
		FROM @ProductPromoList 
		WHERE Id = @cnt

		SET @cnt =@cnt+1
		DECLARE @PromoStartDateInRange DATETIME = @aPromoStartDate--= --@StartDate;-- As StartDate always in between PromoStartDate and PromoEndDate
		DECLARE @PromoEndDateInRange DATETIME = @aPromoEndDate

		IF @Debug = 2 SELECT '[CDN_SP_GetSalePromoCodeStartingInDateRange] asddefbh', @aPromoStartDate PS,@aPromoEndDate PE,@StartDate SS,@EndDate SE

		IF @aPromoStartDate < @StartDate				
			SET @PromoStartDateInRange = @StartDate 							
		ELSE
			SET @PromoStartDateInRange =  @aPromoStartDate
		
		IF @aPromoEndDate >= @EndDate
			SET @PromoEndDateInRange = @EndDate
		ELSE
			SET @PromoEndDateInRange = @aPromoEndDate
		
		IF @Debug = 2 SELECT '[CDN_SP_GetSalePromoCodeStartingInDateRange] ry565',@PromoStartDateInRange PromoStartDateInRange,@PromoEndDateInRange PromoEndDateInRange,DATEDIFF(DD, @PromoStartDateInRange, @PromoEndDateInRange)  DayDifference
			
		IF (DATEDIFF(DD, @PromoStartDateInRange, @PromoEndDateInRange) < 21)
			CONTINUE;

		DECLARE @strPromotionStartEndDateInRange NVARCHAR(20) = convert(VARCHAR, @PromoStartDateInRange, 112) + convert(VARCHAR, @PromoEndDateInRange, 112);

		IF EXISTS (
				SELECT 1
				FROM @Dictionary 
				WHERE DATES = @strPromotionStartEndDateInRange
				)
			CONTINUE;

		INSERT INTO @Dictionary
		VALUES (@strPromotionStartEndDateInRange)

		DECLARE @FOUND_RESULT BIT = 0

		EXEC dbo.CDN_SP_GetSalesFromPromoPeriodInDateRange @PromoStartDateInRange
			,@PromoEndDateInRange
			,@OUTP_PRODUCT
			,@PROD_PARENT
			,@OSS_Outlet
			,@OSS_Supplier
			,@aPromoType
			,@aPromoTotalSales OUT
			,@aPromoDailySales OUT
			,@aSalePromoCode OUT
			,@FOUND_RESULT OUT
			,@Debug 
		IF (@FOUND_RESULT = 1)
		BEGIN			
			SET @PromoTotalUnits = @PromoTotalUnits + @aPromoUnits;
			SET @PromoDailySalesTotal = @PromoDailySalesTotal + @aPromoDailySales;
			SET @PromoTotalSalesTotal = @PromoTotalSalesTotal + @aPromoTotalSales;

			IF @Debug in (1,2) SELECT '[CDN_SP_GetSalePromoCodeStartingInDateRange] 1',@PromoTotalUnits PromoTotalUnits,@PromoDailySalesTotal PromoDailySalesTotal
		END;
		IF @Debug=2 SELECT '[CDN_SP_GetSalePromoCodeStartingInDateRange] 1', @PromoTotalUnits PromoTotalUnits,@PromoDailySalesTotal PromoDailySalesTotal ,@PromoTotalSalesTotal PromoTotalSalesTotal
		SET @PromoCodeCount = @PromoCodeCount + 1;
	END
	
	--CLOSE ProductPromoList;
	--DEALLOCATE ProductPromoList;
	DELETE @ProductPromoList
	
	IF (@PromoCodeCount > 0)
		AND (@PromoDailySalesTotal > 0)
		AND (@PromoTotalSalesTotal > 0)
	BEGIN
		SET @aPromoDailySales = @PromoDailySalesTotal / @PromoCodeCount;
		SET @aPromoTotalSales = @PromoTotalSalesTotal / @PromoCodeCount;
		SET @aPromoUnits = @PromoTotalUnits / @PromoCodeCount;
		SET @Result = 1;
		IF @Debug=2 SELECT '[CDN_SP_GetSalePromoCodeStartingInDateRange] 2',@Result Result, @PromoTotalUnits PromoTotalUnits,@PromoDailySalesTotal PromoDailySalesTotal ,@PromoTotalSalesTotal PromoTotalSalesTotal
	END;


END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, @OUTP_PRODUCT,'CDN_SP_GetSalePromoCodeStartingInDateRange',(convert(VARCHAR, @StartDate, 112) + convert(VARCHAR, @EndDate, 112)));
END CATCH
