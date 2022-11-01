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

namespace Coyote.Console.App.Services
{
    public class WarehouseServices : IWarehouseServices
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;

        public WarehouseServices(IUnitOfWork iWarehouseRepo, IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager)
        {
            _unitOfWork = iWarehouseRepo;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }

        /// <summary>
        /// Get all active Warehouses.
        /// </summary>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<WarehouseResponseViewModel>>> GetAllActiveWarehouse(PagedInputModel filter = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Warehouse>();
                var list = repository.GetAll(x => !x.IsDeleted && !x.Supplier.IsDeleted && !x.HostFormatMasterListItem.IsDeleted, include: x => x.Include(z => z.Supplier).Include(x => x.HostFormatMasterListItem));
                int count = 0;
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        list = list.Where(x => x.Code.ToLower().Contains(filter.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(filter.GlobalFilter.ToLower()));
                   
                    list= list.OrderByDescending(x => x.UpdatedAt);

                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty((filter?.Status)))
                        list = list.Where(x => x.Status);

                    if (!string.IsNullOrEmpty((filter?.Sorting)))
                    {
                        switch (filter.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => x.Code);
                                else
                                    list = list.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => x.Desc);
                                else
                                    list = list.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(filter.Direction))
                                    list = list.OrderBy(x => x.Id);
                                else
                                    list = list.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                var listResult = (await list.ToListAsyncSafe().ConfigureAwait(false));
                var listViewModel = listResult.Select(_iAutoMapper.Mapping<Warehouse, WarehouseResponseViewModel>).ToList();
                count = list.Count();
                foreach (var item in listViewModel)
                {
                    item.SupplierCode = listResult.FirstOrDefault(x => x.Id == item.Id).Supplier.Code;
                    item.SupplierName = listResult.FirstOrDefault(x => x.Id == item.Id).Supplier.Desc;
                    item.HostFormatName = listResult.FirstOrDefault(x => x.Id == item.Id).HostFormatMasterListItem.Name;
                    item.HostFormatCode = listResult.FirstOrDefault(x => x.Id == item.Id).HostFormatMasterListItem.Code;

                }

                return new PagedOutputModel<List<WarehouseResponseViewModel>>(listViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.WarehouseNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get a warehouse using specific Id
        /// </summary>
        /// <param name="wearehouseId"></param>
        /// <returns></returns>
        public async Task<WarehouseResponseViewModel> GetWarehouseById(int wearehouseId)
        {
            try
            {
                if (wearehouseId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Warehouse>();
                    var warehouse = await repository.GetAll(x => !x.IsDeleted && !x.Supplier.IsDeleted && !x.HostFormatMasterListItem.IsDeleted && x.Id == wearehouseId, include: x => x.Include(z => z.Supplier).Include(x => x.HostFormatMasterListItem)).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (warehouse == null)
                    {
                        throw new NotFoundException(ErrorMessages.WarehouseIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var Model = _iAutoMapper.Mapping<Warehouse, WarehouseResponseViewModel>(warehouse);
                    Model.SupplierCode = warehouse.Supplier.Code;
                    Model.SupplierName = warehouse.Supplier.Desc;
                    Model.HostFormatName = warehouse.HostFormatMasterListItem.Name;
                    Model.HostFormatCode = warehouse.HostFormatMasterListItem.Code;
                    return Model;
                }
                throw new NullReferenceException(ErrorMessages.WarehouseIdNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Delete a warehouse.
        /// </summary>
        /// <param name="warehouseId"></param>
        public async Task<bool> DeleteWarehouse(int warehouseId, int userId)
        {
            try
            {
                if (warehouseId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Warehouse>();
                    var exists = await repository.GetById(warehouseId).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        exists.UpdatedById = userId;
                        exists.UpdatedAt = DateTime.UtcNow;
                        exists.IsDeleted = true;
                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.WarehouseDeleted.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.WarehouseIdNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<WarehouseResponseViewModel> Update(WarehouseRequestModel viewModel,int warehouseId, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    WarehouseResponseViewModel responseViewModel = new WarehouseResponseViewModel();

                    if (warehouseId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.WarehouseIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.SupplierId >= 0)
                    {
                        var repository = _unitOfWork?.GetRepository<Warehouse>();
                        if (await repository.GetAll(x => x.Code == viewModel.Code && x.Id != warehouseId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new AlreadyExistsException(ErrorMessages.WarehouseCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                        var exists = await repository.GetById(warehouseId).ConfigureAwait(false);

                        if (exists != null)
                        {
                            if (exists.IsDeleted == true)
                            {
                                throw new NotFoundException(ErrorMessages.WarehouseNotFound.ToString(CultureInfo.CurrentCulture));
                            }

                            var ware = _iAutoMapper.Mapping<WarehouseRequestModel, Warehouse>(viewModel);
                            ware.Id = warehouseId;
                            ware.UpdatedById = userId;
                            ware.CreatedAt = exists.CreatedAt;
                            ware.CreatedById = exists.CreatedById;
                            ware.IsDeleted = false;
                            repository.DetachLocal(_ => _.Id == ware.Id);
                            repository.Update(ware);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                            responseViewModel = await GetWarehouseById(ware.Id).ConfigureAwait(false);
                            return responseViewModel;
                        
                        }
                        throw new NotFoundException(ErrorMessages.WarehouseNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    throw new NotFoundException(ErrorMessages.WarehouseNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<WarehouseResponseViewModel> Insert(WarehouseRequestModel viewModel, int userId)
        {
            WarehouseResponseViewModel responseModel = new WarehouseResponseViewModel();
            try
            {
                if (viewModel != null)
                {

                    if (viewModel.SupplierId > 0)
                    {
                        var repository = _unitOfWork?.GetRepository<Warehouse>();
                        if (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new AlreadyExistsException(ErrorMessages.WarehouseCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                        var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
                        if (!await supplierRepo.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        {
                            throw new BadRequestException(ErrorMessages.WarehouseSupplierNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        var masterRepo = _unitOfWork?.GetRepository<MasterList>();
                        var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                        int? listId = (await masterRepo.GetAll(x => x.Code == "WarehouseHostFormat" && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false))?.Id;
                        if (listId == null || listId == 0)
                        {
                            throw new BadRequestException(ErrorMessages.WarehouseHostNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await listItemRepo.GetAll(x => x.ListId == listId && x.Id == viewModel.HostFormatId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.WarehouseHostNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        var ware = _iAutoMapper.Mapping<WarehouseRequestModel, Warehouse>(viewModel);
                        ware.IsDeleted = false;
                        ware.CreatedAt = DateTime.UtcNow;
                        ware.UpdatedAt = DateTime.UtcNow;
                        ware.CreatedById = userId;
                        ware.UpdatedById = userId;
                        var result = await repository.InsertAsync(ware).ConfigureAwait(false);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        if (result != null)
                        {
                            responseModel = await GetWarehouseById(result.Id).ConfigureAwait(false);
                        }
                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                }
                return responseModel;
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
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }
    }
}
