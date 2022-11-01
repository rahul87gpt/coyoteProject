BEGIN TRANSACTION
--DELETE FROm Product
BEGIN TRY
	INSERT INTO Product (
		Number
		,[Desc]
		,PosDesc
		,STATUS
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
		,LastAPNSold
		,Rrp
		,AltSupplier
		,CreatedAt
		,UpdatedAt
		,createdById
		,UpdatedById
		,isDeleted
		)
--DBCC CHECKIDENT('Product', RESEED, 0)
SELECT Prod_Number
			,ISNULL(Prod_Desc,'') Prod_Desc
			,Prod_POS_DESC
			,(
				CASE 
					WHEN PROD_STATUS = 'ACTIVE'
						THEN 1
					ELSE 0
					END
				) AS STATUS
			,ISNULL(PROD_CARTON_QTY,0) PROD_CARTON_QTY
			,ISNULL(PROD_UNIT_QTY,0) PROD_UNIT_QTY
			,ISNULL(PROD_CARTON_COST,0) PROD_CARTON_COST
			,ISNULL(CASE WHEN PROD_DEPARTMENT IS NULL AND PROD_DEPARTMENT ='' THEN 52 ELSE (
				SELECT id
				FROM Department
				WHERE Code = cast(PROD_DEPARTMENT AS VARCHAR(max))
					AND isdeleted = 0
				)END,52) AS DEPT 
			,ISNULL(CASE WHEN Prod_SUPPLIER IS NULL AND Prod_SUPPLIER ='' THEN 612 ELSE (
				SELECT id
				FROM Supplier
				WHERE code = cast(Prod_SUPPLIER AS VARCHAR(max))
					AND IsDeleted = 0
				)END,612)  AS SUPP 
			,ISNULL(CASE WHEN PROD_COMMODITY IS NULL AND PROD_COMMODITY ='' THEN 887 ELSE(
				SELECT id
				FROM Commodity
				WHERE code = cast(PROD_COMMODITY AS VARCHAR(max))
					AND isdeleted = 0
				)END,887) Comm
			, ISNULL(CASE WHEN Prod_TAX_CODE IS NULL AND Prod_TAX_CODE ='' THEN 38 ELSE(
				SELECT id
				FROM Tax
				WHERE code = cast(Prod_TAX_CODE AS VARCHAR(max))
					AND isdeleted = 0
				) END,38) TAX
			,
			ISNULL(CASE WHEN Prod_Group IS NULL THEN 19824 ELSE
				(SELECT id
				FROM masterlistitems
				WHERE ListId = 4
					AND code = cast(Prod_Group AS VARCHAR(max))
					AND isdeleted = 0
				)END,19824) AS ProdGroup
			,ISNULL(CASE WHEN prod_category IS NULL THEN 19823 ELSE 
				(SELECT id
				FROM masterlistitems
				WHERE ListId = 2
					AND code = cast(prod_category AS VARCHAR(max))
					AND IsDeleted = 0
				) END,19823) AS Cat
			,ISNULL(
				(SELECT id
				FROM masterlistitems
				WHERE ListId = 3
					AND code = cast(Prod_MANUFACTURER AS VARCHAR(max))
					AND IsDeleted = 0
				),19826) AS manufacturer
				
			,ISNULL((
				SELECT id
				FROM masterlistitems
				WHERE ListId = 72
					AND code = cast(PROD_TYPE AS VARCHAR(max))
					AND IsDeleted = 0
				),19822) AS prodType
				--,PROD_TYPE
			,ISNULL(CASE WHEN PROD_NATIONAL IS NULL AND PROD_NATIONAL ='' THEN 19825 ELSE(
				SELECT id
				FROM masterlistitems
				WHERE ListId = 5
					AND code = cast((
							SELECT [NR_NUMBER]
							FROM [DBS99.COYOTEPOS.COM.AU].[AASandbox].[dbo].[NationalRangeTbl]
							WHERE [NR_Number] = PROD_NATIONAL
							) AS VARCHAR(max))
					AND IsDeleted = 0
				)END,19825) AS NR
			,(
				SELECT id
				FROM masterlistitems
				WHERE ListId = 71
					AND code = cast(PROD_UNIT_MEASURE AS VARCHAR(max))
					AND IsDeleted = 0
				) AS UM
			,CASE 
				WHEN PROD_SCALE_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_SCALE_IND
			,CASE 
				WHEN PROD_GM_FLAG_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_GM_FLAG_IND
			,CASE 
				WHEN PROD_SLOW_MOVING_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_SLOW_MOVING_IND
			,CASE 
				WHEN PROD_WAREHOUSE_FROZEN_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_WAREHOUSE_FROZEN_IND
			,CASE 
				WHEN PROD_STORE_FROZEN_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_STORE_FROZEN_IND
			,CASE 
				WHEN PROD_AUST_MADE_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_AUST_MADE_IND
			,CASE 
				WHEN PROD_AUST_OWNED_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_AUST_OWNED_IND
			,CASE 
				WHEN PROD_ORGANIC_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_ORGANIC_IND
			,CASE 
				WHEN PROD_HEART_SMART_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_HEART_SMART_IND
			,CASE 
				WHEN PROD_GENERIC_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_GENERIC_IND
			,CASE 
				WHEN PROD_SEASONAL_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_SEASONAL_IND
			,(
				SELECT id
				FROM Product
				WHERE Number = PROD_PARENT
				) PROD_PARENT
			,PROD_LABEL_QTY
			,PROD_REPLICATE
			,PROD_FREIGHT
			,PROD_SIZE
			,PROD_LITRES
			,CASE 
				WHEN PROD_VARIETY_IND = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_VARIETY_IND
			,PROD_HOST_NUMBER_2
			,Nullif(cast(isnull(CASE 
							WHEN PROD_LAST_APN_SOLD = ''
								THEN 0
							ELSE PROD_LAST_APN_SOLD
							END, 0) AS BIGINT), 0) PROD_LAST_APN_SOLD
			,PROD_RRP
			,CASE 
				WHEN PROD_ALT_SUPPLIER = 'Yes'
					THEN 1
				ELSE 0
				END AS PROD_ALT_SUPPLIER
			,CASE WHEN PROD_DATE_ADDED IS NULL THEN GETUTCDATE() ELSE PROD_DATE_ADDED END  created
			,CASE WHEN PROD_DATE_CHANGED IS NULL THEN GETUTCDATE() ELSE PROD_DATE_CHANGED END  updated
			,1 createdby
			,1 updatedby
			,0 deleted
		FROM PRODTBL
		
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

SELECT * FROm Product