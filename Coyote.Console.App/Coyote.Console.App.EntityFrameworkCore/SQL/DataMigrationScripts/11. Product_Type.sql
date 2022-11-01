Select * from MasterList WHERE Id=72

INSERt INTO MasterListItems(ListId
,code
,Name
,Status
,CreatedById
,CreatedAt
,UpdatedById
,UpdatedAt
,IsDeleted
,AccessId)
SELECT 
 72
, 'Host'
, 'Host'
,1
, 1
,GETUTCDATE()
,1
,GETUTCDATE()
,0
,1
INSERt INTO MasterListItems(ListId
,code
,Name
,Status
,CreatedById
,CreatedAt
,UpdatedById
,UpdatedAt
,IsDeleted
,AccessId)
SELECT 
 72
, 'Direct'
, 'Direct'
,1
, 1
,GETUTCDATE()
,1
,GETUTCDATE()
,0
,1
INSERt INTO MasterListItems(ListId
,code
,Name
,Status
,CreatedById
,CreatedAt
,UpdatedById
,UpdatedAt
,IsDeleted
,AccessId)
SELECT 
 72
, 'InStore'
, 'InStore'
,1
, 1
,GETUTCDATE()
,1
,GETUTCDATE()
,0
,1
