Begin Transaction 
Begin Try

--DBCC CHECKIDENT('CompetitionReward', RESEED,0)
INSERT INTO CompetitionReward
(CompPromoId
,Count
,CreatedById
,UpdatedById
,CreatedAt
,UpdatedAt
,IsDeleted)
SELECT 
   DISTINCT p.Id
  , PRMCOMPRW_COUNT
, 1
, 1
, GETUTCDATE() 
, GETUTCDATE() 
, 0
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_DETAIL dtl
INNER JOIN CompetitionDetail d on d.Code=dtl.PRMCOMPD_PRM_CODE
INNER JOIN PromotionCompetition p ON d.Id=p.CompetitionId
INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_REWARDS rd
ON rd.PRMCOMPRW_PRM_CODE=dtl.PRMCOMPD_PRM_CODE 
  --commit transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 