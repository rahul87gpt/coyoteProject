INSERt INTO MasterListItems
SELECT 
ListId
,Code
,Name
,Col1
,Col2
,Col3
,Col4
,Col5
,Num1
,Num2
,Num3
,Num4
,Num5
,Status
,CreatedById
,GETUTCDATE()
,UpdatedById
,GETUTCDATE()
,IsDeleted
,AccessId 
FROM [CoyoteConsoleApp_Dev].[dbo].MasterListItems where ListId=64