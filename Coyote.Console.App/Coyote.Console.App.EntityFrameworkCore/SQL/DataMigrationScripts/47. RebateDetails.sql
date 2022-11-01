Begin Transaction 
Begin Try
--SELECT * FROM MasterListItems WHERE ListId=83
--DBCC CHECKIDENT('RebateDetails', RESEED,0)
--SELECT * FROM RebateDetails
INSERT INTO RebateDetails
(
      RebateHeaderId
	, ProductId
	, Amount
	, CreatedById
	, UpdatedById
	, CreatedAt
	, UpdatedAt
	, IsDeleted
)
SELECT  
	  New.Id
	, p.Id
	, RDT_Rebate
	, 1 CreatedById
	, 1 UpdatedById
	, GETUTCDATE() CreatedAt
	, GETUTCDATE() UpdatedAt
	, 0  IsDeleted
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].RebateDetailTbl old
INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].RebateHeaderTbl h ON old.RDT_Header_Id=h.RHT_Id
INNER JOIN RebateHeader New ON h.RHT_Code=new.Code
INNER JOIN Product p ON p.Number=old.RDT_Product
COMMIT transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 