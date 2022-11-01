Begin Transaction 
Begin Try
--DBCC CHECKIDENT('PromoOffer', RESEED, 0)
--DELETE FROM PromoOffer
insert into PromoOffer (PromotionId,Status,TotalQTY,TOTALPRICE,GROUP1QTY,GROUP2QTY,GROUP3QTY,GROUP1PRICE,GROUP2PRICE,GROUP3PRICE,[Group],isdeleted,createdAt,updatedAt,createdbyId,updatedbyid)
--SELECT * FROM PromoOffer
SELECT p.Id
--,p.Code
 , 1 STATUS
 , CODE_ALP_1
 , NULLIF(REPLACE(CODE_ALP_2, '$', ''), '') AS AMT1
 , CODE_ALP_3 AS gp1
 , CODE_ALP_4 AS gp2
 , CODE_ALP_5 AS GP3
 , CASE 
  WHEN ISNUMERIC(CODE_ALP_6) = 1
   THEN CODE_ALP_6
  ELSE CODE_ALP_6
  END AS groupPrice1
 , CASE 
  WHEN ISNUMERIC(CODE_ALP_7) = 1
   THEN CODE_ALP_7
  ELSE CODE_ALP_7
  END AS groupPrice2
 , CASE 
  WHEN ISNUMERIC(CODE_ALP_8) = 1
   THEN CODE_ALP_8
  ELSE CODE_ALP_8
  END AS groupPrice3
 , ISNULL((SELECT PRMH_MIX_OFF_COL FROm [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PRMHTBL WHERE PRMH_PROM_CODE=o.Code_key_Alp),' ') AS grp
 , 0 Deleted
 , GETDATE() c
 , GETDATE() u
 , 1 cb
 , 1 ub 
 FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].[Codetbl]  o
 LEFT OUTER JOIN Promotion p on p.Code=o.Code_key_Alp
 WHERE Code_key_type = 'OFFER' AND p.Id Is NOT  NULL
 --AND Code_key_Alp IN ('2019121901','2019121909','2019121908')
 --COMMIT transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 

