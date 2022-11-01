using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Services;
using Coyote.Console.Common;
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
  public class RecipeTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = null;
        private List<Recipe> _recipe = null;
        private PagedInputModel _inputModel = null;

        public RecipeTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _recipe = new List<Recipe>
            {
                new Recipe {ID=1,Description="Test",OutletID=1,ProductID = 1234,IsActive=Status.Active },
                new Recipe {ID=2,Description="Test",OutletID=1,ProductID = 1234,IsActive=Status.Active },
                new Recipe {ID=3,Description="Test",OutletID=1,ProductID = 1234,IsActive=Status.Active },
                new Recipe {ID=4,Description="Test",OutletID=1,ProductID = 1234,IsActive=Status.Active },
                new Recipe {ID=5,Description="Test",OutletID=1,ProductID = 1234,IsActive=Status.Active },
            };

            _inputModel = new PagedInputModel
            {
                MaxResultCount = 10,
            };
        }

        //[Fact]
        //public async Task GetAllRecipe_Test()
        //{
        //    Expression<Func<Recipe, bool>> _filterRecipe = null;
        //    Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>> _orderByRecipe = null;
        //    Func<IQueryable<Recipe>, IIncludableQueryable<Recipe, object>> _includeRecipe = null;
        //    bool _disableTracking = true;
        //    bool _excludeDeleted = true;

        //    var securityViewModel = new SecurityViewModel();

        //    var mockRecipe = Helper.CreateDbSetMock(_recipe);

        //    _mockUnitOfWork.Setup(x => x.GetRepository<Recipe>().GetAll(_filterRecipe, _orderByRecipe, _disableTracking, _excludeDeleted, _includeRecipe)).Returns(mockRecipe.Object);

        //    var mockRecipeService = new RecipeServices(_mockUnitOfWork.Object);

        //    var result = await mockRecipeService.GetAllRecipes(_inputModel, securityViewModel).ConfigureAwait(false);
        //}

    }
}
