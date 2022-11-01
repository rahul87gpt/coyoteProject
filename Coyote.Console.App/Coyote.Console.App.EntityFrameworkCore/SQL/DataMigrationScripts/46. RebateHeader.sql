Begin Transaction 
Begin Try
--SELECT * FROM MasterListItems WHERE ListId=83
--DBCC CHECKIDENT('RebateHeader', RESEED,0)
--DELETE FROM RebateHeader
INSERT INTO RebateHeader
(
Code
,Description
,Type
,ManufacturerId
,ZoneId
,StartDate
,EndDate
,CreatedById
,UpdatedById
,CreatedAt
,UpdatedAt
,IsDeleted
)
SELECT 
	  [RHT_Code]
	, [RHT_Desc]
	, [RHT_Type]
	, (SELECT ID FROM MasterListItems WHERE CODE= [RHT_Manufacturer] AND ListId=3) ManufacturerId
	, (SELECT ID FROM MasterListItems WHERE CODE= [RHT_Zone] AND ListId=1) ZoneId
	, [RHT_Start_Date]
	, [RHT_End_Date]
	, 1 CreatedById
	, 1 UpdatedById
	, GETUTCDATE() CreatedAt
	, GETUTCDATE() UpdatedAt
	, 0  IsDeleted
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].RebateHeaderTbl
COMMIT transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 

