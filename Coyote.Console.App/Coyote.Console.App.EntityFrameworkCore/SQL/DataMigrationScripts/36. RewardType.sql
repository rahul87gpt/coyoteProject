INSERT INTO MasterListItems
select 
  ListId
, Code
, Name
, Col1
, Col2
, Col3
, Col4
, Col5
, Num1
, Num2
, Num3
, Num4
, Num5
, Status
, 1
, GETUTCDATE()
, 1
, GETUTCDATE()
, 0 IsDeleted
, 1 AccessId
FROM [CoyoteConsoleApp_Dev].[dbo].[MasterListItems] WHERE ListId = 84