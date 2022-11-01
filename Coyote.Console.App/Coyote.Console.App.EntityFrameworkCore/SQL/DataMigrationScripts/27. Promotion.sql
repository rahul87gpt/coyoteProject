Begin Transaction 
Begin Try
--DBCC CHECKIDENT('promotion', RESEED, 0)

insert into promotion (
 Code
,PromotionTypeId
,[Desc]
,Status
,SourceId
,ZoneId
,Start
,[End]
,RptGroup
,FrequencyId
,Availibility
,ImagePath
,isdeleted,createdAt,UpdatedAt,CreatedById,UpdatedById)
SELECT 
[PRMH_PROM_CODE]
,(select id from MasterListItems where ListId=10 and Code=[PRMH_PROMOTION_TYPE] and IsDeleted=0) PromotionTypeId
,ISNULL([PRMH_DESC],'')
,case when [PRMH_STATUS]='Active' then 1 else 0 end as [Status]
,(SELECT ID FROm MasterListItems WHERE Code= [PRMH_SOURCE] AND IsDeleted=0 ANd ListId=94) SourceId
--,[PRMH_SOURCE]
,ISNULL((select id from MasterListItems where ListId=1 and Code=[PRMH_OUTLET_ZONE] and IsDeleted=0),19842) as Zone_Outlet
--,[PRMH_PROMOTION_TYPE]
,[PRMH_START]
,[PRMH_END]
,[PRMH_RPT_GROUP]
,19850 as FrequencyId
,[PRMH_MON]+[PRMH_TUE]+[PRMH_WED]+[PRMH_THU]+[PRMH_FRI]+[PRMH_SAT]+[PRMH_SUN] as Availibility
, NULL ImagePath
,0 IsDeleted
,GETDATE()A
,GETDATE()B
,1 V
,1 C 
FROM  [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[PRMHTBL]
--WHERE (select id from MasterListItems where ListId=10 and Code=[PRMH_PROMOTION_TYPE] and IsDeleted=0) IS NULL
--COMMIT transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch

--WHERE (select id from MasterListItems where ListId=1 and Code=[PRMH_OUTLET_ZONE] and IsDeleted=0) IS NULL
--AND [PRMH_OUTLET_ZONE] Is NOT NULL

--PRMH_OUTLET_ZONE NOT FOUNd
--757
--776
--COMP
--CSD
--IMPULSE_CSTCK
--SOS
--TARAGINDI

--Select DISTINCT [PRMH_PROMOTION_TYPE] FROM  [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[PRMHTBL]

