/****** Object:  StoredProcedure [dbo].[GetItemsSalesReport]    Script Date: 25/08/2020 9:15:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: Narendra Mehra
-- =============================================
CREATE PROCEDURE [dbo].[GetItemsSalesReport]
@startDate datetime,
@endDate datetime,
@productStartId bigint = null,
@productEndId bigint = null,
@promoSales bit=0,
@promoCode nvarchar(max) = null,
@summary bit=0,
@drillDown bit=0,
@contineous bit=0,
@variance bit=0,
@wastage bit=0,
@merge bit=0,
@tillId bigint = null,
@storeIds nvarchar(max) = null,
@zoneIds nvarchar(max) = null,
@dayRange nvarchar(max) = null,
@departmentIds nvarchar(max) = null,
@commodityIds nvarchar(max) = null,
@categoryIds nvarchar(max) = null,
@groupIds nvarchar(max) = null,
@suppliers nvarchar(max) = null,
@manufacturerIds nvarchar(max) = null,
@productIds nvarchar(max) = null,
@reportType nvarchar(200)

AS
BEGIN
	SET NOCOUNT ON;
	declare @tranType nvarchar(max) = null,@tranTender nvarchar(max) = null,@item bit=1,@promoId int,@promotionCount int=0
	if(@summary=1 and @drillDown=0)
	begin
	 set @item=0
	end

	if(@merge=1)
	begin
	 if(@wastage=1 and @variance=1)
	 begin
		 set @tranType='ITEMSALE,WASTAGE,VARIANCE'
		 set @tranTender='WASTAGE'
	 end
	 else if(@wastage=1)
	 begin
		 set @tranType='ITEMSALE,WASTAGE'
		 set @tranTender='WASTAGE'
	 end
	 else 	
	 begin
		 set @tranType='ITEMSALE,VARIANCE'		
	 end
	end
	else
	begin
	  set @tranType='ITEMSALE'		
	end

	if(@promoCode is not null)
	begin
	 select @promoId=id from [dbo].[Promotion] with(readuncommitted) where [Code]=@promoCode
	end

    if(@promoCode is not null and @promoSales=1)
	begin
	 select @promotionCount=count(*) from [dbo].[Promotion] with(readuncommitted) 
	  where [Code]=@promoCode 
	        and [PromotionTypeId] in
			(select Id from [dbo].[MasterListItems] with(readuncommitted) 
			   where [Code] in ('OFFER','MIXMATCH') 
			         and ListId=(select Id from [dbo].[MasterList] with(readuncommitted) where [Code]='PROMOTYPE'))
	end

	if(lower(@reportType)='department')
	begin
	set @reportType='Item Sales By Department'
	if(@promotionCount=0)
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
		case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		(select t.DepartmentId as SUM_CODE,
			   SUM(isnull(t.Amt,0)) as SUM_AMT,
			   SUM(isnull(t.Qty,0)) as SUM_QTY, 
			   case when @merge=1 then
				 case when @variance=1 and @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') or (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 when @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 else 
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 end	    
			   else SUM(isnull(t.Cost,0)) end as SUM_COST,
			   SUM(isnull(t.PromoSales,0)) as SUM_PROM_SALES, 
			   SUM(isnull(t.PromoSalesGST,0)) as SUM_PROM_SALES_GST,
			   SUM(isnull(t.Discount,0)) as SUM_DISCOUNT,
			  case when @item=1 then convert(int,t.ProductId) else t.DepartmentId end as TRX_PRODUCT,
			  case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
			   d.[Desc] as CODE_DESC
		from [dbo].[Transaction] t with(readuncommitted) 
		     inner join [dbo].[Department] d with(readuncommitted) on d.Id=t.DepartmentId
			 inner join [dbo].[Product] p with(readuncommitted) on p.Id=t.ProductId
		where (t.[Date] between @startDate and @endDate)
			  and (@tillId is null or t.TillId = @tillId) 
			  and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			  and (@promoSales=0 or (@promoSales=1 and t.PromoSales!=0) and (@promoCode is null or t.PromoSellId=@promoId))
			  and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),','))) 
			  and (@productIds is null or t.ProductId in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (t.[Type] in (select * from [dbo].[SplitString](isnull(@tranType,''),',')) or @tranTender is null or t.[Tender]=@tranTender)	
		group by t.[DepartmentId],t.[Type],t.[Tender],t.[ProductId],p.[Desc],d.[Desc]
			 ) as x order by x.SUM_CODE,x.CODE_DESC
    end
	else 
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
			case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		  (select p.DepartmentId as SUM_CODE,
				SUM(isnull(j.Amount,0)) as SUM_AMT,
				SUM(isnull(j.Quantity,0)) as SUM_QTY,
				SUM(j.Cost)  as SUM_COST,		
				SUM(isnull(j.Amount,0)) as SUM_PROM_SALES, 
				SUM(isnull(j.GSTAmount,0)) as SUM_PROM_SALES_GST,
				SUM(isnull(j.DiscountAmount,0)) as SUM_DISCOUNT,
				case when @item=1 then convert(int,j.ProductId) else p.DepartmentId end as TRX_PRODUCT,
				case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
				d.[Desc] as CODE_DESC
			from [dbo].[JournalHeader] h with(readuncommitted)
			     inner join [dbo].[JournalDetail] j with(readuncommitted) on j.JournalHeaderId=h.Id
				 inner join [dbo].[Product] p with(readuncommitted) on p.Id=j.ProductId 
				 inner join [dbo].[Department] d with(readuncommitted) on d.Id=p.DepartmentId
			where (h.[Date] between @startDate and @endDate)
			  and (@tillId is null or h.TillId = @tillId) 
			  and ((@productStartId is null or j.ProductId>=@productStartId) and (@productEndId is null or j.ProductId<=@productEndId)) 
			  and (j.PromoSellId=@promoId or j.PromoOfferId=@promoId or j.PromoMixMatchId=@promoId or j.PromoMemeberOfferId=@promoId)
			  and (@productIds is null or j.[ProductId] in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or h.[OutletId] in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or p.[CommodityId] in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or p.[DepartmentId] in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or p.[CategoryId] in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or p.[GroupId] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or p.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or p.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (j.[Type]='SALE')
			  and j.[Status]=1	  
			  and j.[DiscountAmount]>0
			group by p.[DepartmentId],j.[ProductId],p.[Desc],d.[Desc]
			) x
	end
   end

   if(lower(@reportType)='commodity')
	begin
	set @reportType='Item Sales By Commodity'
	if(@promotionCount=0)
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
		case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		(select t.CommodityId as SUM_CODE,
			   SUM(isnull(t.Amt,0)) as SUM_AMT,
			   SUM(isnull(t.Qty,0)) as SUM_QTY, 
			   case when @merge=1 then
				 case when @variance=1 and @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') or (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 when @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 else 
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 end	    
			   else SUM(isnull(t.Cost,0)) end as SUM_COST,
			   SUM(isnull(t.PromoSales,0)) as SUM_PROM_SALES, 
			   SUM(isnull(t.PromoSalesGST,0)) as SUM_PROM_SALES_GST,
			   SUM(isnull(t.Discount,0)) as SUM_DISCOUNT,
			   case when @item=1 then convert(int,t.ProductId) else t.[CommodityId] end as TRX_PRODUCT,
			   case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
			   d.[Desc] as CODE_DESC
		from [dbo].[Transaction] t with(readuncommitted)
		     inner join [dbo].[Commodity] d with(readuncommitted) on d.Id=t.CommodityId 
			 inner join [dbo].[Product] p with(readuncommitted) on p.Id=t.ProductId
		where (t.[Date] between @startDate and @endDate)
			  and (@tillId is null or t.TillId = @tillId) 
			  and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			  and (@promoSales=0 or (@promoSales=1 and t.PromoSales!=0) and (@promoCode is null or t.PromoSellId=@promoId))
			  and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),','))) 
			  and (@productIds is null or t.ProductId in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (t.[Type] in (select * from [dbo].[SplitString](isnull(@tranType,''),',')) or @tranTender is null or t.[Tender]=@tranTender)	
		group by t.[CommodityId],t.[Type],t.[Tender],t.[ProductId],p.[Desc],d.[Desc]
			 ) as x order by x.SUM_CODE,x.CODE_DESC
    end
	else 
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
			case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		  (select p.[CommodityId] as SUM_CODE,
				SUM(isnull(j.Amount,0)) as SUM_AMT,
				SUM(isnull(j.Quantity,0)) as SUM_QTY,
				SUM(j.Cost)  as SUM_COST,		
				SUM(isnull(j.Amount,0)) as SUM_PROM_SALES, 
				SUM(isnull(j.GSTAmount,0)) as SUM_PROM_SALES_GST,
				SUM(isnull(j.DiscountAmount,0)) as SUM_DISCOUNT,
				case when @item=1 then convert(int,j.ProductId) else p.[CommodityId] end as TRX_PRODUCT,
				case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
				d.[Desc] as CODE_DESC
			from [dbo].[JournalHeader] h with(readuncommitted)
			     inner join [dbo].[JournalDetail] j with(readuncommitted) on j.JournalHeaderId=h.Id
				 inner join [dbo].[Product] p with(readuncommitted) on p.Id=j.ProductId 
				 inner join [dbo].[Commodity] d with(readuncommitted) on d.Id=p.CommodityId
			where (h.[Date] between @startDate and @endDate)
			  and (@tillId is null or h.TillId = @tillId) 
			  and ((@productStartId is null or j.ProductId>=@productStartId) and (@productEndId is null or j.ProductId<=@productEndId)) 
			  and (j.PromoSellId=@promoId or j.PromoOfferId=@promoId or j.PromoMixMatchId=@promoId or j.PromoMemeberOfferId=@promoId)
			  and (@productIds is null or j.[ProductId] in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or h.[OutletId] in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or p.[CommodityId] in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or p.[DepartmentId] in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or p.[CategoryId] in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or p.[GroupId] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or p.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or p.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (j.[Type]='SALE')
			  and j.[Status]=1	  
			  and j.[DiscountAmount]>0
			group by p.[CommodityId],j.[ProductId],p.[Desc],d.[Desc]
			) x
	end
   end

   if(lower(@reportType)='category')
	begin
	set @reportType='Item Sales By Category'
	if(@promotionCount=0)
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
		case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		(select t.CategoryId as SUM_CODE,
			   SUM(isnull(t.Amt,0)) as SUM_AMT,
			   SUM(isnull(t.Qty,0)) as SUM_QTY, 
			   case when @merge=1 then
				 case when @variance=1 and @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') or (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 when @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 else 
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 end	    
			   else SUM(isnull(t.Cost,0)) end as SUM_COST,
			   SUM(isnull(t.PromoSales,0)) as SUM_PROM_SALES, 
			   SUM(isnull(t.PromoSalesGST,0)) as SUM_PROM_SALES_GST,
			   SUM(isnull(t.Discount,0)) as SUM_DISCOUNT,
			   case when @item=1 then convert(int,t.ProductId) else t.[CategoryId] end as TRX_PRODUCT,
			   case when @item=1 then p.[Desc] else d.[Name] end as PROD_DESC,
			   d.[Name] as CODE_DESC
		from [dbo].[Transaction] t with(readuncommitted)
		     inner join [dbo].[MasterListItems] d with(readuncommitted) on d.Id=t.CategoryId 
			 inner join [dbo].[Product] p with(readuncommitted) on p.Id=t.ProductId
		where (t.[Date] between @startDate and @endDate)
			  and (@tillId is null or t.TillId = @tillId) 
			  and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			  and (@promoSales=0 or (@promoSales=1 and t.PromoSales!=0) and (@promoCode is null or t.PromoSellId=@promoId))
			  and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),','))) 
			  and (@productIds is null or t.ProductId in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (t.[Type] in (select * from [dbo].[SplitString](isnull(@tranType,''),',')) or @tranTender is null or t.[Tender]=@tranTender)	
		group by t.[CategoryId],t.[Type],t.[Tender],t.[ProductId],p.[Desc],d.[Name]
			 ) as x order by x.SUM_CODE,x.CODE_DESC
    end
	else 
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
			case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		  (select p.[CategoryId] as SUM_CODE,
				SUM(isnull(j.Amount,0)) as SUM_AMT,
				SUM(isnull(j.Quantity,0)) as SUM_QTY,
				SUM(j.Cost)  as SUM_COST,		
				SUM(isnull(j.Amount,0)) as SUM_PROM_SALES, 
				SUM(isnull(j.GSTAmount,0)) as SUM_PROM_SALES_GST,
				SUM(isnull(j.DiscountAmount,0)) as SUM_DISCOUNT,
				case when @item=1 then convert(int,j.ProductId) else p.[CategoryId] end as TRX_PRODUCT,
				case when @item=1 then p.[Desc] else d.[Name] end as PROD_DESC,
				d.[Name] as CODE_DESC
			from [dbo].[JournalHeader] h with(readuncommitted) 
			     inner join [dbo].[JournalDetail] j with(readuncommitted) on j.JournalHeaderId=h.Id
				 inner join [dbo].[Product] p with(readuncommitted) on p.Id=j.ProductId 
				 inner join [dbo].[MasterListItems] d with(readuncommitted) on d.Id=p.[CategoryId]
			where (h.[Date] between @startDate and @endDate)
			  and (@tillId is null or h.TillId = @tillId) 
			  and ((@productStartId is null or j.ProductId>=@productStartId) and (@productEndId is null or j.ProductId<=@productEndId)) 
			  and (j.PromoSellId=@promoId or j.PromoOfferId=@promoId or j.PromoMixMatchId=@promoId or j.PromoMemeberOfferId=@promoId)
			  and (@productIds is null or j.[ProductId] in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or h.[OutletId] in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or p.[CommodityId] in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or p.[DepartmentId] in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or p.[CategoryId] in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or p.[GroupId] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or p.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or p.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (j.[Type]='SALE')
			  and j.[Status]=1	  
			  and j.[DiscountAmount]>0
			group by p.[CategoryId],j.[ProductId],p.[Desc],d.[Name]
			) x
	end
   end

   if(lower(@reportType)='group')
	begin
	set @reportType='Item Sales By Group'
	if(@promotionCount=0)
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
		case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		(select t.[Group] as SUM_CODE,
			   SUM(isnull(t.Amt,0)) as SUM_AMT,
			   SUM(isnull(t.Qty,0)) as SUM_QTY, 
			   case when @merge=1 then
				 case when @variance=1 and @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') or (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 when @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 else 
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 end	    
			   else SUM(isnull(t.Cost,0)) end as SUM_COST,
			   SUM(isnull(t.PromoSales,0)) as SUM_PROM_SALES, 
			   SUM(isnull(t.PromoSalesGST,0)) as SUM_PROM_SALES_GST,
			   SUM(isnull(t.Discount,0)) as SUM_DISCOUNT,
			   case when @item=1 then convert(int,t.ProductId) else t.[Group] end as TRX_PRODUCT,
			   case when @item=1 then p.[Desc] else d.[Name] end as PROD_DESC,
			   d.[Name] as CODE_DESC
		from [dbo].[Transaction] t with(readuncommitted)
		     inner join [dbo].[MasterListItems] d with(readuncommitted) on d.Id=t.[Group]
			 inner join [dbo].[Product] p with(readuncommitted) on p.Id=t.ProductId
		where (t.[Date] between @startDate and @endDate)
			  and (@tillId is null or t.TillId = @tillId) 
			  and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			  and (@promoSales=0 or (@promoSales=1 and t.PromoSales!=0) and (@promoCode is null or t.PromoSellId=@promoId))
			  and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),','))) 
			  and (@productIds is null or t.ProductId in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (t.[Type] in (select * from [dbo].[SplitString](isnull(@tranType,''),',')) or @tranTender is null or t.[Tender]=@tranTender)		
		group by t.[Group],t.[Type],t.[Tender],t.[ProductId],p.[Desc],d.[Name]
			 ) as x order by x.SUM_CODE,x.CODE_DESC
    end
	else 
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
			case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		  (select p.[GroupId] as SUM_CODE,
				SUM(isnull(j.Amount,0)) as SUM_AMT,
				SUM(isnull(j.Quantity,0)) as SUM_QTY,
				SUM(j.Cost)  as SUM_COST,		
				SUM(isnull(j.Amount,0)) as SUM_PROM_SALES, 
				SUM(isnull(j.GSTAmount,0)) as SUM_PROM_SALES_GST,
				SUM(isnull(j.DiscountAmount,0)) as SUM_DISCOUNT,
				case when @item=1 then convert(int,j.ProductId) else p.[GroupId] end as TRX_PRODUCT,
				case when @item=1 then p.[Desc] else d.[Name] end as PROD_DESC,
				d.[Name] as CODE_DESC
			from [dbo].[JournalHeader] h with(readuncommitted)
			     inner join [dbo].[JournalDetail] j with(readuncommitted) on j.JournalHeaderId=h.Id
				 inner join [dbo].[Product] p with(readuncommitted) on p.Id=j.ProductId
				 inner join [dbo].[MasterListItems] d with(readuncommitted) on d.Id=p.[GroupId]
			where (h.[Date] between @startDate and @endDate)
			  and (@tillId is null or h.TillId = @tillId) 
			  and ((@productStartId is null or j.ProductId>=@productStartId) and (@productEndId is null or j.ProductId<=@productEndId)) 
			  and (j.PromoSellId=@promoId or j.PromoOfferId=@promoId or j.PromoMixMatchId=@promoId or j.PromoMemeberOfferId=@promoId)
			  and (@productIds is null or j.[ProductId] in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or h.[OutletId] in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or p.[CommodityId] in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or p.[DepartmentId] in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or p.[CategoryId] in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or p.[GroupId] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or p.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or p.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (j.[Type]='SALE')
			  and j.[Status]=1	  
			  and j.[DiscountAmount]>0
			group by p.[GroupId],j.[ProductId],p.[Desc],d.[Name]
			) x
	end
   end

   if(lower(@reportType)='supplier')
	begin
	set @reportType='Item Sales By Supplier'
	if(@promotionCount=0)
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
		case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		(select t.[SupplierId] as SUM_CODE,
			   SUM(isnull(t.Amt,0)) as SUM_AMT,
			   SUM(isnull(t.Qty,0)) as SUM_QTY, 
			   case when @merge=1 then
				 case when @variance=1 and @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') or (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 when @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 else 
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 end	    
			   else SUM(isnull(t.Cost,0)) end as SUM_COST,
			   SUM(isnull(t.PromoSales,0)) as SUM_PROM_SALES, 
			   SUM(isnull(t.PromoSalesGST,0)) as SUM_PROM_SALES_GST,
			   SUM(isnull(t.Discount,0)) as SUM_DISCOUNT,
			   case when @item=1 then convert(int,t.ProductId) else t.[SupplierId] end as TRX_PRODUCT,
			   case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
			   d.[Desc] as CODE_DESC
		from [dbo].[Transaction] t with(readuncommitted)
		     inner join [dbo].[Supplier] d with(readuncommitted) on d.Id=t.[SupplierId]
			 inner join [dbo].[Product] p with(readuncommitted) on p.Id=t.ProductId
		where (t.[Date] between @startDate and @endDate)
			  and (@tillId is null or t.TillId = @tillId) 
			  and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			  and (@promoSales=0 or (@promoSales=1 and t.PromoSales!=0) and (@promoCode is null or t.PromoSellId=@promoId))
			  and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),','))) 
			  and (@productIds is null or t.ProductId in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (t.[Type] in (select * from [dbo].[SplitString](isnull(@tranType,''),',')) or @tranTender is null or t.[Tender]=@tranTender)		
		group by t.[SupplierId],t.[Type],t.[Tender],t.[ProductId],p.[Desc],d.[Desc]
			 ) as x order by x.SUM_CODE,x.CODE_DESC
    end
	else 
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
			case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		  (select p.[SupplierId] as SUM_CODE,
				SUM(isnull(j.Amount,0)) as SUM_AMT,
				SUM(isnull(j.Quantity,0)) as SUM_QTY,
				SUM(j.Cost)  as SUM_COST,		
				SUM(isnull(j.Amount,0)) as SUM_PROM_SALES, 
				SUM(isnull(j.GSTAmount,0)) as SUM_PROM_SALES_GST,
				SUM(isnull(j.DiscountAmount,0)) as SUM_DISCOUNT,
				case when @item=1 then convert(int,j.ProductId) else p.[SupplierId] end as TRX_PRODUCT,
				case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
				d.[Desc] as CODE_DESC
			from [dbo].[JournalHeader] h with(readuncommitted)
			     inner join [dbo].[JournalDetail] j with(readuncommitted) on j.JournalHeaderId=h.Id
				 inner join [dbo].[Product] p with(readuncommitted) on p.Id=j.ProductId
				 inner join [dbo].[Supplier] d with(readuncommitted) on d.Id=p.[SupplierId]
			where (h.[Date] between @startDate and @endDate)
			  and (@tillId is null or h.TillId = @tillId) 
			  and ((@productStartId is null or j.ProductId>=@productStartId) and (@productEndId is null or j.ProductId<=@productEndId)) 
			  and (j.PromoSellId=@promoId or j.PromoOfferId=@promoId or j.PromoMixMatchId=@promoId or j.PromoMemeberOfferId=@promoId)
			  and (@productIds is null or j.[ProductId] in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or h.[OutletId] in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or p.[CommodityId] in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or p.[DepartmentId] in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or p.[CategoryId] in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or p.[GroupId] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or p.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or p.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (j.[Type]='SALE')
			  and j.[Status]=1	  
			  and j.[DiscountAmount]>0
			group by p.[SupplierId],j.[ProductId],p.[Desc],d.[Desc]
			) x
	end
   end

   if(lower(@reportType)='outlet')
	begin
	set @reportType='Item Sales By Outlet'
	if(@promotionCount=0)
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
		case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		(select t.[OutletId] as SUM_CODE,
			   SUM(isnull(t.Amt,0)) as SUM_AMT,
			   SUM(isnull(t.Qty,0)) as SUM_QTY, 
			   case when @merge=1 then
				 case when @variance=1 and @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') or (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 when @wastage=1 then
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'WASTAGE' or t.[Tender] = 'WASTAGE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 else 
				   case when t.[Type]='ITEMSALE' then SUM(isnull(t.Cost,0))
					when (t.[Type] = 'VARIANCE') then (-1 * SUM(isnull(t.Cost,0)))
				   end 
				 end	    
			   else SUM(isnull(t.Cost,0)) end as SUM_COST,
			   SUM(isnull(t.PromoSales,0)) as SUM_PROM_SALES, 
			   SUM(isnull(t.PromoSalesGST,0)) as SUM_PROM_SALES_GST,
			   SUM(isnull(t.Discount,0)) as SUM_DISCOUNT,
			   case when @item=1 then convert(int,t.ProductId) else t.[OutletId] end as TRX_PRODUCT,
			   case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
			   d.[Desc] as CODE_DESC
		from [dbo].[Transaction] t with(readuncommitted)
		     inner join [dbo].[Store] d with(readuncommitted) on d.Id=t.[OutletId]
			 inner join [dbo].[Product] p with(readuncommitted) on p.Id=t.ProductId
		where (t.[Date] between @startDate and @endDate)
			  and (@tillId is null or t.TillId = @tillId) 
			  and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			  and (@promoSales=0 or (@promoSales=1 and t.PromoSales!=0) and (@promoCode is null or t.PromoSellId=@promoId))
			  and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),','))) 
			  and (@productIds is null or t.ProductId in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (t.[Type] in (select * from [dbo].[SplitString](isnull(@tranType,''),',')) or @tranTender is null or t.[Tender]=@tranTender)		
		group by t.[OutletId],t.[Type],t.[Tender],t.[ProductId],p.[Desc],d.[Desc]
			 ) as x order by x.SUM_CODE,x.CODE_DESC
     end
	else 
	begin
		select x.*,(x.SUM_AMT-X.SUM_COST) as SUM_MARGIN, 
			case when x.SUM_AMT=0 then 0 else ((x.SUM_AMT-X.SUM_COST)*100)/x.SUM_AMT end as GP_PCNT from 
		  (select h.[OutletId] as SUM_CODE,
				SUM(isnull(j.Amount,0)) as SUM_AMT,
				SUM(isnull(j.Quantity,0)) as SUM_QTY,
				SUM(j.Cost)  as SUM_COST,		
				SUM(isnull(j.Amount,0)) as SUM_PROM_SALES, 
				SUM(isnull(j.GSTAmount,0)) as SUM_PROM_SALES_GST,
				SUM(isnull(j.DiscountAmount,0)) as SUM_DISCOUNT,
				case when @item=1 then convert(int,j.ProductId) else h.[OutletId] end as TRX_PRODUCT,
				case when @item=1 then p.[Desc] else d.[Desc] end as PROD_DESC,
				d.[Desc] as CODE_DESC
			from [dbo].[JournalHeader] h with(readuncommitted)
			     inner join [dbo].[JournalDetail] j with(readuncommitted) on j.JournalHeaderId=h.Id
				 inner join [dbo].[Product] p with(readuncommitted) on p.Id=j.ProductId
				 inner join [dbo].[Store] d with(readuncommitted) on d.Id=h.[OutletId]
			where (h.[Date] between @startDate and @endDate)
			  and (@tillId is null or h.TillId = @tillId) 
			  and ((@productStartId is null or j.ProductId>=@productStartId) and (@productEndId is null or j.ProductId<=@productEndId)) 
			  and (j.PromoSellId=@promoId or j.PromoOfferId=@promoId or j.PromoMixMatchId=@promoId or j.PromoMemeberOfferId=@promoId)
			  and (@productIds is null or j.[ProductId] in (select * from [dbo].[SplitString](isnull(@productIds,''),','))) 
			  and (@storeIds is null or h.[OutletId] in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			  and (@commodityIds is null or p.[CommodityId] in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			  and (@departmentIds is null or p.[DepartmentId] in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			  and (@categoryIds is null or p.[CategoryId] in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			  and (@groupIds is null or p.[GroupId] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			  and (@suppliers is null or p.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			  and (@manufacturerIds is null or p.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			  and (j.[Type]='SALE')
			  and j.[Status]=1	  
			  and j.[DiscountAmount]>0
			group by h.[OutletId],j.[ProductId],p.[Desc],d.[Desc]
			) x
	end
   end
    exec [dbo].[GetReportsFilterTables] @startDate,@endDate,@productStartId,@productEndId,@promoSales,@promoCode,@summary,@drillDown,@contineous,@variance,@wastage,@merge,@tillId,@storeIds,@zoneIds,@dayRange,@departmentIds,@commodityIds,@categoryIds,@groupIds,@suppliers,@manufacturerIds,null,@reportType
END
