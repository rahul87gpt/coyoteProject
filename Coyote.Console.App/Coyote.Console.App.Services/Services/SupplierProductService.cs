using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Models;
using Coyote.Console.App.Repository;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;
//using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services
{
    public class SupplierProductService : ISupplierProductService
    {
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLoggerManager = null;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IGenericRepository<SupplierProduct> _repository;

        public SupplierProductService(IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager, IUnitOfWork unitOfWork)
        {
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
            _iUnitOfWork = unitOfWork;
            _repository = _iUnitOfWork?.GetRepository<SupplierProduct>();
        }

        /// <summary>
        /// Get all active Supplier Products
        /// </summary>
        /// <returns>List<SupplierProductViewModel></returns>
        public async Task<PagedOutputModel<List<SupplierProductResponseViewModel>>> GetAllActiveSupplierProducts(SupplierProductFilter inputModel)
        {
            try
            {
                var supplierProducts = _repository.GetAll(x => !x.IsDeleted);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))

                        supplierProducts = supplierProducts.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.SupplierItem.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()));


                    if (!string.IsNullOrEmpty((inputModel?.SupplierId)))
                        supplierProducts = supplierProducts.Where(x => x.SupplierId.ToString().ToLower().Contains(inputModel.SupplierId.ToLower()));

                    supplierProducts = supplierProducts.OrderByDescending(x => x.UpdatedAt);
                    count = supplierProducts.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        supplierProducts = supplierProducts.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        supplierProducts = supplierProducts.Where(x => x.Status);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "supplieritem":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    supplierProducts = supplierProducts.OrderBy(x => x.SupplierItem);
                                else
                                    supplierProducts = supplierProducts.OrderByDescending(x => x.SupplierItem);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    supplierProducts = supplierProducts.OrderBy(x => x.Desc);
                                else
                                    supplierProducts = supplierProducts.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    supplierProducts = supplierProducts.OrderBy(x => x.Id);
                                else
                                    supplierProducts = supplierProducts.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                List<SupplierProductResponseViewModel> supplierViewModel;
                supplierViewModel = (await supplierProducts.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<SupplierProduct, SupplierProductResponseViewModel>).ToList();
                return new PagedOutputModel<List<SupplierProductResponseViewModel>>(supplierViewModel, count);
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(ErrorMessages.NoSupplierProductFound.ToString(CultureInfo.CurrentCulture), nfe);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoSupplierProductFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Supplier product using SP
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<SupplierProductResponseViewModel>>> GetAllSupplierProducts(SupplierProductFilter inputModel)
        {

            try
            {
                var repository = _iUnitOfWork?.GetRepository<SupplierProduct>();
                List<SqlParameter> dbParams = new List< SqlParameter>{
                     new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                     new SqlParameter("@SupplierId", inputModel?.SupplierId),
                     new SqlParameter("@SortColumn", inputModel?.Sorting),
                     new SqlParameter("@SortDirection", inputModel?.Direction),
                     new SqlParameter("@SkipCount", inputModel?.SkipCount),
                     new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount)
                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllSupplierProducts, dbParams.ToArray()).ConfigureAwait(false);
                List<SupplierProductResponseViewModel> responseModel =
                    MappingHelpers.ConvertDataTable<SupplierProductResponseViewModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<SupplierProductResponseViewModel>>(responseModel, count);
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
        /// Get products with specific Supplier Id
        /// </summary>
        /// <param name="supplierId">SupplierProduct Id</param>
        /// <returns>SupplierProductViewModel</returns>
        public async Task<List<SupplierProductResponseViewModel>> GetSupplierProductsById(long supplierId)
        {
            try
            {
                if (supplierId > 0)
                {
                    var supplierProducts = await _repository.GetAll(x => x.Id == supplierId && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);
                    var supplierViewModel = supplierProducts.Select(_iAutoMapper.Mapping<SupplierProduct, SupplierProductResponseViewModel>).ToList();
                    if (supplierViewModel == null)
                    {
                        throw new NotFoundException(ErrorMessages.SupplierProductNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    return supplierViewModel;
                }
                throw new NullReferenceCustomException(ErrorMessages.SupplierId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.SupplierProductNotFound, ex);
            }
        }

        /// <summary>
        /// To delete a Supplier Product
        /// </summary>
        /// <param name="supplierId">Supplier Id</param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteSupplierProduct(long supplierProdId, int userId)
        {
            try
            {
                if (supplierProdId > 0)
                {

                    var supplierProductExists = await _repository.GetAll(x => x.Id == supplierProdId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (supplierProductExists != null)
                    {
                        if (!supplierProductExists.IsDeleted)
                        {
                            supplierProductExists.UpdatedById = userId;
                            supplierProductExists.IsDeleted = true;
                            _repository?.Update(supplierProductExists);
                            await (_iUnitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.SupplierProductNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    throw new NullReferenceCustomException(ErrorMessages.SupplierProductNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceCustomException(ErrorMessages.SupplierProductId.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoSupplierProductFound.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<List<SupplierProductResponseViewModel>> Update(SupplierProductRequestModel viewModel, int id, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (id == 0)
                    {
                        throw new NullReferenceCustomException(ErrorMessages.SupplierProductId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                    var suppRepo = _iUnitOfWork.GetRepository<Supplier>();
                    if (!(await suppRepo.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await _repository.GetAll(x => x.SupplierId == viewModel.SupplierId && x.ProductId == viewModel.ProductId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.SupplierProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        var supplierProduct = _iAutoMapper.Mapping<SupplierProductRequestModel, SupplierProduct>(viewModel);
                        supplierProduct.Id = id;
                        supplierProduct.IsDeleted = false;
                        supplierProduct.CreatedAt = exists.CreatedAt;
                        supplierProduct.CreatedById = exists.CreatedById;
                        supplierProduct.UpdatedAt = DateTime.UtcNow;
                        supplierProduct.UpdatedById = userId;
                        _repository.DetachLocal(_ => _.Id == supplierProduct.Id);
                        _repository.Update(supplierProduct);
                        if (await (_iUnitOfWork?.SaveChangesAsync()).ConfigureAwait(false))
                            return await GetSupplierProductsById(id).ConfigureAwait(false);
                        throw new Exception();
                    }
                    else
                    {
                        throw new NotFoundException(ErrorMessages.SupplierProductNotFound.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                }
                else
                {
                    throw new BadRequestException(ErrorMessages.GenericViewModelMandatoryMessage.ToString(System.Globalization.CultureInfo.CurrentCulture));
                }
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (NotFoundException nfe)
            {
                throw new NotFoundException(nfe.Message, nfe);
            }
            catch (BadRequestException bre)
            {
                throw new BadRequestException(bre.Message, bre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoSupplierProductFound.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }

        public async Task<List<SupplierProductResponseViewModel>> Insert(SupplierProductRequestModel viewModel, int userId)
        {
            var responseModel = new List<SupplierProductResponseViewModel>();
            long resultId = 0;
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.SupplierId > 0)
                    {
                        if (viewModel.ProductId > 0)
                        {
                            if ((await _repository.GetAll(x => x.SupplierId == viewModel.SupplierId && x.ProductId == viewModel.ProductId && x.SupplierItem == viewModel.SupplierItem).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new AlreadyExistsException(ErrorMessages.SupplierProductDuplicate.ToString(CultureInfo.CurrentCulture));
                            }

                            var suppRepo = _iUnitOfWork.GetRepository<Supplier>();
                            if (!(await suppRepo.GetAll(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                            {
                                throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                            }

                            var supplierProduct = _iAutoMapper.Mapping<SupplierProductRequestModel, SupplierProduct>(viewModel);

                            supplierProduct.CreatedById = userId;
                            supplierProduct.UpdatedById = userId;
                            supplierProduct.IsDeleted = false;
                            supplierProduct.CreatedAt = DateTime.UtcNow;
                            supplierProduct.UpdatedAt = DateTime.UtcNow;
                            var result = await _repository.InsertAsync(supplierProduct).ConfigureAwait(false);
                            await (_iUnitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            if (result != null)
                            {
                                resultId = result.Id;
                            }

                            responseModel = await GetSupplierProductsById(resultId).ConfigureAwait(false);
                        }
                        else
                        {
                            throw new NullReferenceCustomException(ErrorMessages.SupplierProductId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        throw new NullReferenceCustomException(ErrorMessages.SupplierId.ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                }
                return responseModel;
            }
            catch (AlreadyExistsException aee)
            {
                throw new AlreadyExistsException(aee.Message, aee);
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (NotFoundException nf)
            {
                throw new NotFoundException(nf.Message, nf);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.NoSupplierProductFound.ToString(System.Globalization.CultureInfo.CurrentCulture), ex);
            }
        }
    }
}