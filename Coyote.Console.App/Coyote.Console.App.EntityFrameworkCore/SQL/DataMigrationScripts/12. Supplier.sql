
--DBCC CHECKIDENT('Supplier', RESEED, 0)
BEGIN TRANSACTION

BEGIN TRY
	INSERT INTO [CoyoteConsoleApp_Staging].[dbo].[Supplier] (
		 [Code]
		,[Desc]
		,[Address1]
		,[Address2]
		,[Address3]
		,Phone
		,Fax
		,[Email]
		,[ABN]
		,[UpdateCost]
		,[PromoCode]
		,[ContactName]
		,[CostZone]
		,[GSTFreeItemCode]
		,[GSTFreeItemDesc]
		,[GSTInclItemCode]
		,[GSTInclItemDesc]
		,[XeroName]
		,[IsDeleted]
		,[CreatedAt]
		,[UpdatedAt]
		,[CreatedById]
		,[UpdatedById]
		)
	SELECT 
	     Cast([CODE_KEY_ALP] AS VARCHAR(max)) Code
		,[CODE_DESC] [Desc]
		,[CODE_ALP_1] [Address1]
		,[CODE_ALP_2] [Address2]
		,[CODE_ALP_3] [Address3]
		,[CODE_ALP_5] [Phone]
		,[CODE_ALP_6] [Fax]
		,[CODE_ALP_8] [Email]
		,[CODE_ALP_11] ABN
		,[CODE_ALP_9] [UpdateCost]
		,[CODE_ALP_10] [PromoCode]
		,[CODE_ALP_7] [ContactName]
		,[CODE_ALP_12] [CostZone]
		,[CODE_ALP_13] [GSTFreeItemCode]
		,[CODE_ALP_14] [GSTFreeItemDesc]
		,[CODE_ALP_15] [GSTInclItemCode]
		,[CODE_ALP_16] [GSTInclItemDesc]
		,[CODE_ALP_17] [XeroName]
		,0 [IsDeleted]
		,GETDATE() [CreatedAt]
		,GETDATE() [UpdatedAt]
		,1 [CreatedById]
		,1 [UpdatedById]
	FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].CODETBL 
	WHERE CODE_KEY_TYPE='SUPPLIER'
	
		--commit transaction
END TRY

BEGIN CATCH
	SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

	ROLLBACK TRANSACTION
END CATCH



