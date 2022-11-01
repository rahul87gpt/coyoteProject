INSERt INTO MasterListItems
SELECT * FROM (
Select 
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
,GETUTCDATE() c
,UpdatedById
,GETUTCDATE() u
,IsDeleted
,AccessId from [CoyoteConsoleApp_Dev].[dbo].MasterListItems Where ListId=83 and Id in (19527
,19528
,19529
,19530
,19531
,19532
,19533
,19534
,19536)
UNION 
Select ListId
,'GROUP9' code
,'Group9' nam
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
,GETUTCDATE() c
,UpdatedById
,GETUTCDATE() u
,IsDeleted
,AccessId from [CoyoteConsoleApp_Dev].[dbo].MasterListItems Where ListId=83 and Id =19527)x

SELECT * FROm MasterListItems WHERE ListId=83