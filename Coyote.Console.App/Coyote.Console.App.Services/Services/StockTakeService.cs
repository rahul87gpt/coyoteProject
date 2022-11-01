using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services.Services
{
    public class StockTakeService : IStockTakeService
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMap;
        public StockTakeService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping)
        {
            _unitOfWork = unitOfWork;
            _iAutoMap = autoMapping;
        }

        /// <summary>
        /// Get all active Headers
        /// </summary>
        /// <returns>List<StockTakeHeaderResponseViewModel></returns>
        public async Task<PagedOutputModel<List<StockTakeHeaderResponseViewModel>>> GetAllActiveHeaders(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockTakeHeader>();

                int count = 0;
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

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
                    new SqlParameter("@AccessOutletIds", (StoreIds == true)?AccessStores:null),
                    new SqlParameter("@SkipCount", inputModel?.SkipCount),
                    new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                    new SqlParameter("@SortColumn", inputModel?.Sorting),
                    new SqlParameter("@SortDirection", inputModel?.Direction),
                    new SqlParameter("@IsLogged", inputModel?.IsLogged),
                    new SqlParameter("@Module","Stock"),
                    new SqlParameter("@RoleId",RoleId)

                  };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetStockTakeHeader, dbParams.ToArray()).ConfigureAwait(false);
                var listVM = MappingHelpers.ConvertDataTable<StockTakeHeaderResponseViewModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<StockTakeHeaderResponseViewModel>>(listVM, count);
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

        /// <summary>
        /// Add new header and details
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="userId"></param>
        /// <returns>StockTakeDetailResponseViewModel</returns>
        public async Task<StockTakeHeaderResponseViewModel> Insert(StockTakeHeaderRequestModel requestModel, int userId)
        {
            try
            {
                if (requestModel == null)
                    throw new NullReferenceException();

                var repository = _unitOfWork?.GetRepository<StockTakeHeader>();
                var storeRepo = _unitOfWork?.GetRepository<Store>();
                if (!await (storeRepo?.GetAll()?.Where(x => x.Id == requestModel.OutletId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false))
                {
                    throw new NotFoundException(ErrorMessages.OutletNotFound.ToString(CultureInfo.CurrentCulture));
                }
                if (await (repository?.GetAll()?.Where(x => x.OutletId == requestModel.OutletId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false))
                {
                    throw new AlreadyExistsException(ErrorMessages.StoreTakeHeaderDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                #region Comments
                //var header = _iAutoMap.Mapping<StockTakeHeaderRequestModel, StockTakeHeader>(requestModel);
                //header.IsDeleted = false;
                //header.CreatedById = userId;
                //header.UpdatedById = userId;
                //header.CreatedAt = DateTime.UtcNow;
                //header.UpdatedAt = DateTime.UtcNow;

                //// Insert child
                //var outletProdRepo = _unitOfWork?.GetRepository<OutletProduct>();
                //foreach (var childModel in requestModel.StockTakeDetail)
                //{
                //    if (!(await (outletProdRepo?.GetAll()?.Where(x => x.Id == childModel.OutletProductId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false)))
                //    {
                //        throw new NotFoundException(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture));
                //    }

                //    //handling first item 
                //    header.StockTakeDetails = header.StockTakeDetails ?? new List<StockTakeDetail>();
                //    var newChild = _iAutoMap.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(childModel);
                //    newChild.Id = 0;
                //    newChild.CreatedAt = DateTime.UtcNow;
                //    newChild.CreatedById = userId;
                //    newChild.UpdatedAt = DateTime.UtcNow;
                //    newChild.UpdatedById = userId;
                //    newChild.IsDeleted = false;
                //    header.StockTakeDetails.Add(newChild);
                //}
                //var result = await repository.InsertAsync(header).ConfigureAwait(false);
                //await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                #endregion
                var liParam = await InsertUpdateStockTake(requestModel, userId, 0).ConfigureAwait(false);
                if (liParam > 0)
                {
                    var returnModel = await GetStockTakeById(liParam).ConfigureAwait(false);
                    return returnModel;
                }
                else if (liParam == -3)
                {
                    throw new AlreadyExistsException(ErrorMessages.StoreTakeHeaderDuplicate.ToString(CultureInfo.CurrentCulture));
                }
                else
                {
                    throw new BadRequestException();
                }
            }
            catch (Exception ex)
            {
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

        private async Task<Int32> InsertUpdateStockTake(StockTakeHeaderRequestModel requestModel, int userId, Int64 stockTakeId)
        {
            //create xml for child items
            /***
             <StockTake><StockTakeDetail><OutletProductId>432547</OutletProductId><ProductId>101</ProductId><Description>gsgdyg</Description>
            <OnHandUnits>-2</OnHandUnits><Quantity>10</Quantity><ItemCost>2</ItemCost><ItemCount>10</ItemCount><VarQTY>8</VarQTY>
            <LineCost>20</LineCost><LineTotal>16</LineTotal></StockTakeDetail></StockTake> 
             ***/
            StringBuilder sbStockTakeDetailXml = new StringBuilder();
            #region Generate StockTakeDetailXml
            sbStockTakeDetailXml.Append("<StockTake>");
            foreach (var item in requestModel.StockTakeDetail)
            {
                var varQty = item.Quantity - item.OnHandUnits;
                sbStockTakeDetailXml.Append("<StockTakeDetail>");
                sbStockTakeDetailXml.Append("<OutletProductId>" + item.OutletProductId + "</OutletProductId>");
                sbStockTakeDetailXml.Append("<ProductId>" + item.ProductId + "</ProductId>");
                sbStockTakeDetailXml.Append("<Description>" + item.Desc + "</Description>");
                sbStockTakeDetailXml.Append("<OnHandUnits>" + item.OnHandUnits + "</OnHandUnits>");
                sbStockTakeDetailXml.Append("<Quantity>" + item.Quantity + "</Quantity>");
                sbStockTakeDetailXml.Append("<ItemCount>" + item.Quantity + "</ItemCount>");
                sbStockTakeDetailXml.Append("<ItemCost>" + item.ItemCost + "</ItemCost>");
                sbStockTakeDetailXml.Append("<VarQTY>" + varQty + "</VarQTY>");
                sbStockTakeDetailXml.Append("<LineCost>" + varQty * item.ItemCost + "</LineCost>");
                sbStockTakeDetailXml.Append("<LineTotal>" + item.Quantity * item.ItemCost + "</LineTotal>");
                sbStockTakeDetailXml.Append("</StockTakeDetail>");
            }
            sbStockTakeDetailXml.Append("</StockTake>");
            #endregion

            List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Id", stockTakeId),
                        new SqlParameter("@OutletId", requestModel.OutletId),
                        new SqlParameter("@Desc", requestModel?.Desc),
                        new SqlParameter("@PostToDate", requestModel?.PostToDate),
                        new SqlParameter("@Total", requestModel?.Total),
                        new SqlParameter("@StockTakeDetail", sbStockTakeDetailXml.ToString()),
                        new SqlParameter("@UserId", userId),

                    };
            var liParam = new SqlParameter("@liResult", Convert.ToInt32(0));
            liParam.Direction = ParameterDirection.Output;
            dbParams.Add(liParam);
            var repository = _unitOfWork?.GetRepository<StockTakeHeader>();
            await repository.ExecuteStoredProcedure(StoredProcedures.AddUpdateStockTakeHeader, dbParams.ToArray()).ConfigureAwait(false);
            return Convert.ToInt32(liParam.Value);
        }

        /// <summary>
        /// Update stock take header with details
        /// deletes items that are not in request but in DB
        /// updates items that are in request and DB
        /// inserts items that are in request but not in DB
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>StockTakeDetailResponseViewModel</returns>
        public async Task<StockTakeHeaderResponseViewModel> Update(StockTakeHeaderRequestModel requestModel, long id, int userId)
        {
            try
            {
                if (requestModel == null || id == 0)
                {
                    throw new NullReferenceException();
                }
                //var repository = _unitOfWork?.GetRepository<StockTakeHeader>();
                //var stockDetailRepo = _unitOfWork?.GetRepository<StockTakeDetail>();
                var storeRepo = _unitOfWork?.GetRepository<Store>();

                if (!(await (storeRepo?.GetAll()?.Where(x => x.Id == requestModel.OutletId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false)))
                {
                    throw new NotFoundException(ErrorMessages.OutletNotFound.ToString(CultureInfo.CurrentCulture));
                }
                #region Comments
                //var stockTakeHeader = await (repository?.GetAll()?.Include(d => d.StockTakeDetails)?.Where(x => x.Id == id && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                //if (stockTakeHeader == null)
                //{
                //    throw new NullReferenceException(ErrorMessages.StockTakeDetailsNotFound.ToString(CultureInfo.CurrentCulture));
                //}
                //stockTakeHeader.OutletId = requestModel?.OutletId ?? stockTakeHeader.OutletId;
                //stockTakeHeader.Desc = requestModel?.Desc ?? stockTakeHeader.Desc;
                //stockTakeHeader.PostToDate = requestModel?.PostToDate ?? stockTakeHeader.PostToDate;
                //stockTakeHeader.Total = requestModel?.Total ?? stockTakeHeader.Total;

                //#region Delete Children
                //// Delete children
                //if (stockTakeHeader.StockTakeDetails?.Count > 0)
                //{
                //    foreach (var existingChild in stockTakeHeader.StockTakeDetails.ToList())
                //    {
                //        var child = requestModel.StockTakeDetail.FirstOrDefault(c => c.OutletProductId == existingChild.OutletProductId);
                //        if (child == null)
                //        {
                //            //item is deleted
                //            existingChild.UpdatedAt = DateTime.UtcNow;
                //            existingChild.UpdatedById = userId;
                //            existingChild.IsDeleted = true;
                //            stockDetailRepo.Update(existingChild);
                //        }
                //    }
                //}
                //#endregion
                //repository.Update(stockTakeHeader);

                //#region Update/Insert Children
                //// Update and Insert children
                //var outletProdRepo = _unitOfWork?.GetRepository<OutletProduct>();
                //foreach (var childModel in requestModel.StockTakeDetail)
                //{
                //    if (!(await (outletProdRepo?.GetAll()?.Where(x => x.Id == childModel.OutletProductId && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false)))
                //    {
                //        throw new NotFoundException(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture));
                //    }

                //    var stockTakeDetails = stockTakeHeader.StockTakeDetails?.Where(c => c.OutletProductId == childModel.OutletProductId && !c.IsDeleted).FirstOrDefault();
                //    if (stockTakeDetails != null)
                //    {
                //        // Update child
                //        var updItem = _iAutoMap.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(childModel);
                //        updItem.CreatedAt = stockTakeDetails.CreatedAt;
                //        updItem.CreatedById = stockTakeDetails.CreatedById;
                //        updItem.UpdatedAt = DateTime.UtcNow;
                //        updItem.UpdatedById = userId;
                //        updItem.IsDeleted = false;
                //        updItem.StockTakeHeaderId = stockTakeDetails.StockTakeHeaderId;
                //        updItem.Id = stockTakeDetails.Id;
                //        stockDetailRepo.DetachLocal(_ => _.Id == stockTakeDetails.Id);
                //        stockDetailRepo.Update(updItem);
                //    }
                //    else
                //    {
                //        // Insert child
                //        //handling first item 
                //        var newChild = _iAutoMap.Mapping<StockTakeDetailRequestModel, StockTakeDetail>(childModel);
                //        newChild.VarQty = newChild.Quantity - newChild.OnHandUnits;
                //        newChild.LineCost = (float)Math.Round(newChild.VarQty * Math.Round(newChild.ItemCost, 2, MidpointRounding.AwayFromZero), 2, MidpointRounding.AwayFromZero);
                //        newChild.Id = 0;
                //        newChild.CreatedAt = DateTime.UtcNow;
                //        newChild.CreatedById = userId;
                //        newChild.UpdatedAt = DateTime.UtcNow;
                //        newChild.UpdatedById = userId;
                //        newChild.IsDeleted = false;
                //        newChild.StockTakeHeaderId = id;
                //        await stockDetailRepo.InsertAsync(newChild).ConfigureAwait(false);
                //    }
                //}
                //#endregion
                //await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                #endregion

                var liParam = await InsertUpdateStockTake(requestModel, userId, id).ConfigureAwait(false);
                if (liParam > 0)
                {
                    var returnModel = await GetStockTakeById(liParam).ConfigureAwait(false);
                    return returnModel;
                }
                else if (liParam == -3)
                {
                    throw new AlreadyExistsException(ErrorMessages.StoreTakeHeaderDuplicate.ToString(CultureInfo.CurrentCulture));
                }
                else
                {
                    throw new BadRequestException();
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
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get header and Details by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<StockTakeHeaderResponseViewModel> GetStockTakeById(long id)
        {
            try
            {
                #region Comments
                //var repoStockTakeDetail = _unitOfWork?.GetRepository<StockTakeDetail>();
                //var header = await (repository?.GetAll()?
                //    .Include(s => s.Store)?
                //    .Where(x => x.Id == id && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                //if (header == null)
                //    throw new NotFoundException(ErrorMessages.StockTakeHeaderNotFound.ToString(CultureInfo.CurrentCulture));

                //var headerVM = MappingHelpers.CreateMap(header);

                //var StockDetail = repoStockTakeDetail.GetAll()?
                //    .Include(pm => pm.OutletProduct)?
                //    .ThenInclude(p => p.Product)?.Where(x => x.StockTakeHeaderId == id && !x.IsDeleted)
                //    .OrderBy(x => x.OutletProduct.Product.Number).Select(MappingHelpers.CreateMap).ToList();

                //headerVM?.StockDetail.AddRange(StockDetail);
                //return headerVM;
                #endregion

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Id", id),
                    };
                var liParam = new SqlParameter("@liResult", Convert.ToInt32(0));
                liParam.Direction = ParameterDirection.Output;
                dbParams.Add(liParam);
                var repository = _unitOfWork?.GetRepository<StockTakeHeader>();
                var dataset = await repository.ExecuteStoredProcedure(StoredProcedures.GetStockTakeById, dbParams.ToArray()).ConfigureAwait(false);
                if (Convert.ToInt32(liParam.Value) > 0 && dataset.Tables.Count == 2)
                {
                    var stockTakeHeader = (MappingHelpers.ConvertDataTable<StockTakeHeaderResponseViewModel>(dataset.Tables[0])).First();
                    stockTakeHeader.StockDetail.AddRange(MappingHelpers.ConvertDataTable<StockTakeDetailResponseModel>(dataset.Tables[1]));
                    return stockTakeHeader;
                }
                else
                {
                    throw new Exception(ErrorMessages.StockTakeHeaderNotFound.ToString(CultureInfo.CurrentCulture));
                }
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
        /// Delete Header with details 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteHeader(long id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockTakeHeader>();
                var exists = await (repository?.GetAll()?.Include(d => d.StockTakeDetails)?.Where(x => x.Id == id && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (exists == null)
                    throw new NotFoundException(ErrorMessages.StockTakeHeaderNotFound.ToString(CultureInfo.CurrentCulture));
                exists.IsDeleted = true;
                exists.UpdatedById = userId;
                foreach (var existingChild in exists.StockTakeDetails.ToList())
                {
                    //item is deleted
                    existingChild.UpdatedAt = DateTime.UtcNow;
                    existingChild.UpdatedById = userId;
                    existingChild.IsDeleted = true;
                }
                repository?.Update(exists);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return true;
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
                var repository = _unitOfWork?.GetRepository<StockTakeDetail>();
                var exists = await (repository?.GetAll()?.Where(x => x.StockTakeHeaderId == id && x.Id == itemId && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (exists == null)
                    throw new NullReferenceException(ErrorMessages.StockTakeDetailItemNotFound.ToString(CultureInfo.CurrentCulture));

                exists.IsDeleted = true;
                exists.UpdatedById = userId;
                repository?.Update(exists);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                if (!await (repository?.GetAll()?.Where(x => x.StockTakeHeaderId == id && !x.IsDeleted)?.AnyAsyncSafe()).ConfigureAwait(false))
                {
                    //remove header
                    await DeleteHeader(id, userId).ConfigureAwait(false);
                }
                return true;
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
        /// Get all active Headers
        /// </summary>
        /// <returns>List<StockTakeHeaderResponseViewModel></returns>
        public async Task<PagedOutputModel<List<StockTakeHeaderResponseViewModel>>> GetActiveHeaders(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<StockTakeHeader>();
                //var list = repository.GetAll(x => !x.IsDeleted, null, includes:
                //    new Expression<Func<StockTakeHeader, object>>[] { c => c.StockTakeDetails, c => c.Store });
                var list = repository.GetAll()?.Include(x => x.StockTakeDetails)?.Include(x => x.Store)?.Where(x => !x.IsDeleted);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.OutletId.ToString().ToLower().Equals(inputModel.GlobalFilter.ToLower()));

                    //list = list.Where(x => x.Store.Code.Contains(inputModel.GlobalFilter.ToLower()) || x.Store.Desc.Contains(inputModel.GlobalFilter.ToLower()));

                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                        list = list.Where(x => securityViewModel.StoreIds.Contains(x.OutletId));

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
                List<StockTakeHeaderResponseViewModel> listVM;
                listVM = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();
                return new PagedOutputModel<List<StockTakeHeaderResponseViewModel>>(listVM, count);
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

        public async Task<byte[]> GetExternalStockTakeFile(int storeId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@OutletId", storeId)
                  };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetExternalStockTake, dbParams.ToArray()).ConfigureAwait(false);

                //will be extracted from Paths later
                string timeStamp = DateTime.Now.ToFileTimeUtc().ToString();
                string textPath = $"{Directory.GetCurrentDirectory()}\\Resources\\ExternalStockTake\\{DateTime.Now.ToFileTimeUtc().ToString()}";

                StreamWriter swExtStockFile = new StreamWriter(textPath, true);

                swExtStockFile.Write(Environment.NewLine);
                if (dset.Tables.Count > 0)
                {
                    foreach (DataRow dr in dset.Tables[0].Rows)
                    {
                        swExtStockFile.WriteLine(dr.ItemArray[0].ToString());
                    }
                }

                if (dset.Tables.Count > 1)
                {
                    foreach (DataRow dr in dset.Tables[1].Rows)
                    {
                        foreach (var item in dr.ItemArray)
                            swExtStockFile.WriteLine(item.ToString());
                    }
                }

                swExtStockFile.Flush();
                swExtStockFile.Close();


                byte[] textFile;
                if (File.Exists(textPath))
                {
                    textFile = System.IO.File.ReadAllBytes(textPath);
                    return textFile;
                }
                throw new NotFoundException(ErrorMessages.StockTakeDetailsNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<PagedOutputModel<List<StockTakeTabletResponseModel>>> TabletLoad(StockTakeTabletRequestModel requestModel)
        {
            try
            {
                if (requestModel != null)
                {
                    var repoBulkOrderFromTablet = _unitOfWork?.GetRepository<BulkOrderFromTablet>();

                    if (requestModel.OutletId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                      new SqlParameter("@OutletId", requestModel.OutletId),
                      new SqlParameter("@Number", requestModel.OutletNo),
                      new SqlParameter("@Type", CommonMessages.StockTabletLoad)
                    };

                    var dset = await repoBulkOrderFromTablet.ExecuteStoredProcedure(StoredProcedures.GetStockTakeTabletLoad, dbParams.ToArray()).ConfigureAwait(false);
                    List<StockTakeTabletResponseModel> StockTakeTabletViewModel = MappingHelpers.ConvertDataTable<StockTakeTabletResponseModel>(dset.Tables[0]);
                    var count = 0;
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                    return new PagedOutputModel<List<StockTakeTabletResponseModel>>(StockTakeTabletViewModel, count);
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception ex)
            {
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
        public async Task<StockTakeHeaderResponseViewModel> LoadProductRange(StockTakeLoadProdRangeRequestModel request)
        {
            try
            {
                if (request == null)
                    throw new NullReferenceException();

                var repoBulkOrderFromTablet = _unitOfWork?.GetRepository<BulkOrderFromTablet>();

                if (request.OutLetId == 0)
                {
                    throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));
                }

                var returnModel = await GetStockTakeById(request.Id).ConfigureAwait(false);

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@outletId", request.OutLetId),
                        new SqlParameter("@productStartId", request?.ProductStartId),
                        new SqlParameter("@productEndId", request?.ProductEndId),
                        new SqlParameter("@tillId", request?.TillId),
                        new SqlParameter("@dayRange", request?.DayRange),
                        new SqlParameter("@departmentIds", request?.DepartmentIds),
                        new SqlParameter("@commodityIds", request?.CommodityIds),
                        new SqlParameter("@categoryIds", request?.CategoryIds),
                        new SqlParameter("@groupIds", request?.GroupIds),
                        new SqlParameter("@suppliersIds", request?.SupplierIds),
                        new SqlParameter("@manufacturerIds", request?.ManufacturerIds),
                        new SqlParameter("@productIds", request?.ProductIds),
                        //page size
                        //StockTakeId
                        //UserId
                    };

                var dset = await repoBulkOrderFromTablet.ExecuteStoredProcedure(StoredProcedures.GetStockTakeLoadProductRange, dbParams.ToArray()).ConfigureAwait(false);
                var stockTakeLoadProdViewModel = MappingHelpers.ConvertDataTable<StockTakeDetailResponseModel>(dset.Tables[0]);

                returnModel.StockDetail.AddRange(stockTakeLoadProdViewModel);
                return returnModel;
            }
            catch (Exception ex)
            {
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
        public async Task<StockTakeHeaderResponseViewModel> Refresh(long Id, int userId)
        {
            try
            {
                var returnModel = await GetStockTakeById(Id).ConfigureAwait(false);
                if (returnModel == null)
                {
                    throw new NullReferenceException(ErrorMessages.StockTakeHeaderNotFound.ToString(CultureInfo.CurrentCulture));
                }

                var stockDetailRepo = _unitOfWork?.GetRepository<StockTakeDetail>();

                #region Comments
                //var repository = _unitOfWork?.GetRepository<OutletProduct>();
                //foreach (var model in returnModel.StockDetail)
                //{
                //    var outletProducts = await repository.GetAll().Where(x => x.Id == model.OutletProductId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                //    if (outletProducts.Status == true)
                //    {
                //        var stockDetails = await stockDetailRepo.GetAll().Where(x => x.Id == model.Id && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                //        int calQty = model.Quantity - (int)outletProducts.QtyOnHand;
                //        if (stockDetails != null)
                //        {
                //            if (model.OnHandUnits != outletProducts.QtyOnHand || model.Quantity != calQty)
                //            {
                //                stockDetails.OnHandUnits = (int)outletProducts.QtyOnHand;
                //                stockDetails.Quantity = calQty;
                //                stockDetails.LineTotal = calQty * stockDetails.ItemCost;
                //                stockDetails.UpdatedAt = DateTime.UtcNow;
                //                stockDetails.UpdatedById = userId;
                //                stockDetailRepo.Update(stockDetails);
                //            }
                //        }
                //    }
                //}
                //await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                #endregion


                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@StockTakeHeaderId", Id),
                        new SqlParameter("@UserId", userId),
                    };
                await stockDetailRepo.ExecuteStoredProcedure(StoredProcedures.RefreshStockTakeDetails, dbParams.ToArray()).ConfigureAwait(false);

                return await GetStockTakeById(Id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
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
