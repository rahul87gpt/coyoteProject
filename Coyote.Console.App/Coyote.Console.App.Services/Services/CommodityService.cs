using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services
{
    public class CommodityService : ICommodityService
    {
        private readonly IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLoggerManager = null;
        public CommodityService(IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager, IUnitOfWork commodityRepository)
        {
            _unitOfWork = commodityRepository;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }

        public async Task<PagedOutputModel<List<CommodityResponseModel>>> GetAllActiveCommodities(PagedInputModel inputModel = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Commodity>();
                var commodity = repository.GetAll().Include(c => c.Departments).Where(x => !x.IsDeleted);

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        commodity = commodity.Where(x => x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        commodity = commodity.Where(x => x.Status);

                    commodity = commodity.OrderByDescending(x => x.UpdatedAt);
                    count = commodity.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        commodity = commodity.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    commodity = commodity.OrderBy(x => x.Code);
                                else
                                    commodity = commodity.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    commodity = commodity.OrderBy(x => x.Desc);
                                else
                                    commodity = commodity.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    commodity = commodity.OrderBy(x => x.UpdatedAt);
                                else
                                    commodity = commodity.OrderByDescending(x => x.UpdatedAt);
                                break;
                        }
                    }
                }

                List<CommodityResponseModel> commodityViewModel;
                commodityViewModel = (await commodity.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                return new PagedOutputModel<List<CommodityResponseModel>>(commodityViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<PagedOutputModel<List<CommodityResponseModel>>> GetActiveCommodities(SecurityViewModel securityViewModel,PagedInputModel inputModel = null)
        {
            try
            {
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                var repository = _unitOfWork?.GetRepository<Commodity>();
                var commodity = repository.GetAll().Include(c => c.Departments).Where(x => !x.IsDeleted);

                int count = 0;

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                new SqlParameter("@SkipCount", inputModel?.SkipCount),
                new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                new SqlParameter("@SortColumn", inputModel?.Sorting),
                new SqlParameter("@SortDirection", inputModel?.Direction),
                new SqlParameter("@IsCountRequired", IsRequired.True),
                new SqlParameter("@IsLogged", inputModel?.IsLogged),
                new SqlParameter("@RoleId",RoleId)
            };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveCommodities, dbParams.ToArray()).ConfigureAwait(false);
                List<CommodityResponseModel> commodityViewModel = MappingHelpers.ConvertDataTable<CommodityResponseModel>(dset.Tables[0]);
                count = Convert.ToInt32(commodityViewModel.Count);
                return new PagedOutputModel<List<CommodityResponseModel>>(commodityViewModel, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<CommodityResponseModel> GetCommodityById(int id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Commodity>();
                var commodity = await (repository?.GetAll()?.Include(x => x.Departments)?.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                CommodityResponseModel commodityViewModel = MappingHelpers.CreateMap(commodity);
                if (commodityViewModel == null)
                {
                    throw new NotFoundException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
                }
                return commodityViewModel;

            }
            catch (Exception ex)
            {
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

        public async Task<bool> DeleteCommodity(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Commodity>();
                    var exists = await repository.GetById(Id).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        exists.UpdatedById = userId;
                        exists.IsDeleted = true;
                        //  exists.Code = (exists.Code + "~" + exists.Id);
                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;

                    }
                    throw new NullReferenceException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.CommodityIdNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<CommodityResponseModel> Update(CommodityRequestModel viewModel, int comId, int userId)
        {
            try
            {
                CommodityResponseModel responseModel = new CommodityResponseModel();
                if (viewModel != null)
                {
                    if (comId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.CommodityIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var repository = _unitOfWork?.GetRepository<Commodity>();
                    var exists = await repository.GetAll().Where(x => x.Id == comId).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (exists.Code != viewModel.Code && (await repository.GetAll().Where(x => x.Code == viewModel.Code && !x.IsDeleted && x.Id != comId).AnyAsyncSafe().ConfigureAwait(false)))
                        {
                            throw new AlreadyExistsException(ErrorMessages.CommodityCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }

                        var comm = MappingHelpers.Mapping<CommodityRequestModel, Commodity>(viewModel);
                        comm.Id = comId;
                        comm.IsDeleted = false;
                        comm.CreatedAt = exists.CreatedAt;
                        comm.CreatedById = exists.CreatedById;
                        comm.UpdatedById = userId;
                        comm.Status = true;
                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                        responseModel = await GetCommodityById(comm.Id).ConfigureAwait(false);

                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.CommodityNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
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

        public async Task<CommodityResponseModel> Insert(CommodityRequestModel viewModel, int userId)
        {
            CommodityResponseModel responseModel = new CommodityResponseModel();
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.DepartmentId > 0)
                    {
                        var repository = _unitOfWork?.GetRepository<Commodity>();
                        if ((await repository.GetAll().Where(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                        {
                            throw new AlreadyExistsException(ErrorMessages.CommodityCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                        var comm = MappingHelpers.Mapping<CommodityRequestModel, Commodity>(viewModel);
                        comm.IsDeleted = false;
                        comm.CreatedById = userId;
                        comm.UpdatedById = userId;
                        comm.Status = true;
                        var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        if (result != null)
                        {
                            responseModel = await GetCommodityById(result.Id).ConfigureAwait(false);
                        }
                        return responseModel;
                    }
                    throw new BadRequestException(ErrorMessages.DepartmentIdNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException();

            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
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
    }
}
