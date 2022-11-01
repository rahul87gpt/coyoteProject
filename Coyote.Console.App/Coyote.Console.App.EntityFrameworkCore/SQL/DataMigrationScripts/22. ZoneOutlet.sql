--SELECT * FROM MasterListItems WHERE ListId=1
 --DBCC CHECKIDENT('ZoneOutlet', RESEED, 0)
BEGIN TRANSACTION
BEGIN TRY
INSERt INTO ZoneOutlet(
 StoreId
,ZoneId
,CreatedAt
,UpdatedAt
,CreatedById
,UpdatedById
,IsDeleted)

SELECT 
  ISNULL((SELECT ID FROm STORE WHERE CODE = CODE_KEY_NUM AND IsDeleted=0),114) StoreId
, ISNULL((SELECT ID FROM MasterListItems WHERE ListId =  1 AND Code=CODE_Key_ALP AND IsDeleted=0),19842)
--, CODE_Key_ALP
, CASE WHEN CODE_TIMESTAMP IS NULL THEN GETUTCDATE() ELSE  CODE_TIMESTAMP END
, CASE WHEN CODE_TIMESTAMP IS NULL THEN GETUTCDATE() ELSE  CODE_TIMESTAMP END
, 1
, 1
, 0
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].CODETBL 
WHERE CODE_Key_Type='ZONEOUTLET'
--AND (SELECT ID FROm STORE WHERE CODE = CODE_KEY_NUM AND IsDeleted=0) IS NULL
COMMIT TRANSACTION
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

--SELECT * FROm MasterListItems WHERE ListId=1

----Not Found In the DB99
-----------------------------------------------------------
--P3-REDBULL
--HOSTUPD
--P3-REDBULL
--P3-REDBULL
--P3-REDBULL
--P3-REDBULL
--PEDS16
--P3-REDBULL
--FSOVEG
--P3-REDBULL
--FSOVEG
--P3-REDBULL
--P3-REDBULL
--FSOVEG
--P3-REDBULL
--FSOVEG
--FSOVEG
--P1205-SCHW_SMIT

--FSOVEG


--Outlet
------------------
--- 997 Out letnot found