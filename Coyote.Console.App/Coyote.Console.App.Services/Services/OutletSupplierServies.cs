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

namespace Coyote.Console.App.Services.Services
{
    public class OutletSupplierServies : IOutlerSupplierServices
    {
        private IUnitOfWork _unitOfWork;
        private IAutoMappingServices _iAutoMapper;

        public OutletSupplierServies(IUnitOfWork unitOfWork, IAutoMappingServices autoMappingServices)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMappingServices;
        }

        /// <summary>
        /// Get all Active Outlet Supplier 
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="securityViewModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<OutletSupplierResponseModel>>> GetAllActiveOutletSupplierAsync(PagedInputModel inputModel, SecurityViewModel securityViewModel = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletSupplierSetting>();

                //Do not show settings of deleted Suppliers and deleted stores
                //var outletSuppier = repository.GetAll()?.Include(c => c.Store)?.Include(c => c.Supplier)?.Include(c => c.StateMasterListItem).Include(c => c.DivisionMasterListItem).Where(x => !x.IsDeleted && !x.Store.IsDeleted && !x.Supplier.IsDeleted);

                var outletSuppier = repository.GetAll()?.Include(c => c.Store)?.Include(c => c.Supplier)?.Include(c => c.StateMasterListItem).Include(c => c.DivisionMasterListItem).Where(x => !x.IsDeleted);


                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        outletSuppier = outletSuppier.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Store.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower())
                        || x.Store.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower())
                        || x.Supplier.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower())
                        || x.Supplier.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()));


                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        outletSuppier = outletSuppier.Where(x => x.Status);


                    outletSuppier = outletSuppier.OrderByDescending(x => x.UpdatedAt);

                    count = outletSuppier.Count();
                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        outletSuppier = outletSuppier.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);


                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    outletSuppier = outletSuppier.OrderBy(x => x.Desc);
                                else
                                    outletSuppier = outletSuppier.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    outletSuppier = outletSuppier.OrderBy(x => x.UpdatedAt);
                                else
                                    outletSuppier = outletSuppier.OrderByDescending(x => x.UpdatedAt);
                                break;
                        }
                    }
                }

                List<OutletSupplierResponseModel> outletProductsModels;
                outletProductsModels = (await outletSuppier.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                return new PagedOutputModel<List<OutletSupplierResponseModel>>(outletProductsModels, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.OutletSupplierNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get all active Products
        /// </summary>
        /// <returns>List<ProductViewModel></returns>
        public async Task<PagedOutputModel<List<OutletSupplierResponseModel>>> GetActiveOutletSupplier(PagedInputModel inputModel, SecurityViewModel securityViewModel = null)
        {
            try
            {
                
                var AccessStores = String.Empty;
               
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

                var repository = _unitOfWork?.GetRepository<OutletSupplierResponseModel>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),                                       
                new SqlParameter("@SortColumn", inputModel?.Sorting),
                new SqlParameter("@SortDirection", inputModel?.Direction),
                new SqlParameter("@PageNumber", inputModel?.SkipCount),
                new SqlParameter("@PageSize", inputModel?.MaxResultCount)

            };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveOutletSupplier, dbParams.ToArray()).ConfigureAwait(false);
                List<OutletSupplierResponseModel> outletSupplierViewModel = MappingHelpers.ConvertDataTable<OutletSupplierResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<OutletSupplierResponseModel>>(outletSupplierViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.OutletSupplierNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get active Outlet SUpplier by Id.
        /// </summary>
        /// <param name="outletSupplierId"></param>
        /// <returns></returns>
        public async Task<OutletSupplierResponseModel> GetOuletSupplierById(int outletSupplierId)
        {
            try
            {
                if (outletSupplierId > 0)
                {
                    //var repository = _unitOfWork?.GetRepository<OutletSupplierSetting>();

                    ////var outletSupplier = await repository.GetAll(x => x.Id == outletSupplierId && !x.IsDeleted).Include(c => c.Store).Include(c => c.Supplier).Include(c => c.StateMasterListItem).Include(c => c.DivisionMasterListItem).FirstOrDefaultAsync().ConfigureAwait(false);
                    //var outletSupplier = await repository.GetAll(x => x.Id == outletSupplierId && !x.IsDeleted).Include(c => c.Store).Include(c => c.Supplier).Include(c => c.StateMasterListItem).FirstOrDefaultAsync().ConfigureAwait(false);
                    //if (outletSupplier == null)
                    //{
                    //    throw new NotFoundException(ErrorMessages.OutletSupplierNotFound.ToString(CultureInfo.CurrentCulture));
                    //}

                    var repository = _unitOfWork?.GetRepository<Recipe>();
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@Id", outletSupplierId),
            };

                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActiveOutletSupplier, dbParams.ToArray()).ConfigureAwait(false);
                    List<OutletSupplierResponseModel> outletSupplierViewModel = MappingHelpers.ConvertDataTable<OutletSupplierResponseModel>(dset.Tables[0]);
                    var count = 0;
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                
                    return outletSupplierViewModel[0];
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.OutletSupplierId.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletSupplierNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Add new Oultet Supplier
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OutletSupplierResponseModel> Insert(OutletSupplierRequestModel viewModel, int userId)
        {

            OutletSupplierResponseModel responseModel = new OutletSupplierResponseModel();
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.StoreId > 0)
                    {
                        var genericRepository = _unitOfWork?.GetRepository<Store>();

                        if (!(await genericRepository.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.SupplierId > 0)
                    {
                        var supplierRepository = _unitOfWork?.GetRepository<Supplier>();

                        if (!(await supplierRepository.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.StateId > 0)
                    {

                        var masterListRepostiory = _unitOfWork?.GetRepository<MasterList>();

                        var listId = await masterListRepostiory.GetAll(x => x.Code == MasterListCode.State && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                        var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                        if (!(await masterRepository.GetAll(x => x.Id == viewModel.StateId && x.ListId == listId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidState.ToString(CultureInfo.CurrentCulture));
                        }

                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidState.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.DivisionId > 0)
                    {

                        var masterListRepostiory = _unitOfWork?.GetRepository<MasterList>();

                        var listId = await masterListRepostiory.GetAll(x => x.Code == MasterListCode.Division && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                        var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                        if (!(await masterRepository.GetAll(x => x.Id == viewModel.DivisionId && x.ListId == listId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidDivision.ToString(CultureInfo.CurrentCulture));
                        }

                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidDivision.ToString(CultureInfo.CurrentCulture));
                    }



                    var repository = _unitOfWork?.GetRepository<OutletSupplierSetting>();
                    if ((await repository.GetAll(x => x.SupplierId == viewModel.SupplierId && x.StoreId == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.OutletSupplierDuplicate.ToString(CultureInfo.CurrentCulture));
                    }


                    var outletSupplier = _iAutoMapper.Mapping<OutletSupplierRequestModel, OutletSupplierSetting>(viewModel);

                    outletSupplier.CreatedById = userId;
                    outletSupplier.UpdatedById = userId;

                    var result = await repository.InsertAsync(outletSupplier).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        responseModel = await GetOuletSupplierById(result.Id).ConfigureAwait(false);
                    }
                    return responseModel;
                }
                throw new BadRequestException(ErrorMessages.TillInvalidReq.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
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

        /// <summary>
        /// Update Outlet Supplier.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="outletSupplierId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OutletSupplierResponseModel> Update(OutletSupplierRequestModel viewModel, int outletSupplierId, int userId)
        {
            OutletSupplierResponseModel responseModel = new OutletSupplierResponseModel();
            try
            {
                if (viewModel != null && outletSupplierId > 0)
                {

                    if (viewModel.StoreId > 0)
                    {
                        var genericRepository = _unitOfWork?.GetRepository<Store>();

                        if (!(await genericRepository.GetAll(x => x.Id == viewModel.StoreId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidStore.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.SupplierId > 0)
                    {
                        var supplierRepository = _unitOfWork?.GetRepository<Supplier>();

                        if (!(await supplierRepository.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidSupplierId.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.StateId > 0)
                    {

                        var masterListRepostiory = _unitOfWork?.GetRepository<MasterList>();

                        var listId = await masterListRepostiory.GetAll(x => x.Code == MasterListCode.State && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                        var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                        if (!(await masterRepository.GetAll(x => x.Id == viewModel.StateId && x.ListId == listId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidState.ToString(CultureInfo.CurrentCulture));
                        }

                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidState.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.DivisionId > 0)
                    {

                        var masterListRepostiory = _unitOfWork?.GetRepository<MasterList>();

                        var listId = await masterListRepostiory.GetAll(x => x.Code == MasterListCode.Division && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                        var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();
                        if (!(await masterRepository.GetAll(x => x.Id == viewModel.DivisionId && x.ListId == listId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.InvalidDivision.ToString(CultureInfo.CurrentCulture));
                        }

                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidDivision.ToString(CultureInfo.CurrentCulture));
                    }



                    var repository = _unitOfWork?.GetRepository<OutletSupplierSetting>();
                    if ((await repository.GetAll(x => x.SupplierId == viewModel.SupplierId && x.StoreId == viewModel.StoreId && x.Id != outletSupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.OutletSupplierDuplicate.ToString(CultureInfo.CurrentCulture));
                    }



                    var outletSupplierExist = await repository.GetAll(x => x.Id == outletSupplierId && !x.IsDeleted).FirstAsync().ConfigureAwait(false);
                    if (outletSupplierExist != null)
                    {
                        var outletSupplier = _iAutoMapper.Mapping<OutletSupplierRequestModel, OutletSupplierSetting>(viewModel);
                        outletSupplier.Id = outletSupplierId;
                        outletSupplier.CreatedAt = outletSupplierExist.CreatedAt;
                        outletSupplier.CreatedById = outletSupplierExist.CreatedById;
                        outletSupplier.UpdatedById = userId;

                        //Detaching tracked entry - exists
                        repository.DetachLocal(_ => _.Id == outletSupplier.Id);
                        repository.Update(outletSupplier);

                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                        responseModel = await GetOuletSupplierById(outletSupplierId).ConfigureAwait(false);
                        return responseModel;

                    }
                }
                throw new BadRequestException(ErrorMessages.InvalidOutletSuppier.ToString(CultureInfo.CurrentCulture));
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
        /// Delete an Outlet supplier.
        /// </summary>
        /// <param name="outletSupplierId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int outletSupplierId, int userId)
        {
            try
            {
                if (outletSupplierId > 0)
                {
                    var _repository = _unitOfWork?.GetRepository<OutletSupplierSetting>();
                    var outletSupplierExist = await _repository.GetAll(x => x.Id == outletSupplierId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (outletSupplierExist != null)
                    {
                        outletSupplierExist.UpdatedById = userId;
                        outletSupplierExist.IsDeleted = true;

                        _repository?.Update(outletSupplierExist);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceCustomException(ErrorMessages.OutletSupplierNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceCustomException(ErrorMessages.OutletSupplierId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.OutletSupplierNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

    }
}
