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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services
{
    public class SupplierService : ISupplierService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;


        public SupplierService(IUnitOfWork context, IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager)
        {
            _unitOfWork = context;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }


        /// <summary>
        /// Get all active suppliers
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PagedOutputModel<List<SupplierResponseViewModel>>> GetAllActiveSuppliers(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Supplier>();
                var supplier = repository.GetAll(x => !x.IsDeleted);

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))

                        supplier = supplier.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()));


                    supplier = supplier.OrderByDescending(x => x.UpdatedAt);
                    count = supplier.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        supplier = supplier.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    //if (!string.IsNullOrEmpty(inputModel.Status))
                    //{ supplier = supplier.Where(x => x.Sta); }

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    supplier = supplier.OrderBy(x => x.Code);
                                else
                                    supplier = supplier.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    supplier = supplier.OrderBy(x => x.Desc);
                                else
                                    supplier = supplier.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    supplier = supplier.OrderBy(x => x.Id);
                                else
                                    supplier = supplier.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                List<SupplierResponseViewModel> supplierViewModel;
                supplierViewModel = (await supplier.ToListAsync().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<Supplier, SupplierResponseViewModel>).ToList();
                return new PagedOutputModel<List<SupplierResponseViewModel>>(supplierViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get all active suppliers
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PagedOutputModel<List<SupplierResponseViewModel>>> GetActiveSuppliers(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                var repository = _unitOfWork?.GetRepository<Supplier>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@PageNumber", inputModel?.SkipCount),
                        new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@IsLogged", inputModel?.IsLogged),
                        new SqlParameter("@Module","Supplier"),
                        new SqlParameter("@RoleId",RoleId)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveSupplier, dbParams.ToArray()).ConfigureAwait(false);
                List<SupplierResponseViewModel> suppliersViewModel = MappingHelpers.ConvertDataTable<SupplierResponseViewModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<SupplierResponseViewModel>>(suppliersViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get a supplier with specific Supp_Id
        /// </summary>
        /// <param name="supplierId">Supplier Id</param>
        /// <returns>SupplierViewModel</returns>
        public async Task<SupplierResponseViewModel> GetSuppliersById(int supplierId)
        {
            try
            {
                if (supplierId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Supplier>();
                    var supplier = await repository.GetAll(x => x.Id == supplierId && !x.IsDeleted)
                        .FirstOrDefaultAsync().ConfigureAwait(false);
                    if (supplier == null)
                    {
                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    SupplierResponseViewModel supplierViewModel;
                    supplierViewModel = _iAutoMapper.Mapping<Supplier, SupplierResponseViewModel>(supplier);
                    return supplierViewModel;
                }
                throw new NullReferenceException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
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

        /// <summary>
        /// To delete a supplier
        /// </summary>
        /// <param name="supplierId">Supplier Id</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteSupplier(int supplierId, int userId)
        {
            try
            {
                if (supplierId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Supplier>();
                    var supplier = await repository.GetAll(x => x.Id == supplierId && !x.IsDeleted
                    ).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (supplier != null && !supplier.IsDeleted)
                    {
                        supplier.UpdatedById = userId;
                        supplier.IsDeleted = true;
                        repository?.Update(supplier);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.SupplierId.ToString(CultureInfo.CurrentCulture));
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
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<SupplierResponseViewModel> Update(SupplierRequestModel viewModel, int id, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (id == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.SupplierId.ToString(CultureInfo.CurrentCulture));
                    }
                    var repository = _unitOfWork?.GetRepository<Supplier>();
                    if (await repository.GetAll(x => x.Code == viewModel.Code && x.Id != id && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.SupplierDuplicateCode.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await repository.GetById(id).ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        var comm = _iAutoMapper.Mapping<SupplierRequestModel, Supplier>(viewModel);
                        comm.Id = id;
                        comm.CreatedAt = exists.CreatedAt;
                        comm.CreatedById = exists.CreatedById;
                        comm.UpdatedAt = DateTime.UtcNow;
                        comm.UpdatedById = userId;
                        comm.IsDeleted = false;
                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        if (await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false))
                        {
                            return await GetSuppliersById(id).ConfigureAwait(false);
                        }
                        throw new NullReferenceException();
                    }
                    throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<SupplierResponseViewModel> Insert(SupplierRequestModel viewModel, int userId)
        {
            int resultId = 0;
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<Supplier>();
                    if (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.SupplierDuplicateCode.ToString(CultureInfo.CurrentCulture));
                    }
                    var comm = _iAutoMapper.Mapping<SupplierRequestModel, Supplier>(viewModel);
                    comm.IsDeleted = false;
                    comm.CreatedAt = DateTime.UtcNow;
                    comm.UpdatedAt = DateTime.UtcNow;
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        resultId = result.Id;
                    }
                }
                return await GetSuppliersById(resultId).ConfigureAwait(false);
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
