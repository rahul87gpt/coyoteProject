Begin Transaction 
Begin Try
--SELECT * FROM MasterListItems WHERE ListId=83
--DBCC CHECKIDENT('RebateOutlets', RESEED,0)
--DELETE FROM RebateOutlets
INSERT INTO RebateOutlets
(
	RebateHeaderId
	, StoreId
	, CreatedById
	, UpdatedById
	, CreatedAt
	, UpdatedAt
	, IsDeleted
)
SELECT 
  New.Id RebateHeaderId
, s.Id StoreId
, 1 CreatedById
, 1 UpdatedById
, GETUTCDATE() CreatedAt
, GETUTCDATE() UpdatedAt
, 0  IsDeleted
FROm [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].RebateOutletTbl old
INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].RebateHeaderTbl h ON old.ROT_Hdr_Id=h.RHT_Id
INNER JOIN RebateHeader New ON h.RHT_Code=new.Code
INNER JOIN Store s ON s.Code=old.ROT_Outlet
COMMIT transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch --