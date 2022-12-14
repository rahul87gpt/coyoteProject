/****** Object:  StoredProcedure [dbo].[usp_GetProductNumber]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetProductNumber]
 @SkipCount INT = NULL
,@MaxResultCount   INT = NULL
AS
SET NOCOUNT ON
DECLARE @Number BIGINT = 0
SELECT @Number = MAX(NUMBER) FROM PRODUCT
SELECT ISNULL(@Number,0) + 1 NUMBER

EXECUTE Usp_GetListItemByCode 'CATEGORY',@SkipCount,@MaxResultCount
EXECUTE Usp_GetListItemByCode 'GROUP',@SkipCount,@MaxResultCount
EXECUTE Usp_GetListItemByCode 'MANUFACTURER',@SkipCount,@MaxResultCount
EXECUTE Usp_GetListItemByCode 'NATIONALRANGE',@SkipCount,@MaxResultCount
EXECUTE Usp_GetListItemByCode 'PRODUCT_TYPE',@SkipCount,@MaxResultCount
EXECUTE Usp_GetListItemByCode 'UNITMEASURE',@SkipCount,@MaxResultCount
EXECUTE usp_GetActiveCommodities NULL,@SkipCount,@MaxResultCount,NULL,NULL
EXECUTE usp_GetActiveDepartments NULL,@SkipCount,@MaxResultCount,NULL,NULL
EXECUTE usp_Get_AllActiveSupplier NULL,@SkipCount,@MaxResultCount,NULL,NULL,0
EXECUTE Usp_GetActiveTax NULL,@SkipCount,@MaxResultCount,NULL,NULL,0
GO
