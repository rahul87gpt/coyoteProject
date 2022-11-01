
--SELECT * FROm APN
BEGIN TRANSACTION
--DELETE FROm Product
BEGIN TRY
	INSERT INTO Product (
		Number
,[Desc]
,PosDesc
,Status
,CartonQty
,UnitQty
,CartonCost
,DepartmentId
,SupplierId
,CommodityId
,TaxId
,GroupId
,CategoryId
,ManufacturerId
,TypeId
,NationalRangeId
,UnitMeasureId
,ScaleInd
,GmFlagInd
,SlowMovingInd
,WarehouseFrozenInd
,StoreFrozenInd
,AustMadeInd
,AustOwnedInd
,OrganicInd
,HeartSmartInd
,GenericInd
,SeasonalInd
,Parent
,LabelQty
,Replicate
,Freight
,Size
,Litres
,VarietyInd
,HostNumber
,HostNumber2
,HostNumber3
,HostNumber4
,HostItemType
,HostItemType2
,HostItemType3
,HostItemType4
,HostItemType5
,LastApnSold
,Rrp
,AltSupplier
,DeletedAt
,DeactivatedAt
,CreatedAt
,UpdatedAt
,CreatedById
,UpdatedById
,IsDeleted
,ImagePath
,AccessOutletIds
,StoreId
,TareWeight
,Info
		)
SELECT  
 9999999 Number
,'NEW_UNALLOCATE'[Desc]
,'NEW_UNALLOCATE'PosDesc
,0 [Status]
, 0 CartonQty
, 0 UnitQty
,0 CartonCost
,52 DepartmentId
,612 SupplierId
,887 CommodityId
,38 TaxId
,19824 GroupId
,19823 CategoryId
,19826 ManufacturerId
,19822 TypeId
,19825 NationalRangeId
,NULL UnitMeasureId
,0 ScaleInd
,0 GmFlagInd
,0 SlowMovingInd
,0 WarehouseFrozenInd
,0 StoreFrozenInd
,0 AustMadeInd
,0 AustOwnedInd
,0 OrganicInd
,0 HeartSmartInd
,0 GenericInd
,0 SeasonalInd
,NULL Parent
,NULL LabelQty
,NULL Replicate
,NULL Freight
,NULL Size
,NULL Litres
,0 VarietyInd
,NULL HostNumber
,NULL HostNumber2
,NULL HostNumber3
,NULL HostNumber4
,NULL HostItemType
,NULL HostItemType2
,NULL HostItemType3
,NULL HostItemType4
,NULL HostItemType5
,NULL LastApnSold
,NULL Rrp
,1 AltSupplier
,NULL DeletedAt
,NULL DeactivatedAt
,GETUTCDATE() CreatedAt
,GETUTCDATE() UpdatedAt
,1 CreatedById
,1 UpdatedById
,0 IsDeleted
,NULL ImagePath
,NULL AccessOutletIds
,NULL StoreId
,NULL TareWeight
,NULL Info 
FROM Product 
WHERE Number=1
		--Commit TRANSACTION
END TRY

BEGIN CATCH
	SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;
		
	ROLLBACK TRANSACTION
END CATCH
--SELECT * FROm Product Order By Id desc