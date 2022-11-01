using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace Coyote.Console.App.Services.Tests
{
    public class CommodityTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private readonly Mock<IAutoMappingServices> _mockAutoMapper = null;
        private List<Commodity> _commodity = null;
        private List<Department> _department = null;
        private PagedInputModel _inputModel = null;

        public CommodityTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IAutoMappingServices>();


            _department = new List<Department>
            {
                new Department {Id=1,AdvertisingDisc=1.0,AllowSaleDisc=true,BudgetGroethFactor=1,Desc="TEST",Code="TEST",IsDeleted=false,MapTypeId=1,Status=true }
            };


            _commodity = new List<Commodity>
            {new Commodity{ Id=0,Code="Test",Desc="Test",CoverDays=10,DepartmentId=1,Departments =_department[0] ,IsDeleted=false },
            new Commodity {Id=1,Code="Test",Desc="Test",CoverDays=10,DepartmentId=1,Departments =_department[0] ,IsDeleted=false },
                new Commodity {Id=2,Code="Test",Desc="Test",CoverDays=10,DepartmentId=1,Departments =_department[0],IsDeleted=false },
                new Commodity {Id=3,Code="Test",Desc="Test",CoverDays=10,DepartmentId=1,Departments =_department[0],IsDeleted=false },
                new Commodity {Id=4,Code="Test",Desc="Test",CoverDays=10,DepartmentId=1,Departments =_department[0],IsDeleted=false },
                new Commodity {Id=5,Code="Test",Desc="Test",CoverDays=10,DepartmentId=1,Departments =_department[0],IsDeleted=true },
            };



            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }
        [Fact]
        public async Task GetActiveCommodity_Test()
        {
            Expression<Func<Commodity, bool>> _filterCommodity = null;
            Func<IQueryable<Commodity>, IOrderedQueryable<Commodity>> _orderByCommodity = null;
            Func<IQueryable<Commodity>, IIncludableQueryable<Commodity, object>> _includeCommodity = null;
            bool _disableTracking = true;
            bool _excludeDeleted = true;

            var mockCommodity = Helper.CreateDbSetMock(_commodity);

            _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll(_filterCommodity, _orderByCommodity, _disableTracking, _excludeDeleted, _includeCommodity)).Returns(mockCommodity.Object);

            var mockCommodityService = new CommodityService(_mockAutoMapper.Object, null, _mockUnitOfWork.Object);

            var result = await mockCommodityService.GetAllActiveCommodities(_inputModel).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetCommodityById_Test()
        {
            try
            {
                //Arrange           
                var mockCommodity = Helper.CreateDbSetMock(_commodity);
                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);


                // Act 
                var mockCommodityService = new CommodityService(_mockAutoMapper.Object, null, _mockUnitOfWork.Object);

                // Assert
                var result = await mockCommodityService.GetCommodityById(1).ConfigureAwait(false);
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Fact]
        public async Task AddCommodity_Test()
        {
            try
            {
                //Arrange
                var commodity = new Mock<Commodity>();
                var requestModel = new CommodityRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    DepartmentId = _department[0].Id,
                    GPPcntLevel1 = 1,
                    CoverDays = 10
                };

                var mockCommodity = Helper.CreateDbSetMock(_commodity);

                // Act 

                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);

                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().InsertAsync(It.IsAny<Commodity>())).Returns(Task.FromResult(commodity.Object));

                var mockCommodityService = new CommodityService(_mockAutoMapper.Object, null, _mockUnitOfWork.Object);

                var result = await mockCommodityService.Insert(requestModel, 1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(0, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Fact]
        public async Task UpdateCommodity_Test()
        {
            try
            {
                //Arrange
                var commodity = new Mock<Commodity>();
                var requestModel = new CommodityRequestModel
                {
                    Code = "Add Test",
                    Desc = "Add Test",
                    DepartmentId = _department[0].Id,
                    GPPcntLevel1 = 1,
                    CoverDays = 10
                };

                var mockCommodity = Helper.CreateDbSetMock(_commodity);

                // Act 

                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().GetAll()).Returns(mockCommodity.Object);

                _mockUnitOfWork.Setup(x => x.GetRepository<Commodity>().Update(It.IsAny<Commodity>()));

                var mockCommodityService = new CommodityService(_mockAutoMapper.Object, null, _mockUnitOfWork.Object);

                var result = await mockCommodityService.Update(requestModel, 1,1).ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
