--DBCC CHECKIDENT('Commodity', RESEED, 0)
BEGIN TRANSACTION

BEGIN TRY
	INSERT INTO Commodity (
		Code
		,[Desc]
		,CoverDays
		,DepartmentId
		,GPPcntLevel1
		,GPPcntLevel2
		,GPPcntLevel3
		,GPPcntLevel4
		,STATUS
		,CreatedAt
		,UpdatedAt
		,CreatedById
		,UpdatedById
		,IsDeleted
		)
	
		SELECT code_key_num AS code
			,code_desc
			,NULL AS cover
			,ISNULL((
				SELECT id
				FROM Department
				WHERE Code = cast(Code_Num_1 AS VARCHAR(max))
				),52) dept
			,Code_Num_11
			,Code_Num_12
			,Code_Num_13
			,Code_Num_14
			,0 STATUS
			,GETDATE() created
			,GETDATE() updated
			,1 createdby
			,1 updatedby
			,0 deleted
		FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[CODETBL]
		WHERE Code_key_type = 'commodity'
		
	--COMMIT TRANSACTION
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

