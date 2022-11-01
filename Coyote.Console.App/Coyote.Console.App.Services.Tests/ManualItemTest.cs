using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class ManualItemTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private readonly Mock<IAutoMappingServices> _mockAutoMapper = null;
        private List<ManualSale> _manualSale = null;
        private List<ManualSaleItem> _manualSaleItem = null;
        private List<MasterList> _masterLists = null;
        private List<MasterListItems> _masterListItems = null;
        private List<Store> _store = null;
        private List<Product> _product = null;
        private List<OutletProduct> _outletProduct = null;
        private PagedInputModel _inputModel = null;
        private SecurityViewModel _securityViewModel = null;

        public ManualItemTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IAutoMappingServices>();

            _masterLists = new List<MasterList>
            {
                new MasterList {Id=1,Code="MANUALSALE",Name="MANUALSALE",Status=true,IsDeleted=false},
                new MasterList {Id=2,Code="MANUALSALEI",Name="MANUALSALEI",Status=true,IsDeleted=false}
            };

            _masterListItems = new List<MasterListItems>
            {
                new MasterListItems {Id=1,Code="MANUALSALE",Name="MANUALSALE",Status=true,IsDeleted=false,MasterList=_masterLists[0]},
                new MasterListItems {Id=2,Code="MANUALSALEI",Name="MANUALSALEI",Status=true,IsDeleted=false,MasterList=_masterLists[1]}
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


            _manualSaleItem = new List<ManualSaleItem>
            {
                new ManualSaleItem {Id=1,ManualSaleId=1,OutletId=95,ProductId=2587,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,Product=_product[0],Store=_store[0] },
                new ManualSaleItem {Id=2,ManualSaleId=1,OutletId=95,ProductId=2587,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,Product=_product[0],Store=_store[0] },
                new ManualSaleItem {Id=3,ManualSaleId=2,OutletId=95,ProductId=2983,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,Product=_product[1],Store=_store[0] },
                new ManualSaleItem {Id=4,ManualSaleId=2,OutletId=95,ProductId=2983,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,Product=_product[1],Store=_store[0] },
                new ManualSaleItem {Id=5,ManualSaleId=3,OutletId=95,ProductId=3206,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,Product=_product[2],Store=_store[0] },
                new ManualSaleItem {Id=6,ManualSaleId=3,OutletId=95,ProductId=3206,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,Product=_product[2],Store=_store[0] },
            };

            _manualSale = new List<ManualSale>
            {
                new ManualSale {Id=0,Code="Test",Desc="Test",TypeId=1,IsDeleted=false,ManualSaleItem=_manualSaleItem },
                new ManualSale {Id=1,Code="Test",Desc="Test",TypeId=1,IsDeleted=false,ManualSaleItem=_manualSaleItem },
                new ManualSale {Id=2,Code="Test",Desc="Test",TypeId=1,IsDeleted=false,ManualSaleItem=_manualSaleItem },
                new ManualSale {Id=3,Code="Test",Desc="Test",TypeId=1,IsDeleted=false,ManualSaleItem=_manualSaleItem },
                new ManualSale {Id=4,Code="Test",Desc="Test",TypeId=1,IsDeleted=false,ManualSaleItem=_manualSaleItem },
                new ManualSale {Id=5,Code="Test",Desc="Test",TypeId=1,IsDeleted=true ,ManualSaleItem=_manualSaleItem},
            };

            _manualSaleItem = new List<ManualSaleItem>
            {
                new ManualSaleItem {Id=1,ManualSaleId=1,OutletId=95,ProductId=2587,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,ManualSale=_manualSale[1],Product=_product[0],Store=_store[0] },
                new ManualSaleItem {Id=2,ManualSaleId=1,OutletId=95,ProductId=2587,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,ManualSale=_manualSale[1],Product=_product[0],Store=_store[0] },
                new ManualSaleItem {Id=3,ManualSaleId=2,OutletId=95,ProductId=2983,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,ManualSale=_manualSale[2],Product=_product[1],Store=_store[0] },
                new ManualSaleItem {Id=4,ManualSaleId=2,OutletId=95,ProductId=2983,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,ManualSale=_manualSale[2],Product=_product[1],Store=_store[0] },
                new ManualSaleItem {Id=5,ManualSaleId=3,OutletId=95,ProductId=3206,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,ManualSale=_manualSale[3],Product=_product[2],Store=_store[0] },
                new ManualSaleItem {Id=6,ManualSaleId=3,OutletId=95,ProductId=3206,PriceLevel="high",Qty=1,Price=10,Amount=10,Cost=2,TypeId=2,IsDeleted=false,ManualSale=_manualSale[3],Product=_product[2],Store=_store[0] },
            };
        }

        #region GetManualSale
        [Fact]
        public async Task GetManualSale_Test()
        {
            var mockManualSale = Helper.CreateDbSetMock(_manualSale);

            _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);

            var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

            var result = await mockManualSaleService.GetManualSale(_inputModel, _securityViewModel).ConfigureAwait(false);

            Assert.NotNull(result);
        }
        #endregion

        #region GetManualSaleById
        [Fact]
        public async Task GetManualSaleById_Test()
        {
            var mockManualSale = Helper.CreateDbSetMock(_manualSale);

            _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);

            var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

            var result = await mockManualSaleService.GetManualSaleById(1).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }
        #endregion

        #region AddManualSale
        [Fact]
        public async Task AddManualSale_Test()
        {
            try
            {
                //Arrange
                var manualSale = new Mock<ManualSale>();

                var listManualSaleItemRM = new List<ManualSaleItemRequestModel>() {
                    new ManualSaleItemRequestModel{ProductId=2587,OutletId=95,Qty=2,Price=50,Amount=100,Cost=25,PriceLevel="1" }
                };

                var requestModel = new ManualSaleRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    ManualSaleItemRequestModel = listManualSaleItemRM
                };

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().InsertAsync(It.IsAny<ManualSale>())).Returns(Task.FromResult(manualSale.Object));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                var result = await mockManualSaleService.AddManualSale(requestModel, 1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(0, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task AddManualSale_NullReferenceException_Test()
        {
            try
            {
                //Arrange
                var manualSale = new Mock<ManualSale>();

                var listManualSaleItemRM = new List<ManualSaleItemRequestModel>() {
                    new ManualSaleItemRequestModel{ProductId=2587,OutletId=95,Qty=2,Price=50,Amount=100,Cost=25,PriceLevel="1" }
                };

                var requestModel = new ManualSaleRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    ManualSaleItemRequestModel = listManualSaleItemRM
                };

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().InsertAsync(It.IsAny<ManualSale>())).Returns(Task.FromResult(manualSale.Object));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                Assert.ThrowsAsync<NullReferenceException>(async () => await mockManualSaleService.AddManualSale(null, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task AddManualSale_NotFoundException_Test()
        {
            try
            {
                //Arrange
                var manualSale = new Mock<ManualSale>();

                var listManualSaleItemRM = new List<ManualSaleItemRequestModel>() {
                    new ManualSaleItemRequestModel{ProductId=1,OutletId=95,Qty=2,Price=50,Amount=100,Cost=25,PriceLevel="1" }
                };

                var requestModel = new ManualSaleRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    ManualSaleItemRequestModel = listManualSaleItemRM
                };

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().InsertAsync(It.IsAny<ManualSale>())).Returns(Task.FromResult(manualSale.Object));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                Assert.ThrowsAsync<NotFoundException>(async () => await mockManualSaleService.AddManualSale(requestModel, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region EditManualSale
        [Fact]
        public async Task EditManualSale_Test()
        {
            try
            {
                //Arrange
                var manualSaleMock = new Mock<ManualSale>();
                var manualSaleItemMock = new Mock<ManualSaleItem>();

                var listManualSaleItemRM = new List<ManualSaleItemRequestModel>() {
                    new ManualSaleItemRequestModel{ProductId=2587,OutletId=95,Qty=2,Price=50,Amount=100,Cost=25,PriceLevel="1" }
                };

                var requestModel = new ManualSaleRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    ManualSaleItemRequestModel = listManualSaleItemRM
                };

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().Update(It.IsAny<ManualSale>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().Update(It.IsAny<ManualSaleItem>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleRequestModel, ManualSale>(It.IsAny<ManualSaleRequestModel>())).Returns(manualSaleMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(It.IsAny<ManualSaleItemRequestModel>())).Returns(manualSaleItemMock.Object);
                manualSaleMock.Setup(x => x.ManualSaleItem.Add(It.IsAny<ManualSaleItem>()));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                var result = await mockManualSaleService.EditManualSale(requestModel, 1, 1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task EditManualSale_NullReferenceException_Test()
        {
            try
            {
                //Arrange
                var manualSaleMock = new Mock<ManualSale>();
                var manualSaleItemMock = new Mock<ManualSaleItem>();

                var listManualSaleItemRM = new List<ManualSaleItemRequestModel>() {
                    new ManualSaleItemRequestModel{ProductId=2587,OutletId=95,Qty=2,Price=50,Amount=100,Cost=25,PriceLevel="1" }
                };

                var requestModel = new ManualSaleRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    ManualSaleItemRequestModel = listManualSaleItemRM
                };

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().Update(It.IsAny<ManualSale>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().Update(It.IsAny<ManualSaleItem>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleRequestModel, ManualSale>(It.IsAny<ManualSaleRequestModel>())).Returns(manualSaleMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(It.IsAny<ManualSaleItemRequestModel>())).Returns(manualSaleItemMock.Object);
                manualSaleMock.Setup(x => x.ManualSaleItem.Add(It.IsAny<ManualSaleItem>()));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                Assert.ThrowsAsync<NullReferenceException>(async () => await mockManualSaleService.EditManualSale(null, 1,1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task EditManualSale_NotFoundException_Test()
        {
            try
            {
                //Arrange
                var manualSaleMock = new Mock<ManualSale>();
                var manualSaleItemMock = new Mock<ManualSaleItem>();

                var listManualSaleItemRM = new List<ManualSaleItemRequestModel>() {
                    new ManualSaleItemRequestModel{ProductId=2587,OutletId=95,Qty=2,Price=50,Amount=100,Cost=25,PriceLevel="1" }
                };

                var requestModel = new ManualSaleRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    ManualSaleItemRequestModel = listManualSaleItemRM
                };

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().Update(It.IsAny<ManualSale>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().Update(It.IsAny<ManualSaleItem>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleRequestModel, ManualSale>(It.IsAny<ManualSaleRequestModel>())).Returns(manualSaleMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(It.IsAny<ManualSaleItemRequestModel>())).Returns(manualSaleItemMock.Object);
                manualSaleMock.Setup(x => x.ManualSaleItem.Add(It.IsAny<ManualSaleItem>()));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                Assert.ThrowsAsync<NotFoundException>(async () => await mockManualSaleService.EditManualSale(requestModel, 20, 1).ConfigureAwait(false));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DeleteManualSale
        [Fact]
        public async Task DeleteManualSale_Test()
        {
            try
            {
                //Arrange
                var manualSaleMock = new Mock<ManualSale>();
                var manualSaleItemMock = new Mock<ManualSaleItem>();

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().Update(It.IsAny<ManualSale>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().Update(It.IsAny<ManualSaleItem>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleRequestModel, ManualSale>(It.IsAny<ManualSaleRequestModel>())).Returns(manualSaleMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(It.IsAny<ManualSaleItemRequestModel>())).Returns(manualSaleItemMock.Object);
                manualSaleMock.Setup(x => x.ManualSaleItem.Add(It.IsAny<ManualSaleItem>()));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                var result = await mockManualSaleService.DeleteManualSale(1, 1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(true, result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task DeleteManualSale_NotFoundException_Test()
        {
            try
            {
                //Arrange
                var manualSaleMock = new Mock<ManualSale>();
                var manualSaleItemMock = new Mock<ManualSaleItem>();

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().Update(It.IsAny<ManualSale>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().Update(It.IsAny<ManualSaleItem>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleRequestModel, ManualSale>(It.IsAny<ManualSaleRequestModel>())).Returns(manualSaleMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(It.IsAny<ManualSaleItemRequestModel>())).Returns(manualSaleItemMock.Object);
                manualSaleMock.Setup(x => x.ManualSaleItem.Add(It.IsAny<ManualSaleItem>()));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                Assert.ThrowsAsync<NotFoundException>(async () => await mockManualSaleService.DeleteManualSale(20, 1).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DeleteManualSaleItem
        [Fact]
        public async Task DeleteManualSaleItem_Test()
        {
            try
            {
                //Arrange
                var manualSaleMock = new Mock<ManualSale>();
                var manualSaleItemMock = new Mock<ManualSaleItem>();

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().Update(It.IsAny<ManualSale>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().Update(It.IsAny<ManualSaleItem>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleRequestModel, ManualSale>(It.IsAny<ManualSaleRequestModel>())).Returns(manualSaleMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(It.IsAny<ManualSaleItemRequestModel>())).Returns(manualSaleItemMock.Object);
                manualSaleMock.Setup(x => x.ManualSaleItem.Add(It.IsAny<ManualSaleItem>()));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                var result = await mockManualSaleService.DeleteManualSaleItem(1, 1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(true, result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task DeleteManualSaleItem_NotFoundException_Test()
        {
            try
            {
                //Arrange
                var manualSaleMock = new Mock<ManualSale>();
                var manualSaleItemMock = new Mock<ManualSaleItem>();

                var mockMasterList = Helper.CreateDbSetMock(_masterLists);
                var mockMasterListItem = Helper.CreateDbSetMock(_masterListItems);
                var mockStore = Helper.CreateDbSetMock(_store);
                var mockProduct = Helper.CreateDbSetMock(_product);
                var mockOutletProduct = Helper.CreateDbSetMock(_outletProduct);
                var mockManualSale = Helper.CreateDbSetMock(_manualSale);
                var mockManualSaleItems = Helper.CreateDbSetMock(_manualSaleItem);

                // Act 
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItem.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Store>().GetAll()).Returns(mockStore.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<Product>().GetAll()).Returns(mockProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<OutletProduct>().GetAll()).Returns(mockOutletProduct.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().GetAll()).Returns(mockManualSale.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().GetAll()).Returns(mockManualSaleItems.Object);
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSale>().Update(It.IsAny<ManualSale>()));
                _mockUnitOfWork.Setup(x => x.GetRepository<ManualSaleItem>().Update(It.IsAny<ManualSaleItem>()));

                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleRequestModel, ManualSale>(It.IsAny<ManualSaleRequestModel>())).Returns(manualSaleMock.Object);
                _mockAutoMapper.Setup(mock => mock.Mapping<ManualSaleItemRequestModel, ManualSaleItem>(It.IsAny<ManualSaleItemRequestModel>())).Returns(manualSaleItemMock.Object);
                manualSaleMock.Setup(x => x.ManualSaleItem.Add(It.IsAny<ManualSaleItem>()));

                var mockManualSaleService = new ManualSaleService(_mockUnitOfWork.Object);

                Assert.ThrowsAsync<NotFoundException>(async () => await mockManualSaleService.DeleteManualSaleItem(20, 1).ConfigureAwait(false));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
