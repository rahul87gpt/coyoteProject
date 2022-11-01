Begin Transaction 
Begin Try
--DBCC CHECKIDENT('PromoMemberOffer', RESEED, 0)
insert into PromoMemberOffer (promotionId,productid,[Desc],Status,Action,
HostPromoType,AmtOffNorm1,PromoUnits,Price,Price1,Price2,Price3,price4,
isdeleted,createdAt,updatedAt,createdbyId,updatedById)
--SELECT * FROm PromoMemberOffer
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
	WHERE p.PromotionTypeId= 19847 AND PRMP_MEMBEROFFER Is NOT NULL
  )X
  LEFT OUTER JOIN Product p ON p.Number=x.PRMP_Product
--  WHERE p.Id Is null and  x.PRMP_Product Is not null

--commit transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 