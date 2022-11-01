--Select * from HostSettings
BEGIN TRANSACTION
--DELETE FROm Product
BEGIN TRY
INSERt INTO HostSettings
(
Code
,Description
,InitialLoadFileWeekly
,WeeklyFile
,FilePathID
,NumberFactor
,SupplierID
,WareHouseID
,BuyPromoPrefix
,SellPromoPrefix
,IsActive
,CreatedDate
,ModifiedDate
,CreatedBy
,ModifiedBy
,HostFormatID
)
SELECT 
  CODE_KEY_ALP  [CODE]
, CODE_DESC     [Desc]
, CODE_ALP_1    [Initial Load File]
, CODE_ALP_2    [Weekly Load File]
, (SELECT ID FROM Paths WHERE PathType=(CASE WHEN CODE_ALP_3='METCASH_PATH' THEN 12 END) And OutletID Is NULL)    [Path]
, CODE_ALP_4    [Number factor]
, (SELECT ID FROM SUPPLIER WHERE CODE =CAST(CODE_ALP_5 AS VARCHAR) AND IsDeleted=0)    [Supplier]
, (SELECT ID FROM Warehouse WHERE Code=CAST(CODE_NUM_1 AS VARCHAR) AND IsDeleted=0)    [WareHouse] 
, CODE_ALP_6    [Buy Promo Prefix]
, CODE_ALP_7    [Sell Promo Prefix]
, 1 IsActive
, GETUTCDATE()
, GETUTCDATE()
, 1
, 1
, (SELECT Id FROm MasterListItems WHERE CODE=CAST( CODE_ALP_8 AS varchar) AND ISDeleted=0 AND ListId=7)   [Host Formate]

FROm [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].CODETBL 
WHERE CODE_Key_Type='HOST'

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