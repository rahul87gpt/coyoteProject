using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml;
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
    public class StockAdjustService : IStockAdjustService
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMap;
        public StockAdjustService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMap = autoMapping;
        }
        /// <summary>
        /// Get All active headers
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<StockAdjustHeaderResponseViewModel>>> GetAllActiveHeaders(StockAdjustFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockAdjustHeader>();
                int count = 0;
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

                List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                    new SqlParameter("@OutletId", inputModel?.StoreId),
                    new SqlParameter("@SkipCount", inputModel?.SkipCount),
                    new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                    new SqlParameter("@SortColumn", inputModel?.Sorting),
                    new SqlParameter("@SortDirection", inputModel?.Direction),
                    new SqlParameter("@IsLogged", inputModel?.IsLogged),
                    new SqlParameter("@Module","Stock"),
                    new SqlParameter("@RoleId",RoleId)
                  };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetStockAdjustHeader, dbParams.ToArray()).ConfigureAwait(false);
                var listVM = MappingHelpers.ConvertDataTable<StockAdjustHeaderResponseViewModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<StockAdjustHeaderResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StockAdjustHeaderNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Add new header and details
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<StockAdjustHeaderResponseViewModel> Insert(StockAdjustHeaderRequestModel requestModel, int userId)
        {
            try
            {
                if (requestModel == null)
                    throw new NullReferenceException();

                var repository = _unitOfWork?.GetRepository<StockAdjustHeader>();
                var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                var storeRepo = _unitOfWork?.GetRepository<Store>();

                if (!(await (storeRepo?.GetAll()?.Where(x => x.Id == requestModel.OutletId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false)))
                    throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));

                var header = _iAutoMap.Mapping<StockAdjustHeaderRequestModel, StockAdjustHeader>(requestModel);
                header.IsDeleted = false;
                header.CreatedById = userId;
                header.UpdatedById = userId;
                header.CreatedAt = DateTime.UtcNow;
                header.UpdatedAt = DateTime.UtcNow;

                // Insert child
                var prodRepo = _unitOfWork?.GetRepository<Product>();
                var outletProdRepo = _unitOfWork?.GetRepository<OutletProduct>();
                if (requestModel?.StockAdjustDetail != null && requestModel?.StockAdjustDetail.Count > 0)
                {
                    foreach (var childModel in requestModel.StockAdjustDetail)
                    {
                        if (!(await (listItemRepo?.GetAll()?.Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == childModel.ReasonId && x.MasterList.Code == "ADJUSTCODE").AnyAsyncSafe()).ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.StockAdjustReasonNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await (prodRepo?.GetAll()?.Where(x => x.Id == childModel.ProductId && !x.IsDeleted).AnyAsyncSafe()).ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await (outletProdRepo?.GetAll()?.Where(x => x.Id == childModel.OutletProductId && !x.IsDeleted).AnyAsyncSafe()).ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        //handling first item 
                        header.StockAdjustDetails = header.StockAdjustDetails ?? new List<StockAdjustDetail>();
                        var newChild = _iAutoMap.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(childModel);
                        newChild.Id = 0;
                        newChild.CreatedAt = DateTime.UtcNow;
                        newChild.CreatedById = userId;
                        newChild.UpdatedAt = DateTime.UtcNow;
                        newChild.UpdatedById = userId;
                        newChild.IsDeleted = false;
                        header.StockAdjustDetails.Add(newChild);
                    }
                }
                var result = await repository.InsertAsync(header).ConfigureAwait(false);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                if (requestModel.Total == (requestModel.ConfirmTotal ?? 0))
                {
                    //IF confirm total = waste total then finish the batch (post batch to trx table and delete from Stock Adjustment entry)
                    if (!await FinishStockAdjustTrans(result.Id, userId).ConfigureAwait(false))
                        return new StockAdjustHeaderResponseViewModel();
                }

                return await GetStockAdjustById(result.Id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //Rollback 
                _unitOfWork.Dispose();
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Update stock adjustment header with details
        /// deletes items that are not in request but in DB
        /// updates items that are in request and DB
        /// inserts items that are in request but not in DB
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<StockAdjustHeaderResponseViewModel> Update(StockAdjustHeaderRequestModel requestModel, long id, int userId)
        {
            try
            {
                if (requestModel == null)
                    throw new NullReferenceException();

                var repository = _unitOfWork?.GetRepository<StockAdjustHeader>();
                var stockDetailRepo = _unitOfWork?.GetRepository<StockAdjustDetail>();
                var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                var storeRepo = _unitOfWork?.GetRepository<Store>();
                var prodRepo = _unitOfWork?.GetRepository<Product>();
                var outletProdRepo = _unitOfWork?.GetRepository<OutletProduct>();

                var stockAdjustHeader = await (repository?.GetAll().Include(d => d.StockAdjustDetails)?.Where(x => x.Id == id && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (stockAdjustHeader == null)
                    throw new NullReferenceException(ErrorMessages.StockAdjustHeaderNotFound.ToString(CultureInfo.CurrentCulture));

                if (!(await (storeRepo?.GetAll()?.Where(x => x.Id == requestModel.OutletId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false)))
                {
                    throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
                }

                stockAdjustHeader.OutletId = requestModel?.OutletId ?? stockAdjustHeader.OutletId;
                stockAdjustHeader.PostToDate = requestModel?.PostToDate ?? stockAdjustHeader.PostToDate;
                stockAdjustHeader.Reference = requestModel?.Reference ?? stockAdjustHeader.Reference;
                stockAdjustHeader.Total = requestModel?.Total ?? stockAdjustHeader.Total;
                stockAdjustHeader.Description = requestModel?.Description ?? stockAdjustHeader.Description;
                stockAdjustHeader.UpdatedAt = DateTime.UtcNow;
                stockAdjustHeader.UpdatedById = userId;
                stockAdjustHeader.IsDeleted = false;


                if (requestModel?.StockAdjustDetail != null && requestModel?.StockAdjustDetail.Count > 0)
                {
                    #region Delete Children
                    // Delete children
                    if (stockAdjustHeader?.StockAdjustDetails?.Count > 0)
                    {
                        foreach (var existingChild in stockAdjustHeader.StockAdjustDetails.ToList())
                        {
                            var child = requestModel.StockAdjustDetail.FirstOrDefault(c => c.ProductId == existingChild.ProductId && c.OutletProductId == existingChild.OutletProductId);
                            if (child == null)
                            {
                                //item is deleted
                                existingChild.UpdatedAt = DateTime.UtcNow;
                                existingChild.UpdatedById = userId;
                                existingChild.IsDeleted = true;
                                stockDetailRepo.Update(existingChild);
                            }
                        }
                    }
                    #endregion
                    repository.Update(stockAdjustHeader);
                    #region Update/Insert Children
                    // Update and Insert children
                    foreach (var childModel in requestModel.StockAdjustDetail)
                    {
                        if (!(await (listItemRepo?.GetAll().Include(x => x.MasterList)?.Where(x => x.Id == childModel.ReasonId && !x.IsDeleted && x.MasterList.Code == "ADJUSTCODE")?.AnyAsyncSafe()).ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.StockAdjustReasonNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await (prodRepo?.GetAll()?.Where(x => x.Id == childModel.ProductId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await (outletProdRepo?.GetAll()?.Where(x => x.Id == childModel.OutletProductId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }

                        var existingChild = stockAdjustHeader.StockAdjustDetails?.Where(c => c.ProductId == childModel.ProductId && c.OutletProductId == childModel.OutletProductId && !c.IsDeleted).SingleOrDefault();
                        if (existingChild != null)
                        {
                            // Update child
                            var updItem = _iAutoMap.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(childModel);
                            updItem.CreatedAt = existingChild.CreatedAt;
                            updItem.CreatedById = existingChild.CreatedById;
                            updItem.UpdatedAt = DateTime.UtcNow;
                            updItem.UpdatedById = userId;
                            updItem.IsDeleted = false;
                            updItem.StockAdjustHeaderId = existingChild.StockAdjustHeaderId;
                            updItem.Id = existingChild.Id;
                            stockDetailRepo.DetachLocal(_ => _.Id == existingChild.Id);
                            stockDetailRepo.Update(updItem);
                        }
                        else
                        {
                            // Insert child
                            //handling first item                       
                            var newChild = _iAutoMap.Mapping<StockAdjustDetailRequestModel, StockAdjustDetail>(childModel);
                            newChild.Id = 0;
                            newChild.CreatedAt = DateTime.UtcNow;
                            newChild.CreatedById = userId;
                            newChild.UpdatedAt = DateTime.UtcNow;
                            newChild.UpdatedById = userId;
                            newChild.IsDeleted = false;
                            newChild.StockAdjustHeaderId = stockAdjustHeader.Id;
                            await stockDetailRepo.InsertAsync(newChild).ConfigureAwait(false);
                        }
                    }

                    #endregion
                }
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                await FinishStockAdjustTrans(id, userId).ConfigureAwait(false);
                return await GetStockAdjustById(id).ConfigureAwait(false);
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
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get header and Details by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<StockAdjustHeaderResponseViewModel> GetStockAdjustById(long id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockAdjustHeader>();
                var header = await (repository?.GetAll()?.AsNoTracking()?
                                   .Include(c => c.Store)?
                                   .Include(d => d.StockAdjustDetails)?.ThenInclude(pm => pm.Product)
                                   .Include(d => d.StockAdjustDetails)?.ThenInclude(pm => pm.OutletProduct)
                                   .Include(d => d.StockAdjustDetails)?.ThenInclude(pm => pm.Reason)
                                   .Where(x => x.Id == id && !x.IsDeleted)?
                                   .FirstOrDefaultAsyncSafe()).ConfigureAwait(false);

                if (header == null)
                    throw new NotFoundException(ErrorMessages.StockAdjustHeaderNotFound.ToString(CultureInfo.CurrentCulture));

                var headerVM = MappingHelpers.CreateMap(header);
                var StockDetail = (header.StockAdjustDetails.Where(x => !x.IsDeleted).ToList()).Select(MappingHelpers.CreateMap).ToList();
                if (StockDetail != null)
                    headerVM.StockDetail.AddRange(StockDetail);
                return headerVM;
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
        /// Delete Header with details 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteHeader(long id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockAdjustHeader>();
                var exists = await (repository?.GetAll()?.Include(d => d.StockAdjustDetails)?.Where(x => x.Id == id && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (exists == null)
                    throw new NullReferenceException(ErrorMessages.StockAdjustHeaderNotFound.ToString(CultureInfo.CurrentCulture));

                exists.IsDeleted = true;
                exists.UpdatedById = userId;
                foreach (var existingChild in exists.StockAdjustDetails.ToList())
                {
                    //item is deleted
                    existingChild.UpdatedAt = DateTime.UtcNow;
                    existingChild.UpdatedById = userId;
                    existingChild.IsDeleted = true;
                }
                repository?.Update(exists);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return true;
                //if (await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false))
                //    return true;
                //throw new NullReferenceException(ErrorMessages.NotAcceptedOrCreated.ToString(CultureInfo.CurrentCulture));
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

        /// <summary>
        /// Delete line item from header
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteHeaderDetailItem(long id, long itemId, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockAdjustDetail>();
                var exists = await (repository?.GetAll().Where(x => x.StockAdjustHeaderId == id && x.Id == itemId && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (exists == null)
                    throw new NullReferenceException(ErrorMessages.StockAdjustDetailItemNotFound.ToString(CultureInfo.CurrentCulture));

                exists.IsDeleted = true;
                exists.UpdatedById = userId;
                repository?.Update(exists);
                var result = await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                if (!await (repository?.GetAll()?.Where(x => x.StockAdjustHeaderId == id && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false))
                {
                    //remove header
                    await DeleteHeader(id, userId).ConfigureAwait(false);
                }
                return true;
                //if (result)
                //    return true;
                //throw new NullReferenceException(ErrorMessages.NotAcceptedOrCreated.ToString(CultureInfo.CurrentCulture));
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

        private async Task<bool> FinishStockAdjustTrans(long id, int userId)
        {
            try
            {
                var repoAdjustHeader = _unitOfWork?.GetRepository<StockAdjustHeader>();
                var repoMasterListItems = _unitOfWork?.GetRepository<MasterListItems>();
                var repoTrans = _unitOfWork?.GetRepository<Transaction>();
                var repoTill = _unitOfWork?.GetRepository<Till>();
                var stockAdjust = await (repoAdjustHeader?.GetAll()?.Include(d => d.Store)
                                 .Include(d => d.StockAdjustDetails).ThenInclude(pm => pm.Product)
                                 .Include(d => d.StockAdjustDetails).ThenInclude(pm => pm.OutletProduct)
                                 .Include(d => d.StockAdjustDetails).ThenInclude(pm => pm.Reason)
                                 .Where(x => x.Id == id && !x.IsDeleted)
                                 .FirstOrDefaultAsyncSafe()).ConfigureAwait(false);

                if (stockAdjust != null && stockAdjust?.StockAdjustDetails?.Count > 0)
                {
                    var totalAmount = stockAdjust?.StockAdjustDetails?.Where(x => !x.IsDeleted).Sum(x => x.LineTotal) ?? 0;
                    if (totalAmount == stockAdjust.Total)
                    {
                        var gstPerc = repoMasterListItems?.GetAll().Include(x => x.MasterList)?.FirstOrDefault(x => !x.IsDeleted && x.Code == "GST" && x.MasterList.Code == "TAXCODE")?.Num1 ?? 0;
                        var stockAdjustDetails = stockAdjust.StockAdjustDetails.Where(x => !x.IsDeleted);
                        foreach (var obj in stockAdjustDetails)
                        {
                            float unitQty = 1;
                            float cartonQty = 1;
                            if (obj.Product.UnitQty != 0)
                                unitQty = obj.Product.UnitQty;
                            if (obj.Product.CartonQty != 0)
                                cartonQty = obj.Product.CartonQty;
                            var trans = new Transaction
                            {
                                Date = DateTime.UtcNow.Date,
                                Type = "ADJUSTMENT",
                                ProductId = obj.ProductId,
                                OutletId = stockAdjust.OutletId,
                                TillId = repoTill.GetAll().Where(x => !x.IsDeleted && x.Code == "12312").FirstOrDefault()?.Id, //discussion
                                Sequence = 10,
                                SupplierId = obj.Product.SupplierId,
                                ManufacturerId = obj.Product.ManufacturerId,
                                Group = obj.Product.GroupId,
                                DepartmentId = obj.Product.DepartmentId,
                                CommodityId = obj.Product.CommodityId,
                                CategoryId = obj.Product.CategoryId,
                                SubRange = 0,//discussion
                                Reference = stockAdjust.Reference,
                                UserId = userId,
                                Qty = obj.Quantity,
                                Amt = 0,
                                ExGSTAmt = 0,
                                Cost = obj.LineTotal,
                                ExGSTCost = GenericFunction.GSTCalculation(obj.LineTotal, Convert.ToSingle(gstPerc)), //discussion
                                Discount = 0,
                                Price = 0,
                                PromoSellId = null,
                                PromoBuyId = null,
                                Weekend = DateTime.UtcNow.EndOfWeek(DayOfWeek.Monday),
                                Day = DateTime.UtcNow.ToString("ddd"),
                                NewOnHand = obj.OutletProduct.QtyOnHand + (obj.Quantity * unitQty),
                                Member = 0,
                                Points = 0,
                                CartonQty = cartonQty,
                                UnitQty = unitQty,
                                Parent = obj?.Product?.Parent ?? null,
                                StockMovement = (obj.Quantity * unitQty),
                                Tender = repoMasterListItems?.GetAll().Where(x => x.Id == obj.ReasonId)?.FirstOrDefault()?.Code ?? null,
                                ManualInd = false,
                                GLAccount = null,
                                GLPostedInd = false,
                                PromoSales = 0,
                                PromoSalesGST = 0,
                                Debtor = 0,
                                Flags = null,
                                ReferenceType = null,
                                ReferenceNumber = null,
                                TermsRebateCode = null,
                                TermsRebate = 0,
                                ScanRebateCode = null,
                                ScanRebate = 0,
                                PurchaseRebate = 0,
                                CreatedAt = DateTime.UtcNow,
                                CreatedById = userId,
                                UpdatedAt = DateTime.UtcNow,
                                UpdatedById = userId,
                                IsDeleted = false
                            };
                            await repoTrans.InsertAsync(trans).ConfigureAwait(false);

                        }

                        stockAdjust.IsDeleted = true;
                        stockAdjust.UpdatedById = userId;
                        stockAdjust.UpdatedAt = DateTime.UtcNow;
                        foreach (var obj in stockAdjust.StockAdjustDetails.ToList())
                        {
                            //item is deleted
                            obj.IsDeleted = true;
                            obj.UpdatedById = userId;
                            obj.UpdatedAt = DateTime.UtcNow;
                        }

                        repoAdjustHeader?.Update(stockAdjust);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                        return true;
                    }
                }
                return false;
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

        public async Task<PagedOutputModel<List<StockAdjustHeaderResponseViewModel>>> GetActiveHeaders(StockAdjustFilter inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockAdjustHeader>();
                var list = repository?.GetAll()?.AsNoTracking()?.Include(x => x.Store)?.Where(x => !x.IsDeleted);
                //var list = repository.GetAll(x => !x.IsDeleted, null, includes:
                //    new Expression<Func<StockAdjustHeader, object>>[] { c => c.Store });
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.Store.Code.Contains(inputModel.GlobalFilter.ToLower()) || x.Store.Desc.Contains(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.StoreId)))
                        list = list.Where(x => x.OutletId.ToString().ToLower().Equals(inputModel.StoreId.ToLower()));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    count = list.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Id);
                                else
                                    list = list.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                var listVM = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();
                return new PagedOutputModel<List<StockAdjustHeaderResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StockAdjustHeaderNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<int> GetReferenceNo()
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockAdjustHeader>();
                var list = await (repository?.GetAll().OrderByDescending(x => x.Id).FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (list != null && list?.Reference != null)
                    return Convert.ToInt32(list.Reference) + 1;
                else
                    return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
