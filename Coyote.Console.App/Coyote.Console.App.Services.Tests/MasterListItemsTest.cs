using AutoMapper.Configuration;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class MasterListItemsTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;        
        private readonly SecurityViewModel _securityViewModel = null;
        private List<MasterList> _masterLists = null;
        private List<MasterListItems> _masterListItems = null;
        private PagedInputModel _inputModel = null;

        public MasterListItemsTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _securityViewModel = new SecurityViewModel() { StoreIds = new List<int> { 7 } };

            _masterLists = new List<MasterList>
            {
                new MasterList {Id=1,Code="MANUALSALE",Name="MANUALSALE",Status=true,IsDeleted=false},
                new MasterList {Id=2,Code="MANUALSALEI",Name="MANUALSALEI",Status=true,IsDeleted=false}
            };

            _masterListItems = new List<MasterListItems>
            { new MasterListItems {Id=0,Code="MANUALSALE_0",Name="MANUALSALE",Status=true,IsDeleted=false,MasterList=_masterLists[0]},
                new MasterListItems {Id=1,Code="MANUALSALE",Name="MANUALSALE",Status=true,IsDeleted=false,MasterList=_masterLists[0]},
                new MasterListItems {Id=2,Code="MANUALSALEI",Name="MANUALSALEI",Status=true,IsDeleted=false,MasterList=_masterLists[1]}
            };

            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }

        [Fact]
        public async Task GetMasterListItems_Test()
        {
            Expression<Func<MasterListItems, bool>> _filterMasterListItems = null;
            Func<IQueryable<MasterListItems>, IOrderedQueryable<MasterListItems>> _orderByMasterListItems = null;
            Func<IQueryable<MasterListItems>, IIncludableQueryable<MasterListItems, object>> _includeMasterListItems = null;
            bool _disableTracking = true;
            bool _excludeDeleted = true;

            var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);

            _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll(_filterMasterListItems, _orderByMasterListItems, _disableTracking, _excludeDeleted, _includeMasterListItems)).Returns(mockMasterListItems.Object);

            var mockManualSaleService = new MasterListItemServices(_mockUnitOfWork.Object,null);

            var result = await mockManualSaleService.GetAllActiveMasterListItems(_masterLists[0].Code, _securityViewModel, _inputModel).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetMasterListItemsById_Test()
        {
            //Arrange           
            var mockMasterList = Helper.CreateDbSetMock(_masterLists);
            var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
            _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll()).Returns(mockMasterListItems.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll()).Returns(mockMasterList.Object);


            // Act 
            var mockMasterListItemService = new MasterListItemServices(_mockUnitOfWork.Object,null);

            // Assert
            var result = await mockMasterListItemService.GetMasterListItemsById(1, "MANUALSALE").ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task Insert_TestAsync()
        {

            //Arrange
            var masterListItem = new Mock<MasterListItems>();
            Expression<Func<MasterListItems, bool>> _filterMasterListItems = null;
            Func<IQueryable<MasterListItems>, IOrderedQueryable<MasterListItems>> _orderByMasterListItems = null;
            Func<IQueryable<MasterListItems>, IIncludableQueryable<MasterListItems, object>> _includeMasterListItems = null;
           
            Expression<Func<MasterList, bool>> _filterMasterList = null;
            Func<IQueryable<MasterList>, IOrderedQueryable<MasterList>> _orderByMasterList = null;
            Func<IQueryable<MasterList>, IIncludableQueryable<MasterList, object>> _includeMasterList = null;
            bool _disableTracking = true;
            bool _excludeDeleted = true;

            var mockMasterListItems = Helper.CreateDbSetMock(_masterListItems);
            var mockMasterList = Helper.CreateDbSetMock(_masterLists);

            _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().GetAll(_filterMasterListItems, _orderByMasterListItems, _disableTracking, _excludeDeleted, _includeMasterListItems)).Returns(mockMasterListItems.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<MasterList>().GetAll(_filterMasterList, _orderByMasterList, _disableTracking, _excludeDeleted, _includeMasterList)).Returns(mockMasterList.Object);

            _mockUnitOfWork.Setup(x => x.GetRepository<MasterListItems>().InsertAsync(It.IsAny<MasterListItems>()));//.Returns(Task<masterListItem>);

            var requestModel = new MasterListItemRequestModel
            {
                Code = "ZONE5",
                Name = "ZONE_5",
                Status = true,
                ListId= _masterLists[0].Id
            };

            //Act
            var mockMasterListItemsService = new MasterListItemServices(_mockUnitOfWork.Object, null);

            var result = await mockMasterListItemsService.AddMasterListItem(requestModel, _masterLists[0].Code, 1).ConfigureAwait(false);

            //Assert
            Assert.NotNull(result);
        }
    }
}
