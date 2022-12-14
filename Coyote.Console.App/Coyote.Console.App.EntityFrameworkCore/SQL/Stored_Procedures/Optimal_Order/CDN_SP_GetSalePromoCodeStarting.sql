USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP_GetSalePromoCodeStarting]    Script Date: 9/5/2020 1:46:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CDN_SP_GetSalePromoCodeStarting]
@aRunDate Date
,@OSS_Outlet INT
,@OSS_Supplier nvarchar(10)
,@OUTP_PRODUCT INT
,@PROD_PARENT INT
,@CoverDays INT= 10
,@GetSaleTypeCase BIT = 1
,@FoundPromo BIT = 0 OUT
,@PRMH_PROM_CODE NVARCHAR(30) OUT
,@PRMP_Promo_Units FLOAT OUT
,@PRMH_START DATE OUT
,@PRMH_PROMOTION_TYPE NVARCHAR(30) OUT
,@PRMH_END DATE OUT
,@Debug INT = 0
AS
BEGIN TRY
	DECLARE @SundayAfterNextMonday DATE = DATEADD(DD, @CoverDays, @aRunDate)
	BEGIN ------{ Start GetSalePromoCodeStarting
		IF @GetSaleTypeCase = 0
		BEGIN
			SELECT TOP 1 @FoundPromo = CASE 
					WHEN PRMH_PROM_CODE IS NULL
						THEN 0
					ELSE 1
					END
				,@PRMH_PROM_CODE = PRMH_PROM_CODE
				,@PRMP_Promo_Units = PRMP_Promo_Units
				,@PRMH_START = PRMH_START
				,@PRMH_PROMOTION_TYPE = PRMH_PROMOTION_TYPE
				,@PRMH_END = PRMH_END
			FROM dbo.PrmhTbl WITH (NOLOCK)
			JOIN dbo.PRMPTBL WITH (NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE AND PRMP_PRODUCT= @OUTP_PRODUCT AND PRMH_STATUS = 'Active'
			JOIN dbo.OutpTbl WITH (NOLOCK) ON PRMP_PRODUCT = Outp_Product AND OUTP_STATUS = 'Active' AND OUTP_PRODUCT=@OUTP_PRODUCT AND  OUTP_OUTLET = @OSS_Outlet
			
			LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
			BB.Outlet = @OUTP_PRODUCT AND BB.supplier = @OSS_Supplier AND BB.BEST_BUY = 1 AND Product = @OUTP_PRODUCT
			
			WHERE PRMH_OUTLET_ZONE IN (
					SELECT CODE_KEY_ALP
					FROM dbo.CODETBL
					WHERE CODE_KEY_TYPE = 'ZONEOUTLET'
						AND CODE_KEY_NUM = @OSS_Outlet
					)
				AND (OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)				
				AND (
					( @aRunDate BETWEEN PRMH_START AND PRMH_END) OR ( @SundayAfterNextMonday BETWEEN PRMH_START AND PRMH_END)
					)
				--AND PRMH_END < @SundayAfterNextMonday
				AND PRMH_PROMOTION_TYPE IN (
					'OFFER'
					,'SELLING'
					,'MIXMATCH'
					)
				AND (
					PRMP_PRODUCT = @OUTP_PRODUCT
					OR PRMP_PRODUCT = @PROD_PARENT
					)
			ORDER BY PRMP_Promo_Units DESC
			END
		ELSE
		BEGIN
			SELECT TOP 1 @FoundPromo = CASE 
					WHEN PRMH_PROM_CODE IS NULL
						THEN 0
					ELSE 1
					END
				,@PRMH_PROM_CODE = PRMH_PROM_CODE 
				,@PRMP_Promo_Units = PRMP_Promo_Units
				,@PRMH_START = PRMH_START
				,@PRMH_PROMOTION_TYPE = PRMH_PROMOTION_TYPE
				,@PRMH_END = PRMH_END
			FROM dbo.PrmhTbl WITH (NOLOCK)
			JOIN dbo.PRMPTBL WITH (NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE AND PRMP_PRODUCT= @OUTP_PRODUCT  AND PRMH_STATUS = 'Active'
			JOIN dbo.OutpTbl WITH (NOLOCK) ON PRMP_PRODUCT = Outp_Product AND OUTP_STATUS = 'Active' AND OUTP_PRODUCT=@OUTP_PRODUCT AND  OUTP_OUTLET = @OSS_Outlet
			
			LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
			BB.Outlet = @OUTP_PRODUCT AND BB.supplier = @OSS_Supplier AND BB.BEST_BUY = 1 AND Product = @OUTP_PRODUCT
			
			WHERE PRMH_OUTLET_ZONE IN (
					SELECT CODE_KEY_ALP
					FROM dbo.CODETBL
					WHERE CODE_KEY_TYPE = 'ZONEOUTLET'
						AND CODE_KEY_NUM = @OSS_Outlet
					)
				--'  and PRMP_STATUS = ''Active'' ' +  // No need to check PRMP_STATUS as suggested by Stephen on 19/03/19
				AND (OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)
				
				AND ((  @aRunDate BETWEEN PRMH_START AND PRMH_END) OR ( @SundayAfterNextMonday BETWEEN PRMH_START AND PRMH_END))
				---AND PRMH_END < @SundayAfterNextMonday
				AND PRMH_PROMOTION_TYPE IN ('OFFER','SELLING','MIXMATCH')
				AND (PRMP_PRODUCT = @OUTP_PRODUCT OR PRMP_PRODUCT = @PROD_PARENT)
			ORDER BY PRMP_Promo_Units DESC
		
			IF @Debug = 2				
			SELECT TOP 1 '[CDN_SP_GetSalePromoCodeStarting]' CoverdaysPromoAvailable,*
			FROM dbo.PrmhTbl WITH (NOLOCK)
			JOIN dbo.PRMPTBL WITH (NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE AND PRMP_PRODUCT= @OUTP_PRODUCT  AND PRMH_STATUS = 'Active'
			JOIN dbo.OutpTbl WITH (NOLOCK) ON PRMP_PRODUCT = Outp_Product AND OUTP_STATUS = 'Active' AND OUTP_PRODUCT=@OUTP_PRODUCT AND  OUTP_OUTLET = @OSS_Outlet
			
			LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
			BB.Outlet = @OUTP_PRODUCT AND BB.supplier = @OSS_Supplier AND BB.BEST_BUY = 1 AND Product = @OUTP_PRODUCT
			
			WHERE PRMH_OUTLET_ZONE IN (
					SELECT CODE_KEY_ALP
					FROM dbo.CODETBL
					WHERE CODE_KEY_TYPE = 'ZONEOUTLET'
						AND CODE_KEY_NUM = @OSS_Outlet
					)
				--'  and PRMP_STATUS = ''Active'' ' +  // No need to check PRMP_STATUS as suggested by Stephen on 19/03/19
				AND (OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)
				
				AND (@aRunDate BETWEEN PRMH_START AND PRMH_END )
				AND PRMH_END < @SundayAfterNextMonday
				AND PRMH_PROMOTION_TYPE IN ('OFFER','SELLING','MIXMATCH')
				AND (PRMP_PRODUCT = @OUTP_PRODUCT OR PRMP_PRODUCT = @PROD_PARENT)
			ORDER BY PRMP_Promo_Units DESC

			END
	END ------} End   GetSalePromoCodeStarting

END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, @OUTP_PRODUCT,'CDN_SP_GetSalePromoCodeStarting',NULL);
END CATCH
