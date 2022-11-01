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
    public class HostSettingsService: IHostSettingsService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;

        public HostSettingsService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMapping;
        }
        /// <summary>
        /// Get all HostSettings
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PagedOutputModel<List<HostSettingsResponseModel>>> GetAllHostSettings(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<HostSettings>();
                var hostSettings = repository.GetAll().Include(c => c.Path).Include(c => c.Supplier).Include(c => c.Warehouse).Include(c => c.HostFormatWareHouse).Where(x => x.IsActive == Status.Active);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                    {
                        hostSettings = hostSettings.Where(x => x.Description.ToLower().Contains(inputModel.GlobalFilter.ToLower()));
                    }

                    hostSettings = hostSettings.OrderByDescending(x => x.ModifiedDate); 
                    count = hostSettings.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        hostSettings = hostSettings.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    hostSettings = hostSettings.OrderBy(x => x.Code);
                                else
                                    hostSettings = hostSettings.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    hostSettings = hostSettings.OrderBy(x => x.Description);
                                else
                                    hostSettings = hostSettings.OrderByDescending(x => x.Description);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    hostSettings = hostSettings.OrderBy(x => x.ID);
                                else
                                    hostSettings = hostSettings.OrderByDescending(x => x.ID);
                                break;
                        }
                    }

                }
                List<HostSettingsResponseModel> hostSettingsResponses;
                hostSettingsResponses = (await hostSettings.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateHostSettingsMap).ToList();
                return new PagedOutputModel<List<HostSettingsResponseModel>>(hostSettingsResponses, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.HostSettingsNotFound.ToString(CultureInfo.CurrentCulture));
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
        public async Task<HostSettingsResponseModel> GetHostSettingsById(int Id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<HostSettings>();
                var hostSettings = await repository.GetAll().Include(c => c.Path).Include(c => c.Supplier).Include(c => c.Warehouse).Include(c => c.HostFormatWareHouse).Where(x => x.ID == Id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (hostSettings == null)
                    throw new NotFoundException(ErrorMessages.HostSettingsNotFound.ToString(CultureInfo.CurrentCulture));

                return MappingHelpers.CreateHostSettingsMap(hostSettings);
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
        public async Task<HostSettingsResponseModel> AddHostSettings(HostSettingsRequestModel viewModel, int userId)
        {

            try
            {
                if (viewModel == null)
                    throw new NullReferenceException();

                if (viewModel.WareHouseID == 0)
                {
                    throw new BadRequestException(ErrorMessages.WarehouseRequired.ToString(CultureInfo.CurrentCulture));
                }

                if (viewModel.FilePathID == 0)
                {
                    throw new BadRequestException(ErrorMessages.PathIdReq.ToString(CultureInfo.CurrentCulture));
                }

                var repository = _unitOfWork?.GetRepository<HostSettings>();
                var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
              
                if (await repository.GetAll().Where(x => x.Code == viewModel.Code && x.IsActive == Status.Active).AnyAsync().ConfigureAwait(false))
                {
                    throw new AlreadyExistsException(ErrorMessages.CodeDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                // HostFormat
                if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == viewModel.HostFormatID && x.MasterList.Code == MasterListCode.WAREHOUSEHOSTFORMAT).AnyAsyncSafe().ConfigureAwait(false)))
                {
                    throw new BadRequestException(ErrorMessages.HostFormatNotFound.ToString(CultureInfo.CurrentCulture));
                }

                var comm = _iAutoMapper.Mapping<HostSettingsRequestModel, HostSettings>(viewModel);
                comm.Code = viewModel.Code;
                comm.Description = viewModel.Description;
                comm.InitialLoadFileWeekly = viewModel.InitialLoadFileWeekly;
                comm.WeeklyFile = viewModel.WeeklyFile;
                comm.FilePathID = viewModel.FilePathID;
                comm.NumberFactor = viewModel.NumberFactor;
                comm.SupplierID = viewModel.SupplierID;
                comm.WareHouseID = viewModel.WareHouseID;
                comm.HostFormatID = viewModel.HostFormatID;
                comm.SellPromoPrefix = viewModel.SellPromoPrefix;
                comm.BuyPromoPrefix = viewModel.BuyPromoPrefix;
                comm.IsActive = Status.Active;
                comm.CreatedBy = userId;
                comm.CreatedDate = DateTime.UtcNow;
                comm.ModifiedBy = userId;
                comm.ModifiedDate = DateTime.UtcNow;
                var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetHostSettingsById(result.ID).ConfigureAwait(false);
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
        public async Task<HostSettingsResponseModel> EditHostSettings(HostSettingsRequestModel viewModel, int id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<HostSettings>();

                if (viewModel == null)
                    throw new NullReferenceException();

                if (viewModel.WareHouseID == 0)
                {
                    throw new BadRequestException(ErrorMessages.WarehouseRequired.ToString(CultureInfo.CurrentCulture));
                }

                var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                // HostFormat
                if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == viewModel.HostFormatID && x.MasterList.Code == MasterListCode.WAREHOUSEHOSTFORMAT).AnyAsyncSafe().ConfigureAwait(false)))
                {
                    throw new BadRequestException(ErrorMessages.HostFormatNotFound.ToString(CultureInfo.CurrentCulture));
                }

                if (viewModel.FilePathID == 0)
                {
                    throw new BadRequestException(ErrorMessages.PathIdReq.ToString(CultureInfo.CurrentCulture));
                }

                if (await repository.GetAll().Where(x => x.Code == viewModel.Code && x.ID != id && x.IsActive == Status.Active).AnyAsync().ConfigureAwait(false))
                {
                    throw new AlreadyExistsException(ErrorMessages.CodeDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var exists = await repository.GetAll().Where(x => x.ID == id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (exists == null)
                    throw new NotFoundException(ErrorMessages.HostSettingsNotFound.ToString(CultureInfo.CurrentCulture));

                exists.Code = viewModel?.Code?? exists.Code;
                exists.Description = viewModel?.Description?? exists.Description;
                exists.InitialLoadFileWeekly = viewModel?.InitialLoadFileWeekly ?? exists.InitialLoadFileWeekly;
                exists.WeeklyFile = viewModel?.WeeklyFile ?? exists.WeeklyFile;
                exists.FilePathID = viewModel.FilePathID;
                exists.NumberFactor = viewModel?.NumberFactor ?? exists.NumberFactor;
                exists.SupplierID = viewModel?.SupplierID ?? exists.SupplierID;
                exists.WareHouseID = viewModel?.WareHouseID ?? exists.WareHouseID;
                exists.HostFormatID = viewModel?.HostFormatID ?? exists.HostFormatID; 
                exists.SellPromoPrefix = viewModel?.SellPromoPrefix ?? exists.SellPromoPrefix;
                exists.BuyPromoPrefix = viewModel?.BuyPromoPrefix ?? exists.BuyPromoPrefix;             
                exists.CreatedDate = exists.CreatedDate;
                exists.ModifiedBy = userId;
                exists.ModifiedDate = DateTime.UtcNow;
                repository.DetachLocal(_ => _.ID == exists.ID);
                repository.Update(exists);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetHostSettingsById(id).ConfigureAwait(false);
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
        public async Task<bool> DeleteHostSettings(int Id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<HostSettings>();
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
