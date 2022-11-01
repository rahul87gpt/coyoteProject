using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Moq;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class StockAdjustTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private readonly Mock<IAutoMappingServices> _mockAutoMapper = null;
        private List<StockAdjustHeader> _stockAdjustHeader = null;
        private List<StockAdjustDetail> _stockAdjustDetail = null;
        private List<Till> _tillLists = null;
        private List<MasterList> _masterLists = null;
        private List<MasterListItems> _masterListItems = null;
        private List<Store> _store = null;
        private List<Product> _product = null;
        private List<OutletProduct> _outletProduct = null;
        private PagedInputModel _inputModel = null;
        private StockAdjustFilter _stockAdjustFilter = null;

        #region Setup
        public StockAdjustTest()
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
                new Store {Id=95,Code="700",Desc="ENOGGERA",GroupId=18,Status=true },
                new Store {Id=95,Code="701",Desc="ZZ CLAYFIELD",GroupId=18,Status=true },
                new Store {Id=95,Code="702",Desc="KANGAROO POINT",GroupId=18,Status=true },
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
                new OutletProduct {Id=44125,StoreId=95,ProductId=3206,NormalPrice1=3,CartonCostInv=7,CartonCost=7,QtyOnHand=1,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[2] },
                new OutletProduct {Id=44127,StoreId=95,ProductId=2983,NormalPrice1=6,CartonCostInv=7,CartonCost=7,QtyOnHand=3,MinOnHand=2,SupplierId=111,MinReorderQty=1,ChangeLabelInd=true,LabelQty=1,Status=true,IsDeleted=false,Store=_store[0],Product=_product[1] },
            };

            _stockAdjustDetail = new List<StockAdjustDetail>
            {
                new StockAdjustDetail {Id=1,StockAdjustHeaderId=1,LineNo=0,ProductId=2587,OutletProductId=44115,Quantity=10,LineTotal=75,ItemCost=2,ReasonId=19565,IsDeleted=false,Reason=_masterListItems[1],Product=_product[0],OutletProduct=_outletProduct[0]},
                new StockAdjustDetail {Id=2,StockAdjustHeaderId=1,LineNo=0,ProductId=2983,OutletProductId=44127,Quantity=10,LineTotal=70,ItemCost=7,ReasonId=19565,IsDeleted=false,Reason=_masterListItems[1],Product=_product[1],OutletProduct=_outletProduct[2]},
            };

            _stockAdjustHeader = new List<StockAdjustHeader>
            {
                new StockAdjustHeader {Id=0,OutletId=95,PostToDate=DateTime.UtcNow,Reference=null,Total=145,IsDeleted=false,StockAdjustDetails=_stockAdjustDetail,Store= _store[0]},
                new StockAdjustHeader {Id=1,OutletId=95,PostToDate=DateTime.UtcNow,Reference=null,Total=145,IsDeleted=false,StockAdjustDetails=_stockAdjustDetail,Store= _store[0]},
                new StockAdjustHeader {Id=2,OutletId=95,PostToDate=DateTime.UtcNow,Reference=null,Total=30,IsDeleted=false ,StockAdjustDetails=_stockAdjustDetail,Store= _store[0]},
                new StockAdjustHeader {Id=3,OutletId=95,PostToDate=DateTime.UtcNow,Reference=null,Total=120,IsDeleted=false ,StockAdjustDetails=_stockAdjustDetail,Store= _store[0]},
                new StockAdjustHeader {Id=4,OutletId=95,PostToDate=DateTime.UtcNow,Reference=null,Total=120,IsDeleted=false ,StockAdjustDetails=_stockAdjustDetail,Store= _store[0]},
                new StockAdjustHeader {Id=5,OutletId=95,PostToDate=DateTime.UtcNow,Reference=null,Total=120,IsDeleted=false ,StockAdjustDetails=_stockAdjustDetail,Store= _store[0]},
            };

            _stockAdjustDetail = new List<StockAdjustDetail>
            {
                new StockAdjustDetail {Id=0,StockAdjustHeaderId=1,LineNo=0,ProductId=2587,OutletProductId=44115,Quantity=10,LineTotal=0,ItemCost=2,ReasonId=19565,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[0],Reason=_masterListItems[1],Product=_product[0],OutletProduct=_outletProduct[0]},
                new StockAdjustDetail {Id=1,StockAdjustHeaderId=1,LineNo=0,ProductId=2587,OutletProductId=44115,Quantity=10,LineTotal=75,ItemCost=2,ReasonId=19565,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[0],Reason=_masterListItems[1],Product=_product[0],OutletProduct=_outletProduct[0]},
                new StockAdjustDetail {Id=2,StockAdjustHeaderId=1,LineNo=0,ProductId=2983,OutletProductId=44127,Quantity=10,LineTotal=70,ItemCost=7,ReasonId=19565,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[0],Reason=_masterListItems[1],Product=_product[1],OutletProduct=_outletProduct[2]},
                new StockAdjustDetail {Id=3,StockAdjustHeaderId=2,LineNo=0,ProductId=3206,OutletProductId=44125,Quantity=-10,LineTotal=-70,ItemCost=7,ReasonId=19565,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[1],Reason=_masterListItems[1],Product=_product[2],OutletProduct=_outletProduct[1]},
                new StockAdjustDetail {Id=4,StockAdjustHeaderId=2,LineNo=0,ProductId=2587,OutletProductId=44115,Quantity=15,LineTotal=100,ItemCost=10,ReasonId=19565,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[1],Reason=_masterListItems[1],Product=_product[0],OutletProduct=_outletProduct[0]},
                new StockAdjustDetail {Id=5,StockAdjustHeaderId=3,LineNo=0,ProductId=3206,OutletProductId=44125,Quantity=12,LineTotal=85,ItemCost=9,ReasonId=19563,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[2],Reason=_masterListItems[0],Product=_product[2],OutletProduct=_outletProduct[1]},
                new StockAdjustDetail {Id=6,StockAdjustHeaderId=3,LineNo=0,ProductId=2983,OutletProductId=44127,Quantity=5,LineTotal=35,ItemCost=5,ReasonId=19563,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[2],Reason=_masterListItems[0],Product=_product[1],OutletProduct=_outletProduct[2]},
                new StockAdjustDetail {Id=7,StockAdjustHeaderId=4,LineNo=0,ProductId=3206,OutletProductId=44125,Quantity=12,LineTotal=85,ItemCost=9,ReasonId=19563,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[3],Reason=_masterListItems[0],Product=_product[2],OutletProduct=_outletProduct[1]},
                new StockAdjustDetail {Id=8,StockAdjustHeaderId=4,LineNo=0,ProductId=2983,OutletProductId=44127,Quantity=5,LineTotal=35,ItemCost=5,ReasonId=19563,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[3],Reason=_masterListItems[0],Product=_product[1],OutletProduct=_outletProduct[2]},
                new StockAdjustDetail {Id=9,StockAdjustHeaderId=5,LineNo=0,ProductId=3206,OutletProductId=44125,Quantity=12,LineTotal=85,ItemCost=9,ReasonId=19563,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[4],Reason=_masterListItems[0],Product=_product[2],OutletProduct=_outletProduct[1]},
                new StockAdjustDetail {Id=10,StockAdjustHeaderId=5,LineNo=0,ProductId=2983,OutletProductId=44127,Quantity=5,LineTotal=35,ItemCost=5,ReasonId=19563,IsDeleted=false,StockAdjustHeader=_stockAdjustHeader[4],Reason=_masterListItems[0],Product=_product[1],OutletProduct=_outletProduct[2]},
            };

            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }
        #endregion

        #region Get Method
        //[Fact]
        //public async Task GetAllActiveHeaders_Test()
        //{
        //    try
        //    {
        //        //Arrange           
        //        var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
        //        _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
        //        _stockAdjustFilter = new StockAdjustFilter
        //        {
        //            StoreId = "95",
        //            MaxResultCount = 10
        //        };

        //        // Act 
        //        var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

        //        // Assert
        //        var result = await mockStockAdjustService.GetAllActiveHeaders(_stockAdjustFilter).ConfigureAwait(false);
        //        Assert.NotNull(result);
        //        Assert.Equal(5, result.Data.Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [Fact]
        public async Task GetStockAdjustById_Test()
        {
            try
            {
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockAdjustService.GetStockAdjustById(1).ConfigureAwait(false);
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
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _stockAdjustFilter = new StockAdjustFilter
                {
                    StoreId = "95",
                    MaxResultCount = 10
                };

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockAdjustService.GetActiveHeaders(_stockAdjustFilter).ConfigureAwait(false);
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
                var listSD = new List<StockAdjustDetailRequestModel>() {
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=2587,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 }
                };
                var requestModel = new StockAdjustHeaderRequestModel
                {
                    OutletId = 95,
                    PostToDate = DateTime.UtcNow,
                    Reference = "Test",
                    Total = 50,
                    StockAdjustDetail = listSD
                };

                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockTillItems = Helper.CreateDbSetMock(_tillLists);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTillItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().InsertAsync(It.IsAny<StockAdjustHeader>())).Returns(Task.FromResult(stockAdjustHeaderMock.Object)); 
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().InsertAsync(It.IsAny<StockAdjustDetail>())).Returns(Task.FromResult(stockAdjustDetailsMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().Update(It.IsAny<Transaction>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NotFoundException>(async () => await mockStockAdjustService.Insert(requestModel, 1).ConfigureAwait(false));
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
                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockTillItems = Helper.CreateDbSetMock(_tillLists);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTillItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().InsertAsync(It.IsAny<StockAdjustHeader>())).Returns(Task.FromResult(stockAdjustHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().InsertAsync(It.IsAny<StockAdjustDetail>())).Returns(Task.FromResult(stockAdjustDetailsMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().Update(It.IsAny<Transaction>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockAdjustService.Insert(null, 1).ConfigureAwait(false));
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
                var requestModel = new StockAdjustHeaderRequestModel
                {
                    OutletId = 1,
                    PostToDate = DateTime.UtcNow,
                    Reference = "Test",
                    Total = 20
                };

                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockTillItems = Helper.CreateDbSetMock(_tillLists);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTillItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().InsertAsync(It.IsAny<StockAdjustHeader>())).Returns(Task.FromResult(stockAdjustHeaderMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().InsertAsync(It.IsAny<StockAdjustDetail>())).Returns(Task.FromResult(stockAdjustDetailsMock.Object));
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().Update(It.IsAny<Transaction>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NotFoundException>(async () => await mockStockAdjustService.Insert(requestModel, 1).ConfigureAwait(false));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Update
        [Fact]
        public async Task Update_Valid_Test()
        {
            try
            {
                var listSD = new List<StockAdjustDetailRequestModel>() {
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=2587,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 },
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=3206,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 }
                };
                var requestModel = new StockAdjustHeaderRequestModel
                {
                    OutletId = 95,
                    PostToDate = DateTime.UtcNow,
                    Reference = "Test",
                    Total = 20,
                    StockAdjustDetail = listSD
                };

                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockTillItems = Helper.CreateDbSetMock(_tillLists);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTillItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().Update(It.IsAny<StockAdjustHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().Update(It.IsAny<StockAdjustDetail>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().Update(It.IsAny<Transaction>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockAdjustService.Update(requestModel, 1, 1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task Update_NullReferenceException_Second_Test()
        {
            try
            {
                var listSD = new List<StockAdjustDetailRequestModel>() {
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=2587,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 },
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=3206,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 }
                };
                var requestModel = new StockAdjustHeaderRequestModel
                {
                    OutletId = 95,
                    PostToDate = DateTime.UtcNow,
                    Reference = "Test",
                    Total = 20,
                    StockAdjustDetail = listSD
                };


                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockTillItems = Helper.CreateDbSetMock(_tillLists);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTillItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().Update(It.IsAny<StockAdjustHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().Update(It.IsAny<StockAdjustDetail>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().Update(It.IsAny<Transaction>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockAdjustService.Update(null, 1, 1).ConfigureAwait(false));

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
                var listSD = new List<StockAdjustDetailRequestModel>() {
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=2587,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 },
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=3206,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 }
                };
                var requestModel = new StockAdjustHeaderRequestModel
                {
                    OutletId = 1,
                    PostToDate = DateTime.UtcNow,
                    Reference = "Test",
                    Total = 20,
                    StockAdjustDetail = listSD
                };

                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockTillItems = Helper.CreateDbSetMock(_tillLists);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTillItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().Update(It.IsAny<StockAdjustHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().Update(It.IsAny<StockAdjustDetail>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<Transaction>().Update(It.IsAny<Transaction>()));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NotFoundException>(async () => await mockStockAdjustService.Update(requestModel, 1, 1).ConfigureAwait(false));

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
                var listSD = new List<StockAdjustDetailRequestModel>() {
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=2587,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 },
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=3206,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 }
                };

                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().Update(It.IsAny<StockAdjustHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().Update(It.IsAny<StockAdjustDetail>()));
                //_mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().DetachLocal(x => x.Id == 1));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockAdjustService.DeleteHeader(1, 1).ConfigureAwait(false);
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
                var listSD = new List<StockAdjustDetailRequestModel>() {
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=2587,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 },
                    new StockAdjustDetailRequestModel{ LineNo=10,ProductId=3206,OutletProductId=44115,ItemCost=50,LineTotal=50,Quantity=1,ReasonId=19565 }
                };

                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().Update(It.IsAny<StockAdjustHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().Update(It.IsAny<StockAdjustDetail>()));
                //_mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().DetachLocal(x => x.Id == 1));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockAdjustService.DeleteHeader(15, 1).ConfigureAwait(false));
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
                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().Update(It.IsAny<StockAdjustHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().Update(It.IsAny<StockAdjustDetail>()));
                //_mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().DetachLocal(x => x.Id == 1));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockAdjustService.DeleteHeaderDetailItem(1, 1, 1).ConfigureAwait(false);
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
                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);
                var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().Update(It.IsAny<StockAdjustHeader>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().Update(It.IsAny<StockAdjustDetail>()));
                //_mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().DetachLocal(x => x.Id == 1));
                _mockUnitOfWork.Setup(x => x.SaveChangesAsync());

                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => await mockStockAdjustService.DeleteHeaderDetailItem(15, 1, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ReferenceNo
        [Fact]
        public async Task ReferenceNo_Valid_Test()
        {
            try
            {
                var stockAdjustHeaderMock = new Mock<StockAdjustHeader>();
                var stockAdjustDetailsMock = new Mock<StockAdjustDetail>();
                //Arrange
                var mockStockAdjustHeader = Helper.CreateDbSetMock(_stockAdjustHeader);
                var mockStockAdjustDetails = Helper.CreateDbSetMock(_stockAdjustDetail);            

                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustHeader>().GetAll()).Returns(mockStockAdjustHeader.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<StockAdjustDetail>().GetAll()).Returns(mockStockAdjustDetails.Object);
                    _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(It.IsAny<StockAdjustHeaderRequestModel>())).Returns(stockAdjustHeaderMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(It.IsAny<StockAdjustDetailRequestModel>())).Returns(stockAdjustDetailsMock.Object);
                stockAdjustHeaderMock.Setup(x => x.StockAdjustDetails.Add(It.IsAny<StockAdjustDetail>()));

                // Act 
                var mockStockAdjustService = new StockAdjustService(_mockUnitOfWork.Object, _mockAutoMapper.Object);

                // Assert
                var result = await mockStockAdjustService.GetReferenceNo().ConfigureAwait(false);
                Assert.NotNull(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
