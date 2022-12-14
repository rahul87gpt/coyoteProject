/****** Object:  StoredProcedure [dbo].[GetPromotionDetailsByProductId]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPromotionDetailsByProductId]
@ProductId BIGINT 
AS
BEGIN
	SELECT 
		 prmMix_prd.[Action]
		,prm_tp.[Name] PromotionType
		,prm_freqncy.[Code] Frequency
		,prm_freqncy.[Name] FrequencyDesc
		,prm_zn.[Name] [Zone]
		,CASE WHEN prm.SourceId ='1' THEN 'MANUAL' ELSE 'HOST' END [Source]
		,NULL CartoonCost
		,NULL MixMatch
		,NULL Offer
		,NULL Price1
		,NULL Price2
		,NULL Price3
		,NULL Price4
		,NULL PromotionProduct
		,prm.Id
		,prm.Code
		,prm.PromotionTypeId
		,prm.[Desc]
		,prm.[Status]
		,prm.SourceId
		,prm.ZoneId
		,prm.[Start]
		,prm.[End]
		,prm.RptGroup
		,prm.FrequencyId
		,prm.Availibility
		,prm.ImagePath
		,prm.IsDeleted
		,prm.CreatedAt
		,prm.UpdatedAt
		,prm.CreatedById
		,prm.UpdatedById  
	FROM PromoMixmatchProduct prmMix_prd 
	INNER JOIN PromoMixmatch prmMix ON prmMix_prd.PromotionMixmatchId=prmMix.Id 
	INNER JOIN Promotion prm ON prmMix.PromotionId=prm.id
	INNER JOIN MasterListItems prm_freqncy ON prm.FrequencyId=prm_freqncy.Id
	INNER JOIN MasterListItems prm_tp ON prm.PromotionTypeId=prm_tp.Id
	INNER JOIN MasterListItems prm_zn ON prm.ZoneId=prm_zn.Id
	WHERE prmMix_prd.ProductId=@ProductId AND prmMix_prd.IsDeleted = 0
	UNION
	SELECT 
		 prmOff_prd.[Action]
		,prm_tp.[Name] PromotionType
		,prm_freqncy.[Code] Frequency
		,prm_freqncy.[Name] FrequencyDesc
		,prm_zn.[Name] [Zone]
		,CASE WHEN prm.SourceId ='1' THEN 'MANUAL' ELSE 'HOST' END [Source]
		,NULL CartoonCost
		,NULL MixMatch
		,NULL Offer
		,NULL Price1
		,NULL Price2
		,NULL Price3
		,NULL Price4
		,NULL PromotionProduct
		,prm.Id
		,prm.Code
		,prm.PromotionTypeId
		,prm.[Desc]
		,prm.[Status]
		,prm.SourceId
		,prm.ZoneId
		,prm.[Start]
		,prm.[End]
		,prm.RptGroup
		,prm.FrequencyId
		,prm.Availibility
		,prm.ImagePath
		,prm.IsDeleted
		,prm.CreatedAt
		,prm.UpdatedAt
		,prm.CreatedById
		,prm.UpdatedById 
	FROM PromoOfferProduct prmOff_prd 
	INNER JOIN PromoOffer prmOff ON prmOff_prd.PromotionOfferId=prmOff.Id 
	INNER JOIN Promotion prm ON prmOff.PromotionId=prm.id
	INNER JOIN MasterListItems prm_freqncy ON prm.FrequencyId=prm_freqncy.Id
	INNER JOIN MasterListItems prm_tp ON prm.PromotionTypeId=prm_tp.Id
	INNER JOIN MasterListItems prm_zn ON prm.ZoneId=prm_zn.Id
	WHERE prmOff_prd.ProductId=@ProductId AND prmOff_prd.IsDeleted = 0
	UNION
	SELECT 
		 prmSell.[Action]
		,prm_tp.[Name] PromotionType
		,prm_freqncy.[Code] Frequency
		,prm_freqncy.[Name] FrequencyDesc
		,prm_zn.[Name] [Zone]
		,CASE WHEN prm.SourceId ='1' THEN 'MANUAL' ELSE 'HOST' END [Source]
		,NULL CartoonCost
		,NULL MixMatch
		,NULL Offer
		,prmSell.Price1 Price1
		,prmSell.Price2 Price2
		,prmSell.Price3 Price3
		,prmSell.Price4 Price4
		,NULL PromotionProduct
		,prm.Id
		,prm.Code
		,prm.PromotionTypeId
		,prm.[Desc]
		,prm.[Status]
		,prm.SourceId
		,prm.ZoneId
		,prm.[Start]
		,prm.[End]
		,prm.RptGroup
		,prm.FrequencyId
		,prm.Availibility
		,prm.ImagePath
		,prm.IsDeleted
		,prm.CreatedAt
		,prm.UpdatedAt
		,prm.CreatedById
		,prm.UpdatedById 
	FROM PromoSelling prmSell 
	INNER JOIN Promotion prm ON prmSell.PromotionId=prm.id
	INNER JOIN MasterListItems prm_freqncy ON prm.FrequencyId=prm_freqncy.Id
	INNER JOIN MasterListItems prm_tp ON prm.PromotionTypeId=prm_tp.Id
	INNER JOIN MasterListItems prm_zn ON prm.ZoneId=prm_zn.Id
	WHERE prmSell.ProductId=@ProductId AND prmSell.IsDeleted = 0
	UNION
	SELECT 
		 prmBuy.[Action]
		,prm_tp.[Name] PromotionType
		,prm_freqncy.[Code] Frequency
		,prm_freqncy.[Name] FrequencyDesc
		,prm_zn.[Name] [Zone]
		,CASE WHEN prm.SourceId ='1' THEN 'MANUAL' ELSE 'HOST' END [Source]
		,prmBuy.CartonCost CartoonCost
		,NULL MixMatch
		,NULL Offer
		,NULL Price1
		,NULL Price2
		,NULL Price3
		,NULL Price4
		,NULL PromotionProduct
		,prm.Id
		,prm.Code
		,prm.PromotionTypeId
		,prm.[Desc]
		,prm.[Status]
		,prm.SourceId
		,prm.ZoneId
		,prm.[Start]
		,prm.[End]
		,prm.RptGroup
		,prm.FrequencyId
		,prm.Availibility
		,prm.ImagePath
		,prm.IsDeleted
		,prm.CreatedAt
		,prm.UpdatedAt
		,prm.CreatedById
		,prm.UpdatedById 
	FROM PromoBuying prmBuy 
	INNER JOIN Promotion prm ON prmBuy.PromotionId=prm.id 
	INNER JOIN MasterListItems prm_freqncy ON prm.FrequencyId=prm_freqncy.Id
	INNER JOIN MasterListItems prm_tp ON prm.PromotionTypeId=prm_tp.Id
	INNER JOIN MasterListItems prm_zn ON prm.ZoneId=prm_zn.Id
	WHERE prmBuy.ProductId=@ProductId AND prmBuy.IsDeleted = 0
	UNION
	SELECT 
		 prmMemOff.[Action]
		,prm_tp.[Name] PromotionType
		,prm_freqncy.[Code] Frequency
		,prm_freqncy.[Name] FrequencyDesc
		,prm_zn.[Name] [Zone]
		,CASE WHEN prm.SourceId ='1' THEN 'MANUAL' ELSE 'HOST' END [Source]
		,NULL CartoonCost
		,NULL MixMatch
		,NULL Offer
		,prmMemOff.Price1 Price1
		,prmMemOff.Price2 Price2
		,prmMemOff.Price3 Price3
		,prmMemOff.Price4 Price4
		,NULL PromotionProduct
		,prm.Id
		,prm.Code
		,prm.PromotionTypeId
		,prm.[Desc]
		,prm.[Status]
		,prm.SourceId
		,prm.ZoneId
		,prm.[Start]
		,prm.[End]
		,prm.RptGroup
		,prm.FrequencyId
		,prm.Availibility
		,prm.ImagePath
		,prm.IsDeleted
		,prm.CreatedAt
		,prm.UpdatedAt
		,prm.CreatedById
		,prm.UpdatedById 
	FROM PromoMemberOffer prmMemOff 
	INNER JOIN Promotion prm ON prmMemOff.PromotionId=prm.id
	INNER JOIN MasterListItems prm_freqncy ON prm.FrequencyId=prm_freqncy.Id
	INNER JOIN MasterListItems prm_tp ON prm.PromotionTypeId=prm_tp.Id
	INNER JOIN MasterListItems prm_zn ON prm.ZoneId=prm_zn.Id
	WHERE prmMemOff.ProductId=@ProductId AND prmMemOff.IsDeleted = 0
	UNION
	SELECT 
		 NULL [Action]
		,prm_tp.[Name] PromotionType
		,NULL Frequency
		,NULL FrequencyDesc
		,prm_zn.[Name] [Zone]
		,CASE WHEN prm.SourceId ='1' THEN 'MANUAL' ELSE 'HOST' END [Source]
		,NULL CartoonCost
		,NULL MixMatch
		,NULL Offer
		,NULL Price1
		,NULL Price2
		,NULL Price3
		,NULL Price4
		,NULL PromotionProduct
		,prm.Id
		,prm.Code
		,prm.PromotionTypeId
		,prm.[Desc]
		,prm.[Status]
		,prm.SourceId
		,prm.ZoneId
		,prm.[Start]
		,prm.[End]
		,prm.RptGroup
		,prm.FrequencyId
		,prm.Availibility
		,prm.ImagePath
		,prm.IsDeleted
		,prm.CreatedAt
		,prm.UpdatedAt
		,prm.CreatedById
		,prm.UpdatedById 
	FROM PromotionCompetition prmcompt 
	INNER JOIN CompetitionDetail comp_dtl ON prmcompt.ProductId=prmcompt.ProductId AND prmcompt.CompetitionId=comp_dtl.Id
	INNER JOIN Promotion prm ON comp_dtl.PromotionId=prm.id
	INNER JOIN MasterListItems prm_tp ON prm.PromotionTypeId=prm_tp.Id
	INNER JOIN MasterListItems prm_zn ON prm.ZoneId=prm_zn.Id
	WHERE prmcompt.ProductId=@ProductId AND prmcompt.IsDeleted = 0
END
GO
