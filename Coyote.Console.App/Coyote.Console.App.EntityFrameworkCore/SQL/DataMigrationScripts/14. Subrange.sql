
SELECT * FROm MasterList WHERE CODE='SUBRANGE'
INSERt INTO MasterListItems(ListId,code,Name,Status,CreatedById,CreatedAt,UpdatedById,UpdatedAt,IsDeleted,AccessId)
SELECT 53, 'DRY', 'DRY',1, 1,GETUTCDATE(),1,GETUTCDATE(),0,1
INSERt INTO MasterListItems(ListId,code,Name,Status,CreatedById,CreatedAt,UpdatedById,UpdatedAt,IsDeleted,AccessId)
SELECT 53, 'DAIRY', 'DAIRY',1, 1,GETUTCDATE(),1,GETUTCDATE(),0,1
INSERt INTO MasterListItems(ListId,code,Name,Status,CreatedById,CreatedAt,UpdatedById,UpdatedAt,IsDeleted,AccessId)
SELECT 53, 'FROZEN', 'FROZEN',1, 1,GETUTCDATE(),1,GETUTCDATE(),0,1