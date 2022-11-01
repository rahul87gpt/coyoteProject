Begin Transaction 
Begin Try
--DBCC CHECKIDENT('PromoSelling', RESEED, 0)
--SELECT * from PromoSelling
INSERT INTO PromoSelling (
		promotionId
		,productid
		,[Desc]
		,STATUS
		,Action
		,HostPromoType
		,AmtOffNorm1
		,PromoUnits
		,Price
		,Price1
		,Price2
		,Price3
		,price4
		,isdeleted
		,createdAt
		,updatedAt
		,createdbyId
		,updatedById
		)
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
	WHERE p.PromotionTypeId= 19845 --AND PRMP_MEMBEROFFER Is NOT NULL
  )X
  LEFT OUTER JOIN Product p ON p.Number=x.PRMP_Product
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