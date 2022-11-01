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
SELECT * FROM (
SELECT 
d.Id
,(SELECT ID FROm PRODUCT  WHERE NUMBER=rd.PRMCOMPRW_VALUE)  PRODNUMBER
,(SELECT [Desc] FROm PRODUCT  WHERE NUMBER=rd.PRMCOMPRW_VALUE)  [Desc]
, 1  [St]
, 1 c
,1 u
, GETUTCDATE()  cd
, GETUTCDATE()  ud
, 0 del
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_DETAIL dtl
INNER JOIN CompetitionDetail d on d.Code=dtl.PRMCOMPD_PRM_CODE
INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_REWARDS rd
ON rd.PRMCOMPRW_PRM_CODE=dtl.PRMCOMPD_PRM_CODE 
UNION
SELECT d.Id
,(SELECT ID FROm PRODUCT  WHERE NUMBER=rd.PRMCOMPTR_VALUE )  PRODNUMBER
,(SELECT [Desc] FROm PRODUCT  WHERE NUMBER=rd.PRMCOMPTR_VALUE )  [Des]
, 1  [St]
, 1 c
,1 u
, GETUTCDATE()  Cd
, GETUTCDATE()  ud
, 0  del
FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_DETAIL dtl
INNER JOIN CompetitionDetail d on d.Code=dtl.PRMCOMPD_PRM_CODE
INNER JOIN [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_Triggers rd
ON rd.PRMCOMPTR_PRM_CODE=dtl.PRMCOMPD_PRM_CODE 
)X
  --commit transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 