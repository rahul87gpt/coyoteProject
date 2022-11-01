USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_DoAutoOrdering]    Script Date: 9/5/2020 1:42:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--DECLARE @aRunDate DATE = CAST(DATEADD(DD,0,GETDATE()) AS DATE)
--exec [CDN_SP_DoAutoOrdering] @aRunDate, @SelectedOutlet=735,@SelectedSupplier='1',@DebugProduct=1116220,@Debug=1
-- SELECT TOP 10 * FROM [OutletSupplierScheduleTbl] WHERE OSS_DOWGenerateOrder = 4	AND OSS_Supplier = '1'

ALTER PROCEDURE [dbo].[CDN_SP_DoAutoOrdering] @aRunDate AS DATE = NULL
	,@SelectedOutlet AS INT = NULL
	,@SelectedSupplier VARCHAR(100) = NULL
	,@aShowOptimalReport AS BIT = 0
	,@DoReCalc AS BIT = 0
	,@dayspan INT = 28
	,@Debug int = 0
	,@DebugProduct INT = NULL
AS
BEGIN TRY
	IF @DEBUG = 1 SELECT '[CDN_SP_DoAutoOrdering]' CDN_SP_DoAutoOrdering,@DebugProduct DebugProduct
	SET NOCOUNT ON
	IF (@aRunDate IS NULL)
		SET @aRunDate = CAST(GETDATE() as DATE)
	DECLARE @aSalesEndDate AS DATE = DATEADD(DD, -1, @aRunDate)
	DECLARE @aSalesStartDate AS DATE = DATEADD(DD, -@dayspan + 1, @aSalesEndDate)

	IF OBJECT_ID(N'tempdb.dbo.##OutletSupplierScheduleTbl', N'U') IS NOT NULL
	BEGIN
		--DROP TABLE ##OutletSupplierScheduleTbl
		EXEC [dbo].[CDN_SP_GetSchedule] @aRunDate
		RETURN
	END

	IF @DEBUG = 1 SELECT '[CDN_SP_DoAutoOrdering]' CDN_SP_DoAutoOrdering,* FROM [dbo].[OutletSupplierScheduleTbl]  WITH (NOLOCK) WHERE ( (@aShowOptimalReport = 1 AND OSS_Outlet=@SelectedOutlet)  
	OR (@aShowOptimalReport = 0 AND OSS_DOWGenerateOrder =  DATEPART(DW, @aRunDate) 
	-- AND (CAST(OSS_LastRun AS DATE) = @aRunDate OR OSS_LastRun IS NULL) 
	AND (@SelectedOutlet IS NULL OR OSS_Outlet=@SelectedOutlet) AND (@SelectedSupplier IS NULL OR OSS_Supplier=@SelectedSupplier)))
	
	SELECT *,0 is_Processed INTO  ##OutletSupplierScheduleTbl FROM 
	[dbo].[OutletSupplierScheduleTbl]  WITH (NOLOCK)
	WHERE
	( (@aShowOptimalReport = 1 AND OSS_Outlet=@SelectedOutlet) 
	  OR
	 (@aShowOptimalReport = 0 AND OSS_DOWGenerateOrder =  DATEPART(DW, @aRunDate) 
	 AND ( @DebugProduct IS NULL OR (CAST(OSS_LastRun AS DATE) < @aRunDate OR OSS_LastRun IS NULL)) 
	 AND (@SelectedOutlet IS NULL OR OSS_Outlet=@SelectedOutlet)
	 AND (@SelectedSupplier IS NULL OR OSS_Supplier=@SelectedSupplier)))
	
	----function DoOrderForListItems Start {
	
	DECLARE @OSS_Id INT
		,@OSS_Outlet INT
		,@OSS_Supplier VARCHAR(30)
		,@OSS_DOWGenerateOrder INT
		,@OSS_MultipleOrdersInAWeek BIT
		,@OSS_CoverDays INT
		,@OSS_InvoiceOrderOffset INT
		,@OSS_ReceiveOrderOffset INT
		,@OSS_DiscountThresholdThree FLOAT
		,@OSS_CoverDaysDiscountThreshold3 INT
		,@OSS_DiscountThresholdTwo FLOAT
		,@OSS_CoverDaysDiscountThreshold2 INT
		,@OSS_DiscountThresholdOne FLOAT
		,@OSS_CoverDaysDiscountThreshold1 INT
		,@OSS_OrderNonDefaultSupplier BIT
		,@cnt INT = 0

	SELECT @cnt = count(*) FROM ##OutletSupplierScheduleTbl WHERE is_Processed = 0

	WHILE @cnt > 0
	BEGIN
		SELECT TOP 1 @OSS_Id = OSS_Id
			,@OSS_Outlet = OSS_Outlet
			,@OSS_MultipleOrdersInAWeek = OSS_MultipleOrdersInAWeek
			,@OSS_Supplier = OSS_Supplier
			,@OSS_DOWGenerateOrder = OSS_DOWGenerateOrder
			,@OSS_CoverDays = OSS_CoverDays
			,@OSS_InvoiceOrderOffset = OSS_InvoiceOrderOffset			
			,@OSS_ReceiveOrderOffset =OSS_ReceiveOrderOffset
			,@OSS_DiscountThresholdThree =OSS_DiscountThresholdThree
			,@OSS_CoverDaysDiscountThreshold3 =OSS_CoverDaysDiscountThreshold3
			,@OSS_DiscountThresholdTwo =OSS_DiscountThresholdTwo
			,@OSS_CoverDaysDiscountThreshold2 =OSS_CoverDaysDiscountThreshold2
			,@OSS_DiscountThresholdOne =OSS_DiscountThresholdOne
			,@OSS_CoverDaysDiscountThreshold1 =OSS_CoverDaysDiscountThreshold1
			,@OSS_OrderNonDefaultSupplier =OSS_OrderNonDefaultSupplier
		FROM ##OutletSupplierScheduleTbl
		WHERE is_Processed = 0
	
		EXEC [dbo].CDN_SP__DoOptimalOrder 
			@aRunDate
			,@aSalesStartDate
			,@aSalesEndDate
			,@DoReCalc
			,@aShowOptimalReport
			,@OSS_Id
			,@OSS_Outlet
			,@OSS_DOWGenerateOrder
			,@OSS_MultipleOrdersInAWeek
			,@OSS_Supplier
			,@OSS_CoverDays
			,@OSS_InvoiceOrderOffset
			,@OSS_ReceiveOrderOffset 
			,@OSS_DiscountThresholdThree 
			,@OSS_CoverDaysDiscountThreshold3 
			,@OSS_DiscountThresholdTwo 
			,@OSS_CoverDaysDiscountThreshold2 
			,@OSS_DiscountThresholdOne 
			,@OSS_CoverDaysDiscountThreshold1 
			,@OSS_OrderNonDefaultSupplier 
			,@Debug =@Debug
			,@DebugProduct=@DebugProduct
		IF @Debug = 1 SELECT '[CDN_SP_DoAutoOrdering]' CDN_SP_DoAutoOrdering,@DebugProduct DebugProduct

		
		IF @DebugProduct IS NULL
		BEGIN
			UPDATE OutletSupplierScheduleTbl SET OSS_LastRun=GETDATE() WHERE OSS_Id = @OSS_Id
		END 

		UPDATE ##OutletSupplierScheduleTbl SET is_Processed = 1,OSS_LastRun=GETDATE() WHERE OSS_Id = @OSS_Id 
		

		SELECT @cnt = @cnt-1   --COUNT(*) FROM ##OutletSupplierScheduleTbl WHERE is_Processed = 0
	END;

	IF OBJECT_ID(N'tempdb.dbo.##OutletSupplierScheduleTbl', N'U') IS NOT NULL
		DROP TABLE ##OutletSupplierScheduleTbl


---------------------------CHEAPER SUPPLIER-------------------------
	IF OBJECT_ID(N'tempdb.dbo.##OutletSupplierScheduleCheaperTbl', N'U') IS NOT NULL
		DROP TABLE ##OutletSupplierScheduleCheaperTbl


	SELECT *,0 is_Processed INTO ##OutletSupplierScheduleCheaperTbl 
	FROM [dbo].[OutletSupplierScheduleTbl]  WITH (NOLOCK)
	INNER JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) 
	ON OSS_Outlet = BB.Outlet AND BB.supplier = OSS_Supplier AND BB.BEST_BUY = 1 AND BB.Product IS NOT NULL 
	WHERE
	( (@aShowOptimalReport = 1 AND OSS_Outlet=@SelectedOutlet) 
	  OR
	 (@aShowOptimalReport = 0 AND OSS_DOWGenerateOrder =  DATEPART(DW, @aRunDate) 
	-- AND (CAST(OSS_LastRun AS DATE) < @aRunDate OR OSS_LastRun IS NULL) 
	 AND (@SelectedOutlet IS NULL OR OSS_Outlet=@SelectedOutlet)
	 AND (@SelectedSupplier IS NULL OR OSS_Supplier=@SelectedSupplier)))
	


	SELECT @cnt = count(*) FROM ##OutletSupplierScheduleCheaperTbl WHERE is_Processed = 0
	
	IF @Debug = 1
	SELECT '[CDN_SP_DoAutoOrdering] CHAEPER LIST' CDN_SP_DoAutoOrdering,@cnt TotalCheaperProducts,*,0 is_Processed
	FROM [dbo].[OutletSupplierScheduleTbl]  WITH (NOLOCK)
	INNER JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) 
	ON OSS_Outlet = BB.Outlet AND BB.supplier = OSS_Supplier AND BB.BEST_BUY = 1 AND BB.Product IS NOT NULL 
	WHERE
	( (@aShowOptimalReport = 1 AND OSS_Outlet=@SelectedOutlet) 
	  OR
	 (@aShowOptimalReport = 0 AND OSS_DOWGenerateOrder =  DATEPART(DW, @aRunDate) 
	-- AND (CAST(OSS_LastRun AS DATE) < @aRunDate OR OSS_LastRun IS NULL) 
	 AND (@SelectedOutlet IS NULL OR OSS_Outlet=@SelectedOutlet)
	 AND (@SelectedSupplier IS NULL OR OSS_Supplier=@SelectedSupplier)))
	

	WHILE @cnt > 0
	BEGIN
		SET @DebugProduct=NULL
		SELECT TOP 1 @OSS_Id = OSS_Id
			,@OSS_Outlet = OSS_Outlet
			,@OSS_MultipleOrdersInAWeek = OSS_MultipleOrdersInAWeek
			,@OSS_Supplier = OSS_Supplier
			,@OSS_DOWGenerateOrder = OSS_DOWGenerateOrder
			,@OSS_CoverDays = OSS_CoverDays
			,@OSS_InvoiceOrderOffset = OSS_InvoiceOrderOffset			
			,@OSS_ReceiveOrderOffset =OSS_ReceiveOrderOffset
			,@OSS_DiscountThresholdThree =OSS_DiscountThresholdThree
			,@OSS_CoverDaysDiscountThreshold3 =OSS_CoverDaysDiscountThreshold3
			,@OSS_DiscountThresholdTwo =OSS_DiscountThresholdTwo
			,@OSS_CoverDaysDiscountThreshold2 =OSS_CoverDaysDiscountThreshold2
			,@OSS_DiscountThresholdOne =OSS_DiscountThresholdOne
			,@OSS_CoverDaysDiscountThreshold1 =OSS_CoverDaysDiscountThreshold1
			,@OSS_OrderNonDefaultSupplier =OSS_OrderNonDefaultSupplier
			,@DebugProduct=Product
		FROM ##OutletSupplierScheduleCheaperTbl
		WHERE is_Processed = 0
	
		EXEC [dbo].CDN_SP__DoOptimalOrder 
			@aRunDate
			,@aSalesStartDate
			,@aSalesEndDate
			,@DoReCalc
			,@aShowOptimalReport
			,@OSS_Id
			,@OSS_Outlet
			,@OSS_DOWGenerateOrder
			,@OSS_MultipleOrdersInAWeek
			,@OSS_Supplier
			,@OSS_CoverDays
			,@OSS_InvoiceOrderOffset
			,@OSS_ReceiveOrderOffset 
			,@OSS_DiscountThresholdThree 
			,@OSS_CoverDaysDiscountThreshold3 
			,@OSS_DiscountThresholdTwo 
			,@OSS_CoverDaysDiscountThreshold2 
			,@OSS_DiscountThresholdOne 
			,@OSS_CoverDaysDiscountThreshold1 
			,@OSS_OrderNonDefaultSupplier 
			,@Debug =@Debug
			,@DebugProduct=@DebugProduct
		IF @Debug = 1 SELECT '[CDN_SP_DoAutoOrdering] CHAEPER' CDN_SP_DoAutoOrdering,@DebugProduct DebugProduct,@cnt TotalCountRemain

		UPDATE ##OutletSupplierScheduleCheaperTbl SET is_Processed = 1,OSS_LastRun=GETDATE() WHERE OSS_Id = @OSS_Id and Product = @DebugProduct

		SELECT @cnt = @cnt-1 --COUNT(*) FROM ##OutletSupplierScheduleCheaperTbl WHERE is_Processed = 0 
	END;

	IF OBJECT_ID(N'tempdb.dbo.##OutletSupplierScheduleCheaperTbl', N'U') IS NOT NULL
		DROP TABLE ##OutletSupplierScheduleCheaperTbl

---------------------------CHEAPER SUPPLIER-------------------------




END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,    ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, NULL,'CDN_SP_DoAutoOrdering',NULL);
END CATCH