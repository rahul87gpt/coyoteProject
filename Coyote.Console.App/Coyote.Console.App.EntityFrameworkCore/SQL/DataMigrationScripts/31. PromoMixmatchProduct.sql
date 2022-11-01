Begin Transaction 
Begin Try

--DBCC CHECKIDENT('PromoMixmatchProduct', RESEED,0)
insert into PromoMixmatchProduct (PromotionMixmatchId,ProductId,[Desc],Status,Action,HostPromoType,AmtOffNorm1,PromoUnits,IsDeleted,CreatedAt,UpdatedAt,CreatedById,UpdatedById)
--DELETE FROM PromoMixmatchProduct
--SELECT * FROm Product Where Id=280037
SELECT 
   x.Id
 ,ISNULL(p.Id,280037) Prod
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
 , 0 d
 , GETDATE() c
 , GETDATE() u
 , 1 cb
 , 1 ub 

FROM (
    SELECT 
		new.Id,offer.*
	FROM PRMPTBL offer
	INNER JOIN Promotion p ON offer.PRMP_PROM_CODE=p.Code
	INNER JOIN PromoMixmatch new ON p.Id=new.PromotionId
	--INNER JOIN Promotion p ON New.PromotionId=p.Id
	WHERE p.PromotionTypeId= 19848
)X
LEFT OUTER JOIN Product p ON p.Number=x.PRMP_Product
  --commit transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 

