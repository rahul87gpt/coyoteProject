Begin Transaction 
Begin Try
--SELECT * FROM MasterListItems WHERE ListId=83
--DBCC CHECKIDENT('CompetitionTrigger', RESEED,0)
--DELETE FROM CompetitionTrigger
INSERT INTO CompetitionTrigger
(CompPromoId
,TriggerProductGroupID
,Share
,LoyaltyFactor
,CreatedById
,UpdatedById
,CreatedAt
,UpdatedAt
,IsDeleted)
SELECT 
   DISTINCT p.Id
  ,CASE 
     WHEN PRMCOMPTR_GROUP = 1
      THEN 19883
     WHEN PRMCOMPTR_GROUP= 2
      THEN 19885
     WHEN PRMCOMPTR_GROUP = 3
      THEN 19886
     WHEN PRMCOMPTR_GROUP = 4
      THEN 19887
     WHEN PRMCOMPTR_GROUP = 5
      THEN 19888
	  WHEN PRMCOMPTR_GROUP = 6
      THEN 19889
	  WHEN PRMCOMPTR_GROUP = 7
      THEN 19890
	  WHEN PRMCOMPTR_GROUP = 8
      THEN 19891
	   WHEN PRMCOMPTR_GROUP = 9
      THEN 19892
	  	  WHEN PRMCOMPTR_GROUP = 10
      THEN 19884
	 -- ELSE 19884
     END AS [Group]
	 ,PRMCOMPTR_SHARE
	 ,PRMCOMPTR_LFACTOR
, 1
, 1
, GETUTCDATE() 
, GETUTCDATE() 
, 0
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_DETAIL dtl
INNER JOIN CompetitionDetail d on d.Code=dtl.PRMCOMPD_PRM_CODE
INNER JOIN PromotionCompetition p ON d.Id=p.CompetitionId
INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_Triggers rd
ON rd.PRMCOMPTR_PRM_CODE=dtl.PRMCOMPD_PRM_CODE  AND d.Code= rd.PRMCOMPTR_PRM_CODE
WHERE (CASE 
     WHEN PRMCOMPTR_GROUP = 1
      THEN 19883
     WHEN PRMCOMPTR_GROUP= 2
      THEN 19885
     WHEN PRMCOMPTR_GROUP = 3
      THEN 19886
     WHEN PRMCOMPTR_GROUP = 4
      THEN 19887
     WHEN PRMCOMPTR_GROUP = 5
      THEN 19888
	  WHEN PRMCOMPTR_GROUP = 6
      THEN 19889
	  WHEN PRMCOMPTR_GROUP = 7
      THEN 19890
	  WHEN PRMCOMPTR_GROUP = 8
      THEN 19891
	   WHEN PRMCOMPTR_GROUP = 9
      THEN 19892
	  	  WHEN PRMCOMPTR_GROUP = 10
      THEN 19884
	 -- ELSE 19884
     END) Is NOT NULL
-- WHERE PRMCOMPTR_VALUE NOT IN (SELECT PRMCOMPRW_VALUE FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_REWARDS)
COMMIT transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 