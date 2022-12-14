/****** Object:  StoredProcedure [dbo].[StoreDashboardSales]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[StoreDashboardSales]
@StoreId int,
@ZoneId int,
@ThisYearDateFrom datetime,
@ThisYearDateTo datetime,
@LastYearDateFrom datetime,
@LastYearDateTo datetime,
@DepDateFrom datetime,
@DepDateTo datetime
AS
Begin
declare @count int 
select @count = count(1) from ZoneOutlet where ZoneId=@ZoneId
select * from (
(Select DATEPART(WK,Weekend) WeekNo,Weekend ThisYearStoreWeekend, SUM_AMT as ThisYearStoreSale,SUM_QTY as ThisYearStoreCustomer,(SUM_AMT/SUM_QTY) as ThisYearStoreAvgBasket, 
((SUM_AMT-SUM_COST)*100)/SUM_AMT as ThisStoreYearGP from (select Weekend,SUM(QTY)  AS SUM_QTY, SUM(COST) AS SUM_COST, SUM(AMT)  AS SUM_AMT from [transactionReport] 
WHERE ([Date] BETWEEN @ThisYearDateFrom AND @ThisYearDateTo) AND ([TYPE] = 'SUMCUST') AND (OUTLETid = @StoreId) GROUP BY Weekend ) ThisYearStore ) ThisYearStore 
inner join
(Select DATEPART(WK,Weekend) WeekNo,Weekend LastYearStoreWeekend, SUM_AMT as LastYearStoreSale,SUM_QTY as LastYearStoreCustomer, (SUM_AMT/SUM_QTY) as LastYearStoreAvgBasket, 
((SUM_AMT-SUM_COST)*100)/SUM_AMT as LastStoreYearGP from (SELECT Weekend,SUM(QTY)  AS SUM_QTY, SUM(COST) AS SUM_COST, SUM(AMT)  AS SUM_AMT from [transactionReport] 
WHERE ([Date] BETWEEN @LastYearDateFrom AND @LastYearDateTo) AND (TYPE='SUMCUST') AND (OUTLETid=@StoreId) GROUP BY Weekend) LastYearStore) LastYearStore 
on ThisYearStore.WeekNo = LastYearStore.WeekNo
inner join 
(Select DATEPART(WK,Weekend) WeekNo,Weekend ThisYearZoneWeekend, (SUM_AMT/@count) as ThisYearZoneSale,(SUM_QTY/@count) as ThisYearZoneCustomer, (SUM_AMT/SUM_QTY) as ThisYearZoneAvgBasket, 
((SUM_AMT-SUM_COST)*100)/SUM_AMT as ThisYearZoneGP from (SELECT Weekend, SUM(QTY)  AS SUM_QTY, SUM(COST) AS SUM_COST, SUM(AMT)  AS SUM_AMT FROM TransactionReport 
WHERE (DATE BETWEEN @ThisYearDateFrom AND @ThisYearDateTo)  AND (TYPE = 'SUMCUST') AND ( (OUTLETId in (select OutletId from ZoneOutlet where ZoneId=@ZoneId))) GROUP BY Weekend  
) ThisYearZone) ThisYearZone on ThisYearStore.WeekNo = ThisYearZone.WeekNo
 inner Join
(Select DATEPART(WK,Weekend) WeekNo,Weekend LastYearZoneWeekend, (SUM_AMT/@count) as LastYearZoneSale,(SUM_QTY/@count) as LastYearZoneCustomer, (SUM_AMT/SUM_QTY) as LastYearZoneAvgBasket, 
((SUM_AMT-SUM_COST)*100)/SUM_AMT as LastYearZoneGP from (SELECT Weekend,SUM(QTY)  AS SUM_QTY, SUM(COST) AS SUM_COST, SUM(AMT)  AS SUM_AMT FROM TransactionReport
WHERE (Date BETWEEN @LastYearDateFrom AND @LastYearDateTo) AND (TYPE = 'SUMCUST') AND ((OUTLETId in (select OutletId from ZoneOutlet where ZoneId=@ZoneId))) GROUP BY Weekend  
) LastYearZone) LastYearZone on ThisYearZone.WeekNo = LastYearZone.WeekNo
)

Select MyStore.[Desc],MyStore.MyStore,Zone.Zone,BestStore.BestStore from 
---DepartmentMyStore
(Select dep.[desc],SUM_AMT/cust.SUM_QTY MyStore from (SELECT DEPARTMENTId, OutletId, SUM(QTY)  AS SUM_QTY, SUM(COST) AS SUM_COST, SUM(AMT) AS SUM_AMT,dep.[Desc]
FROM TransactionReport trx inner join Department dep on trx.DepartmentId=dep.Id WHERE (DATE BETWEEN @DepDateFrom AND @DepDateTo) AND (TYPE = 'SUMDEPT') AND (OUTLETid=@StoreId) 
GROUP BY DEPARTMENTId, OUTLETId, dep.[Desc]) Dep 
inner join 
(SELECT OUTLETid, SUM(QTY) AS SUM_QTY FROM TransactionReport WHERE (DATE BETWEEN @DepDateFrom AND @DepDateTo) AND TYPE = 'SUMCUST' AND  (OUTLETid=@StoreId) GROUP BY OUTLETid 
) Cust on Dep.OUTLETid=Cust.OUTLETid) MyStore
inner join 
---DepartmentZone
(Select [Desc],Sum(amt)/@count Zone from (Select Dep.OutletId,Dep.SUM_COST,(Dep.SUM_AMT/Cust.SUM_QTY) amt,Dep.[Desc] from (SELECT DepartmentId, OutletId, SUM(QTY)  AS SUM_QTY, 
SUM(COST) AS SUM_COST, SUM(AMT)  AS SUM_AMT,dep.[Desc] FROM TransactionReport trx inner join Department dep on  trx.DepartmentId=dep.Id 
WHERE (DATE BETWEEN @DepDateFrom AND @DepDateTo) AND (TYPE =  'SUMDEPT') AND (OutletId in (select OutletId from ZoneOutlet where ZoneId=@ZoneId and OutletId !=@StoreId)) 
GROUP BY DEPARTMENTId, OUTLETId, dep.[Desc] ) Dep inner join (SELECT OutletId, SUM(QTY)  AS SUM_QTY FROM TransactionReport WHERE (DATE BETWEEN @DepDateFrom AND @DepDateTo)
AND TYPE = 'SUMCUST' AND (OutletId in (select OutletId from ZoneOutlet where ZoneId=@ZoneId and OutletId !=@StoreId)) GROUP BY OutletId ) Cust on Dep.OutletId=Cust.OutletId
) aa group by [Desc]) Zone on MyStore.[Desc]=Zone.[Desc]
inner join
--BestStore
(Select [Desc],Max(amt) BestStore from (Select Dep.OutletId,Dep.SUM_COST,(Dep.SUM_AMT/Cust.SUM_QTY) amt,Dep.[Desc] from (SELECT DepartmentId, OutletId, SUM(QTY)  AS SUM_QTY, 
SUM(COST) AS SUM_COST, SUM(AMT)  AS SUM_AMT,dep.[Desc] FROM TransactionReport trx inner join Department dep on trx.DepartmentId=dep.Id 
WHERE (DATE BETWEEN @DepDateFrom AND @DepDateTo) AND (TYPE =  'SUMDEPT') AND ( OutletId in (select OutletId from ZoneOutlet where ZoneId=@ZoneId and OutletId !=@StoreId)) 
GROUP BY DepartmentId, OutletId, dep.[Desc] ) Dep inner join (SELECT OutletId, SUM(QTY) AS SUM_QTY FROM TransactionReport  WHERE (DATE BETWEEN @DepDateFrom AND @DepDateTo)
AND TYPE = 'SUMCUST'  AND (OutletId in (select OutletId from ZoneOutlet where ZoneId=@ZoneId and OutletId !=@StoreId)) GROUP BY OutletId ) Cust
on Dep.OutletId=Cust.OutletId) aa group by [Desc]) BestStore on MyStore.[Desc]=BestStore.[Desc]


END

GO
