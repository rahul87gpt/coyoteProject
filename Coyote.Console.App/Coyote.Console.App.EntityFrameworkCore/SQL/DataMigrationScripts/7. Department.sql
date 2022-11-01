--SELECT * FROM Department

--BudgetGroethFactor=Code_ALP_2
--RoyaltyDisc=Code_ALP_3
--AdvertisingDisc=Code_ALP_4
--AllowSaleDisc=Code_ALP_5
--ExcludeWastageOptimalOrdering=Code_ALP_6

--DBCC CHECKIDENT('Department', RESEED, 0)
insert into Department (Code
,[Desc]
,MapTypeId
,BudgetGroethFactor
,RoyaltyDisc
,AdvertisingDisc
,AllowSaleDisc
,ExcludeWastageOptimalOrdering
,IsDefault
,Status
,CreatedAt
,UpdatedAt
,CreatedById
,UpdatedById
,IsDeleted)
SELECT 
   cast(CODE_KEY_NUM AS VARCHAR(max)) Code
 , Code_Desc [Desc]
 , (SELECT Id FROM MasterListItems WHERE COde= Code_ALP_1 AND ListId=65) MapTypeId
 , Code_ALP_2 BudgetGroethFactor
 , CODE_ALP_3 RoyaltyDisc
 , CODE_ALP_4 AdvertisingDisc
 , CODE_ALP_5 AllowSaleDisc
 , CODE_ALP_6 ExcludeWastageOptimalOrdering
 , 0
 , 1
 , GETUTCDATE()
 , GETUTCDATE()
 , 1
 , 1
 , 0 
 FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[CODETBL] 
 WHERE Code_key_type = 'DEPARTMENT'
 
 
 -- INSERt INTO Department
-- (Code
-- ,[Desc]
-- ,MapTypeId
-- ,BudgetGroethFactor
-- ,RoyaltyDisc
-- ,AdvertisingDisc
-- ,AllowSaleDisc
-- ,ExcludeWastageOptimalOrdering
-- ,IsDefault
-- ,[Status]
-- ,CreatedAt
-- ,UpdatedAt
-- ,CreatedById
-- ,UpdatedById
-- ,IsDeleted)

-- SELECT 
 -- '999'
-- ,'NEW_UNALLOCATE'
-- ,MapTypeId
-- ,BudgetGroethFactor
-- ,RoyaltyDisc
-- ,AdvertisingDisc
-- ,AllowSaleDisc
-- ,ExcludeWastageOptimalOrdering
-- ,IsDefault
-- ,Status
-- ,CreatedAt
-- ,UpdatedAt
-- ,CreatedById
-- ,UpdatedById
-- ,IsDeleted
 -- FROm Department WHERE CODE=0
 