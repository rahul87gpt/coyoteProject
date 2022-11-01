Begin Transaction 
Begin Try
--DBCC CHECKIDENT('PromoBuying', RESEED, 0)
--SELECT COUNT(1) from PromoBuying
INSERT INTO PromoBuying (
		PromotionId
		,ProductId
		,[Desc]
		,Status
		,Action
		,HostPromoType
		,AmtOffNorm1
		,PromoUnits
		,CostStart
		,CostEnd
		,CostIsPromInd
		,CartonCost
		,CartonQty
		,SupplierId
		,IsDeleted
		,CreatedAt
		,UpdatedAt
		,CreatedById
		,UpdatedById
		)
SELECT 
   x.Id
,  PROD
,  PRMP_Desc
,  [STATUS]
,  [Action]
,  [Host]
,  PRMP_AMT_OFF_NORM_1
,  PRMP_Promo_Units
,   PRMP_COST_START
  , PRMP_COST_END
  , CASE WHEN PRMP_COST_IS_PROM_IND='Yes' THEN 1 ELSE 0 END 
  , ISNULL(PRMP_CARTON_COST,0)
  , ISNULL(PRMP_CARTON_QTY,0)
  , s.Id
  , d,c,u,cb,UB
FROm (
SELECT 
     x.Id 
  , ISNULL(p.Id,280037) Prod
  , PRMP_Desc
  , CASE 
   WHEN PRMP_Status = 'Active'
    THEN 1
   ELSE 0
   END AS STATUS
  , nullif(PRMP_ACTION, '') Action
  , nullif(PRMP_HOST_PROM_TYPE, '') AS Host
  , PRMP_AMT_OFF_NORM_1
  , PRMP_Promo_Units
  , PRMP_COST_START
  , PRMP_COST_END
  , PRMP_COST_IS_PROM_IND
  , PRMP_CARTON_COST
  , PRMP_CARTON_QTY
  ,PRMP_SUPPLIER
  --, (SELECT ID FROM SUPPLIER WHERE CODE =PRMP_SUPPLIER AND IsDeleted=0) SupplierId
  ,ISNULL(PRMP_PRice_1,0) as Price
  ,PRMP_PRICE_2
  ,PRMP_PRICE_3
  ,PRMP_PRICE_4
  ,PRMP_PRICE_5
  , 0 d
  , GETUTCDATE() c
  , GETUTCDATE() u
  , 1 cb
  , 1 ub 
  FROM  --#PROMOOFFER x
  (
	SELECT 
		p.Id,offer.*
	FROM PRMPTBL offer
	INNER JOIN Promotion p ON offer. PRMP_PROM_Code=p.Code
	WHERE p.PromotionTypeId= 19844 --AND PRMP_MEMBEROFFER Is NOT NULL
  )X
  LEFT OUTER JOIN Product p ON p.Number=x.PRMP_Product
 )x
 LEFT OUTER JOIN Supplier s ON s.Code=x.PRMP_SUPPLIER
 --WHERE PRMP_Promo_Units Is NOT NULL
-- )X
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
  --SELECT * FROM Promotion WHERE PromotionTypeId= 19845
  --Select * from MasterListItems WHERE ListId=10