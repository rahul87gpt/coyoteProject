using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;

namespace Coyote.Console.App.Services.Services
{
    public class RecipeServices : IRecipeServices
    {
        private IUnitOfWork _unitOfWork = null;
        public RecipeServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all recipes
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<RecipeHeaderResponseModel>>> GetAllRecipes(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                var repository = _unitOfWork?.GetRepository<Recipe>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                     new SqlParameter("@SortColumn", inputModel?.Sorting),
                     new SqlParameter("@SortDirection", inputModel?.Direction),
                     new SqlParameter("@PageNumber", inputModel?.SkipCount),
                     new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                     new SqlParameter("@IsLogged", inputModel?.IsLogged),
                     new SqlParameter("@Module","Rebate"),
                     new SqlParameter("@RoleId",RoleId)

                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllRecipe, dbParams.ToArray()).ConfigureAwait(false);
                List<RecipeHeaderResponseModel> recipeReponseModel = MappingHelpers.ConvertDataTable<RecipeHeaderResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<RecipeHeaderResponseModel>>(recipeReponseModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get recipes by Product Id
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<RecipeSPReponseModel>>> GetAllRecipesByProduct(long ProductId, PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Recipe>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                     new SqlParameter("@SortColumn", inputModel?.Sorting),
                     new SqlParameter("@SortDirection", inputModel?.Direction),
                     new SqlParameter("@PageNumber", inputModel?.SkipCount),
                     new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                     new SqlParameter("@Product", ProductId),
                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllRecipe, dbParams.ToArray()).ConfigureAwait(false);

                List<RecipeSPReponseModel> recipeReponseModel = new List<RecipeSPReponseModel>();
                if (dset.Tables.Count > 0)
                {
                    recipeReponseModel = MappingHelpers.ConvertDataTable<RecipeSPReponseModel>(dset.Tables[0]);
                }
                var count = 0;
                if (dset.Tables.Count > 1)
                {
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                }
                return new PagedOutputModel<List<RecipeSPReponseModel>>(recipeReponseModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get recipe by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<RecipeReponseModel> GetRecipeById(long Id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Recipe>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@Id", Id),
            };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllRecipe, dbParams.ToArray()).ConfigureAwait(false);

                List<RecipeSPReponseModel> recipeReponseModel = new List<RecipeSPReponseModel>();
                if (dset.Tables.Count > 0)
                {
                    recipeReponseModel = MappingHelpers.ConvertDataTable<RecipeSPReponseModel>(dset.Tables[0]);
                }

                var count = 0;
                if (dset.Tables.Count > 1)
                {
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                }

                var responseModel = new RecipeReponseModel();

                responseModel.RecipeHeader = recipeReponseModel.Where(x => x.IsParents == 1).Select(x => new RecipeHeaderResponseModel
                {
                    Description = x.Description,
                    ID = x.ID,
                    OutletID = x.OutletID,
                    OutletCode = x.OutletCode,
                    OutletDesc = x.OutletDesc,
                    ProductID = x.ProductID,
                    ProductNumber = x.ProductNumber,
                    Qty = x.Qty
                }).FirstOrDefault();

                var detailsList = recipeReponseModel.Where(x => x.IsParents != 1).Select(MappingHelpers.Mapping<RecipeSPReponseModel, RecipeDetailResponseModel>).ToList();

                if (detailsList != null)
                {
                    responseModel.RecipeDetail.AddRange(detailsList);
                }

                return responseModel;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Insert Recipe
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<RecipeReponseModel> InsertRecipe(RecipeRequestModel viewModel, int userId)
        {
            RecipeReponseModel responseModel = new RecipeReponseModel();
            try
            {
                var repository = _unitOfWork?.GetRepository<Recipe>();
                if (viewModel != null)
                {
                    if (viewModel.RecipeHeader == null)
                    {
                        throw new BadRequestException(ErrorMessages.RecipeInvalid.ToString(CultureInfo.CurrentCulture));
                    }

                    var recipeRequestModel = new List<RecipeSPRequestModel>();
                    var header = new RecipeSPRequestModel
                    {

                        Description = viewModel.RecipeHeader?.Description,
                        ProductID = viewModel.RecipeHeader.ProductID,
                        Qty = viewModel.RecipeHeader.Qty,
                        OutletID = viewModel.RecipeHeader.OutletID,
                        IsParents = 1,
                        IngredientProductID = null
                    };

                    if (viewModel?.RecipeDetail != null)
                    {
                        recipeRequestModel = viewModel?.RecipeDetail?.Select(MappingHelpers.CreateMap).ToList();
                    }
                    recipeRequestModel.Add(header);
                    var recipeRequestDataTable = MappingHelpers.ToDataTable(recipeRequestModel, true);

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@RecipeId", 0),
                new SqlParameter("@UserId", userId),
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = "@RecipeDataTable",
                  TypeName ="dbo.RecipeRequestType",
                  Value = recipeRequestDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter("@ActionPerformed",ActionPerformed.Create.ToString()),

            };
                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.AddUpdateRecipe, dbParams.ToArray()).ConfigureAwait(false);

                    if (dset?.Tables?.Count > 0)
                    {
                        if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                        {
                            throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                        }
                        else
                        {
                            responseModel = await GetRecipeById(Convert.ToInt32(dset.Tables[0].Rows[0]["RecipeId"])).ConfigureAwait(false);
                        }
                    }
                    return responseModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.DeptId.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (AlreadyExistsException aee)
            {
                throw new AlreadyExistsException(aee.Message, aee);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoOutletProductFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="recipeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<RecipeReponseModel> UpdateRecipe(RecipeRequestModel viewModel, long recipeId, int userId)
        {
            try
            {
                RecipeReponseModel responseModel = new RecipeReponseModel();
                if (viewModel != null)
                {
                    if (recipeId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.RecipeIdReq.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.RecipeHeader == null)
                    {
                        throw new BadRequestException(ErrorMessages.RecipeInvalid.ToString(CultureInfo.CurrentCulture));
                    }

                    var recipeRequestModel = new List<RecipeSPRequestModel>();
                    var header = new RecipeSPRequestModel
                    {

                        Description = viewModel.RecipeHeader?.Description,
                        ProductID = viewModel.RecipeHeader.ProductID,
                        Qty = viewModel.RecipeHeader.Qty,
                        OutletID = viewModel.RecipeHeader.OutletID,
                        IsParents = 1,
                        IngredientProductID = null
                    };
                    if (viewModel?.RecipeDetail != null)
                    {
                        recipeRequestModel = viewModel?.RecipeDetail?.Select(MappingHelpers.CreateMap).ToList();
                    }
                    recipeRequestModel.Add(header);


                    var repository = _unitOfWork?.GetRepository<Recipe>();
                    var exists = await (repository.GetAll().Where(x => x.ID == recipeId).FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                    if (exists != null)
                    {
                        var recipeRequestDataTable = MappingHelpers.ToDataTable(recipeRequestModel, true);

                        List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@RecipeId", recipeId),
                new SqlParameter("@UserId", userId),
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = "@RecipeDataTable",
                  TypeName ="dbo.RecipeRequestType",
                  Value = recipeRequestDataTable,
                  SqlDbType = SqlDbType.Structured
                },
                new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString()),

            };
                        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.AddUpdateRecipe, dbParams.ToArray()).ConfigureAwait(false);

                        if (dset?.Tables?.Count > 0)
                        {
                            if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                            {
                                throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                            }
                            else
                            {
                                responseModel = await GetRecipeById(recipeId).ConfigureAwait(false);
                            }
                        }
                    }
                    else
                    {
                        throw new NullReferenceCustomException(ErrorMessages.RecipeNotExist.ToString(CultureInfo.CurrentCulture));
                    }

                    return responseModel;
                }

                throw new NullReferenceCustomException(ErrorMessages.RecipeInvalid.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {

                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }

        /// <summary>
        /// Delete Recipe
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRecipe(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Recipe>();
                    var exists = await repository.GetById(Id).ConfigureAwait(false);

                    if (exists != null && exists.IsActive != Status.Deleted)
                    {
                        var toDelete = await repository.GetAll().Where(x => x.ProductID == exists.ProductID && x.OutletID == exists.OutletID)
                      .ToListAsyncSafe().ConfigureAwait(false);

                        foreach (var item in toDelete)
                        {
                            item.ModifiedBy = userId;
                            item.IsActive = Status.Deleted;
                            repository?.Update(item);
                        }
                    }
                    else
                    {
                        throw new NullReferenceException(ErrorMessages.RecipeNotExist.ToString(CultureInfo.CurrentCulture));
                    }
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    return true;

                }
                throw new NullReferenceException(ErrorMessages.RecipeNotExist.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
