BEGIN TRANSACTION 
BEGIN TRY
--SELECT * FROM MasterListItems WHERE ListId=83
--DBCC CHECKIDENT('Recipe', RESEED,0)
--DELETE FROM Recipe
--SELECT * FROM Recipe
INSERT INTO Recipe
(
	  ProductID
	, OutletID
	, IngredientProductID
	, RecipeTimeStamp
	, Description
	, Qty
	, IsParents
	, IsActive
	, CreatedDate
	, ModifiedDate
	, CreatedBy
	, ModifiedBy
)
SELECT
	  p.Id ProductId 
	, s.Id OutletId
	, ingrnd.Id IngredientProductId
	, CASE WHEN RECP_TIMESTAMP IS NULL THEN GETUTCDATE() ELSE RECP_TIMESTAMP END
	, RECP_RECIPE_DESC
	, RECP_QTY 
	, (CASE WHEN ingrnd.Id IS NULL THEN 1 ELSE 0 END) IsParents
	, 1 Active
	, GETUTCDATE() CreatedAt
	, GETUTCDATE() UpdatedAt
	, 1 CreatedById
	, 1 UpdatedById
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].RECPTBL old
LEFT OUTER JOIN store s ON old.RECP_OUTLET=s.code
INNER JOIN Product p ON p.Number=old.RECP_PRODUCT
LEFT OUTER JOIN Product ingrnd ON ingrnd.Number=old.RECP_INGREDIENT_PRODUCT
COMMIT TRANSACTION
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  ROLLBACK TRANSACTION
end catch 