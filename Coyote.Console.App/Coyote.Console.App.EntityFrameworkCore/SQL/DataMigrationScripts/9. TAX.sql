begin transaction
 begin try 
 INSERT INTO TAX
SELECT 
  CODE_KEY_ALP Code
, CODE_DESC [Desc]
, CODE_NUM_1 Factor
, 1 Status
, GETUTCDATE() CreatedAt
, GETUTCDATE() UpdatedAt
, 1 CreatedById
, 1 UpdatedById
, 0 IsDeleted
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[CODETBL] where [CODE_KEY_TYPE] = 'TAXCODE'
--commit transaction


end try

begin catch
   SELECT
ERROR_NUMBER() AS ErrorNumber,
ERROR_SEVERITY() AS ErrorSeverity,
ERROR_STATE() AS ErrorState,
ERROR_PROCEDURE() AS ErrorProcedure,
ERROR_LINE() AS ErrorLine,
ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch

