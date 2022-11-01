Begin Transaction 
Begin Try

--DBCC CHECKIDENT('PromotionCompetition', RESEED,0)

INSERT INTO PromotionCompetition
(
 CompetitionId
,ProductId
,[Desc]
,[Status]
,CreatedById
,UpdatedById
,CreatedAt
,UpdatedAt
,IsDeleted)
SELECT 
   d.Id
  ,(SELECT ID FROm PRODUCT  WHERE NUMBER=rd.PRMCOMPTR_VALUE)
, (SELECT [Desc] FROm PRODUCT  WHERE NUMBER=rd.PRMCOMPTR_VALUE)
, 1
, 1
,1
, GETUTCDATE() 
, GETUTCDATE() 
, 0
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_DETAIL dtl
INNER JOIN CompetitionDetail d on d.Code=dtl.PRMCOMPD_PRM_CODE
INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_Triggers rd
ON rd.PRMCOMPTR_PRM_CODE=dtl.PRMCOMPD_PRM_CODE 
  --commit transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 