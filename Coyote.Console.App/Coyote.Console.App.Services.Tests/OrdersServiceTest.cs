using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class OrdersServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private readonly Mock<IAutoMappingServices> _mockAutoMapper = null;
        private readonly Mock<IOptions<FileDirectorySettings>> _mockFileDirectorySettings = null;
        private readonly Mock<IOptions<LIONOrderSettings>> _mocklLONOrderSettings = null;
        private readonly Mock<IOptions<CocaColaOrderSettings>> _mockCocaColaOrderSettings = null;
        private readonly Mock<IOptions<DistributorOrderSettings>> _mockDistributorOrderSettings = null;
        private readonly Mock<IOptions<SFTPSettings>> _mockSFTPSettings = null;
        private readonly Mock<ISendMailService> _mockSendEmailService = null;
        private readonly Mock<ISFTPService> _mockSFTPService = null;
        private readonly Mock<IEDIMetcashService> _mockEDIMetcashService = null;

        private List<Till> _tillLists = null;
        private List<APN> _apns = null;
        private List<MasterList> _masterLists = null;
        private List<MasterListItems> _masterListItems = null;
        private List<Store> _store = null;
        private List<Product> _product = null;
        private List<Department> _department = null;
        private List<Commodity> _commodity = null;
        private List<OutletProduct> _outletProduct = null;
        private List<Supplier> _supplier = null;
        private List<SupplierProduct> _supplierProduct = null;
        private List<ZoneOutlet> _zoneOutlets = null;
        private List<OrderHeader> _orderHeader = null;
        private List<OrderDetail> _orderDetail = null;
        private List<Transaction> _transaction = null;
        private PagedInputModel _inputModel = null;

        #region Setup
        public OrdersServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IAutoMappingServices>();
            _mockFileDirectorySettings = new Mock<IOptions<FileDirectorySettings>>();
            _mocklLONOrderSettings = new Mock<IOptions<LIONOrderSettings>>();
            _mockCocaColaOrderSettings = new Mock<IOptions<CocaColaOrderSettings>>();
            _mockDistributorOrderSettings = new Mock<IOptions<DistributorOrderSettings>>();
            _mockSFTPSettings = new Mock<IOptions<SFTPSettings>>();
            _mockSendEmailService = new Mock<ISendMailService>();
            _mockSFTPService = new Mock<ISFTPService>();
            _mockEDIMetcashService = new Mock<IEDIMetcashService>();
            _tillLists = new List<Till>
            {
                new Till {Id=1,Code="12312",Desc="ADJUST CODE",Status=true,IsDeleted=false}
            };

            _masterLists = new List<MasterList>
            {
                 new MasterList {Id=1,Code="ORDERTYPE",Name="ORDERTYPE",Status=true,IsDeleted=false},
                 new MasterList {Id=2,Code="ORDERSTATUS",Name="ORDERSTATUS",Status=true,IsDeleted=false},
                 new MasterList {Id=3,Code="OrderCreationType",Name="OrderCreationType",Status=true,IsDeleted=false},
                 new MasterList {Id=4,Code="OrderDocStatus",Name="OrderDocStatus",Status=true,IsDeleted=false},
                 new MasterList {Id=5,Code="ZONE",Name="ZONE",Status=true,IsDeleted=false},
                 new MasterList {Id=6,Code="ORDERDETAILTYPE",Name="ORDERDETAILTYPE",Status=true,IsDeleted=false}

            };

            _masterListItems = new List<MasterListItems>
            {
                new MasterListItems {Id=19647,Code="DELIVERY",Name="DELIVERY",Status=true,IsDeleted=false,MasterList=_masterLists[0]},
                new MasterListItems {Id=19648,Code="INVOICE",Name="INVOICE",Status=true,IsDeleted=false,MasterList=_masterLists[0]},
                new MasterListItems {Id=19649,Code="ORDER",Name="ORDER",Status=true,IsDeleted=false,MasterList=_masterLists[0]},
                new MasterListItems {Id=19653,Code="NEW",Name="NEW",Status=true,IsDeleted=false,MasterList=_masterLists[1]},
                new MasterListItems {Id=19654,Code="ORDER",Name="ORDER",Status=true,IsDeleted=false,MasterList=_masterLists[1]},
                new MasterListItems {Id=103,Code="Auto",Name="Auto",Status=true,IsDeleted=false,MasterList=_masterLists[2]},
                new MasterListItems {Id=105,Code="Manual",Name="Manual",Status=true,IsDeleted=false,MasterList=_masterLists[2]},
                new MasterListItems {Id=98,Code="NEW",Name="NEW",Status=true,IsDeleted=false,MasterList=_masterLists[3]},
                new MasterListItems {Id=99,Code="ORDER",Name="ORDER",Status=true,IsDeleted=false,MasterList=_masterLists[3]},
                new MasterListItems {Id=658,Code="700",Name="ENOGGERA",Status=true,IsDeleted=false,MasterList=_masterLists[4]},
                new MasterListItems {Id=659,Code="701",Name="CLAYFIELD",Status=true,IsDeleted=false,MasterList=_masterLists[4]},
                new MasterListItems {Id=660,Code="704",Name="MARY ST",Status=true,IsDeleted=false,MasterList=_masterLists[4]},
                new MasterListItems {Id=661,Code="705",Name="CALOUNDRA",Status=true,IsDeleted=false,MasterList=_masterLists[4]},
                new MasterListItems {Id=19486,Code="NORMALBUY",Name="NORMALBUY",Status=true,IsDeleted=false,MasterList=_masterLists[5]},
                new MasterListItems {Id=19488,Code="INVESTMENTBUY",Name="INVESTMENTBUY",Status=true,IsDeleted=false,MasterList=_masterLists[5]},

            };

            _store = new List<Store>
            {
                new Store {Id=95,Code="792",Desc="KANGAROO POINT",GroupId=18,Status=true },
                new Store {Id=5,Code="700",Desc="ENOGGERA",GroupId=18,Status=true },
                new Store {Id=6,Code="701",Desc="ZZ CLAYFIELD",GroupId=18,Status=true },
                new Store {Id=7,Code="702",Desc="KANGAROO POINT",GroupId=18,Status=true },
            };

            _apns = new List<APN>();

            _zoneOutlets = new List<ZoneOutlet> {
                new ZoneOutlet {Id=1,StoreId=95,ZoneId=658,CreatedAt=DateTime.UtcNow,IsDeleted=false,ZoneMasterListItems=_masterListItems[9],Store=_store[0] },
                new ZoneOutlet {Id=2,StoreId=5,ZoneId=659,CreatedAt=DateTime.UtcNow,IsDeleted=false,ZoneMasterListItems=_masterListItems[10],Store=_store[1] },
                new ZoneOutlet {Id=3,StoreId=6,ZoneId=660,CreatedAt=DateTime.UtcNow,IsDeleted=false,ZoneMasterListItems=_masterListItems[11],Store=_store[2] },
                new ZoneOutlet {Id=4,StoreId=7,ZoneId=661,CreatedAt=DateTime.UtcNow,IsDeleted=false,ZoneMasterListItems=_masterListItems[12],Store=_store[3] },
            };

            _supplier = new List<Supplier>()
            {
                new Supplier { Id=1,Code="S01",Desc="CONGA FOODS",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=2,Code="S02",Desc="CONGA FOODS1",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=3,Code="S03",Desc="CONGA FOODS2",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=4,Code="S04",Desc="CONGA FOODS3",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=5,Code="S05",Desc="CONGA FOODS4",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false}
            };

            _department = new List<Department>()
            {
                new Department{ Id=1,Code="A1",Desc="GROCERY",MapTypeId=92,BudgetGroethFactor=1,RoyaltyDisc=3,AdvertisingDisc=2,AllowSaleDisc=true,ExcludeWastageOptimalOrdering=true,IsDefault=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1},
                new Department{ Id=2,Code="A2",Desc="GROCERY2",MapTypeId=92,BudgetGroethFactor=1,RoyaltyDisc=3,AdvertisingDisc=2,AllowSaleDisc=true,ExcludeWastageOptimalOrdering=true,IsDefault=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1},
                new Department{ Id=3,Code="A3",Desc="GROCERY3",MapTypeId=92,BudgetGroethFactor=1,RoyaltyDisc=3,AdvertisingDisc=2,AllowSaleDisc=true,ExcludeWastageOptimalOrdering=true,IsDefault=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1},
                new Department{ Id=4,Code="A4",Desc="GROCERY4",MapTypeId=92,BudgetGroethFactor=1,RoyaltyDisc=3,AdvertisingDisc=2,AllowSaleDisc=true,ExcludeWastageOptimalOrdering=true,IsDefault=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1},
                new Department{ Id=5,Code="A5",Desc="GROCERY5",MapTypeId=92,BudgetGroethFactor=1,RoyaltyDisc=3,AdvertisingDisc=2,AllowSaleDisc=true,ExcludeWastageOptimalOrdering=true,IsDefault=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1},
            };

            _commodity = new List<Commodity>()
            {
                new Commodity{ Id=1,Code="C1",Desc="AA",CoverDays=92,DepartmentId=1,GPPcntLevel1=3,GPPcntLevel2=2,GPPcntLevel3=3,IsDeleted=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,Departments=_department[0]},
                new Commodity{ Id=2,Code="C2",Desc="AW",CoverDays=92,DepartmentId=2,GPPcntLevel1=3,GPPcntLevel2=2,GPPcntLevel3=4,IsDeleted=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,Departments=_department[1]},
                new Commodity{ Id=3,Code="C3",Desc="DE",CoverDays=92,DepartmentId=3,GPPcntLevel1=3,GPPcntLevel2=2,GPPcntLevel3=5,IsDeleted=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,Departments=_department[2]},
                new Commodity{ Id=4,Code="C4",Desc="ED",CoverDays=92,DepartmentId=3,GPPcntLevel1=3,GPPcntLevel2=2,GPPcntLevel3=4,IsDeleted=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,Departments=_department[2]},
                new Commodity{ Id=5,Code="C5",Desc="ED",CoverDays=92,DepartmentId=3,GPPcntLevel1=3,GPPcntLevel2=2,GPPcntLevel3=5,IsDeleted=false,Status=true,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,Departments=_department[2]},
            };

            _product = new List<Product>
            {
                new Product {Id=1,Number=1403096,Desc="Test1",PosDesc="",CartonQty=20,CartonCost=42,UnitQty=1,DepartmentId=1,SupplierId=1,CommodityId=412,TaxId=1,GroupId=19434,CategoryId=257,ManufacturerId=12351,TypeId=19838,NationalRangeId=632,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[0],Department=_department[0],Commodity=_commodity[0]},
                new Product {Id=2,Number=1803440,Desc="Test2",PosDesc="",CartonQty=25,CartonCost=45,UnitQty=100,DepartmentId=2,SupplierId=2,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=630,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[1],Department=_department[1],Commodity=_commodity[1]},
                new Product {Id=3,Number=1803102,Desc="Test3",PosDesc="",CartonQty=75,CartonCost=97,UnitQty=40,DepartmentId=2,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[1],Commodity=_commodity[1]},
                new Product {Id=4,Number=1803103,Desc="Test4",PosDesc="",CartonQty=78,CartonCost=74,UnitQty=54,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[2],Commodity=_commodity[2]},
                new Product {Id=5,Number=1803104,Desc="Test5",PosDesc="",CartonQty=47,CartonCost=45,UnitQty=45,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[2],Commodity=_commodity[2]},
                new Product {Id=6,Number=1803104,Desc="Test6",PosDesc="",CartonQty=25,CartonCost=85,UnitQty=14,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[2],Commodity=_commodity[2]},
                new Product {Id=7,Number=1803104,Desc="Test7",PosDesc="",CartonQty=45,CartonCost=65,UnitQty=52,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[2],Commodity=_commodity[2]},
                new Product {Id=8,Number=1803104,Desc="Test8",PosDesc="",CartonQty=45,CartonCost=85,UnitQty=32,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[2],Commodity=_commodity[2]},
                new Product {Id=9,Number=1803104,Desc="Test9",PosDesc="",CartonQty=82,CartonCost=96,UnitQty=36,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[2],Commodity=_commodity[2]},
                new Product {Id=9,Number=1803105,Desc="Test10",PosDesc="",CartonQty=82,CartonCost=96,UnitQty=36,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2],Department=_department[2],Commodity=_commodity[2]},

            };

            _supplierProduct = new List<SupplierProduct>
            {
                new SupplierProduct {Id=1,Desc="Test1",SupplierId=1,ProductId=1,CartonCost=42,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[0],Product=_product[0]},
                new SupplierProduct {Id=2,Desc="Test2",SupplierId=1,ProductId=1,CartonCost=45,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[0],Product=_product[0]},
                new SupplierProduct {Id=3,Desc="Test3",SupplierId=2,ProductId=2,CartonCost=97,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[1],Product=_product[1]},
                new SupplierProduct {Id=4,Desc="Test4",SupplierId=2,ProductId=2,CartonCost=74,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[1],Product=_product[1]},
                new SupplierProduct {Id=5,Desc="Test5",SupplierId=3,ProductId=3,CartonCost=45,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[2],Product=_product[2]},
                new SupplierProduct {Id=6,Desc="Test6",SupplierId=3,ProductId=3,CartonCost=85,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[2],Product=_product[2]},
                new SupplierProduct {Id=7,Desc="Test7",SupplierId=4,ProductId=4,CartonCost=65,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[3],Product=_product[3]},
                new SupplierProduct {Id=8,Desc="Test8",SupplierId=4,ProductId=4,CartonCost=85,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[3],Product=_product[3]},
                new SupplierProduct {Id=9,Desc="Test9",SupplierId=5,ProductId=5,CartonCost=96,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[4],Product=_product[4]},
                new SupplierProduct {Id=9,Desc="Test10",SupplierId=5,ProductId=5,CartonCost=96,MinReorderQty=1,BestBuy=true,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,Status=true,Supplier=_supplier[4],Product=_product[4]},
            };

            _outletProduct = new List<OutletProduct>
            {
                new OutletProduct {Id=1,StoreId=95,ProductId=1,NormalPrice1=2,CartonCostInv=42,CartonCost=42,QtyOnHand=43,MinOnHand=2,SupplierId=111,MinReorderQty=2,ChangeLabelInd=true,LabelQty=1,PickingBinNo=2,Status=true,IsDeleted=false,Store=_store[0],Product=_product[0] },
                new OutletProduct {Id=2,StoreId=95,ProductId=2,NormalPrice1=3,CartonCostInv=7,CartonCost=7,QtyOnHand=1,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=0,Status=true,IsDeleted=false,Store=_store[0],Product=_product[1] },
                new OutletProduct {Id=3,StoreId=95,ProductId=3,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=3,Status=true,IsDeleted=false,Store=_store[0],Product=_product[2] },
                new OutletProduct {Id=4,StoreId=95,ProductId=4,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=0,Status=true,IsDeleted=false,Store=_store[0],Product=_product[3] },
                new OutletProduct {Id=5,StoreId=95,ProductId=5,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=0,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=0,Status=true,IsDeleted=false,Store=_store[0],Product=_product[4] },
                new OutletProduct {Id=6,StoreId=95,ProductId=6,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=2,Status=true,IsDeleted=false,Store=_store[0],Product=_product[5] },
                new OutletProduct {Id=7,StoreId=95,ProductId=7,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=3,Status=true,IsDeleted=false,Store=_store[0],Product=_product[6] },
                new OutletProduct {Id=8,StoreId=95,ProductId=8,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=0,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=0,Status=true,IsDeleted=false,Store=_store[0],Product=_product[7] },
                new OutletProduct {Id=9,StoreId=95,ProductId=9,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=4,Status=true,IsDeleted=false,Store=_store[0],Product=_product[8] },
                new OutletProduct {Id=10,StoreId=95,ProductId=10,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,PickingBinNo=0,Status=true,IsDeleted=false,Store=_store[0],Product=_product[9] },
               };

            _orderDetail = new List<OrderDetail>
            {
                new OrderDetail{ Id=1,OrderHeaderId=1,OrderTypeId=19486,LineNo=1,ProductId=1,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[0]},
                new OrderDetail{ Id=2,OrderHeaderId=1,OrderTypeId=19486,LineNo=1,ProductId=1,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[0]},
                new OrderDetail{ Id=3,OrderHeaderId=2,OrderTypeId=19486,LineNo=1,ProductId=2,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[1]},
                new OrderDetail{ Id=4,OrderHeaderId=2,OrderTypeId=19486,LineNo=1,ProductId=2,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[1]},
                new OrderDetail{ Id=5,OrderHeaderId=3,OrderTypeId=19486,LineNo=1,ProductId=3,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[2]},
                new OrderDetail{ Id=6,OrderHeaderId=3,OrderTypeId=19486,LineNo=1,ProductId=3,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[2]},
                new OrderDetail{ Id=7,OrderHeaderId=4,OrderTypeId=19486,LineNo=1,ProductId=4,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[3]},
                new OrderDetail{ Id=8,OrderHeaderId=4,OrderTypeId=19486,LineNo=1,ProductId=4,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[3]},
                new OrderDetail{ Id=9,OrderHeaderId=5,OrderTypeId=19486,LineNo=1,ProductId=5,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[13],Product=_product[4]},
                new OrderDetail{ Id=10,OrderHeaderId=5,OrderTypeId=19488,LineNo=1,ProductId=5,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[4]},
                new OrderDetail{ Id=11,OrderHeaderId=6,OrderTypeId=19488,LineNo=1,ProductId=6,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[5]},
                new OrderDetail{ Id=12,OrderHeaderId=6,OrderTypeId=19488,LineNo=1,ProductId=6,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[5]},
                new OrderDetail{ Id=13,OrderHeaderId=7,OrderTypeId=19488,LineNo=1,ProductId=7,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[6]},
                new OrderDetail{ Id=14,OrderHeaderId=7,OrderTypeId=19488,LineNo=1,ProductId=7,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[6]},
                new OrderDetail{ Id=15,OrderHeaderId=8,OrderTypeId=19488,LineNo=1,ProductId=8,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[7]},
                new OrderDetail{ Id=16,OrderHeaderId=8,OrderTypeId=19488,LineNo=1,ProductId=8,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[7]},
                new OrderDetail{ Id=17,OrderHeaderId=9,OrderTypeId=19488,LineNo=1,ProductId=9,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[8]},
                new OrderDetail{ Id=18,OrderHeaderId=9,OrderTypeId=19488,LineNo=1,ProductId=9,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[8]},
                new OrderDetail{ Id=19,OrderHeaderId=10,OrderTypeId=19488,LineNo=1,ProductId=10,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[9]},
                new OrderDetail{ Id=20,OrderHeaderId=10,OrderTypeId=19488,LineNo=1,ProductId=10,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderType=_masterListItems[14],Product=_product[9]},

            };

            _orderHeader = new List<OrderHeader>
            {
                new OrderHeader { Id=0,OutletId=95,OrderNo=1,SupplierId=1,TypeId=19649,StatusId=98,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=10,SubTotalFreight=10,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=1,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[0],Supplier=_supplier[0],Type=_masterListItems[2],Status=_masterListItems[7],CreationType=_masterListItems[5],OrderDetail=_orderDetail},
                new OrderHeader { Id=1,OutletId=95,OrderNo=1,SupplierId=1,TypeId=19649,StatusId=98,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=10,SubTotalFreight=10,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=1,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[0],Supplier=_supplier[0],Type=_masterListItems[2],Status=_masterListItems[7],CreationType=_masterListItems[5],OrderDetail=_orderDetail},
                new OrderHeader { Id=2,OutletId=5,OrderNo=8296,SupplierId=1,TypeId=19649,StatusId=98,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="15",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1033,SubTotalFreight=4582,SubTotalAdmin=125,SubTotalSubsidy=50,SubTotalDisc=140,SubTotalTax=125,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[1],Supplier=_supplier[0],Type=_masterListItems[2],Status=_masterListItems[7],CreationType=_masterListItems[5],OrderDetail=_orderDetail},
                new OrderHeader { Id=3,OutletId=6,OrderNo=518,SupplierId=2,TypeId=19649,StatusId=98,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="18522",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1150,SubTotalFreight=25200,SubTotalAdmin=10,SubTotalSubsidy=150,SubTotalDisc=155,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=10,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[2],Supplier=_supplier[1],Type=_masterListItems[2],Status=_masterListItems[7],CreationType=_masterListItems[5],OrderDetail=_orderDetail},
                new OrderHeader { Id=4,OutletId=95,OrderNo=517,SupplierId=2,TypeId=19648,StatusId=99,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="17520",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1450,SubTotalFreight=1452,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[0],Supplier=_supplier[1],Type=_masterListItems[1],Status=_masterListItems[8],CreationType=_masterListItems[5],OrderDetail=_orderDetail},
                new OrderHeader { Id=5,OutletId=95,OrderNo=516,SupplierId=3,TypeId=19648,StatusId=99,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1420",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1458,SubTotalFreight=1522,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=25,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[0],Supplier=_supplier[2],Type=_masterListItems[1],Status=_masterListItems[8],CreationType=_masterListItems[5],OrderDetail=_orderDetail},
                new OrderHeader { Id=6,OutletId=7,OrderNo=515,SupplierId=3,TypeId=19649,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="8458",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=5852,SubTotalFreight=14522,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=36,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[3],Supplier=_supplier[2],Type=_masterListItems[2],Status=_masterListItems[4],CreationType=_masterListItems[6],OrderDetail=_orderDetail},
                new OrderHeader { Id=7,OutletId=95,OrderNo=9512,SupplierId=4,TypeId=19649,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="4785",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=7582,SubTotalFreight=1458,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[0],Supplier=_supplier[3],Type=_masterListItems[2],Status=_masterListItems[4],CreationType=_masterListItems[6],OrderDetail=_orderDetail},
                new OrderHeader { Id=8,OutletId=5,OrderNo=9511,SupplierId=4,TypeId=19648,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="175",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=4582,SubTotalFreight=1145,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=7,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[1],Supplier=_supplier[3],Type=_masterListItems[1],Status=_masterListItems[4],CreationType=_masterListItems[6],OrderDetail=_orderDetail},
                new OrderHeader { Id=9,OutletId=95,OrderNo=2961,SupplierId=5,TypeId=19648,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1600",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=4588,SubTotalFreight=585,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[0],Supplier=_supplier[4],Type=_masterListItems[1],Status=_masterListItems[4],CreationType=_masterListItems[6],OrderDetail=_orderDetail},
                new OrderHeader { Id=10,OutletId=95,OrderNo=153454,SupplierId=5,TypeId=19647,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1452",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=6958,SubTotalFreight=458,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=11,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[0],Supplier=_supplier[4],Type=_masterListItems[0],Status=_masterListItems[4],CreationType=_masterListItems[6],OrderDetail=_orderDetail}
            };

            _orderDetail = new List<OrderDetail>
            {
                new OrderDetail{ Id=1,OrderHeaderId=1,OrderTypeId=19486,LineNo=1,ProductId=1,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[0],OrderType=_masterListItems[13],Product=_product[0]},
                new OrderDetail{ Id=2,OrderHeaderId=1,OrderTypeId=19486,LineNo=1,ProductId=1,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[0],OrderType=_masterListItems[13],Product=_product[0]},
                new OrderDetail{ Id=3,OrderHeaderId=2,OrderTypeId=19486,LineNo=1,ProductId=2,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[1],OrderType=_masterListItems[13],Product=_product[1]},
                new OrderDetail{ Id=4,OrderHeaderId=2,OrderTypeId=19486,LineNo=1,ProductId=2,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[1],OrderType=_masterListItems[13],Product=_product[1]},
                new OrderDetail{ Id=5,OrderHeaderId=3,OrderTypeId=19486,LineNo=1,ProductId=3,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[2],OrderType=_masterListItems[13],Product=_product[2]},
                new OrderDetail{ Id=6,OrderHeaderId=3,OrderTypeId=19486,LineNo=1,ProductId=3,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[2],OrderType=_masterListItems[13],Product=_product[2]},
                new OrderDetail{ Id=7,OrderHeaderId=4,OrderTypeId=19486,LineNo=1,ProductId=4,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[3],OrderType=_masterListItems[13],Product=_product[3]},
                new OrderDetail{ Id=8,OrderHeaderId=4,OrderTypeId=19486,LineNo=1,ProductId=4,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[3],OrderType=_masterListItems[13],Product=_product[3]},
                new OrderDetail{ Id=9,OrderHeaderId=5,OrderTypeId=19486,LineNo=1,ProductId=5,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[4],OrderType=_masterListItems[13],Product=_product[4]},
                new OrderDetail{ Id=10,OrderHeaderId=5,OrderTypeId=19488,LineNo=1,ProductId=5,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[4],OrderType=_masterListItems[14],Product=_product[4]},
                new OrderDetail{ Id=11,OrderHeaderId=6,OrderTypeId=19488,LineNo=1,ProductId=6,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[5],OrderType=_masterListItems[14],Product=_product[5]},
                new OrderDetail{ Id=12,OrderHeaderId=6,OrderTypeId=19488,LineNo=1,ProductId=6,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[5],OrderType=_masterListItems[14],Product=_product[5]},
                new OrderDetail{ Id=13,OrderHeaderId=7,OrderTypeId=19488,LineNo=1,ProductId=7,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[6],OrderType=_masterListItems[14],Product=_product[6]},
                new OrderDetail{ Id=14,OrderHeaderId=7,OrderTypeId=19488,LineNo=1,ProductId=7,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[6],OrderType=_masterListItems[14],Product=_product[6]},
                new OrderDetail{ Id=15,OrderHeaderId=8,OrderTypeId=19488,LineNo=1,ProductId=8,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[7],OrderType=_masterListItems[14],Product=_product[7]},
                new OrderDetail{ Id=16,OrderHeaderId=8,OrderTypeId=19488,LineNo=1,ProductId=8,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[7],OrderType=_masterListItems[14],Product=_product[7]},
                new OrderDetail{ Id=17,OrderHeaderId=9,OrderTypeId=19488,LineNo=1,ProductId=9,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[8],OrderType=_masterListItems[14],Product=_product[8]},
                new OrderDetail{ Id=18,OrderHeaderId=9,OrderTypeId=19488,LineNo=1,ProductId=9,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[8],OrderType=_masterListItems[14],Product=_product[8]},
                new OrderDetail{ Id=19,OrderHeaderId=10,OrderTypeId=19488,LineNo=1,ProductId=10,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[9],OrderType=_masterListItems[14],Product=_product[9]},
                new OrderDetail{ Id=20,OrderHeaderId=10,OrderTypeId=19488,LineNo=1,ProductId=10,SupplierProductId=null,CartonCost=19,Cartons=0,Units=12,TotalUnits=12,DeliverCartons=null,DeliverUnits=null,DeliverTotalUnits=null,LineTotal=19,CartonQty=10,BonusInd=null,PostedDate=DateTime.UtcNow,FinalLineTotal=19,FinalCartonCost=19,OnHand=null,OnOrder=null,UnitsSold=null,CoverUnits=null,NonPromoMinOnHand=null,PromoMinOnHand=null,NonPromoAvgDaily=null,PromoAvgDaily=null,NormalCoverDays=null,CoverDaysUsed=null,MinReorderQty=null,Perishable=null,NonPromoSales56Days=null,PromoSales56Days=null,BuyPromoCode=null,BuyPromoDisc=null,BuyPromoEndDate=null,SalePromoCode=null,SalePromoEndDate=null,CheckSuuplier=null,CheaperSupplierId=null,NewProduct=false,CreatedAt=DateTime.UtcNow,UpdatedAt=DateTime.UtcNow,CreatedById=1,UpdatedById=1,IsDeleted=false,OrderHeader=_orderHeader[9],OrderType=_masterListItems[14],Product=_product[9]},

            };

            _transaction = new List<Transaction>
            {
                new Transaction{ Id=1,Date=DateTime.UtcNow.AddDays(-1),Type="ITEMSALE",ProductId=1,OutletId=95,TillId=1,Sequence=0,SupplierId=1,ManufacturerId=1,Group=1,DepartmentId=1,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[0],Department=_department[0],Till=_tillLists[0]},
                new Transaction{ Id=2,Date=DateTime.UtcNow.AddDays(-1),Type="ITEMSALE",ProductId=2,OutletId=95,TillId=1,Sequence=0,SupplierId=1,ManufacturerId=1,Group=1,DepartmentId=1,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[0],Department=_department[0],Till=_tillLists[0]},
                new Transaction{ Id=3,Date=DateTime.UtcNow.AddDays(-2),Type="ITEMSALE",ProductId=3,OutletId=95,TillId=1,Sequence=0,SupplierId=2,ManufacturerId=1,Group=1,DepartmentId=2,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[1],Department=_department[1],Till=_tillLists[0]},
                new Transaction{ Id=4,Date=DateTime.UtcNow.AddDays(-3),Type="ITEMSALE",ProductId=4,OutletId=95,TillId=1,Sequence=0,SupplierId=2,ManufacturerId=1,Group=1,DepartmentId=2,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[1],Department=_department[1],Till=_tillLists[0]},
                new Transaction{ Id=5,Date=DateTime.UtcNow.AddDays(-4),Type="ITEMSALE",ProductId=5,OutletId=95,TillId=1,Sequence=0,SupplierId=3,ManufacturerId=1,Group=1,DepartmentId=3,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[2],Department=_department[2],Till=_tillLists[0]},
                new Transaction{ Id=6,Date=DateTime.UtcNow.AddDays(-1),Type="ITEMSALE",ProductId=6,OutletId=95,TillId=1,Sequence=0,SupplierId=3,ManufacturerId=1,Group=1,DepartmentId=3,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[2],Department=_department[2],Till=_tillLists[0]},
                new Transaction{ Id=7,Date=DateTime.UtcNow.AddDays(-2),Type="ITEMSALE",ProductId=7,OutletId=95,TillId=1,Sequence=0,SupplierId=4,ManufacturerId=1,Group=1,DepartmentId=3,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[3],Department=_department[2],Till=_tillLists[0]},
                new Transaction{ Id=8,Date=DateTime.UtcNow.AddDays(-1),Type="ITEMSALE",ProductId=8,OutletId=95,TillId=1,Sequence=0,SupplierId=4,ManufacturerId=1,Group=1,DepartmentId=3,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[3],Department=_department[2],Till=_tillLists[0]},
                new Transaction{ Id=9,Date=DateTime.UtcNow.AddDays(-1),Type="ITEMSALE",ProductId=9,OutletId=95,TillId=1,Sequence=0,SupplierId=5,ManufacturerId=1,Group=1,DepartmentId=3,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[4],Department=_department[2],Till=_tillLists[0]},
                new Transaction{ Id=10,Date=DateTime.UtcNow.AddDays(-10),Type="ITEMSALE",ProductId=10,OutletId=95,TillId=1,Sequence=0,SupplierId=5,ManufacturerId=1,Group=1,DepartmentId=3,CommodityId=1,CategoryId=1,SubRange=0,Reference="TILLJNL",UserId=1,Qty=10,Amt=140,Cost=14,Discount=0,Price=14,Weekend=DateTime.UtcNow,Day="Mon",NewOnHand=0,Member=0,Points=0,CartonQty=10,UnitQty=1,Parent=0,StockMovement=10,Tender="",ManualInd=true,GLAccount=null,GLPostedInd=true,PromoSales=0,PromoSalesGST=0,Debtor=0,Flags=null,ReferenceType=null,ReferenceNumber=null,TermsRebate=null,TermsRebateCode=null,ScanRebate=null,ScanRebateCode=null,PurchaseRebate=null,PurchaseRebateCode=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,PromoBuyId=null,PromoSellId=null,ExGSTAmt=0,ExGSTCost=0,Supplier=_supplier[4],Department=_department[2],Till=_tillLists[0]},
              };

            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }
        #endregion

        #region Invoice history
        [Fact]
        public async Task InvoiceHistory_First_Valid_Test()
        {
            try
            {
                //Arrange           
                var zones = new List<int>
                {
                    658,569,660,661
                };
                var outlet = new List<int>
                {
                    95,6,7,8
                };
                var suppliers = new List<int>
                {
                   1,3,2,4,5
                };
                var inputModel = new OrderInvoiceRequestModel
                {
                    UseInvoiceDates = true,
                    ShowOrderHistory = true,
                    Zones = zones,
                    Outlets = outlet,
                    Suppliers = suppliers,
                    InvoiceDateFrom = DateTime.UtcNow.Date.AddDays(-3),
                    InvoiceDateTo = DateTime.UtcNow.Date
                };
                var securityModel = new SecurityViewModel();

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.GetAllActiveOrderHeaders(inputModel, securityModel).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(6, result.Data.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task InvoiceHistory_Second_Valid_Test()
        {
            try
            {
                //Arrange           
                var zones = new List<int>
                {
                    658,569,660,661
                };
                var outlet = new List<int>
                {
                    95,6,7,8
                };
                var suppliers = new List<int>
                {
                   1,3,2,4,5
                };
                var inputModel = new OrderInvoiceRequestModel
                {
                    UseInvoiceDates = false,
                    ShowOrderHistory = true,
                    Zones = zones,
                    Outlets = outlet,
                    Suppliers = suppliers,
                    OrderPostedDateFrom = DateTime.UtcNow.Date.AddDays(-3),
                    OrderPostedDateTo = DateTime.UtcNow.Date
                };
                var securityModel = new SecurityViewModel();

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object,_mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.GetAllActiveOrderHeaders(inputModel, securityModel).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(6, result.Data.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteHeader_Valid_Test()
        {
            try
            {
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().Update(It.IsAny<OrderHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().Update(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.DeleteOrder(1, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(true, result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task DeleteHeader_NullReferenceException_Test()
        {
            try
            {
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().Update(It.IsAny<OrderHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().Update(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);


                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockOrdersService.DeleteOrder(25, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DeleteHeaderDetailItem
        [Fact]
        public async Task DeleteHeaderDetailItem_Valid_Test()
        {
            try
            {
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().Update(It.IsAny<OrderHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().Update(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.DeleteOrderDetailItem(1, 1, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(true, result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task DeleteHeaderDetailItem_NullReferenceException_Test()
        {
            try
            {
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().Update(It.IsAny<OrderHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().Update(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockOrdersService.DeleteOrderDetailItem(256, 1, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetOrderById
        [Fact]
        public async Task GetOrderById_Test()
        {
            try
            {
                //Arrange      
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.GetOrderById(1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(1, result.OrderHeaders.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Insert
        [Fact]
        public async Task Insert_Order_Valid_Test()
        {
            try
            {
                var orderDetailRequestModelList = new List<OrderDetailRequestModel>() {
                    new OrderDetailRequestModel{ OrderTypeId=19649,LineNo=10,ProductId=4,CartonCost=100,Cartons=150,Units=25,TotalUnits=25,LineTotal=50,CartonQty=50,PostedDate=DateTime.UtcNow,BuyPromoCode="Test",SalePromoCode="Test",Desc="Test" }
                };

                var orderRequestModel = new OrderRequestModel
                {
                    OrderNo = 1,
                    SupplierId = 1,
                    OutletId = 95,
                    Reference = "Test",
                    CoverDays = 2,
                    CreatedDate = DateTime.UtcNow,
                    CreationTypeId = 103,
                    TypeId = 19649,
                    StatusId = 19653,
                    OrderDetails = orderDetailRequestModelList
                };

                var orderHeaderMock = new Mock<OrderHeader>();
                var orderDetailMock = new Mock<OrderDetail>();

                var mockStore = Helper.CreateDbSetMock(_store);
                var mockTill = Helper.CreateDbSetMock(_tillLists);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTill.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().InsertAsync(It.IsAny<OrderHeader>())).Returns(Task.FromResult(orderHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().InsertAsync(It.IsAny<OrderDetail>())).Returns(Task.FromResult(orderDetailMock.Object));

                _mockAutoMapper.Setup(mock => mock.Mapping<OrderRequestModel, OrderHeader>(It.IsAny<OrderRequestModel>())).Returns(orderHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<OrderDetailRequestModel, OrderDetail>(It.IsAny<OrderDetailRequestModel>())).Returns(orderDetailMock.Object);
                orderHeaderMock.Setup(x => x.OrderDetail.Add(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.Insert(orderRequestModel, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(0, result.OrderHeaders.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Insert_Invoice_Valid_Test()
        {
            try
            {
                var orderDetailRequestModelList = new List<OrderDetailRequestModel>() {
                    new OrderDetailRequestModel{ OrderTypeId=19648,LineNo=10,ProductId=4,CartonCost=100,Cartons=150,Units=25,TotalUnits=25,LineTotal=50,CartonQty=50,PostedDate=DateTime.UtcNow,BuyPromoCode="Test",SalePromoCode="Test",Desc="Test" }
                };

                var orderRequestModel = new OrderRequestModel
                {
                    OrderNo = 1,
                    SupplierId = 1,
                    OutletId = 95,
                    Reference = "Test",
                    CoverDays = 2,
                    CreatedDate = DateTime.UtcNow,
                    CreationTypeId = 103,
                    TypeId = 19648,
                    StatusId = 19653,
                    InvoiceNo = "12345",
                    InvoiceDate = DateTime.UtcNow,
                    InvoiceTotal = 100,
                    OrderDetails = orderDetailRequestModelList
                };

                var orderHeaderMock = new Mock<OrderHeader>();
                var orderDetailMock = new Mock<OrderDetail>();

                var mockStore = Helper.CreateDbSetMock(_store);
                var mockTill = Helper.CreateDbSetMock(_tillLists);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTill.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().InsertAsync(It.IsAny<OrderHeader>())).Returns(Task.FromResult(orderHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().InsertAsync(It.IsAny<OrderDetail>())).Returns(Task.FromResult(orderDetailMock.Object));

                _mockAutoMapper.Setup(mock => mock.Mapping<OrderRequestModel, OrderHeader>(It.IsAny<OrderRequestModel>())).Returns(orderHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<OrderDetailRequestModel, OrderDetail>(It.IsAny<OrderDetailRequestModel>())).Returns(orderDetailMock.Object);
                orderHeaderMock.Setup(x => x.OrderDetail.Add(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.Insert(orderRequestModel, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(0, result.OrderHeaders.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Insert_Deliver_Valid_Test()
        {
            try
            {
                var orderDetailRequestModelList = new List<OrderDetailRequestModel>() {
                    new OrderDetailRequestModel{ OrderTypeId=19647,LineNo=10,ProductId=4,CartonCost=100,Cartons=150,Units=25,TotalUnits=25,LineTotal=50,CartonQty=50,PostedDate=DateTime.UtcNow,BuyPromoCode="Test",SalePromoCode="Test",Desc="Test" }
                };

                var orderRequestModel = new OrderRequestModel
                {
                    OrderNo = 1,
                    SupplierId = 1,
                    OutletId = 95,
                    Reference = "Test",
                    CoverDays = 2,
                    CreatedDate = DateTime.UtcNow,
                    CreationTypeId = 103,
                    TypeId = 19647,
                    StatusId = 19653,
                    DeliveryNo = "12345",
                    DeliveryDate = DateTime.UtcNow,
                    OrderDetails = orderDetailRequestModelList
                };

                var orderHeaderMock = new Mock<OrderHeader>();
                var orderDetailMock = new Mock<OrderDetail>();

                var mockStore = Helper.CreateDbSetMock(_store);
                var mockTill = Helper.CreateDbSetMock(_tillLists);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTill.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().InsertAsync(It.IsAny<OrderHeader>())).Returns(Task.FromResult(orderHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().InsertAsync(It.IsAny<OrderDetail>())).Returns(Task.FromResult(orderDetailMock.Object));

                _mockAutoMapper.Setup(mock => mock.Mapping<OrderRequestModel, OrderHeader>(It.IsAny<OrderRequestModel>())).Returns(orderHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<OrderDetailRequestModel, OrderDetail>(It.IsAny<OrderDetailRequestModel>())).Returns(orderDetailMock.Object);
                orderHeaderMock.Setup(x => x.OrderDetail.Add(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.Insert(orderRequestModel, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(0, result.OrderHeaders.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Update
        [Fact]
        public async Task Update_Order_Valid_Test()
        {
            try
            {
                var orderDetailRequestModelList = new List<OrderDetailRequestModel>() {
                    new OrderDetailRequestModel{ OrderTypeId=19649,LineNo=10,ProductId=4,CartonCost=100,Cartons=150,Units=25,TotalUnits=25,LineTotal=50,CartonQty=50,PostedDate=DateTime.UtcNow,BuyPromoCode="Test",SalePromoCode="Test",Desc="Test" }
                };

                var orderRequestModel = new OrderRequestModel
                {
                    OrderNo = 122,
                    SupplierId = 1,
                    OutletId = 95,
                    Reference = "Test",
                    CoverDays = 2,
                    CreatedDate = DateTime.UtcNow,
                    CreationTypeId = 103,
                    TypeId = 19649,
                    StatusId = 19653,
                    OrderDetails = orderDetailRequestModelList
                };

                var orderHeaderMock = new Mock<OrderHeader>();
                var orderDetailMock = new Mock<OrderDetail>();

                var mockStore = Helper.CreateDbSetMock(_store);
                var mockTill = Helper.CreateDbSetMock(_tillLists);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTill.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().InsertAsync(It.IsAny<OrderHeader>())).Returns(Task.FromResult(orderHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().InsertAsync(It.IsAny<OrderDetail>())).Returns(Task.FromResult(orderDetailMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().Update(It.IsAny<OrderHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().Update(It.IsAny<OrderDetail>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<OrderRequestModel, OrderHeader>(It.IsAny<OrderRequestModel>())).Returns(orderHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<OrderDetailRequestModel, OrderDetail>(It.IsAny<OrderDetailRequestModel>())).Returns(orderDetailMock.Object);
                orderHeaderMock.Setup(x => x.OrderDetail.Add(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.Update(orderRequestModel, 1, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(1, result.OrderHeaders.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Update_NullReferenceException_Test()
        {
            try
            {
                var orderDetailRequestModelList = new List<OrderDetailRequestModel>() {
                    new OrderDetailRequestModel{ OrderTypeId=19649,LineNo=10,ProductId=4,CartonCost=100,Cartons=150,Units=25,TotalUnits=25,LineTotal=50,CartonQty=50,PostedDate=DateTime.UtcNow,BuyPromoCode="Test",SalePromoCode="Test",Desc="Test" }
                };

                var orderRequestModel = new OrderRequestModel
                {
                    OrderNo = 122,
                    SupplierId = 1,
                    OutletId = 95,
                    Reference = "Test",
                    CoverDays = 2,
                    CreatedDate = DateTime.UtcNow,
                    CreationTypeId = 103,
                    TypeId = 19649,
                    StatusId = 19653,
                    OrderDetails = orderDetailRequestModelList
                };

                var orderHeaderMock = new Mock<OrderHeader>();
                var orderDetailMock = new Mock<OrderDetail>();

                var mockStore = Helper.CreateDbSetMock(_store);
                var mockTill = Helper.CreateDbSetMock(_tillLists);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockSupplier = Helper.CreateDbSetMock(_supplier);
                var mockDepartment = Helper.CreateDbSetMock(_department);
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockTransaction = Helper.CreateDbSetMock(_transaction);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                var mockOrderDetail = Helper.CreateDbSetMock(_orderDetail);

                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTill.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Supplier>().GetAll()).Returns(mockSupplier.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Department>().GetAll()).Returns(mockDepartment.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().GetAll()).Returns(mockTransaction.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().GetAll()).Returns(mockOrderDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().InsertAsync(It.IsAny<OrderHeader>())).Returns(Task.FromResult(orderHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().InsertAsync(It.IsAny<OrderDetail>())).Returns(Task.FromResult(orderDetailMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().Update(It.IsAny<OrderHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>().Update(It.IsAny<OrderDetail>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<OrderRequestModel, OrderHeader>(It.IsAny<OrderRequestModel>())).Returns(orderHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<OrderDetailRequestModel, OrderDetail>(It.IsAny<OrderDetailRequestModel>())).Returns(orderDetailMock.Object);
                orderHeaderMock.Setup(x => x.OrderDetail.Add(It.IsAny<OrderDetail>()));

                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockOrdersService.Update(null, 1, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region GetOrderById
        [Fact]
        public async Task GetNewOrderNumber_Test()
        {
            try
            {

                //Arrange      
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockZoneOutlets = Helper.CreateDbSetMock(_zoneOutlets);
                var mockOrderHeader = Helper.CreateDbSetMock(_orderHeader);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ZoneOutlet>().GetAll()).Returns(mockZoneOutlets.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OrderHeader>().GetAll()).Returns(mockOrderHeader.Object);

                // Act 
                var mockOrdersService = new OrdersService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mocklLONOrderSettings.Object, _mockSendEmailService.Object, _mockCocaColaOrderSettings.Object, _mockSFTPService.Object, _mockSFTPSettings.Object, _mockDistributorOrderSettings.Object, _mockFileDirectorySettings.Object, _mockEDIMetcashService.Object);

                // Assert
                var result = await mockOrdersService.GetNewOrderNumber(95).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(951, result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
