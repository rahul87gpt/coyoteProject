/****** Object:  StoredProcedure [dbo].[GetReportsFilterTables]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Narendra
-- =============================================
CREATE PROCEDURE [dbo].[GetReportsFilterTables] 
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
@cashierIds nvarchar(max) = null,
@reportName nvarchar(250)=null

AS
BEGIN
	SET NOCOUNT ON;
	declare @selectOutlets nvarchar(100)='(Selected Outlets)'
	if @storeIds is not null
	begin		
		if((select count(1) from [dbo].[SplitString](@storeIds, ','))=1)
		begin
			select top(1)@selectOutlets=([Desc]+' '+cast([Code] as varchar(10))) from [dbo].[Store] with(readuncommitted) 
			 where Id in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))
		end
	end	

	--return startDate, endDate, contineous, productStartId, productEndId, tillId
    select convert(varchar, @startDate, 105) as startDate, 
		convert(varchar, @endDate, 105) as endDate,		
		@selectOutlets as selectOutlets,
		isnull(@productStartId,0) as productStartId,
		isnull(@productEndId,0) as productEndId,
		isnull((select [Desc]+' '+Cast(Code as varchar(10)) from [dbo].[Till] where id=@tillId),'') as till, 
		@contineous as contineous,
		@drillDown as drillDown,
		@summary as summary,
		@promoSales as promoSales,
		@promoCode as promoCode,
		@merge as [merge],
		@variance as variance,
		@wastage as wastage,
		@reportName as reportName

	--Store
	if(@storeIds is not null)
	begin
	(select [Desc]+' '+Cast([Code] as varchar(10)) as [Store] from [dbo].[Store] with(readuncommitted) 
	   where Id in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
	end
	else
	begin
	 select '' as [Store]
	end

   --Department
   	if(@departmentIds is not null)
	begin
     select [Desc]+' '+Cast([Code] as varchar(10)) as [Dept] from [dbo].[Department] with(readuncommitted)
	   where Id in  (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))
	end
	else
	begin
	 select '' as [Dept]
	end

	--Commodity
	if(@commodityIds is not null)
	begin
    select [Desc]+' '+Cast([Code] as varchar(10)) as [Commodity] from [dbo].[Commodity] with(readuncommitted)
	   where Id in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))
	end
	else
	begin
	 select '' as [Commodity]
	end

	--Category
	if(@categoryIds is not null)
	begin
    select [Name]+' '+Cast([Code] as varchar(10)) as [Category] from [dbo].[MasterListItems] with(readuncommitted)
		  where listid=(select Id from [dbo].[MasterList] with(readuncommitted) where code='CATEGORY') and 
		        Id in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))
	end
	else
	begin
	 select '' as [Category]
	end

	--Groups
	if(@groupIds is not null)
	begin
    select [Name]+' '+Cast([Code] as varchar(10)) as [Groups] from [dbo].[MasterListItems] with(readuncommitted)
		  where listid=(select Id from [dbo].[MasterList] with(readuncommitted) where code='GROUP') and 
		        Id in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))
	end
	else
	begin
	 select '' as [Groups]
	end

	--Supplier
	if(@suppliers is not null)
	begin
    select [Desc]+' '+Cast(Code as varchar(10)) as [Supplier] from [dbo].[Supplier] with(readuncommitted) 
		 where Id in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))
	end
	else
	begin
	 select '' as [Supplier]
	end

	--Manufacturer
	if(@manufacturerIds is not null)
	begin
    select [Name]+' '+Cast(Code as varchar(10)) as [Manufacturer] from [dbo].[MasterListItems] with(readuncommitted)
		  where listid=(select Id from [dbo].[MasterList] with(readuncommitted) where code='MANUFACTURER') and
		        Id in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))
	end
	else
	begin
	 select '' as [Manufacturer]
	end
   
   	--Cashier
	if(@cashierIds is not null)
	begin
    select [FirstName]+' '+ [Surname] as [Cashier] from [dbo].[Cashier] with(readuncommitted)
		  where Id in (select * from [dbo].[SplitString](isnull(@cashierIds,''),','))
	end
	else
	begin
	 select '' as [Cashier]
	end

  	--Zone
	if(@zoneIds is not null)
	begin
   select [Name]+' '+Cast(Code as varchar(10)) as [Zones] from [dbo].[MasterListItems] with(readuncommitted)
		  where listid=(select Id from [dbo].[MasterList] with(readuncommitted) where code='ZONE') and
		        Id in (select * from [dbo].[SplitString](isnull(@zoneIds,''),','))
	end
	else
	begin
	 select '' as [Zones]
	end

   --DayRange
   if(@dayRange is not null)
	begin
    select * from [dbo].[SplitString](isnull(@dayRange,''),',') 
	end
	else
	begin
	 select '' as [Item]
	end
END

GO
