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
    public class TillsServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private List<Till> _tills = null;
        private PagedInputModel _inputModel = null;
        private SecurityViewModel _securityViewModel = null;

        public TillsServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _securityViewModel = new SecurityViewModel() { StoreIds = new List<int> { 7 } };

            _tills = new List<Till>
            {
                new Till{ Id=0,Code="Test_0",IsDeleted=false,OutletId=5 },
                new Till {Id=1,Code="Test",IsDeleted=false ,OutletId=5},
                new Till {Id=2,Code="TEST_1",Desc="TEST_1",IsDeleted=false,OutletId=5},
                new Till {Id=3,Code="TEST_2",Desc="TEST_2",IsDeleted=false,OutletId=5},
                new Till {Id=4,Code="TEST_3",Desc="TEST_3",IsDeleted=false ,OutletId=5},
                new Till {Id=5,Code="TEST_4",Desc="TEST_4",IsDeleted=true ,OutletId=5},
            };



            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }

        [Fact]
        public async Task GetTill_Test()
        {
            //Expression<Func<StoreGroup, bool>> _filterStoreGroup = null;
            //Func<IQueryable<StoreGroup>, IOrderedQueryable<StoreGroup>> _orderByStoreGroup = null;
            //Func<IQueryable<StoreGroup>, IIncludableQueryable<StoreGroup, object>> _includeStoreGroup = null;
            //bool _disableTracking = true;
            //bool _excludeDeleted = true;

            var mockTills = Helper.CreateDbSetMock(_tills);

            _mockUnitOfWork.Setup(x => x.GetRepository<Till>().GetAll()).Returns(mockTills.Object);

            var mockTillService = new TillServices(_mockUnitOfWork.Object, null);

            var result = await mockTillService.GetAllActiveTillAsync(_inputModel,_securityViewModel).ConfigureAwait(false);

            //Assert
            Assert.NotNull(result);
        }


    }


}
