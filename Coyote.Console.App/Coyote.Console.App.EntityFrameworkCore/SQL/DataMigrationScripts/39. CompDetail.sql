Begin Transaction 
Begin Try

--DBCC CHECKIDENT('CompetitionDetail', RESEED,0)
INSERT INTO CompetitionDetail (
 PromotionId
 , Code
 , [Desc]
 , ZoneId
 , TypeId
 , ResetCycleId
 , LoyaltyFactor
 , ComplDiscount
 , STATUS
 , PosReceiptPrint
 , Message
 , RewardTypeId
 , Discount
 , TriggerTypeId
 , activationpoints
 , RewardThreshold
 , CreatedById
 , UpdatedById
 , CreatedAt
 , UpdatedAt
 , IsDeleted
 , ResetCycle
 , RewardExpiration
 , ImagePath
 , ForcePrint
 )
SELECT (SELECT id FROM promotion WHERE code = PRMCOMPD_PRM_Code and PromotionTypeId=19846) AS promoId
 , (SELECT code FROM promotion WHERE code = PRMCOMPD_PRM_Code) AS code
 , (SELECT [Desc] FROM promotion WHERE code = PRMCOMPD_PRM_Code) AS Descr
 , (SELECT zoneid FROM promotion WHERE code = PRMCOMPD_PRM_Code) AS zone
 
 --, (SELECT PromotionTypeId FROM promotion WHERE code = PRMCOMPD_PRM_Code) AS typeId
 ,(
	SELECT id FROM masterlistitems WHERE listid = 80
	AND 
	CODE= (
	 CASE 
     WHEN PRMCOMPD_TYPE = 0
      THEN 'CYCLICREWARD'
     WHEN PRMCOMPD_TYPE = 1
      THEN 'LADDERCOMPETITION'
     WHEN PRMCOMPD_TYPE = 2
      THEN 'LUCKYDRAW'
     WHEN PRMCOMPD_TYPE = 3
      THEN 'COMPETITIONWITHCYCLICREWARD'
     WHEN PRMCOMPD_TYPE = 4
      THEN 'ONGOINGLOYALTYPOINTS'
     END
	)
 )TypeID
 
 , (
  SELECT id FROM masterlistitems WHERE listid = 81
   AND Code = (
    CASE 
     WHEN PRMCOMPD_RESET_CYCLE = 0
      THEN 'None'
     WHEN PRMCOMPD_RESET_CYCLE = 1
      THEN 'WeeklyMondaytoFriday'
     WHEN PRMCOMPD_RESET_CYCLE = 2
      THEN 'WeeklyMondaytoSunday'
     WHEN PRMCOMPD_RESET_CYCLE = 3
      THEN 'Bi-WeeklyMondaytoFriday'
     WHEN PRMCOMPD_RESET_CYCLE = 4
      THEN 'Bi-WeeklyMondaytoSunday'
     WHEN PRMCOMPD_RESET_CYCLE = 5
      THEN 'Monthly'
     ELSE 'Annual'
     END
    )
  ) AS ResetCycleId
 , PRMCOMPD_LOYALTY_FACTOR
 , PRMCOMPD_COMPL_DISCOUNT
 , (SELECT STATUS FROM promotion WHERE code = PRMCOMPD_PRM_Code ) AS STATUS
 , 0 AS POSReceipt
 , PrmCOMPD_WRITEUP
 , (
  SELECT id FROM masterlistitems WHERE listid = 84
   AND Code = (
    CASE 
     WHEN PRMCOMPD_RWD_TYPE = 0
      THEN 'None'
     WHEN PRMCOMPD_RWD_TYPE = 1
      THEN '%Discount(OnLine)'
     WHEN PRMCOMPD_RWD_TYPE = 2
      THEN '%Discount(OnTotal)'
     WHEN PRMCOMPD_RWD_TYPE = 3
      THEN 'LoyaltyPoints'
     WHEN PRMCOMPD_RWD_TYPE = 4
      THEN 'COMPLIMENTARYPRODUCT'
     ELSE 'AutoEntrytoCompetition'
     END
    )
  ) AS RewardType 
 , CASE WHEN PRMCOMPD_RWD_Value='2018-CTE' OR  PRMCOMPD_RWD_Value=''  THEN '0' ELSE ISNULL(PRMCOMPD_RWD_Value,0) END 
 , (
  SELECT id FROM masterlistitems WHERE listid = 82
   AND Code = (
    CASE 
     WHEN PRMCOMPD_TRIG_TYPE = 0
      THEN 'None'
     WHEN PRMCOMPD_TRIG_TYPE = 1
      THEN 'PointsTotal'
     WHEN PRMCOMPD_TRIG_TYPE = 2
      THEN 'AmtTotal'
     WHEN PRMCOMPD_TRIG_TYPE = 3
      THEN 'MedleyCompliance(AllinGroup)'
     ELSE 'MedleyCompliance(MultiGrp,OnefromEach)'
     END
    )
  ) AS TriggerTypeId
 , PRMCOMPD_TRIG_VALUE
 , PRMCOMPD_TRIG_THRESHOLD
 , 1
 , 1
 , GETUTCDATE()
 , GETUTCDATE()
 ,1
 ,PRMCOMPD_REDEMP_EXP
 , NULL IMAGEPATH
 ,PRMCOMPD_FORCE_PRINT
 , 0 FROM [DBS99.COYOTEPOS.COM.AU].[AASandBox].[dbo].PROMO_COMP_DETAIL
 WHERE (SELECT id FROM promotion WHERE code = PRMCOMPD_PRM_Code and PromotionTypeId=19846) IS NOT NULL
 

    --commit transaction
end try
begin catch
   SELECT ERROR_NUMBER() AS ErrorNumber,ERROR_SEVERITY() AS ErrorSeverity,ERROR_STATE() AS ErrorState,ERROR_PROCEDURE() AS ErrorProcedure,
   ERROR_LINE() AS ErrorLine,ERROR_MESSAGE() AS ErrorMessage;
  rollback transaction
end catch 