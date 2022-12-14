USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_GetSchedule]    Script Date: 9/5/2020 1:48:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CDN_SP_GetSchedule]
@RunDate Date,
@CallingType BIT=NULL -- 0 MEANS DELETE TABLE 1 MEANS EXECUTE OPTIMAL ORDER
AS
BEGIN
	IF OBJECT_ID(N'tempdb.dbo.##OutletSupplierScheduleTbl', N'U') IS NOT NULL
	BEGIN
		IF @CallingType = 0
		BEGIN
			DROP TABLE ##OutletSupplierScheduleTbl
			EXEC [CDN_SP_GetSchedule] @RunDate
		END
		ELSE
			SELECT CAST(1 as bit) PROCESSING
				  ,CAST(IS_Processed as bit) Processed 
				  ,OSS_Id Id
				  ,OSS_Outlet Outlet
				  ,OSS_Supplier Supplier
				  ,OSS_DOWGenerateOrder					DOWGenerateOrder 
				  ,OSS_SendOrderOffset 					SendOrderOffset 
				  ,OSS_ReceiveOrderOffset				ReceiveOrderOffset
				  ,OSS_LastRun							LastRun
				  ,OSS_DateCreated						DateCreated
				  ,OSS_DateLastChanged					DateLastChanged
				  ,OSS_DateLastChangedUser				DateLastChangedUser
				  ,OSS_InvoiceOrderOffset				InvoiceOrderOffset
				  ,OSS_DiscountThresholdOne				DiscountThresholdOne
				  ,OSS_DiscountThresholdTwo				DiscountThresholdTwo
				  ,OSS_DiscountThresholdThree			DiscountThresholdThree
				  ,OSS_CoverDaysDiscountThreshold1		CoverDaysDiscountThreshold1
				  ,OSS_CoverDaysDiscountThreshold2		CoverDaysDiscountThreshold2
				  ,OSS_CoverDaysDiscountThreshold3		CoverDaysDiscountThreshold3
				  ,OSS_CoverDays						CoverDays
				  ,OSS_MultipleOrdersInAWeek			MultipleOrdersInAWeek
				  ,OSS_OrderNonDefaultSupplier			OrderNonDefaultSupplier
				From ##OutletSupplierScheduleTbl
	END
	ELSE
		IF @CallingType = 1 
			BEGIN
				EXEC CDN_SP_DoAutoOrdering @RunDate
			END
		ELSE
				SELECT 
				CAST(0 as bit) PROCESSING,
				CAST(CASE WHEN OSS_LastRun >= @RunDate THEN 1 ELSE 0 END as bit) Processed 
				  ,OSS_Id Id
				  ,OSS_Outlet Outlet
				  ,OSS_Supplier Supplier
				  ,OSS_DOWGenerateOrder					DOWGenerateOrder 
				  ,OSS_SendOrderOffset 					SendOrderOffset 
				  ,OSS_ReceiveOrderOffset				ReceiveOrderOffset
				  ,OSS_LastRun							LastRun
				  ,OSS_DateCreated						DateCreated
				  ,OSS_DateLastChanged					DateLastChanged
				  ,OSS_DateLastChangedUser				DateLastChangedUser
				  ,OSS_InvoiceOrderOffset				InvoiceOrderOffset
				  ,OSS_DiscountThresholdOne				DiscountThresholdOne
				  ,OSS_DiscountThresholdTwo				DiscountThresholdTwo
				  ,OSS_DiscountThresholdThree			DiscountThresholdThree
				  ,OSS_CoverDaysDiscountThreshold1		CoverDaysDiscountThreshold1
				  ,OSS_CoverDaysDiscountThreshold2		CoverDaysDiscountThreshold2
				  ,OSS_CoverDaysDiscountThreshold3		CoverDaysDiscountThreshold3
				  ,OSS_CoverDays						CoverDays
				  ,OSS_MultipleOrdersInAWeek			MultipleOrdersInAWeek
				  ,OSS_OrderNonDefaultSupplier			OrderNonDefaultSupplier
		From OutletSupplierScheduleTbl 
		WHERE OSS_DOWGenerateOrder = DATEPART(DW,@RunDate)
		--AND (CAST(OSS_LastRun AS DATE) < @RunDate OR OSS_LastRun IS NULL)
END

