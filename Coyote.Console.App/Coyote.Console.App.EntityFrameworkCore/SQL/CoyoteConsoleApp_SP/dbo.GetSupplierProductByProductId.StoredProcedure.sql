/****** Object:  StoredProcedure [dbo].[GetSupplierProductByProductId]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSupplierProductByProductId]
@ProductId BIGINT 
AS
BEGIN
	SELECT 
		 suplir_prd.Id
		,suplir_prd.SupplierId
		,suplir_prd.SupplierItem
		,suplir_prd.ProductId
		,suplir_prd.[Desc]
		,suplir_prd.[Status]
		,suplir_prd.CartonCost
		,suplir_prd.MinReorderQty
		,suplir_prd.BestBuy
		,suplir.[Code] SupplierCode
		,suplir.[Desc] SupplierDesc
		,NULL LastInvoiceCost
		,suplir_prd.CreatedAt
		,suplir_prd.UpdatedAt
		,suplir_prd.CreatedById
		,suplir_prd.UpdatedById  
	FROM SupplierProduct suplir_prd 
	INNER JOIN Supplier suplir ON suplir_prd.SupplierId=suplir.Id
	WHERE suplir_prd.ProductId=@ProductId and suplir_prd.IsDeleted=0
END
GO
