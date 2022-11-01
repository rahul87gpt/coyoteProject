Begin Transaction 
Begin Try
--DBCC CHECKIDENT('PrintLabelType', RESEED,0)
--DELETE FROM PrintLabelType
--SELECT * FROM PrintLabelType
INSERT INTO PrintLabelType
(
	  Code
	, [Desc]
	, LablesPerPage
	, Status
	, CreatedAt
	, UpdatedAt
	, CreatedById
	, UpdatedById
	, IsDeleted
	, PrintBarCodeType
)
SELECT 
        [CODE_KEY_ALP]
      , [CODE_DESC]
	  , [CODE_NUM_1]
      , 1 Status
	  ,	GETUTCDATE() CreatedAt
	  , GETUTCDATE()UpdatedAt
	  , 1 CreatedById
	  , 1 UpdatedById
	  , 0 IsDeleted
      , [CODE_ALP_1]
      
  FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[CODETBL]
  WHERE [CODE_KEY_TYPE] like '%LABELTYPE%'
  --AND [CODE_TIMESTAMP] Is NOT NULL
  COMMIT TRANSACTION
END TRY
BEGIN CATCH
   SELECT ERROR_NUMBER() AS ERRORNUMBER,ERROR_SEVERITY() AS ERRORSEVERITY,ERROR_STATE() AS ERRORSTATE,ERROR_PROCEDURE() AS ERRORPROCEDURE,
   ERROR_LINE() AS ERRORLINE,ERROR_MESSAGE() AS ERRORMESSAGE;
  ROLLBACK TRANSACTION
END CATCH 