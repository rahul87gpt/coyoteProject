using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class ProductServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private readonly Mock<IAutoMappingServices> _mockAutoMapper = null;
        private readonly Mock<IUserLoggerServices> _mockUserLoggerServices = null;

        private List<Till> _tillLists = null;
        private List<APN> _apns = null;
        private List<MasterList> _masterLists = null;
        private List<MasterListItems> _masterListItems = null;
        private List<Store> _store = null;
        private List<Product> _product = null;
        private List<OutletProduct> _outletProduct = null;
        private List<Supplier> _supplier = null;
        private List<ZoneOutlet> _zoneOutlets = null;
        private List<OrderHeader> _orderHeader = null;
        private List<OrderDetail> _orderDetail = null;
        private PagedInputModel _inputModel = null;

        #region Setup
        public ProductServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IAutoMappingServices>();
            _mockUserLoggerServices = new Mock<IUserLoggerServices>();

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
                 new MasterList {Id=5,Code="ZONE",Name="ZONE",Status=true,IsDeleted=false}
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
                new ZoneOutlet {Id=1,StoreId=5,ZoneId=659,CreatedAt=DateTime.UtcNow,IsDeleted=false,ZoneMasterListItems=_masterListItems[10],Store=_store[1] },
                new ZoneOutlet {Id=1,StoreId=6,ZoneId=660,CreatedAt=DateTime.UtcNow,IsDeleted=false,ZoneMasterListItems=_masterListItems[11],Store=_store[2] },
                new ZoneOutlet {Id=1,StoreId=7,ZoneId=661,CreatedAt=DateTime.UtcNow,IsDeleted=false,ZoneMasterListItems=_masterListItems[12],Store=_store[3] },

            };

            _supplier = new List<Supplier>()
            {
                new Supplier { Id=1,Code="S01",Desc="CONGA FOODS",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=2,Code="S02",Desc="CONGA FOODS1",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=3,Code="S03",Desc="CONGA FOODS2",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=4,Code="S04",Desc="CONGA FOODS3",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false},
                new Supplier { Id=5,Code="S05",Desc="CONGA FOODS4",Address1="A",Address2="AA",Address3="ADDD",Phone=null,Fax=null,Email="Test@gmail.com",ABN="A",UpdateCost="No",PromoSupplier=null,Contact="",CostZone="ZCGC01",GSTFreeItemCode=null,GSTFreeItemDesc=null,GSTInclItemCode=null,GSTInclItemDesc=null,XeroName=null,IsDeleted=false}
            };

            _product = new List<Product>
            {
                new Product {Id=2587,Number=1403096,Desc="Y/FDZ BLUEBERRY BLISS BALL 40GM",PosDesc="",CartonQty=20,CartonCost=42,UnitQty=1,DepartmentId=19,SupplierId=1,CommodityId=412,TaxId=1,GroupId=19434,CategoryId=257,ManufacturerId=12351,TypeId=19838,NationalRangeId=632,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[0],CategoryMasterListItem=_masterListItems[0]},
                new Product {Id=2983,Number=1803440,Desc="Y/FDZ BLUEBERRY BLISS BALL 40GM",PosDesc="",CartonQty=1,CartonCost=5,UnitQty=100,DepartmentId=3,SupplierId=2,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=630,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[1]},
                new Product {Id=3206,Number=1803102,Desc="Y/FDZ BLUEBERRY BLISS BALL 40GM",PosDesc="",CartonQty=1,CartonCost=7,UnitQty=1,DepartmentId=3,SupplierId=3,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,UnitMeasureId=1,ScaleInd=null,GmFlagInd=null,SlowMovingInd=null,WarehouseFrozenInd=null,StoreFrozenInd=null,AustMadeInd=null,AustOwnedInd=null,OrganicInd=null,HeartSmartInd=null,GenericInd=null,SeasonalInd=null,Parent=null,LabelQty=1,Replicate=null,Freight=null,Size=null,Litres=null,VarietyInd=null,HostNumber=null,HostNumber2=null,HostNumber3=null,HostItemType="W",HostItemType2=null,HostItemType3=null,HostItemType4=null,HostItemType5=null,LastApnSold=null,Rrp=0,AltSupplier=null,DeletedAt=null,DeactivatedAt=null,CreatedAt=DateTime.Now,UpdatedAt=DateTime.Now,CreatedById=1,UpdatedById=1,IsDeleted=false,ImagePath=null,AccessOutletIds="95",TareWeight=null,Info=null,Status=true,Supplier=_supplier[2]},
            };

            _outletProduct = new List<OutletProduct>
            {
                new OutletProduct {Id=44115,StoreId=95,ProductId=2587,NormalPrice1=2,CartonCostInv=42,CartonCost=42,QtyOnHand=43,MinOnHand=2,SupplierId=111,MinReorderQty=2,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[0] },
                new OutletProduct {Id=44125,StoreId=95,ProductId=3206,NormalPrice1=3,CartonCostInv=7,CartonCost=7,QtyOnHand=1,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[2] },
                new OutletProduct {Id=44127,StoreId=95,ProductId=2983,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[1] },
            };

            _orderHeader = new List<OrderHeader>
            {
                new OrderHeader { Id=1,OutletId=95,OrderNo=1,SupplierId=1,TypeId=19649,StatusId=98,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=10,SubTotalFreight=10,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=1,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[0],Supplier=_supplier[0],Type=_masterListItems[2],Status=_masterListItems[7],CreationType=_masterListItems[5]},
                new OrderHeader { Id=2,OutletId=5,OrderNo=8296,SupplierId=1,TypeId=19649,StatusId=98,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="15",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1033,SubTotalFreight=4582,SubTotalAdmin=125,SubTotalSubsidy=50,SubTotalDisc=140,SubTotalTax=125,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[1],Supplier=_supplier[0],Type=_masterListItems[2],Status=_masterListItems[7],CreationType=_masterListItems[5]},
                new OrderHeader { Id=3,OutletId=6,OrderNo=518,SupplierId=2,TypeId=19649,StatusId=98,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="18522",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1150,SubTotalFreight=25200,SubTotalAdmin=10,SubTotalSubsidy=150,SubTotalDisc=155,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=10,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[2],Supplier=_supplier[1],Type=_masterListItems[2],Status=_masterListItems[7],CreationType=_masterListItems[5]},
                new OrderHeader { Id=4,OutletId=95,OrderNo=517,SupplierId=2,TypeId=19648,StatusId=99,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="17520",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1450,SubTotalFreight=1452,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[0],Supplier=_supplier[1],Type=_masterListItems[1],Status=_masterListItems[8],CreationType=_masterListItems[5]},
                new OrderHeader { Id=5,OutletId=95,OrderNo=516,SupplierId=3,TypeId=19648,StatusId=99,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1420",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=1458,SubTotalFreight=1522,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=25,CoverDays=null,CreationTypeId=103,IsDeleted=false,Store=_store[0],Supplier=_supplier[2],Type=_masterListItems[1],Status=_masterListItems[8],CreationType=_masterListItems[5]},
                new OrderHeader { Id=6,OutletId=7,OrderNo=515,SupplierId=3,TypeId=19649,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="8458",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=5852,SubTotalFreight=14522,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=36,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[3],Supplier=_supplier[2],Type=_masterListItems[2],Status=_masterListItems[4],CreationType=_masterListItems[6]},
                new OrderHeader { Id=7,OutletId=95,OrderNo=9512,SupplierId=4,TypeId=19649,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="4785",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=7582,SubTotalFreight=1458,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[0],Supplier=_supplier[3],Type=_masterListItems[2],Status=_masterListItems[4],CreationType=_masterListItems[6]},
                new OrderHeader { Id=8,OutletId=5,OrderNo=9511,SupplierId=4,TypeId=19648,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="175",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=4582,SubTotalFreight=1145,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=7,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[1],Supplier=_supplier[3],Type=_masterListItems[1],Status=_masterListItems[4],CreationType=_masterListItems[6]},
                new OrderHeader { Id=9,OutletId=95,OrderNo=2961,SupplierId=5,TypeId=19648,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1600",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=4588,SubTotalFreight=585,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=14,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[0],Supplier=_supplier[4],Type=_masterListItems[1],Status=_masterListItems[4],CreationType=_masterListItems[6]},
                new OrderHeader { Id=10,OutletId=95,OrderNo=153454,SupplierId=5,TypeId=19647,StatusId=19654,CreatedDate=DateTime.UtcNow,PostedDate=DateTime.UtcNow.AddDays(-2),Reference="Test",DeliveryNo=null,DeliveryDate=DateTime.UtcNow,InvoiceNo="1452",InvoiceDate=DateTime.UtcNow.AddDays(-2),InvoiceTotal=6958,SubTotalFreight=458,SubTotalAdmin=10,SubTotalSubsidy=10,SubTotalDisc=10,SubTotalTax=1,Posted=DateTime.UtcNow,GstAmt=11,CoverDays=null,CreationTypeId=105,IsDeleted=false,Store=_store[0],Supplier=_supplier[4],Type=_masterListItems[0],Status=_masterListItems[4],CreationType=_masterListItems[6]}
            };

            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }
        #endregion

        [Fact]
        public async Task Product_Without_APN_With_SP()
        {
            try
            {
                //Arrange           
                var securityModel = new SecurityViewModel();
                var inputModel = new ProductFilter
                {
                    StoreId = "95",
                    MaxResultCount = 100
                };
                bool StoreIds = true;
                var dataSet = _product.ConvertToDataSet("Product");
                var dbParams = new List<SqlParameter>{
                new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                new SqlParameter("@Id", inputModel?.Id),
                new SqlParameter("@Status", inputModel?.Status),
                new SqlParameter("@Description", inputModel?.Description),
                new SqlParameter("@Number", inputModel?.Number),
                new SqlParameter("@DeptId", inputModel?.Dept),
                new SqlParameter("@Replicate", inputModel?.Replicate),
                new SqlParameter("@StoreId", inputModel?.StoreId),
                new SqlParameter("@SortColumn", inputModel?.Sorting),
                new SqlParameter("@SortDirection", inputModel?.Direction),
                new SqlParameter("@SupplierId", inputModel?.SupplierId),
                new SqlParameter("@SkipCount", inputModel?.SkipCount),
                new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                new SqlParameter("@AccessOutletIds", (StoreIds == true)?securityModel?.StoreIds:null),
                new SqlParameter("@IsWithoutAPNCall", IsRequired.True),
                };
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().ExecuteStoredProcedure(StoredProcedures.GetActiveProducts, dbParams.ToArray())).Returns(Task.FromResult(_product.ConvertToDataSet("Product")));

                // Act 
                var mockOrdersService = new ProductService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mockUserLoggerServices.Object);

                // Assert
                var result = await mockOrdersService.GetProductsWithoutAPN(inputModel, securityModel).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(6, result.Data.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Product_Without_APN()
        {
            try
            {
                var outlet = new List<int>
                {
                    95,6,7,8
                };
                var inputModel = new PagedInputModel
                {
                    MaxResultCount = 100
                };

                var mockProduct = Helper.CreateDbSetMock(_product);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);

                // Act 
                var mockOrdersService = new ProductService(_mockUnitOfWork.Object, _mockAutoMapper.Object, _mockUserLoggerServices.Object);

                // Assert
                var result = await mockOrdersService.GetAllProductsWithoutAPN(inputModel).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(6, result.Data.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
