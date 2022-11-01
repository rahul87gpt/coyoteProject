BEGIN TRANSACTION
BEGIN TRY
INSERt INTO MasterListItems(ListId
,code
,Name
,Status
,CreatedById
,CreatedAt
,UpdatedById
,UpdatedAt
,IsDeleted
,AccessId)

SELECT 
  1
, '99999'
, 'NEW_UNALLOCATE'
, 1
, 1
, GETUTCDATE()
, 1
, GETUTCDATE()
, 0
, 1
--COMMIT TRANSACTION
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