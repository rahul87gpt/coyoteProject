using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface IRecipeServices
    {
        Task<PagedOutputModel<List<RecipeHeaderResponseModel>>> GetAllRecipes(PagedInputModel inputModel,SecurityViewModel securityViewModel);

        Task<RecipeReponseModel> GetRecipeById(long Id);
        Task<RecipeReponseModel> InsertRecipe(RecipeRequestModel viewModel, int userId);
        Task<RecipeReponseModel> UpdateRecipe(RecipeRequestModel viewModel, long recipeId, int userId);
        Task<bool> DeleteRecipe(int Id, int userId);
        Task<PagedOutputModel<List<RecipeSPReponseModel>>> GetAllRecipesByProduct(long ProductId, PagedInputModel inputModel);
    }
}
