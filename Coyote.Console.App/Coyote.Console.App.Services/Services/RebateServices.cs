using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Services
{
    public class RebateServices : IRebateServices
    {
        private IUnitOfWork _unitOfWork = null;

        public RebateServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Get all Rebate Headers
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<RebateHeaderResponseModel>>> GetAllActiveHeaders(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {

                var repository = _unitOfWork?.GetRepository<RebateHeader>();

                int count = 0;
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

                List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                    new SqlParameter("@PageNumber", inputModel?.SkipCount),
                    new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                    new SqlParameter("@SortColumn", inputModel?.Sorting),
                    new SqlParameter("@SortDirection", inputModel?.Direction),
                    new SqlParameter("@IsLogged", inputModel?.IsLogged),
                    new SqlParameter("@Module","Rebate"),
                    new SqlParameter("@RoleId",RoleId)
                  };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllRebate, dbParams.ToArray()).ConfigureAwait(false);
                var listVM = MappingHelpers.ConvertDataTable<RebateHeaderResponseModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<RebateHeaderResponseModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StockTakeHeaderNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<RebateResponseModel> GetRebateById(long Id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<RebateHeader>();

                var rebate = await repository.GetAll().Include(c => c.RebateOutlets).Include(c => c.RebateDetails).ThenInclude(c => c.Product).Where(x => !x.IsDeleted).Include(c => c.Manufacturer).Include(c => c.Zone).Where(x => x.Id == Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                var response = MappingHelpers.CreateRebateMap(rebate);
                if (response == null)
                {
                    throw new BadRequestException(ErrorMessages.RebateNotFound.ToString(CultureInfo.CurrentCulture));
                }

                return response;
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<RebateResponseModel> AddRebateAsync(RebateRequestModel requestModel, int UserId)
        {
            try
            {
                if (requestModel != null)
                {
                    var repository = _unitOfWork.GetRepository<RebateHeader>();

                    if (string.IsNullOrEmpty(requestModel.Code))
                    {
                        throw new BadRequestException(ErrorMessages.InvalidCode.ToString(CultureInfo.CurrentCulture));
                    }
                    if (string.IsNullOrEmpty(requestModel.Description))
                    {
                        throw new BadRequestException(ErrorMessages.DescRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.ManufacturerId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ManufacturerId.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((requestModel.ZoneId == null || requestModel.ZoneId <= 0) && (requestModel.RebateOutletsList == null || requestModel.RebateOutletsList.Count == 0))
                    {
                        throw new BadRequestException(ErrorMessages.ZoneOrOutlet.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.RebateDetailsList.Count == 0)
                    {
                        throw new BadRequestException(ErrorMessages.RebateItem.ToString(CultureInfo.CurrentCulture));
                    }


                    var inputModel = MappingHelpers.Mapping<RebateRequestModel, RebateHeader>(requestModel);
                    inputModel.UpdatedById = UserId;
                    inputModel.CreatedById = UserId;

                    if (inputModel.ZoneId == null && requestModel.RebateOutletsList.Count > 0)
                    {
                        var storeRepo = _unitOfWork.GetRepository<Store>();
                        foreach (var sid in requestModel.RebateOutletsList)
                        {
                            if (!await storeRepo.GetAll().Where(x => x.Id == sid && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                            {
                                throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                            }
                        }


                        inputModel.RebateOutlets = new List<RebateOutlets>();

                        foreach (int storeId in requestModel.RebateOutletsList)
                        {
                            var RebateOutlet = new RebateOutlets();
                            RebateOutlet.RebateHeaderId = inputModel.Id;
                            RebateOutlet.StoreId = storeId;
                            RebateOutlet.CreatedById = UserId;
                            RebateOutlet.UpdatedById = UserId;
                            RebateOutlet.CreatedAt = DateTime.UtcNow;
                            RebateOutlet.UpdatedAt = DateTime.UtcNow;

                            inputModel.RebateOutlets.Add(RebateOutlet);
                        }
                    }                   

                    if (requestModel.RebateDetailsList.Count > 0)
                    {
                        var prodRepo = _unitOfWork.GetRepository<Product>();


                        if (!await prodRepo.GetAll().Where(x => !x.Id.Equals(requestModel.RebateDetailsList.Select(x => x.ProductId).Any())).AnyAsyncSafe().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidProductId.ToString(CultureInfo.CurrentCulture));
                        }

                        inputModel.RebateDetails = new List<RebateDetails>();
                        foreach (var detail in requestModel.RebateDetailsList)
                        {
                            var RebateProduct = new RebateDetails();
                            RebateProduct.ProductId = detail.ProductId;
                            RebateProduct.Amount = detail.Amount;
                            RebateProduct.RebateHeaderId = inputModel.Id;
                            RebateProduct.CreatedById = UserId;
                            RebateProduct.UpdatedById = UserId;
                            RebateProduct.CreatedAt = DateTime.UtcNow;
                            RebateProduct.UpdatedAt = DateTime.UtcNow;

                            inputModel.RebateDetails.Add(RebateProduct);
                        }

                    }

                    var result = await repository.InsertAsync(inputModel).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                    if (result != null && result.Id > 0)
                    {
                        return (await GetRebateById(result.Id).ConfigureAwait(false));
                    }
                }
                throw new BadRequestException(ErrorMessages.RebateInvalid.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }


        }

        public async Task<RebateResponseModel> UpdateRebate(RebateRequestModel requestModel, int id, int UserId)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException(ErrorMessages.RebateIdReq.ToString(CultureInfo.CurrentCulture));
                }

                if (requestModel != null)
                {
                    var repository = _unitOfWork.GetRepository<RebateHeader>();

                    if (string.IsNullOrEmpty(requestModel.Description))
                    {
                        throw new BadRequestException(ErrorMessages.DescRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.ManufacturerId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ManufacturerId.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((requestModel.ZoneId == null || requestModel.ZoneId <= 0) && (requestModel.RebateOutletsList == null || requestModel.RebateOutletsList.Count == 0))
                    {
                        throw new BadRequestException(ErrorMessages.ZoneOrOutlet.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.RebateDetailsList.Count == 0)
                    {
                        throw new BadRequestException(ErrorMessages.RebateItem.ToString(CultureInfo.CurrentCulture));
                    }

                    var exists = await repository.GetAll().Include(c => c.RebateDetails).Include(c => c.RebateOutlets).Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (exists == null)
                    {
                        throw new BadRequestException(ErrorMessages.RebateNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var inputModel = MappingHelpers.Mapping<RebateRequestModel, RebateHeader>(requestModel);
                    inputModel.UpdatedById = UserId;
                    inputModel.CreatedById = exists.CreatedById;
                    inputModel.CreatedAt = exists.CreatedAt;
                    inputModel.Code = exists.Code;
                    inputModel.Id = id;

                    if (requestModel.RebateDetailsList.Count == 0)
                    {
                        throw new BadRequestException(ErrorMessages.RebateItem.ToString(CultureInfo.CurrentCulture));
                    }

                    //Delete Stores
                    if (exists.RebateOutlets.Count > 0)
                    {
                        var RebstoreRepo = _unitOfWork.GetRepository<RebateOutlets>();
                        foreach (var existingChild in exists.RebateOutlets.ToList())
                        {
                            var child = requestModel.RebateOutletsList.FirstOrDefault(c => c == existingChild.StoreId);
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                            if (child == null || child <= 0)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                            {
                                //item is deleted
                                existingChild.UpdatedAt = DateTime.UtcNow;
                                existingChild.UpdatedById = UserId;
                                existingChild.IsDeleted = true;
                                RebstoreRepo.Update(existingChild);
                            }
                        }
                    }
                    //Add/Update Stores
                    if (requestModel.RebateOutletsList != null && requestModel.RebateOutletsList.Count > 0)
                    {
                        var storeRepo = _unitOfWork.GetRepository<Store>();

                        foreach (var sid in requestModel.RebateOutletsList)
                        {
                            if (!await storeRepo.GetAll().Where(x => x.Id == sid && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                            {
                                throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                            }
                        }


                        if (inputModel.RebateOutlets == null)
                            inputModel.RebateOutlets = new List<RebateOutlets>();

                        foreach (int storeId in requestModel.RebateOutletsList)
                        {
                            if (!exists.RebateOutlets.Where(x => x.StoreId == storeId && x.RebateHeaderId == id && !x.IsDeleted).Any())
                            {
                                var RebateOutlet = new RebateOutlets();
                                RebateOutlet.RebateHeaderId = inputModel.Id;
                                RebateOutlet.StoreId = storeId;
                                RebateOutlet.CreatedById = UserId;
                                RebateOutlet.UpdatedById = UserId;
                                RebateOutlet.CreatedAt = DateTime.UtcNow;
                                RebateOutlet.UpdatedAt = DateTime.UtcNow;

                                inputModel.RebateOutlets.Add(RebateOutlet);
                            }
                        }
                    }

                    //Delete Details
                    if (exists.RebateDetails.Count > 0)
                    {
                        var RebProdRepo = _unitOfWork.GetRepository<RebateDetails>();
                        foreach (var existingChild in exists.RebateDetails.ToList())
                        {
                            var child = requestModel.RebateDetailsList.FirstOrDefault(c => c.ProductId == existingChild.ProductId);
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                            if (child == null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                            {
                                //item is deleted
                                existingChild.UpdatedAt = DateTime.UtcNow;
                                existingChild.UpdatedById = UserId;
                                existingChild.IsDeleted = true;
                                RebProdRepo.Update(existingChild);
                            }
                        }
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    }

                    //Add/Update Product Details
                    if (requestModel.RebateDetailsList.Count > 0)
                    {
                        var detRepo = _unitOfWork.GetRepository<RebateDetails>();

                        var prodRepo = _unitOfWork.GetRepository<Product>();

                        if (!await prodRepo.GetAll().Where(x => !x.Id.Equals(requestModel.RebateDetailsList.Select(x => x.ProductId).Any())).AnyAsyncSafe().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidProductId.ToString(CultureInfo.CurrentCulture));
                        }


                        if (inputModel.RebateDetails == null)
                            inputModel.RebateDetails = new List<RebateDetails>();
                        foreach (var detail in requestModel.RebateDetailsList)
                        {
                            //for Add
                            var existProdAdd = exists.RebateDetails.Where(x => x.ProductId == detail.ProductId && x.RebateHeaderId == id && !x.IsDeleted).FirstOrDefault();
                            if (existProdAdd == null)
                            {
                                var RebateProduct = new RebateDetails();
                                RebateProduct.ProductId = detail.ProductId;
                                RebateProduct.Amount = detail.Amount;
                                RebateProduct.RebateHeaderId = inputModel.Id;
                                RebateProduct.CreatedById = UserId;
                                RebateProduct.UpdatedById = UserId;
                                RebateProduct.CreatedAt = DateTime.UtcNow;
                                RebateProduct.UpdatedAt = DateTime.UtcNow;

                                inputModel.RebateDetails.Add(RebateProduct);
                            }

                            // for update 
                            var existProd = exists.RebateDetails.Where(x => x.ProductId == detail.ProductId && x.RebateHeaderId == id && x.Amount != detail.Amount && !x.IsDeleted).FirstOrDefault();
                            if (existProd != null)
                            {
                                existProd.Amount = detail.Amount;
                                existProd.UpdatedById = UserId;
                                existProd.UpdatedAt = DateTime.UtcNow;

                                detRepo.DetachLocal(x => x.Id == existProd.Id);
                                detRepo.Update(existProd);
                            }
                            //else
                            //{
                            //    var RebateProduct = new RebateDetails();
                            //    RebateProduct.ProductId = detail.ProductId;
                            //    RebateProduct.Amount = detail.Amount;
                            //    RebateProduct.RebateHeaderId = inputModel.Id;
                            //    RebateProduct.CreatedById = UserId;
                            //    RebateProduct.UpdatedById = UserId;
                            //    RebateProduct.CreatedAt = DateTime.UtcNow;
                            //    RebateProduct.UpdatedAt = DateTime.UtcNow;

                            //    inputModel.RebateDetails.Add(RebateProduct);
                            //}
                        }
                    }

                    repository.DetachLocal(x => x.Id == id);
                    repository.Update(inputModel);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                }
                return (await GetRebateById(id).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }


        }
        public async Task<bool> DeleteRebate(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<RebateHeader>();
                    var exists = await repository.GetAll().Include(c => c.RebateDetails).Include(c => c.RebateOutlets).Where(x => x.Id == Id && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (exists != null)
                    {
                        exists.IsDeleted = true;
                        foreach (var detail in exists.RebateDetails)
                        {
                            detail.IsDeleted = true;
                            detail.UpdatedById = userId;
                            detail.UpdatedAt = DateTime.UtcNow;
                        }
                        foreach (var outlet in exists.RebateOutlets)
                        {
                            outlet.IsDeleted = true;
                            outlet.UpdatedById = userId;
                            outlet.UpdatedAt = DateTime.UtcNow;
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


