--SELECT * FROm APN
--DBCC CHECKIDENT('APN', RESEED, 0)
BEGIN TRANSACTION
--DELETE FROm Product
BEGIN TRY

INSERT INTO APN(
Number
,ProductId
,SoldDate
,Status
,CreatedAt
,UpdatedAt
,CreatedById
,UpdatedById
,IsDeleted
,[Desc]
)
SELECT 
	 APN_NUMBER
	,ISNULL(p.Id,280037) ProductId
	,APN_DATE_SOLD
	,1 AS STATUS
	,GETDATE() created
	,GETDATE() updated
	,1 createdby
	,1 updatedby
	,0 deleted
	,ISNULL(p.[Desc],' ') AS [Desc]
FROM APNTBL a
LEFT OUTER JOIN Product p ON a.APN_Product=p.Number
		--Commit TRANSACTION
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
--WHERE APN_Number=934583600010096




--PRODUCT has not found in the DB 99 while migrating APN

--638395
--10000016
--642726
--10000017
--10000019
--10000015
--10000018
--661932
--661712
--10000022
--638379
--10000023