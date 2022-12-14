/****** Object:  StoredProcedure [dbo].[usp_Get_AllActiveSupplier]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[usp_Get_AllActiveSupplier]  
 @GlobalFilter NVARCHAR(100) = NULL  
,@PageNumber INT = NULL  
,@PageSize   INT = NULL  
,@SortColumn NVARCHAR(50) = NULL  
,@SortDirection NVARCHAR(50) = NULL
,@IsCountRequired BIT = NULL  
AS  
BEGIN  
 DECLARE @TotalRecordCount INT = 0, @SQLQuery NVARCHAR(MAX)  

IF OBJECT_ID('tempdb..#Supplier') IS NOT NULL  
  DROP TABLE #Supplier  
  
 CREATE TABLE #Supplier_Result(  
  [Id] [int] NOT NULL,  
  [Code] [nvarchar](30) NOT NULL,  
  [Desc] [nvarchar](80) NOT NULL,  
  [Address1] [nvarchar](80) NULL,  
  [Address2] [nvarchar](80) NULL,  
  [Address3] [nvarchar](80) NULL,  
  [Phone] [nvarchar](20) NULL,  
  [Fax] [nvarchar](20) NULL,  
  [Email] [nvarchar](30) NULL,  
  [ABN] [nvarchar](15) NULL,  
  [ContactName] [nvarchar](30) NULL,  
  [UpdateCost] [nvarchar](3) NULL,  
  [PromoCode] [nvarchar](30) NULL,  
  [CostZone] [nvarchar](30) NULL,  
  [GSTFreeItemCode] [nvarchar](30) NULL,  
  [GSTFreeItemDesc] [nvarchar](80) NULL,  
  [GSTInclItemCode] [nvarchar](30) NULL,  
  [GSTInclItemDesc] [nvarchar](80) NULL,  
  [XeroName] [nvarchar](50) NULL,  
  [CreatedAt] [datetime] NOT NULL,  
  [UpdatedAt] [datetime] NOT NULL,  
  [CreatedById] [int] NOT NULL,  
  [UpdatedById] [int] NOT NULL,  
  )  
 SELECT * INTO #Supplier FROM(  
 SELECT Id  
 ,Code  
 ,[Desc]  
 ,Address1  
 ,Address2  
 ,Address3  
 ,Phone  
 ,Fax  
 ,Email  
 ,ABN  
 ,ContactName  
 ,UpdateCost  
 ,PromoCode  
 ,CostZone  
 ,GSTFreeItemCode  
 ,GSTFreeItemDesc  
 ,GSTInclItemCode  
 ,GSTInclItemDesc  
 ,XeroName  
 ,CreatedAt  
 ,UpdatedAt  
 ,CreatedById  
 ,UpdatedById   
 FROM Supplier supler  
 WHERE supler.IsDeleted=0  
 AND   
 (@GlobalFilter IS NULL OR (supler.Code like '%'+@GlobalFilter+'%' OR supler.[Desc] like '%'+@GlobalFilter+'%'))  
 )X  
 SET @TotalRecordCount = @@ROWCOUNT  
  
 IF @PageNumber IS NOT NULL AND @PageSize IS NOT NULL  
 BEGIN  
  INSERT INTO #Supplier_Result   
  SELECT * FROM  #Supplier   
  ORDER BY UpdatedAt DESC  
  OFFSET (@PageNumber) ROWS FETCH NEXT @PageSize ROWS ONLY  
 END   
 ELSE  
 BEGIN  
  INSERT INTO #Supplier_Result   
  SELECT * FROM  #Supplier   
  ORDER BY UpdatedAt DESC  
 END  
 IF @SortColumn IS NULL  
 BEGIN  
  SELECT * FROM #Supplier_Result ORDER BY UpdatedAt DESC   
 END  
 ELSE  
 BEGIN  
  IF @SortDirection IS NULL  
   BEGIN  
    SET @SQLQuery = ''  
    SET @SQLQuery = @SQLQuery + ' SELECT * FROM #Supplier_Result ORDER BY '+@SortColumn+' ASC'  
    EXECUTE(@SQLQuery)  
    PRINT @SQLQuery  
   END  
  ELSE  
   BEGIN  
    SET @SQLQuery = ''  
    SET @SQLQuery = @SQLQuery + ' SELECT * FROM #Supplier_Result ORDER BY '+@SortColumn+' DESC'  
    EXECUTE(@SQLQuery)  
    PRINT @SQLQuery  
   END   
 END  
 IF @IsCountRequired IS NULL OR @IsCountRequired = 1
 BEGIN
	SELECT @TotalRecordCount TotalRecordCount  
 END 
 DROP TABLE #Supplier  
 DROP TABLE #Supplier_Result  
END
GO
