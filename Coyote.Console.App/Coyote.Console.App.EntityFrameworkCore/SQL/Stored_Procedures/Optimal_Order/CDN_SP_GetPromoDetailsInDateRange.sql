USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_GetPromoDetailsInDateRange]    Script Date: 9/5/2020 1:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CDN_SP_GetPromoDetailsInDateRange] @aRunDate DATETIME
	,@OSS_Outlet INT
	,@OSS_Supplier VARCHAR(100)
	,@OUTP_PRODUCT INT
	,@PROD_PARENT INT
	,@SlotDays INT = 28
	,@aPromoUnits FLOAT = NULL OUT
	,@aPromoTotalSales FLOAT = NULL OUT
	,@aPromoDailySales FLOAT = NULL OUT
	,@aSalePromoCode NVARCHAR(100) = NULL OUT
	,@aStrSalePromoEndDate NVARCHAR(100) = NULL OUT
	,@Result BIT = 0 OUT
	,@Debug INT = 0
AS
BEGIN TRY
	DECLARE @EndDate DATETIME = DATEADD(DD, -1, @aRunDate)
	DECLARE @StartDate DATETIME = DATEADD(DD, -@SlotDays+1, @EndDate)
	DECLARE @PROMO_FOUND BIT = 0
	DECLARE @aPromoStartDate DATETIME
	DECLARE @aPromoEndDate DATETIME
	DECLARE @aPromoType VARCHAR(100)

	WHILE DATEDIFF(DD, @StartDate, @EndDate) <= 180 --OR @PROMO_FOUND = 0  Commented bcz not found in archa's code
	BEGIN
		------{ Start GetSalePromoCodeStartingInDateRange
		IF @Debug =2 SELECT 'CDN_SP_GetPromoDetailsInDateRange heredfd' CDN_SP_GetPromoDetailsInDateRange, @StartDate startdate, @EndDate enddate,DATEDIFF(DD, @StartDate, @EndDate)
		EXEC dbo.CDN_SP_GetSalePromoCodeStartingInDateRange @StartDate
				,@EndDate
				,@OUTP_PRODUCT
				,@PROD_PARENT
				,@OSS_Outlet
				,@OSS_Supplier
				,1
				,@aPromoType OUT
				,@aPromoUnits OUT
				,@aPromoStartDate OUT
				,@aPromoEndDate OUT
				,@aSalePromoCode OUT
				,@aPromoTotalSales OUT
				,@aPromoDailySales OUT
				,@PROMO_FOUND OUT
				,@Debug=@Debug
		------{ End   GetSalePromoCodeStartingInDateRange

		IF @PROMO_FOUND = 1 -- from GetSalePromoCodeStartingInDateRange
		BEGIN
			SET @Result = 1

			BREAK
		END
		ELSE
		BEGIN
			IF DATEDIFF(DD, DATEADD(DD, - @SlotDays, @StartDate), @EndDate) <= 180
				SET @StartDate = DATEADD(DD, - @SlotDays, @StartDate)
			ELSE
			BEGIN
				DECLARE @DeductDays INT = 180 - DATEDIFF(DD, @StartDate, @EndDate)

				IF @DeductDays > 0
					SET @StartDate = DATEADD(DD, - @DeductDays, @StartDate)
				ELSE
					BREAK
			END
			IF @Debug =2 SELECT 'CDN_SP_GetPromoDetailsInDateRange' CDN_SP_GetPromoDetailsInDateRange,@SlotDays SlotDays,@DeductDays DeductDays, @StartDate startdate, @EndDate enddate,DATEDIFF(DD, @StartDate, @EndDate)
		END
	END
END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, @OUTP_PRODUCT,'CDN_SP_GetPromoDetailsInDateRange',NULL);
END CATCH