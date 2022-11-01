 --CODE_TIMESTAMP = CreatedAt and UpdatedAt in each code table
  --SELECT  *  FROm HostSettings order by id desc
  BEGIN TRANSACTION
 
--DBCC CHECKIDENT('CostPriceZones', RESEED, 0)
BEGIN TRY
SELECT * FROM CostPriceZones
  
  INSERT INTO CostPriceZones( 
     Code
	,Type
	,Description
	,HostSettingID
	,Factor1
	,Factor2
	,Factor3
	,SuspUpdOutlet
	,IsActive
	,CreatedDate
	,ModifiedDate
	,CreatedBy
	,ModifiedBy)
  SELECT 
   CODE_KEY_ALP [Code]
 , 1 
 , CODE_DESC  [desc]
 , ISNULL((SELECT ID FROM HostSettings WHERE Code=CODE_ALP_1 ),25)  [HOST]
 , CODE_ALP_2 [Factore 1 :  Service Fee]
 , CODE_ALP_3  [Factore 2 :  Dry Del Fee]
 , CODE_ALP_4 [Factore 1 :  Presiable Del Fee]
 , CASE WHEN CODE_ALP_5='Y' THEN 1 ELSE 0 END  [Suspend Outlet]
 , 1
 , CASE WHEN CODE_TIMESTAMP IS NULL THEN GETUTCDATE() ELSE  CODE_TIMESTAMP END
 , CASE WHEN CODE_TIMESTAMP IS NULL THEN GETUTCDATE() ELSE  CODE_TIMESTAMP END
 , 1 
 , 1
 FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[CODETBL] where [CODE_KEY_TYPE] like 'COStZONE'

 		--Commit TRANSACTION
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