using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Moq;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class StockTakeTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private readonly Mock<IAutoMappingServices> _mockAutoMapper = null;
        private List<StockTakeHeader> _stockTakeHeader = null;
        private List<StockTakeDetail> _stockTakeDetail = null;
        private List<Till> _tillLists = null;
        private List<MasterList> _masterLists = null;
        private List<MasterListItems> _masterListItems = null;
        private List<Store> _store = null;
        private List<Product> _product = null;
        private List<OutletProduct> _outletProduct = null;
        private PagedInputModel _inputModel = null;

        #region Setup
        public StockTakeTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IAutoMappingServices>();

            _tillLists = new List<Till>
            {
                new Till {Id=1,Code="12312",Desc="ADJUST CODE",Status=true,IsDeleted=false}
            };

            _masterLists = new List<MasterList>
            {
                new MasterList {Id=14,Code="ADJUSTCODE",Name="ADJUST CODE",Status=true,IsDeleted=false}
            };
            _masterListItems = new List<MasterListItems>
            {
                new MasterListItems {Id=19563,Code="WASTAGE",Name="WASTAGE",Status=true,IsDeleted=false,MasterList=_masterLists[0]},
                new MasterListItems {Id=19565,Code="DAMAGED",Name="DAMAGED GOODS",Status=true,IsDeleted=false,MasterList=_masterLists[0]}
            };

            _store = new List<Store>
            {
                new Store {Id=95,Code="792",Desc="KANGAROO POINT",GroupId=18,Status=true },
                new Store {Id=5,Code="700",Desc="ENOGGERA",GroupId=18,Status=true },
                new Store {Id=7,Code="701",Desc="ZZ CLAYFIELD",GroupId=18,Status=true },
                new Store {Id=8,Code="702",Desc="KANGAROO POINT 2",GroupId=18,Status=true },
                new Store {Id=9,Code="703",Desc="KANGAROO POINT 3",GroupId=18,Status=true },
                new Store {Id=10,Code="704",Desc="KANGAROO POINT 4",GroupId=18,Status=true },
                new Store {Id=11,Code="705",Desc="KANGAROO POINT 5",GroupId=18,Status=true },
            };

            _product = new List<Product>
            {
                new Product {Id=2587,Number=1403096,Desc="Y/FDZ BLUEBERRY BLISS BALL 40GM",CartonQty=20,CartonCost=42,UnitQty=1,DepartmentId=19,SupplierId=111,CommodityId=412,TaxId=1,GroupId=19434,CategoryId=257,ManufacturerId=12351,TypeId=19838,NationalRangeId=632,Status=true },
                new Product {Id=2983,Number=1803440,Desc="Y/FDZ BLUEBERRY BLISS BALL 40GM",CartonQty=1,CartonCost=5,UnitQty=100,DepartmentId=3,SupplierId=111,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=630,Status=true },
                new Product {Id=3206,Number=1803102,Desc="Y/FDZ BLUEBERRY BLISS BALL 40GM",CartonQty=1,CartonCost=7,UnitQty=1,DepartmentId=3,SupplierId=111,CommodityId=572,TaxId=1,GroupId=19434,CategoryId=136,ManufacturerId=12351,TypeId=19838,NationalRangeId=19551,Status=true },
            };

            _outletProduct = new List<OutletProduct>
            {
                new OutletProduct {Id=44115,StoreId=95,ProductId=2587,NormalPrice1=2,CartonCostInv=42,CartonCost=42,QtyOnHand=43,MinOnHand=2,SupplierId=111,MinReorderQty=2,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[0] },
                new OutletProduct {Id=44125,StoreId=95,ProductId=2983,NormalPrice1=3,CartonCostInv=7,CartonCost=7,QtyOnHand=1,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[1] },
                new OutletProduct {Id=44127,StoreId=95,ProductId=3206,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[2] },
            };

            _stockTakeDetail = new List<StockTakeDetail>
            {
                new StockTakeDetail {Id=1,StockTakeHeaderId=1,OutletProductId=44115,Desc="Test1",OnHandUnits=43,Quantity=10,ItemCount=2,LineCost=0,LineTotal=75,ItemCost=2,IsDeleted=false,OutletProduct=_outletProduct[0]},
                new StockTakeDetail {Id=2,StockTakeHeaderId=1,OutletProductId=44125,Desc="Test2",OnHandUnits=43,Quantity=10,ItemCount=2,LineCost=0,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[1]},
            };

            _stockTakeHeader = new List<StockTakeHeader>
            {
                new StockTakeHeader {Id=0,OutletId=95,PostToDate=DateTime.UtcNow,Desc="KANGAROO POINT",Total=145,IsDeleted=false,StockTakeDetails=_stockTakeDetail,Store=_store[0]},
                new StockTakeHeader {Id=1,OutletId=5,PostToDate=DateTime.UtcNow,Desc="ENOGGERA",Total=145,IsDeleted=false,StockTakeDetails=_stockTakeDetail,Store=_store[1]},
                new StockTakeHeader {Id=2,OutletId=7,PostToDate=DateTime.UtcNow,Desc="AA CHINTONS WAREHOUSE",Total=30,IsDeleted=false ,StockTakeDetails=_stockTakeDetail,Store=_store[2]},
                new StockTakeHeader {Id=3,OutletId=8,PostToDate=DateTime.UtcNow,Desc="CHINTONS WAREHOUSE",Total=120,IsDeleted=false ,StockTakeDetails=_stockTakeDetail,Store=_store[3]},
                new StockTakeHeader {Id=4,OutletId=9,PostToDate=DateTime.UtcNow,Desc="LPT WAREHOUSE",Total=120,IsDeleted=false ,StockTakeDetails=_stockTakeDetail,Store=_store[4]},
                new StockTakeHeader {Id=5,OutletId=10,PostToDate=DateTime.UtcNow,Desc="TERCHINTONS",Total=120,IsDeleted=false ,StockTakeDetails=_stockTakeDetail,Store=_store[5]},
            };

            _stockTakeDetail = new List<StockTakeDetail>
            {
                new StockTakeDetail {Id=1,StockTakeHeaderId=1,OutletProductId=44115,Desc="Test1",OnHandUnits=43,Quantity=10,ItemCount=2,LineCost=145,LineTotal=75,ItemCost=2,IsDeleted=false,OutletProduct=_outletProduct[0]},
                new StockTakeDetail {Id=2,StockTakeHeaderId=1,OutletProductId=44125,Desc="Test2",OnHandUnits=80,Quantity=5,ItemCount=5,LineCost=145,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[1]},
                new StockTakeDetail {Id=3,StockTakeHeaderId=2,OutletProductId=44127,Desc="Test3",OnHandUnits=35,Quantity=14,ItemCount=15,LineCost=145,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[2]},
                new StockTakeDetail {Id=4,StockTakeHeaderId=2,OutletProductId=44115,Desc="Test4",OnHandUnits=95,Quantity=45,ItemCount=4,LineCost=145,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[0]},
                new StockTakeDetail {Id=5,StockTakeHeaderId=3,OutletProductId=44125,Desc="Test5",OnHandUnits=73,Quantity=85,ItemCount=0,LineCost=145,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[1]},
                new StockTakeDetail {Id=6,StockTakeHeaderId=3,OutletProductId=44127,Desc="Test6",OnHandUnits=11,Quantity=47,ItemCount=15,LineCost=145,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[2]},
                new StockTakeDetail {Id=7,StockTakeHeaderId=4,OutletProductId=44115,Desc="Test7",OnHandUnits=52,Quantity=30,ItemCount=15,LineCost=45,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[0]},
                new StockTakeDetail {Id=8,StockTakeHeaderId=4,OutletProductId=44125,Desc="Test8",OnHandUnits=14,Quantity=55,ItemCount=5,LineCost=41,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[1]},
                new StockTakeDetail {Id=9,StockTakeHeaderId=5,OutletProductId=44127,Desc="Test9",OnHandUnits=85,Quantity=24,ItemCount=5,LineCost=145,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[2]},
                new StockTakeDetail {Id=10,StockTakeHeaderId=5,OutletProductId=44115,Desc="Test10",OnHandUnits=61,Quantity=11,ItemCount=5,LineCost=145,LineTotal=70,ItemCost=7,IsDeleted=false,OutletProduct=_outletProduct[0]},
            };

            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }
        #endregion

        #region Get Method
        [Fact]
        public async Task GetStockTakeById_Test()
        {
            try
            {
                //Arrange           
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockStockTakeDetail = Helper.CreateDbSetMock(_stockTakeDetail);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().GetAll()).Returns(mockStockTakeDetail.Object);

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockTakeService.GetStockTakeById(1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task GetActiveHeaders_Test()
        {
            try
            {
                //Arrange           
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _inputModel = new PagedInputModel
                {
                    MaxResultCount = 10
                };
                var _securityViewModel = new SecurityViewModel();

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockTakeService.GetActiveHeaders(_inputModel, _securityViewModel).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(6, result.Data.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Insert Method
        [Fact]
        public async Task Insert_Valid_Test()
        {
            try
            {
                var listSD = new List<StockTakeDetailRequestModel>() {
                    new StockTakeDetailRequestModel { ProductId=2587,OutletProductId=44115,Desc="Test",ItemCost=50,LineTotal=50,Quantity=1,OnHandUnits=10,ItemCount=10,LineCost=14 }
                };
                var requestModel = new StockTakeHeaderRequestModel
                {
                    OutletId = 11,
                    Desc = "Test",
                    PostToDate = DateTime.UtcNow,
                    Total = 20,
                    StockTakeDetail = listSD
                };

                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().InsertAsync(It.IsAny<StockTakeHeader>())).Returns(Task.FromResult(stockTakeHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().InsertAsync(It.IsAny<StockTakeDetail>())).Returns(Task.FromResult(stockTakeDetailMock.Object));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockTakeService.Insert(requestModel, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(0, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Insert_NullReferenceException_Test()
        {
            try
            {
                var listSD = new List<StockTakeDetailRequestModel>() {
                    new StockTakeDetailRequestModel { ProductId=2587,OutletProductId=44115,Desc="Test",ItemCost=50,LineTotal=50,Quantity=1,OnHandUnits=10,ItemCount=10,LineCost=14 }
                };
                var requestModel = new StockTakeHeaderRequestModel
                {
                    OutletId = 95,
                    Desc = "Test",
                    PostToDate = DateTime.UtcNow,
                    Total = 20,
                    StockTakeDetail = listSD
                };

                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().InsertAsync(It.IsAny<StockTakeHeader>())).Returns(Task.FromResult(stockTakeHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().InsertAsync(It.IsAny<StockTakeDetail>())).Returns(Task.FromResult(stockTakeDetailMock.Object));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockTakeService.Insert(null, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Insert_NotFoundException_Test()
        {
            try
            {
                var listSD = new List<StockTakeDetailRequestModel>() {
                    new StockTakeDetailRequestModel { ProductId=2587,OutletProductId=44115,Desc="Test",ItemCost=50,LineTotal=50,Quantity=1,OnHandUnits=10,ItemCount=10,LineCost=14 }
                };
                var requestModel = new StockTakeHeaderRequestModel
                {
                    OutletId = 1,
                    Desc = "Test",
                    PostToDate = DateTime.UtcNow,
                    Total = 20,
                    StockTakeDetail = listSD
                };

                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().InsertAsync(It.IsAny<StockTakeHeader>())).Returns(Task.FromResult(stockTakeHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().InsertAsync(It.IsAny<StockTakeDetail>())).Returns(Task.FromResult(stockTakeDetailMock.Object));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NotFoundException>(async () => await mockStockTakeService.Insert(requestModel, 1).ConfigureAwait(false));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Update Method
        [Fact]
        public async Task Update_Valid_Test()
        {
            try
            {
                var listSD = new List<StockTakeDetailRequestModel>() {
                    new StockTakeDetailRequestModel { ProductId=2587,OutletProductId=44115,Desc="Test",ItemCost=50,LineTotal=50,Quantity=1,OnHandUnits=10,ItemCount=10,LineCost=14 }
                };
                var requestModel = new StockTakeHeaderRequestModel
                {
                    OutletId = 11,
                    Desc = "Test",
                    PostToDate = DateTime.UtcNow,
                    Total = 20,
                    StockTakeDetail = listSD
                };

                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().Update(It.IsAny<StockTakeHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().Update(It.IsAny<StockTakeDetail>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockTakeService.Update(requestModel, 1, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
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
                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().Update(It.IsAny<StockTakeHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().Update(It.IsAny<StockTakeDetail>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockTakeService.Update(null,1, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Update_NotFoundException_Test()
        {
            try
            {
                var listSD = new List<StockTakeDetailRequestModel>() {
                    new StockTakeDetailRequestModel { ProductId=2587,OutletProductId=44115,Desc="Test",ItemCost=50,LineTotal=50,Quantity=1,OnHandUnits=10,ItemCount=10,LineCost=14 }
                };
                var requestModel = new StockTakeHeaderRequestModel
                {
                    OutletId = 245,
                    Desc = "Test",
                    PostToDate = DateTime.UtcNow,
                    Total = 20,
                    StockTakeDetail = listSD
                };

                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().Update(It.IsAny<StockTakeHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().Update(It.IsAny<StockTakeDetail>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NotFoundException>(async () => await mockStockTakeService.Update(requestModel,1, 1).ConfigureAwait(false));

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
                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().Update(It.IsAny<StockTakeHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().Update(It.IsAny<StockTakeDetail>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockTakeService.DeleteHeader(1, 1).ConfigureAwait(false);
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
                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().Update(It.IsAny<StockTakeHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().Update(It.IsAny<StockTakeDetail>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockTakeService.DeleteHeader(15, 1).ConfigureAwait(false));
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
                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockStockAdjustDetail = Helper.CreateDbSetMock(_stockTakeDetail);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().GetAll()).Returns(mockStockAdjustDetail.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().Update(It.IsAny<StockTakeHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().Update(It.IsAny<StockTakeDetail>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockTakeService.DeleteHeaderDetailItem(1, 1, 1).ConfigureAwait(false);
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
                var stockTakeHeaderMock = new Mock<StockTakeHeader>();
                var stockTakeDetailMock = new Mock<StockTakeDetail>();

                //Arrange
                var mockStockTakeHeader = Helper.CreateDbSetMock(_stockTakeHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().GetAll()).Returns(mockStockTakeHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeHeader>().Update(It.IsAny<StockTakeHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockTakeDetail>().Update(It.IsAny<StockTakeDetail>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(It.IsAny<StockTakeHeaderRequestModel>())).Returns(stockTakeHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(It.IsAny<StockTakeDetailRequestModel>())).Returns(stockTakeDetailMock.Object);
                stockTakeHeaderMock.Setup(x => x.StockTakeDetails.Add(It.IsAny<StockTakeDetail>()));

                // Act 
                var mockStockTakeService = new StockTakeService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockTakeService.DeleteHeaderDetailItem(15, 1, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
