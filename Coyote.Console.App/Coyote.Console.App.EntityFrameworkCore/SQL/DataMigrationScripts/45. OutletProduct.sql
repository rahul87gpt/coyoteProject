--SELECT * FROM Product order by Id desc
BEGIN TRANSACTION
 BEGIN TRY 
 --DBCC CHECKIDENT('OutletProduct', RESEED,0)
 --DELETE FROM OutletProduct
 INSERT INTO OutletProduct
 (
 StoreId
,ProductId
,SupplierId
,Status
,Till
,OpenPrice
,NormalPrice1
,NormalPrice2
,NormalPrice3
,NormalPrice4
,NormalPrice5
,CartonCost
,CartonCostHost
,CartonCostInv
,CartonCostAvg
,PromoPrice1
,PromoPrice2
,PromoPrice3
,PromoPrice4
,PromoPrice5
,PromoCartonCost
,QtyOnHand
,MinOnHand
,MaxOnHand
,MinReorderQty
,PickingBinNo
,ChangeLabelInd
,ChangeTillInd
,HoldNorm
,ChangeLabelPrinted
,LabelQty
,ShortLabelInd
,SkipReorder
,SpecPrice
,SpecCode
,SpecFrom
,SpecTo
,GenCode
,SpecCartonCost
,ScalePlu
,FifoStock
,Mrp
,CreatedAt
,UpdatedAt
,CreatedById
,UpdatedById
,IsDeleted
,PromoBuyId
,PromoCompId
,PromoMemOffPrice1
,PromoMemOffPrice2
,PromoMemOffPrice3
,PromoMemOffPrice4
,PromoMemOffPrice5
,PromoMemeberOfferId
,PromoMixMatch1Id
,PromoMixMatch2Id
,PromoOffer1Id
,PromoOffer2Id
,PromoOffer3Id
,PromoOffer4Id
,PromoSellId
,AllMember
,PromoBuy
,PromoComp
,PromoMemOff
,PromoMix1
,PromoMix2
,PromoOffer1
,PromoOffer2
,PromoOffer3
,PromoOffer4
,PromoSell
 )
SELECT
	s.Id StoreId
--,OUTP_OUTLET
    , p.Id  ProductId
--,OUTP_PRODUCT
	,(CASE WHEN sup.Id Is NULL AND OUTP_SUPPLIER Is NOT NULL THEN 612 ELSE sup.Id END) SupplierId
--,OUTP_SUPPLIER
	, CASE 
	WHEN [OUTP_STATUS] = 'Active'
	THEN 1
	ELSE 0
	END [STATUS]
	, CASE 
	WHEN [OUTP_TILL_YN] = 'Y'
	THEN 1
	ELSE 0
	END AS	Till
	, CASE 
	WHEN [OUTP_OPEN_PRICE_YN] = 'Y'
	THEN 1
	WHEN [OUTP_OPEN_PRICE_YN] IS NULL
	THEN NULL
	ELSE 0
	END AS OpenPrice	
	, isnull(NULLIF([OUTP_NORM_PRICE_1], ''), 0) NormalPrice1
	,  [OUTP_NORM_PRICE_2]  NormalPrice2
	,  [OUTP_NORM_PRICE_3]  NormalPrice3
	,  [OUTP_NORM_PRICE_4]  NormalPrice4
	,  [OUTP_NORM_PRICE_5]  NormalPrice5
	, ISNULL(NULLIF([OUTP_CARTON_COST], ''), 0) CartonCost
	, [OUTP_CARTON_COST_HOST] CartonCostHost
	, [OUTP_CARTON_COST_INV]  CartonCostInv
	, [OUTP_CARTON_COST_AVG]  CartonCostAvg
	, [OUTP_PROM_PRICE_1] PromoPrice1
	, [OUTP_PROM_PRICE_2] PromoPrice2
	, [OUTP_PROM_PRICE_3] PromoPrice3
	, [OUTP_PROM_PRICE_4] PromoPrice4
	, [OUTP_PROM_PRICE_5] PromoPrice5
	, [OUTP_PROM_CTN_COST] PromoCartonCost
	, Isnull(NULLIF([OUTP_QTY_ONHAND], ''), 0) QtyOnHand
	, isnull(NULLIF([OUTP_MIN_ONHAND], ''), 0) MinOnHand
	,  [OUTP_MAX_ONHAND]  MaxOnHand 
	,  [OUTP_MIN_REORDER_QTY]  MinReorderQty
	,  [OUTP_PICKING_BIN_NO]  PickingBinNo
	, CASE 
	WHEN [OUTP_CHANGE_LABEL_IND] = 'Y'
	THEN 1
	WHEN [OUTP_CHANGE_LABEL_IND] = 'Yes'
	THEN 1
	ELSE 0
	END ChangeLabelInd
	, CASE 
	WHEN [OUTP_CHANGE_TILL_IND] = 'Yes'
	THEN 1
	ELSE 0
	END ChangeTillInd
	,  [OUTP_HOLD_NORM]  HoldNorm
	,  [OUTP_CHANGE_LABEL_PRINTED]  ChangeLabelPrinted
	,  [OUTP_LABEL_QTY] LabelQty 
	, CASE 
	WHEN [OUTP_SHORT_LABEL_IND] = 'Yes'
	THEN 1
	ELSE 0
	END ShortLabelInd
	, CASE 
	WHEN [OUTP_SKIP_REORDER_YN] = 'Y'
	THEN 1
	ELSE 0
	END SkipReorder
	,  [OUTP_SPEC_PRICE]  SpecPrice
	,  [OUTP_SPEC_CODE]  SpecCode
	, [OUTP_SPEC_FROM] SpecFrom
	, [OUTP_SPEC_TO] SpecTo
	, [OUTP_GEN_CODE]  GenCode
	, [OUTP_SPEC_CARTON_COST]  SpecCartonCost
	, [OUTP_SCALE_PLU]  ScalePlu
	, CASE 
	WHEN [OUTP_FIFO_STOCK_YN] = 'Y'
	THEN 1
	ELSE 0
	END FifoStock
	, [OUTP_Max_Retail_Price] Mrp
	, CASE WHEN OUTP_TIMESTAMP IS NULL THEN GETUTCDATE() ELSE OUTP_TIMESTAMP END	CreatedAt
	,  CASE WHEN OUTP_TIMESTAMP IS NULL THEN GETUTCDATE() ELSE OUTP_TIMESTAMP END	UpdatedAt
	, 1	CreatedById
	, 1	UpdatedById
	, 0	IsDeleted
	, buy.Id
--,OUTP_BUY_PROM_CODE
--,OUTP_COMP
	,com.Id
	--, (SELECT ID FROM Promotion WHERE code=OUTP_BUY_PROM_CODE AND IsDeleted=0 AND PromotionTypeId =19844)		PromoBuyId
	--, (Select Id from CompetitionDetail WHERE  Code =OUTP_COMP AND IsDeleted=0)	PromoCompId
	, OUTP_MEM_OFF_PRICE_1	PromoMemOffPrice1
	, OUTP_MEM_OFF_PRICE_2	PromoMemOffPrice2
	, OUTP_MEM_OFF_PRICE_3	PromoMemOffPrice3
	, OUTP_MEM_OFF_PRICE_4	PromoMemOffPrice4
	, OUTP_MEM_OFF_PRICE_5	PromoMemOffPrice5
--,OUTP_MEMBEROFFER
	,MemberOffer.Id
--,OUTP_MIXMATCH
	,MixMatch1.Id
--,OUTP_MIXMATCH2
	,MixMatch2.Id
--,OUTP_OFFER
--,OUTP_OFFER2
--,OUTP_OFFER3
--,OUTP_OFFER4
	,PromoOffer1.Id
	,PromoOffer2.Id
	,PromoOffer3.Id
	,PromoOffer4.Id
--,OUTP_SELL_PROM_CODE
	,sell.Id
	--, (SELECT ID FROM Promotion WHERE code=OUTP_OFFER   AND IsDeleted=0 AND PromotionTypeId =19843)	PromoOffer1Id
	--, (SELECT ID FROM Promotion WHERE code=OUTP_OFFER2  AND IsDeleted=0 AND PromotionTypeId =19843)	PromoOffer2Id
	--, (SELECT ID FROM Promotion WHERE code=OUTrP_OFFER3  AND IsDeleted=0 AND PromotionTypeId =19843)	PromoOffer3Id
	--, (SELECT ID FROM Promotion WHERE code=OUTP_OFFER4  AND IsDeleted=0 AND PromotionTypeId =19843)	PromoOffer4Id
	--, (SELECT ID FROM Promotion WHERE code=OUTP_SELL_PROM_CODE AND IsDeleted=0 AND PromotionTypeId =19845)	PromoSellId
	, CASE WHEN OUTP_ALL_MEMBERS_YN='Y' THEN 1 ELSE 0 END 	AllMember
	, CASE WHEN OUTP_PROM_BUY_YN='Y' THEN 1 ELSE 0 END	PromoBuy
	, CASE WHEN OUTP_PROM_COMP_YN='Y' THEN 1 ELSE 0 END	PromoComp
	, CASE WHEN OUTP_PROM_MEMBEROFFER_YN='Y' THEN 1 ELSE 0 END	PromoMemOff
	, CASE WHEN OUTP_PROM_MIX_YN='Y' THEN 1 ELSE 0 END	PromoMix1
	, CASE WHEN OUTP_PROM_MIX2_YN='Y' THEN 1 ELSE 0 END	PromoMix2
	, CASE WHEN OUTP_PROM_OFFER_YN='Y' THEN 1 ELSE 0 END	PromoOffer1
	, CASE WHEN OUTP_PROM_OFFER2_YN='Y' THEN 1 ELSE 0 END	PromoOffer2
	, CASE WHEN OUTP_PROM_OFFER3_YN='Y' THEN 1 ELSE 0 END	PromoOffer3
	, CASE WHEN OUTP_PROM_OFFER4_YN='Y' THEN 1 ELSE 0 END	PromoOffer4
	, CASE WHEN OUTP_PROM_SELL_YN='Y' THEN 1 ELSE 0 END	PromoSell

FROM OUTPTBL o
LEFT JOIN Store s ON s.Code=o.OUTP_OUTLET
LEFT JOIN Product p ON p.Number=o.OUTP_PRODUCT
LEFT JOIN Supplier sup ON sup.Code=o.OUTP_SUPPLIER
LEFT OUTER JOIN
(
	SELECT * FROM PROMOTION p 
	WHERE PromotionTypeId =19844
)buy ON buy.Code=o.OUTP_BUY_PROM_CODE
LEFT OUTER JOIN
(
	SELECT * FROM PROMOTION p 
	WHERE PromotionTypeId =19845
)sell ON sell.Code=o.OUTP_SELL_PROM_CODE
LEFT OUTER JOIN
(
	Select c.* from CompetitionDetail c
	INNER JOIN Promotion p ON p.Id=c.PromotionId
	WHERE PromotionTypeId =19846
)com ON com.Code=o.OUTP_COMP
LEFT OUTER JOIN 
(
	SELECT * FROM Promotion 
	WHERE PromotionTypeId =19847
)MemberOffer ON MemberOffer.Code=OUTP_MEMBEROFFER
LEFT OUTER JOIN
(
	SELECT * FROM Promotion 
	WHERE PromotionTypeId =19848
)MixMatch1 ON MixMatch1.Code=o.OUTP_MIXMATCH
LEFT OUTER JOIN
(
	SELECT * FROM Promotion 
	WHERE  PromotionTypeId =19848
)MixMatch2 ON MixMatch2.Code=o.OUTP_MIXMATCH2
LEFT OUTER JOIN 
(
	SELECT * FROM Promotion 
	WHERE  PromotionTypeId =19843
)PromoOffer1 ON PromoOffer1.Code=OUTP_OFFER
LEFT OUTER JOIN 
(
	SELECT * FROM Promotion 
	WHERE  PromotionTypeId =19843
)PromoOffer2 ON PromoOffer2.Code=OUTP_OFFER2
LEFT OUTER JOIN 
(
	SELECT * FROM Promotion 
	WHERE  PromotionTypeId =19843
)PromoOffer3 ON PromoOffer3.Code=OUTP_OFFER3
LEFT OUTER JOIN 
(
	SELECT * FROM Promotion 
	WHERE  PromotionTypeId =19843
)PromoOffer4 ON PromoOffer4.Code=OUTP_OFFER4

WHERE 
s.Id Is NOT NULL AND p.Id Is NOT NULL
--commit transaction

end try

begin catch
   SELECT
ERROR_NUMBER() AS ErrorNumber,
ERROR_SEVERITY() AS ErrorSeverity,
ERROR_STATE() AS ErrorState,
ERROR_PROCEDURE() AS ErrorProcedure,
ERROR_LINE() AS ErrorLine,
ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch
