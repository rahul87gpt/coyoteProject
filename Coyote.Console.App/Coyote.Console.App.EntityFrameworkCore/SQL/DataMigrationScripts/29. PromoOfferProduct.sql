--DBCC CHECKIDENT('PromoOfferProduct', RESEED, 0)
--DELETE FROm PromoOfferProduct
Begin Transaction 
Begin Try
--SELECT * FROm PromoOfferProduct
insert into PromoOfferProduct (PromotionOfferId,ProductId,[Desc],Status,Action,HostPromoType,AmtOffNorm1,PromoUnits,IsDeleted,CreatedAt,UpdatedAt,CreatedById,UpdatedById,OfferGroup)

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
  , 0 d
  , GETDATE() c
  , GETDATE() u
  , 1 cb
  , 1 ub 
  , ISNULL(PRMP_OFFER_GROUP,' ')
  FROM  --#PROMOOFFER x
  (
	SELECT 
		new.Id,offer.*
	FROM PRMPTBL offer
	INNER JOIN Promotion p ON offer. PRMP_PROM_Code=p.Code
	INNER JOIN PromoOffer new ON p. Id=new.PromotionId
	--INNER JOIN Promotion p ON New.PromotionId=p.Id
	WHERE p.PromotionTypeId= 19843
  )X
  LEFT OUTER JOIN Product p ON p.Number=x.PRMP_Product
--  WHERE p.Id Is null and  x.PRMP_Product Is not null

   --COMMIT TRANSACTION
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 

















  ----------------------------- PRODUCNT NUMBR NOT FOUND
 /*31914
79478
86354
95101
98106
98574
98879
1090
35135
75413
95112
95489
97881
98846
98914
99596
99876
99879
10000025
10000035
10000051
10000052
1089
20020
35865
86327
95118
97821
99741
99977
1913130
2018579
10000026
7071
19993
60665
70312
73246
76999
81483
97997
1913774
10000030
10000034*/