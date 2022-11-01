using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using Microsoft.EntityFrameworkCore;


namespace Coyote.Console.App.Services.Services
{
    public class CostPriceZonesService : ICostPriceZonesService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;

        public CostPriceZonesService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMapping;
        }
        /// <summary>
        /// Get all HostSettings
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PagedOutputModel<List<CostPriceZonesResponseModel>>> GetAllCostPriceZones(PagedInputModel inputModel, int Types)
        {
            try
            {
                var Zone = Types == 1 ? CostPriceZoneType.Cost : CostPriceZoneType.Price;
                var repository = _unitOfWork?.GetRepository<CostPriceZones>();
                var costPriceZones = repository.GetAll().Include(c => c.HostSettings).Where(x => x.IsActive == Status.Active && x.Type == Zone);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                    {
                        costPriceZones = costPriceZones.Where(x => x.Description.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()));
                    }

                    costPriceZones = costPriceZones.OrderByDescending(x => x.HostSettingID);
                    count = costPriceZones.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        costPriceZones = costPriceZones.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    costPriceZones = costPriceZones.OrderBy(x => x.Code);
                                else
                                    costPriceZones = costPriceZones.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    costPriceZones = costPriceZones.OrderBy(x => x.Description);
                                else
                                    costPriceZones = costPriceZones.OrderByDescending(x => x.Description);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    costPriceZones = costPriceZones.OrderBy(x => x.ID);
                                else
                                    costPriceZones = costPriceZones.OrderByDescending(x => x.ID);
                                break;
                        }
                    }

                }
                List<CostPriceZonesResponseModel> costPriceZonesResponseModels;
                costPriceZonesResponseModels = (await costPriceZones.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateCostPriceZonesMap).ToList();
                return new PagedOutputModel<List<CostPriceZonesResponseModel>>(costPriceZonesResponseModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.CostPriceZonesNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }
        /// <summary>
        /// Get HostSettings by Id
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<CostPriceZonesResponseModel> GetCostPriceZonesById(int Id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<CostPriceZones>();
                var costPriceZones = await repository.GetAll().Include(c => c.HostSettings).Where(x => x.ID == Id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (costPriceZones == null)
                    throw new NotFoundException(ErrorMessages.PathNotFound.ToString(CultureInfo.CurrentCulture));

                return MappingHelpers.CreateCostPriceZonesMap(costPriceZones);
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

        /// <summary>
        /// Insert HostSettings
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="hostSettingsiD"></param>
        /// <param name="userId"></param>
        /// <returns>List</returns>
        public async Task<CostPriceZonesResponseModel> AddCostPriceZones(CostPriceZonesRequestModel viewModel, int userId,int Types)
        {

            try
            {
                var Zone = Types == 1 ? CostPriceZoneType.Cost : CostPriceZoneType.Price;

                if (viewModel == null)
                    throw new NullReferenceException();

                if (viewModel.HostSettingID == 0 || viewModel.HostSettingID == null)
                {
                    throw new BadRequestException(ErrorMessages.HostSettingRequired.ToString(CultureInfo.CurrentCulture));
                }

                var repository = _unitOfWork?.GetRepository<CostPriceZones>();

                if (await repository.GetAll().Where(x => x.Code == viewModel.Code && x.Type == Zone && x.IsActive == Status.Active).AnyAsync().ConfigureAwait(false))
                {
                    throw new AlreadyExistsException(ErrorMessages.CodeDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var comm = _iAutoMapper.Mapping<CostPriceZonesRequestModel, CostPriceZones>(viewModel);
                comm.Code = viewModel.Code;
                comm.Description = viewModel.Description;
                comm.Type = Types == 1 ? CostPriceZoneType.Cost : CostPriceZoneType.Price;
                comm.HostSettingID = viewModel.HostSettingID;
                comm.Factor1 = viewModel.Factor1;
                comm.Factor2 = viewModel.Factor2;
                comm.Factor3 = viewModel.Factor3;
                comm.SuspUpdOutlet = viewModel.SuspUpdOutlet;
                comm.IsActive = Status.Active;
                comm.CreatedBy = userId;
                comm.CreatedDate = DateTime.UtcNow;
                comm.ModifiedBy = userId;
                comm.ModifiedDate = DateTime.UtcNow;
                var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetCostPriceZonesById(result.ID).ConfigureAwait(false);
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

        /// <summary>
        ///  Update Path
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="hostSettingsiD"></param>
        /// <param name="userId"></param>
        /// <returns>List<PathRequestModel></returns>
        /// 
        public async Task<CostPriceZonesResponseModel> EditCostPriceZones(CostPriceZonesRequestModel viewModel, int id, int userId, int Types)
        {
            try
            {
                var Zone = Types == 1 ? CostPriceZoneType.Cost : CostPriceZoneType.Price;

                var repository = _unitOfWork?.GetRepository<CostPriceZones>();

                if (viewModel == null)
                    throw new NullReferenceException();

                if (viewModel.HostSettingID == 0 || viewModel.HostSettingID == null)
                {
                    throw new BadRequestException(ErrorMessages.HostSettingRequired.ToString(CultureInfo.CurrentCulture));
                }

                if (await repository.GetAll().Where(x => x.Code == viewModel.Code && x.Type == Zone && x.ID != id && x.IsActive == Status.Active).AnyAsync().ConfigureAwait(false))
                {
                    throw new AlreadyExistsException(ErrorMessages.CodeDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var exists = await repository.GetAll().Where(x => x.ID == id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (exists == null)
                    throw new NotFoundException(ErrorMessages.PathNotFound.ToString(CultureInfo.CurrentCulture));

                exists.Code = viewModel?.Code ?? exists.Code;
                exists.Description = viewModel?.Description ?? exists.Description;
                exists.Type = Types == 1 ? CostPriceZoneType.Cost : CostPriceZoneType.Price;
                exists.HostSettingID = viewModel?.HostSettingID ?? exists.HostSettingID;
                exists.Factor1 = viewModel?.Factor1 ?? exists.Factor1;
                exists.Factor2 = viewModel?.Factor2 ?? exists.Factor2;
                exists.Factor3 = viewModel?.Factor3 ?? exists.Factor3;
                exists.SuspUpdOutlet = viewModel?.SuspUpdOutlet ?? exists.SuspUpdOutlet;
                exists.ID = id;
                exists.ModifiedBy = userId;
                exists.ModifiedDate = DateTime.UtcNow;
                repository.DetachLocal(_ => _.ID == exists.ID);
                repository.Update(exists);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetCostPriceZonesById(id).ConfigureAwait(false);
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

        /// <summary>
        /// Delete
        /// </summary>     
        public async Task<bool> DeleteCostPriceZones(int Id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<CostPriceZones>();
                var exists = await repository.GetAll().Where(x => x.ID == Id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (exists == null)
                    throw new NotFoundException(ErrorMessages.PathNotFound.ToString(CultureInfo.CurrentCulture));

                exists.ModifiedBy = userId;
                exists.IsActive = Status.Deleted;
                exists.ModifiedDate = DateTime.UtcNow;
                repository?.Update(exists);

                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return true;
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
