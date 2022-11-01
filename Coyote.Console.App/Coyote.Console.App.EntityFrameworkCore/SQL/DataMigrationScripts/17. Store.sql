BEGIN TRANSACTION
BEGIN TRY 
   INSERT INTO STORE (
 Code
,GroupId
,Address1
,Address2
,Address3
,PhoneNumber
,Fax
,PostCode
,[Status]
,[Desc]
,Email
,SellingInd
,StockInd
,DelName
,DelAddr1
,DelAddr2
,DelAddr3
,DelPostCode
,CostType
,Abn
,BudgetGrowthFact
,EntityNumber
,WarehouseId
,PriceFromLevel
,Latitude
,Longitude
,CreatedAt
,UpdatedAt
,OpenHours
,FuelSite
,NameOnApp
,AddressOnApp
,DisplayOnApp
,AppOrders
,CreatedById
,UpdatedById
,IsDeleted
,CostZoneId
,PriceLevelDesc1
,PriceLevelDesc2
,PriceLevelDesc3
,PriceLevelDesc4
,PriceLevelDesc5
,PriceZoneId
,LabelTypePromoId
,LabelTypeShelfId
,LabelTypeShortId
,OutletPriceFromOutletId
,CostInd)
	SELECT 
	  OUTL_OUTLET Code
	,(SELECT ID FROM StoreGroup WHERE CODE=STOR_STOREGROUP  ) GroupId
	--,STOR_STOREGROUP
	, OUTL_ADDR_1 Address1
	, OUTL_ADDR_2 Address2
	, OUTL_ADDR_3 Address3
	, OUTL_PHONE PhoneNumber
	, OUTL_FAX Fax
	, OUTL_POST_CODE PostCode
	, CASE WHEN OUTL_STATUS='Active' THEN 1 ELSE 0 END  [Status]
	, OUTL_DESC [Desc]
	,  OUTL_Email Email
	, CASE WHEN OUTL_SELLING_IND='Yes' THEN 1 ELSE 0 END  SellingInd
	, CASE WHEN OUTL_STOCK_IND='Yes' THEN 1 ELSE 0 END  StockInd
	,  OUTL_DEL_NAME DelName
	,  OUTL_DEL_ADDR_1 DelAddr1
	,  OUTL_DEL_ADDR_2 DelAddr2
	,  OUTL_DEL_ADDR_3 DelAddr3
	,  OUTL_DEL_POST_CODE DelPostCode
	,  OUTL_COST_TYPE CostType
	, STOR_ABN Abn
	, OUTL_BUDGET_GROWTH_FACT BudgetGrowthFact
	,  OUTL_Entity_Number EntityNumber
	,  (SELECT Id FROm Warehouse WHERE CODE=OUTL_WAREHOUSE AND IsDeleted=0) WarehouseId
	,  OUTL_PRICE1_FROM_LEV PriceFromLevel
	,  CAST(OUTL_Latitude AS FLOAT) Latitude
	,  CAST(OUTL_Longitude AS FLOAT) Longitude
	,CASE WHEN OUTL_TIMESTAMP IS NULL THEN  GETDATE() ELSE OUTL_TIMESTAMP END -- OUTL_TIMESTAMP CreatedAt
	,CASE WHEN OUTL_Last_Modified_Date IS NULL THEN GETDATE() ELSE OUTL_Last_Modified_Date END -- OUTL_Last_Modified_Date UpdatedAt
	, OUTL_Open_Hours OpenHours
	, ISNULL(OUTL_Fuel_Site,0) FuelSite
	, OUTL_Name_On_App NameOnApp
	, OUTL_Address_On_App AddressOnApp
	, ISNULL(OUTL_Display_On_App,0) DisplayOnApp
	, ISNULL(OUTL_App_Orders,0) AppOrders
	,1 CreatedById
	,1 UpdatedById
	, 0 IsDeleted
	,NULL --OUTL_COST_ZONE CostZoneId
	, OUTL_PRICE_LEVEL1_DESC PriceLevelDesc1
	, OUTL_PRICE_LEVEL2_DESC PriceLevelDesc2
	, OUTL_PRICE_LEVEL3_DESC PriceLevelDesc3
	, OUTL_PRICE_LEVEL4_DESC PriceLevelDesc4
	, OUTL_PRICE_LEVEL5_DESC PriceLevelDesc5
	,NULL--OUTL_PRICE_ZONE PriceZoneId
	,NULL--OUTL_LABELTYPE_PROMO LabelTypePromoId
	,NULL --OUTL_LABELTYPE_NORMAL LabelTypeShelfId
	,NULL--OUTL_LABELTYPE_SHORT LabelTypeShortId
	, OUTL_PRICE_FROM_OUTLET OutletPriceFromOutletId
	, CASE WHEN OUTL_INV_UPD_COST_IND='Yes' THEN 1 ELSE 0 END  CostInd
	FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].OUTLTBL o
	INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].StorTBL s ON s.STOR_STORE=o.OUTL_OUTLET
----COMMIT transaction
--DBCC CHECKIDENT('Store', RESEED, 0)
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