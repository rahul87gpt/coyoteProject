GO
/****** Object:  StoredProcedure [dbo].[DeactivateProductList]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[DeactivateProductList] -- DeactivateProductList '2020-05-02',95,'2,11,19','19'
@Date datetime,
@StoreId int,
@DepartmentInc nvarchar(max)=null,
@DepartmentExc nvarchar(500)=null,
@CommodityInc nvarchar(500)=null,
@CommodityExc nvarchar(500)=null,
@QtyOnHandZero bit=1
AS
SELECT p.Number, op.QtyOnHand, p.[Desc], d.[Desc] as Department FROM OutletProduct op
JOIN Product p ON  p.Id=op.ProductId
inner join Department  d on p.DepartmentId=d.Id
 WHERE (op.StoreId=@StoreId) AND (op.Status = 1) 
 AND ((@QtyOnHandZero =1 and op.QtyOnHand = 0) OR (@QtyOnHandZero =0 and op.QtyOnHand <> 0)) 
 AND (@DepartmentExc is null or p.DepartmentId not in (select * from SplitString(@DepartmentExc, ','))) 
 AND (@CommodityExc is null or p.CommodityId not in (select * from SplitString(@CommodityExc, ','))) 
 AND (@DepartmentInc is null or p.DepartmentId in (select * from SplitString(@DepartmentInc, ','))) 
 AND (@CommodityInc is null or p.CommodityId in (select * from SplitString(@CommodityInc, ','))) 
 AND (p.Id not in 
  (select Id from Product where Parent is not null or 
     (id in (select Parent from Product where Parent is not null))))
 and (p.DepartmentId not in (8,15)) and (P.CommodityId not in (869)) 
 AND (op.SupplierId <> 71) AND (op.SupplierId <> 314) AND 
 (NOT (EXISTS
   (SELECT TRXTBL.ProductId FROM [Transaction] TRXTBL WHERE TRXTBL.OutletId = op.StoreId
   AND TRXTBL.ProductId = op.ProductId AND TRXTBL.DATE > cast(@Date as date) AND TRXTBL.TYPE = 'ITEMSALE')))
ORDER BY p.DepartmentId, p.[Desc] 

 
GO
