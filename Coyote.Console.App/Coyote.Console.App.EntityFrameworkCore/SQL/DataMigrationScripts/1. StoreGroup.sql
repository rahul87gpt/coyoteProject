INSERT INTO [CoyoteConsoleApp_Staging].[dbo].[StoreGroup] (
  [Code]
 , [Name]
 , [Status]
 , [CreatedAt]
 , [UpdatedAt]
 , [CreatedById]
 , [UpdatedById]
 , [IsDeleted]
 ) 
 SELECT CASE 
  WHEN [CODE_KEY_NUM] IS NULL
   THEN NULL
  ELSE cast([CODE_KEY_NUM] AS VARCHAR(max))
  END
 , CASE 
  WHEN [CODE_DESC] IS NULL
   THEN NULL
  ELSE cast([CODE_DESC] AS VARCHAR(max))
  END
 , cast('1' AS VARCHAR(max))
 , getutcdate()
 , getutcdate()
 , 1
 , 1
 , 0 
 FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[CODETBL] 
 WHERE code_key_type LIKE 'STOREGROUP'
 --DELETE FROm StoreGroup
 --DBCC CHECKIDENT('StoreGroup', RESEED, 0)

 --SELECT * FROm StoreGroup