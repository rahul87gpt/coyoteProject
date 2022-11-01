using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore.Query;
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
    public class StoreGroupTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private List<StoreGroup> _storeGroup = null;
        private PagedInputModel _inputModel = null;

        public StoreGroupTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _storeGroup = new List<StoreGroup>
            {new StoreGroup{ Id=0,Code="Test_0",IsDeleted=false },
            new StoreGroup {Id=1,Code="Test",IsDeleted=false },
                new StoreGroup {Id=2,Code="TEST_1",Name="TEST_1",IsDeleted=false},
                new StoreGroup {Id=3,Code="TEST_2",Name="TEST_2",IsDeleted=false },
                new StoreGroup {Id=4,Code="TEST_3",Name="TEST_3",IsDeleted=false },
                new StoreGroup {Id=5,Code="TEST_4",Name="TEST_4",IsDeleted=true },
            };

            

            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }

        [Fact]
        public async Task GetStoreGroup_Test()
        {
            Expression<Func<StoreGroup, bool>> _filterStoreGroup = null;
            Func<IQueryable<StoreGroup>, IOrderedQueryable<StoreGroup>> _orderByStoreGroup = null;
            Func<IQueryable<StoreGroup>, IIncludableQueryable<StoreGroup, object>> _includeStoreGroup = null;
            bool _disableTracking = true;
            bool _excludeDeleted = true;

            var mockStoreGroup = Helper.CreateDbSetMock(_storeGroup);

            _mockUnitOfWork.Setup(x => x.GetRepository<StoreGroup>().GetAll(_filterStoreGroup, _orderByStoreGroup, _disableTracking, _excludeDeleted, _includeStoreGroup)).Returns(mockStoreGroup.Object);

            var mockCommodityService = new StoreGroupServices(_mockUnitOfWork.Object, null);

            var result = await mockCommodityService.GetAllActiveStoreGroups(_inputModel).ConfigureAwait(false);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetStoreGroupById_Test()
        {

            //Arrange           
            var mockStoreGroup = Helper.CreateDbSetMock(_storeGroup);
            _mockUnitOfWork.Setup(x => x.GetRepository<StoreGroup>().GetAll()).Returns(mockStoreGroup.Object);

            var mockCommodityService = new StoreGroupServices(_mockUnitOfWork.Object, null);

            var result = await mockCommodityService.GetStoreGroupsById(3).ConfigureAwait(false);
        }

        [Fact]
        public async Task AddStoreGroup_Test()
        {
            try
            {
                //Arrange
                var storeGroup = new Mock<StoreGroup>();
                var requestModel = new StoreGroupRequestModel
                {
                    Code = "Add Test",
                    Name = "Add Test",
                    AddedAt=DateTime.UtcNow,
                    Status=true
                };

                var mockStoreGroup = Helper.CreateDbSetMock(_storeGroup);

                // Act 

                _mockUnitOfWork.Setup(x => x.GetRepository<StoreGroup>().GetAll()).Returns(mockStoreGroup.Object);

                _mockUnitOfWork.Setup(x => x.GetRepository<StoreGroup>().InsertAsync(It.IsAny<StoreGroup>())).Returns(Task.FromResult(storeGroup.Object));

                var mockStoreGroupService = new StoreGroupServices(_mockUnitOfWork.Object,null);

                var result = await mockStoreGroupService.Insert(requestModel, 1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(0, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task UpdateStoreGroup_Test()
        {
            try
            {
                //Arrange
                var storeGroup = new Mock<StoreGroup>();
                var requestModel = new StoreGroupRequestModel
                {
                    Code = "Add Test",
                    Name = "Add Test",
                    AddedAt = DateTime.UtcNow,
                    Status = true
                };

                var mockStoreGroup = Helper.CreateDbSetMock(_storeGroup);

                // Act 

                _mockUnitOfWork.Setup(x => x.GetRepository<StoreGroup>().GetAll()).Returns(mockStoreGroup.Object);

                _mockUnitOfWork.Setup(x => x.GetRepository<StoreGroup>().Update(It.IsAny<StoreGroup>()));

                var mockStoreGroupService = new StoreGroupServices(_mockUnitOfWork.Object, null);

                var result = await mockStoreGroupService.Update(requestModel, 1, 1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public async Task GetCorporateTree_Test()
        {
            Expression<Func<StoreGroup, bool>> _filterStoreGroup = null;
            Func<IQueryable<StoreGroup>, IOrderedQueryable<StoreGroup>> _orderByStoreGroup = null;
            Func<IQueryable<StoreGroup>, IIncludableQueryable<StoreGroup, object>> _includeStoreGroup = null;
            bool _disableTracking = true;
            bool _excludeDeleted = true;

            var mockStoreGroup = Helper.CreateDbSetMock(_storeGroup);

            _mockUnitOfWork.Setup(x => x.GetRepository<StoreGroup>().GetAll(_filterStoreGroup, _orderByStoreGroup, _disableTracking, _excludeDeleted, _includeStoreGroup)).Returns(mockStoreGroup.Object);

            var mockCommodityService = new StoreGroupServices(_mockUnitOfWork.Object, null);

            var result = await mockCommodityService.GetCorporateTree(null).ConfigureAwait(false);
            Assert.NotNull(result);
        }
    }
}
