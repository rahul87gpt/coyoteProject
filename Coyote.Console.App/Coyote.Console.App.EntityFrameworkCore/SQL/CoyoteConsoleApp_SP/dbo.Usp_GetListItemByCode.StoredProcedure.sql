/****** Object:  StoredProcedure [dbo].[Usp_GetListItemByCode]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetListItemByCode]
 @ListCode NVARCHAR(100)
,@SkipCount INT = NULL
,@MaxResultCount   INT = NULL
AS
BEGIN
	DECLARE @SqlQuery NVARCHAR(MAX)=NULL
	SET @SqlQuery=''
	SET @SqlQuery = @SqlQuery +'
	SELECT 
		 mstListItm.Id
		,mstListItm.ListId
		,mstListItm.Code
		,mstListItm.[Name]
		,mstListItm.Col1
		,mstListItm.Col2
		,mstListItm.Col3
		,mstListItm.Col4
		,mstListItm.Col5
		,mstListItm.Num1
		,mstListItm.Num2
		,mstListItm.Num3
		,mstListItm.Num4
		,mstListItm.Num5
		,mstListItm.Status
		,mstListItm.CreatedById
		,mstListItm.CreatedAt
		,mstListItm.UpdatedById
		,mstListItm.UpdatedAt
		,mstListItm.IsDeleted 
	FROM MasterList mstList
	INNER JOIN MasterListItems mstListItm ON mstList.Id=mstListItm.ListId
	WHERE mstList.IsDeleted=0 
	  AND mstListItm.IsDeleted=0 
	  AND mstList.Code='''+@ListCode+'''
	   ORDER BY mstListItm.UpdatedAt'
	IF @MaxResultCount IS NOT NULL
	BEGIN
		SET @SqlQuery = @SqlQuery +' OFFSET ('+CAST(@SkipCount AS VARCHAR)+') ROWS FETCH NEXT ISNULL('+CAST(@MaxResultCount AS VARCHAR)+',0) ROWS ONLY'
	END

	EXECUTE(@SqlQuery)
END
GO
