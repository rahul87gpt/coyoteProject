USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_GetCheaperSupplier]    Script Date: 9/5/2020 1:43:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CDN_SP_GetCheaperSupplier] @aProduct INT
	,@aProdParent INT
	,@OSS_Outlet INT
	,@OSS_Supplier VARCHAR(30) OUT
	,@aRunDate DATETIME
	,@COST FLOAT OUT
	,@CoverDaysUsed INT OUT
	,@CheaperResult VARCHAR(30) = NULL OUT
	,@Debug INT
	,@OSS_Id INT
	,@OSS_DOWGenerateOrder INT
AS
BEGIN TRY
	DECLARE @MinPrmCtnCost FLOAT
		,@CheaperSupplier VARCHAR(30)


	SELECT TOP 1 @MinPrmCtnCost = PRMP_CARTON_COST
		,@CheaperSupplier = PRMP_SUPPLIER
	FROM [dbo].PRMPTBL WITH (NOLOCK)
	JOIN [dbo].PRMHTBL WITH (NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE
	JOIN [dbo].PRODTBL WITH (NOLOCK) ON Prod_Number = PRMP_PRODUCT
	--JOIN [dbo].OUTPTBL WITH (NOLOCK) ON OUTP_PRODUCT = PRMP_PRODUCT AND OUTP_OUTLET = @OSS_Outlet AND OUTP_SUPPLIER = @OSS_Supplier
	WHERE PRMH_PROMOTION_TYPE = 'BUYING'
		AND PRMH_START <= @aRunDate
		AND PRMH_END >= DATEADD(DD,@CoverDaysUsed, @aRunDate)
		AND PRMH_STATUS = 'Active'
		AND PRMP_CARTON_COST > 0
		AND @COST > PRMP_CARTON_COST
		AND PRMH_OUTLET_ZONE IN (
			SELECT CODE_KEY_ALP
			FROM CodeTbl
			WHERE Code_Key_Type = 'ZONEOUTLET'
				AND CODE_KEY_NUM = @OSS_Outlet
			)
		AND PRMP_SUPPLIER <> @OSS_Supplier
		AND (
			PRMP_PRODUCT = @aProduct
			OR PRMP_PRODUCT = @aProdParent
			)
	--group by PRMH_END, PRMP_PROM_CODE
	ORDER BY PRMP_CARTON_COST ASC
	
	IF (@MinPrmCtnCost IS NULL)
		RETURN
	

	DECLARE @Cheaper_OSS_Id BIT,@Cheaper_OSS_DOWGenerateOrder INT,@Cheaper_OSS_LastRun date
	SELECT TOP 1 
	@Cheaper_OSS_DOWGenerateOrder=
		CASE WHEN OSS_DOWGenerateOrder >= @OSS_DOWGenerateOrder 
			THEN OSS_DOWGenerateOrder-@OSS_DOWGenerateOrder 
			ELSE OSS_DOWGenerateOrder+7-@OSS_DOWGenerateOrder END
	,@Cheaper_OSS_Id = OSS_Id,@Cheaper_OSS_LastRun = OSS_LastRun
	FROM OutletSupplierScheduleTbl 
	WHERE 
	oss_supplier = @CheaperSupplier
	AND oss_outlet = @OSS_Outlet
	ORDER BY CASE WHEN OSS_DOWGenerateOrder >= @OSS_DOWGenerateOrder 
			THEN OSS_DOWGenerateOrder-@OSS_DOWGenerateOrder 
			ELSE OSS_DOWGenerateOrder+7-@OSS_DOWGenerateOrder END
	
	IF @Debug in(1,2) 
	SELECT '[CDN_SP_GetCheaperSupplier]'[CDN_SP_GetCheaperSupplier],
	
		CASE WHEN OSS_DOWGenerateOrder >= @OSS_DOWGenerateOrder 
			THEN OSS_DOWGenerateOrder-@OSS_DOWGenerateOrder 
			ELSE OSS_DOWGenerateOrder+7-@OSS_DOWGenerateOrder END
	, OSS_Id, OSS_LastRun, *
	FROM OutletSupplierScheduleTbl 
	WHERE
	oss_supplier = @CheaperSupplier
	AND oss_outlet = @OSS_Outlet
	ORDER BY CASE WHEN OSS_DOWGenerateOrder >= @OSS_DOWGenerateOrder 
			THEN OSS_DOWGenerateOrder-@OSS_DOWGenerateOrder 
			ELSE OSS_DOWGenerateOrder+7-@OSS_DOWGenerateOrder END

	IF @Debug in(1,2) SELECT TOP 1 '[CDN_SP_GetCheaperSupplier]' [CDN_SP_GetCheaperSupplier],@Cheaper_OSS_DOWGenerateOrder Cheaper_OSS_DOWGenerateOrder,
	@MinPrmCtnCost MinPrmCtnCost,@COST ActualCOST ,@CheaperSupplier CheaperSupplier,@OSS_Supplier DefaultSupplier, *
	FROM [dbo].PRMPTBL WITH (NOLOCK)
	JOIN [dbo].PRMHTBL WITH (NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE
	JOIN [dbo].PRODTBL WITH (NOLOCK) ON Prod_Number = PRMP_PRODUCT
	--JOIN [dbo].OUTPTBL WITH (NOLOCK) ON OUTP_PRODUCT = PRMP_PRODUCT AND OUTP_OUTLET = @OSS_Outlet AND OUTP_SUPPLIER = @OSS_Supplier
	WHERE PRMH_PROMOTION_TYPE = 'BUYING'
		AND PRMH_START <= @aRunDate
		AND PRMH_END >= DATEADD(DD,@CoverDaysUsed, @aRunDate)
		AND PRMH_STATUS = 'Active'
		AND PRMP_CARTON_COST > 0
		AND @COST > PRMP_CARTON_COST
		AND PRMH_OUTLET_ZONE IN (
			SELECT CODE_KEY_ALP
			FROM CodeTbl
			WHERE Code_Key_Type = 'ZONEOUTLET'
				AND CODE_KEY_NUM = @OSS_Outlet
			)
		AND PRMP_SUPPLIER <> @OSS_Supplier
		AND (
			PRMP_PRODUCT = @aProduct
			OR PRMP_PRODUCT = @aProdParent
			)
	--group by PRMH_END, PRMP_PROM_CODE
	ORDER BY PRMP_CARTON_COST 

	IF (@Cheaper_OSS_Id IS NULL)
		RETURN


	IF EXISTS (SELECT 1 FROM  [dbo].CDN_BEST_BY_SUPPLIER WHERE Outlet=@OSS_Outlet AND supplier = @CheaperSupplier AND Product = @aProduct )
		UPDATE [dbo].CDN_BEST_BY_SUPPLIER SET [BEST_BUY] = 1, [UpdatedAt] = GETDATE(), [CoverDayUsed] = @Cheaper_OSS_DOWGenerateOrder, [NormalCartonCost] = @COST , [CheaperCartonCost] = @MinPrmCtnCost  WHERE Outlet=@OSS_Outlet AND supplier = @CheaperSupplier AND Product = @aProduct 
	ELSE
		INSERT [dbo].CDN_BEST_BY_SUPPLIER VALUES(@OSS_Outlet,@CheaperSupplier,@aProduct,1,GETDATE(),GETDATE(),@Cheaper_OSS_DOWGenerateOrder,@COST,@MinPrmCtnCost)

	
	SET @CheaperResult = @CheaperSupplier
	SET @COST = @MinPrmCtnCost
	SET @CoverDaysUsed = @Cheaper_OSS_DOWGenerateOrder
	
END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, @aProduct,'CDN_SP_GetCheaperSupplier',NULL);
END CATCH