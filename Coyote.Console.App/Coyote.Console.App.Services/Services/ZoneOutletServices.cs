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
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services
{
    public class ZoneOutletServices : IZoneOutletServices
    {

        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLoggerManager = null;

        public ZoneOutletServices(IUnitOfWork repo, IAutoMappingServices iAutoMapperService, ILoggerManager iLoggerManager)
        {
            this._unitOfWork = repo;
            _iAutoMapper = iAutoMapperService;
            _iLoggerManager = iLoggerManager;
        }

        /// <summary>
        /// To delete zone outlets of a zone.
        /// </summary>
        /// <param name="ZoneId">Zone Outlet Id</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteZoneOutlet(int ZoneId, int userId)
        {
            try
            {
                if (ZoneId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<ZoneOutlet>();
                    var zoneOutletExists = await repository.GetById(ZoneId).ConfigureAwait(false);
                    //var zoneOutletExists = await repository.GetAll(x => x.ZoneId == ZoneId).ToListAsyncSafe().ConfigureAwait(false);
                    if (zoneOutletExists == null)
                    {
                        throw new NullReferenceException(ErrorMessages.ZoneOutletNotFound.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                    if (zoneOutletExists.IsDeleted != true)
                    {
                        zoneOutletExists.UpdatedById = userId;
                        zoneOutletExists.UpdatedAt = DateTime.UtcNow;
                        zoneOutletExists.IsDeleted = true;
                        repository?.Update(zoneOutletExists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                }
                throw new NullReferenceException(ErrorMessages.ZoneOutletId.ToString(System.Globalization.CultureInfo.CurrentCulture));

            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }

        }

        /// <summary>
        /// Get all active Zone Outlet.
        /// </summary>
        /// <returns> List<ZoneOutletViewModel></returns>
        public async Task<PagedOutputModel<List<ZoneOutletGetAllResponseModel>>> GetAllActiveZoneOutlet(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<ZoneOutlet>();
                var zoneOutlets = repository.GetAll(x => !x.IsDeleted, includes: new Expression<Func<ZoneOutlet, object>>[] { z => z.Store, z => z.ZoneMasterListItems });

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        zoneOutlets = zoneOutlets.Where(x => x.Store.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.ZoneMasterListItems.Name.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                        zoneOutlets = zoneOutlets.Where(x => securityViewModel.StoreIds.Contains(x.StoreId));

                    zoneOutlets = zoneOutlets.OrderByDescending(x => x.UpdatedAt);
                    count = zoneOutlets.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        zoneOutlets = zoneOutlets.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    switch (inputModel.Sorting?.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "zone":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                zoneOutlets = zoneOutlets.OrderBy(x => x.ZoneId);
                            else
                                zoneOutlets = zoneOutlets.OrderByDescending(x => x.ZoneId);
                            break;
                        case "store":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                zoneOutlets = zoneOutlets.OrderBy(x => x.StoreId);
                            else
                                zoneOutlets = zoneOutlets.OrderByDescending(x => x.StoreId);
                            break;
                        case "name":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                zoneOutlets = zoneOutlets.OrderBy(x => x.ZoneMasterListItems.Name);
                            else
                                zoneOutlets = zoneOutlets.OrderByDescending(x => x.ZoneMasterListItems.Name);
                            break;
                        case "code":
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                zoneOutlets = zoneOutlets.OrderBy(x => x.ZoneMasterListItems.Code);
                            else
                                zoneOutlets = zoneOutlets.OrderByDescending(x => x.ZoneMasterListItems.Code);
                            break;
                        default:
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                zoneOutlets = zoneOutlets.OrderByDescending(x => x.UpdatedAt);
                            else
                                zoneOutlets = zoneOutlets.OrderBy(x => x.UpdatedAt);
                            break;
                    }
                }

                List<ZoneOutletGetAllResponseModel> zoneOutletViewModels;
                var t = (await zoneOutlets.ToListAsyncSafe().ConfigureAwait(false));
                zoneOutletViewModels = t.Select(_iAutoMapper.Mapping<ZoneOutlet, ZoneOutletGetAllResponseModel>).ToList();

                foreach (var zOutlet in zoneOutletViewModels)
                {

                    zOutlet.StoreCode = zoneOutlets.Where(x => x.Id == zOutlet.Id).Select(x => x.Store.Desc).FirstOrDefault();
                    zOutlet.StoreCode = zoneOutlets.Where(x => x.Id == zOutlet.Id).Select(x => x.Store.Code).FirstOrDefault();

                    zOutlet.ZoneName = zoneOutlets.Where(x => x.Id == zOutlet.Id).Select(x => x.ZoneMasterListItems.Name).FirstOrDefault();
                    zOutlet.ZoneCode = zoneOutlets.Where(x => x.Id == zOutlet.Id).Select(x => x.ZoneMasterListItems.Code).FirstOrDefault();
                }
                var allZones = (await zoneOutlets.ToListAsyncSafe().ConfigureAwait(false)).GroupBy(x => x.ZoneId).ToList();

                return new PagedOutputModel<List<ZoneOutletGetAllResponseModel>>(zoneOutletViewModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.NoZoneOutletFound.ToString(System.Globalization.CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    _iLoggerManager.LogWarning(ex.Message);
                    throw new NotFoundException(ex.Message);
                }
                _iLoggerManager.LogError(ex.Message, ex);
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }



        public async Task<PagedOutputModel<List<ZoneOutletGetAllResponseModel>>> GetActiveZoneOutlet(ZoneOutletInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<ZoneOutlet>();

                bool StoreIds = false;

                var AccessStores = String.Empty;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                {
                    foreach (var storeId in securityViewModel.StoreIds)
                        AccessStores += storeId + ",";
                    StoreIds = true;
                }

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@AccessStoreIds", StoreIds?AccessStores:null),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@StoreId", inputModel?.StoreId),
                        new SqlParameter("@ZoneId", inputModel?.ZoneId),
                    };

                var count = 0;

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllZoneOutlet, dbParams.ToArray()).ConfigureAwait(false);
                List<ZoneOutletGetAllResponseModel> zoneOutletViewModels = MappingHelpers.ConvertDataTable<ZoneOutletGetAllResponseModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<ZoneOutletGetAllResponseModel>>(zoneOutletViewModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.NoZoneOutletFound.ToString(System.Globalization.CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    _iLoggerManager.LogWarning(ex.Message);
                    throw new NotFoundException(ex.Message);
                }
                _iLoggerManager.LogError(ex.Message, ex);
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }


        /// <summary>
        /// Get Zone Outlet with a specific Id
        /// </summary>
        /// <param name="ZoneId">Zone Outlet Id</param>
        /// <returns></returns>
        public async Task<ZoneOutletResponseModel> GetZoneOutletById(int ZoneId)
        {
            try
            {
                if (ZoneId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<ZoneOutlet>();
                    var zoneOutlet = repository.GetAll(x => x.ZoneId == ZoneId && x.IsDeleted != true, include: x => x.Include(z => z.Store).Include(z => z.ZoneMasterListItems));
                    //  var zoneOutlet = await repository.GetById(ZoneOutletId).ConfigureAwait(false);

                    var repoStores = _unitOfWork?.GetRepository<Store>();
                    var allStores = repoStores.GetAll(x => !x.IsDeleted);

                    var zone = await zoneOutlet.Select(x => x.ZoneMasterListItems).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (zone == null)
                    {
                        throw new NotFoundException(ErrorMessages.ZoneNotFound.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                    ZoneOutletResponseModel zoneOutletViewModel = new ZoneOutletResponseModel
                    {
                        ZoneId = zone.Id,
                        ZoneCode = zone.Code,
                        ZoneName = zone.Name,
                        Stores = await allStores.Select(x =>
                              new ZoneOutletStore
                              {
                                  StoreId = x.Id,
                                  StoreCode = x.Code,
                                  StoreName = x.Desc,
                                  IsSelected = zoneOutlet.Any(y => y.StoreId == x.Id)
                              }).OrderBy(c => c.StoreName).ToListAsyncSafe().ConfigureAwait(false)
                    };
                    return zoneOutletViewModel;

                }
                else
                {
                    throw new NullReferenceException(ErrorMessages.ZoneOutletId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<ZoneOutletResponseModel> Update(ZoneOutletRequestModel viewModel, int userId)
        {
            try
            {
                var response = new ZoneOutletResponseModel();

                if (viewModel != null)
                {
                    if (!string.IsNullOrEmpty(viewModel.StoreIds))
                    {
                        if (viewModel.ZoneId > 0)
                        {
                            var repository = _unitOfWork?.GetRepository<ZoneOutlet>();
                            var storeList = viewModel.StoreIds.Split(',');

                            if (storeList != null)
                            {
                                if (storeList.Length > 0)
                                {
                                    var outletsToDelete = await repository.GetAll(x => x.ZoneId == viewModel.ZoneId && x.IsDeleted != true).ToListAsyncSafe().ConfigureAwait(false);

                                    foreach (var outlet in outletsToDelete)
                                    {
                                        if (!storeList.Contains(outlet.StoreId.ToString()))
                                        {
                                            outlet.IsDeleted = true;
                                            //Detaching tracked entry - exists
                                            repository.DetachLocal(_ => _.Id == outlet.Id);
                                            repository.Update(outlet);
                                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                        }
                                    }
                                }
                            }
                            foreach (var store in storeList)
                            {
                                int storeId = 0;
                                try
                                {
                                    storeId = Convert.ToInt32(store);

                                    var outletExists = await repository.GetAll(x => x.StoreId == storeId && x.ZoneId == viewModel.ZoneId && x.IsDeleted == false).FirstOrDefaultAsync().ConfigureAwait(false);
                                    if (outletExists != null)
                                    {
                                        outletExists.UpdatedById = userId;
                                        outletExists.UpdatedAt = DateTime.UtcNow;
                                        //Detaching tracked entry - exists
                                        repository.DetachLocal(_ => _.Id == outletExists.Id);
                                        repository.Update(outletExists);
                                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        var comm = new ZoneOutlet();
                                        comm.ZoneId = viewModel.ZoneId;
                                        comm.StoreId = storeId;
                                        comm.IsDeleted = false;
                                        comm.CreatedById = userId;
                                        comm.UpdatedById = userId;
                                        comm.CreatedAt = DateTime.UtcNow;
                                        comm.UpdatedAt = DateTime.UtcNow;

                                        var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw new NullReferenceException(ErrorMessages.InvalidStore.ToString(System.Globalization.CultureInfo.CurrentCulture) + storeId.ToString());
                                }

                            }
                            response = await GetZoneOutletById(viewModel.ZoneId).ConfigureAwait(false);
                            return response;
                        }
                        throw new NullReferenceException(ErrorMessages.UserZoneInvaid.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                    throw new NullReferenceException(ErrorMessages.StoreId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                }
                throw new NotFoundException(ErrorMessages.ZoneOutletNotFound.ToString(System.Globalization.CultureInfo.CurrentCulture));
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
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }

        }

        public async Task<ZoneOutletResponseModel> Insert(ZoneOutletRequestModel viewModel, int userId)
        {
            var response = new ZoneOutletResponseModel();
            try
            {
                if (viewModel != null)
                {

                    if (!string.IsNullOrEmpty(viewModel.StoreIds))
                    {
                        if (viewModel.ZoneId > 0)
                        {
                            var repository = _unitOfWork?.GetRepository<ZoneOutlet>();
                            var storeList = viewModel.StoreIds.Split(',');
                            foreach (var store in storeList)
                            {
                                int storeId = 0;

                                try
                                {
                                    storeId = Convert.ToInt32(store);
                                    if (await repository.GetAll(x => x.StoreId == storeId && x.ZoneId == viewModel.ZoneId && x.IsDeleted == false).AnyAsync().ConfigureAwait(false))
                                    {
                                        throw new AlreadyExistsException(ErrorMessages.ZoneOutletDuplicateId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                                    }
                                }
                                catch (Exception)
                                {
                                    throw new NullReferenceException(ErrorMessages.InvalidStore.ToString(System.Globalization.CultureInfo.CurrentCulture) + storeId.ToString());
                                }
                                var comm = new ZoneOutlet();

                                comm.ZoneId = viewModel.ZoneId;
                                comm.StoreId = storeId;
                                comm.IsDeleted = false;
                                comm.CreatedById = userId;
                                comm.UpdatedById = userId;
                                comm.CreatedAt = DateTime.UtcNow;
                                comm.UpdatedAt = DateTime.UtcNow;

                                var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                                if (result != null)
                                {
                                    var resultId = result.ZoneId;
                                    response = await GetZoneOutletById(resultId).ConfigureAwait(false);

                                }

                            }

                            return response;
                        }
                        throw new NullReferenceException(ErrorMessages.UserZoneInvaid.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                    throw new NullReferenceException(ErrorMessages.StoreId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                }
                return response;
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
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }

        }


        public async Task<List<MasterListItemResponseViewModel>> GetZoneOutletByStoreId(int storeId)
        {
            try
            {
                var responseModel = new List<MasterListItemResponseViewModel>();
                if (storeId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<ZoneOutlet>();
                    var zoneOutlet = await repository.GetAll().Include(x => x.ZoneMasterListItems).Where(x => x.StoreId == storeId && x.IsDeleted != true && !x.ZoneMasterListItems.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);

                    if (zoneOutlet != null)
                    {

                        foreach (var zone in zoneOutlet)
                        {
                            var zones = MappingHelpers.Mapping<MasterListItems, MasterListItemResponseViewModel>(zone.ZoneMasterListItems);
                            responseModel.Add(zones);
                            responseModel = responseModel.OrderBy(x => x.Name).ToList();
                        }
                    }

                    return responseModel;
                }
                else
                {
                    throw new NullReferenceException(ErrorMessages.StoreId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
