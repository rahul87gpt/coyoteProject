using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Coyote.Console.App.Models;
using Coyote.Console.App.Models.EdiMetcashModels;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using EDIServiceReference;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Coyote.Console.App.Services.Services
{
    public class OrdersService : IOrdersService
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMap;
        private FileDirectorySettings fileDirectorySettings { get; set; }
        private LIONOrderSettings lIONOrderSettings { get; set; }
        private CocaColaOrderSettings cocaColaOrderSettings { get; set; }
        private DistributorOrderSettings distributorOrderSettings { get; set; }
        private SFTPSettings sFTPSettings { get; set; }
        public IEDIMetcashService EdiMetcashService { get; }

        private ISendMailService _sendEmailService = null;
        private ISFTPService _sFTPService;
        public OrdersService(IUnitOfWork unitOfWork, IAutoMappingServices iAutoMap, IOptions<LIONOrderSettings> lionOrderSettings, ISendMailService iSendEmailService, IOptions<CocaColaOrderSettings> cocaColaOptions, ISFTPService sFTPService, IOptions<SFTPSettings> sftpSettings, IOptions<DistributorOrderSettings> distributorOptions, IOptions<FileDirectorySettings> fileDirectory, IEDIMetcashService ediMetcashService)
        {
            _unitOfWork = unitOfWork;
            _iAutoMap = iAutoMap;
            lIONOrderSettings = lionOrderSettings?.Value;
            _sendEmailService = iSendEmailService;
            _sFTPService = sFTPService;
            EdiMetcashService = ediMetcashService;
            cocaColaOrderSettings = cocaColaOptions?.Value;
            sFTPSettings = sftpSettings?.Value;
            distributorOrderSettings = distributorOptions?.Value;
            fileDirectorySettings = fileDirectory?.Value;
        }

        /// <summary>
        /// Delete Order Header and its details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteOrder(long id, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OrderHeader>();
                var exists = await repository.GetAll().Include(d => d.OrderDetail).Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (exists == null)
                    throw new NullReferenceException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));

                exists.IsDeleted = true;
                exists.UpdatedById = userId;
                if (exists.OrderDetail != null)
                {
                    foreach (var existingChild in exists.OrderDetail.ToList())
                    {
                        //item is deleted
                        existingChild.UpdatedAt = DateTime.UtcNow;
                        existingChild.UpdatedById = userId;
                        existingChild.IsDeleted = true;
                    }
                }
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

        /// <summary>
        /// Delete order detail items and if items are 0 then delete header
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteOrderDetailItem(long id, long itemId, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OrderDetail>();
                var exists = await (repository?.GetAll()?.Where(x => x.OrderHeaderId == id && x.Id == itemId && !x.IsDeleted)?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false);
                if (exists == null)
                    throw new NullReferenceException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));
                exists.IsDeleted = true;
                exists.UpdatedById = userId;
                repository?.Update(exists);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                if (!await repository.GetAll().Where(x => x.OrderHeaderId == id && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                {
                    //remove header
                    await DeleteOrder(id, userId).ConfigureAwait(false);
                }
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

        /// <summary>
        /// Get allactive headers //generate query for Order headers and for invoice history
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="securityViewModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<OrderHeaderResponseViewModel>>> GetAllActiveOrderHeaders(OrderInvoiceRequestModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                if (inputModel == null)
                    inputModel = new OrderInvoiceRequestModel();

                var repository = _unitOfWork?.GetRepository<OrderHeader>();
                //var list = repository.GetAll(x => !x.IsDeleted, null, includes: new Expression<Func<OrderHeader, object>>[] { c => c.Store, c => c.Supplier, c => c.CreationType, c => c.Type, c => c.Status });
                var list = repository.GetAll().Include(x => x.Store).Include(x => x.Supplier).Include(x => x.Type).Include(x => x.Status).Include(x => x.CreationType)
                    .Where(x => !x.IsDeleted);

                if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                    list = list.Where(x => x.InvoiceNo.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Supplier.Desc.Contains(inputModel.GlobalFilter.ToLower()) || x.Supplier.Code.Contains(inputModel.GlobalFilter.ToLower())
                    || x.Store.Code.Contains(inputModel.GlobalFilter.ToLower()) || x.Store.Desc.Contains(inputModel.GlobalFilter.ToLower()) || x.CreationType.Code.Contains(inputModel.GlobalFilter.ToLower())
                    || x.CreationType.Name.Contains(inputModel.GlobalFilter.ToLower()) || x.Type.Name.Contains(inputModel.GlobalFilter.ToLower()) || x.Type.Code.Contains(inputModel.GlobalFilter.ToLower())
                    || x.Status.Name.Contains(inputModel.GlobalFilter.ToLower()) || x.Status.Code.Contains(inputModel.GlobalFilter.ToLower()));

                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                    list = list.Where(x => securityViewModel.StoreIds.Contains(x.OutletId));

                if (inputModel.ShowOrderHistory)
                {
                    //invoice history
                    var masterlistitemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                    if (!await (masterlistitemRepo?.GetAll()?.Include(x => x.MasterList)?.Where(x => !x.IsDeleted && x.Code == "NEW" && x.MasterList.Code == "OrderDocStatus")?.AnyAsyncSafe()).ConfigureAwait(false))
                        throw new NotFoundException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));
                    var orderDocId = (await (masterlistitemRepo?.GetAll()?.Include(x => x.MasterList)?.Where(x => !x.IsDeleted && x.Code == "NEW" && x.MasterList.Code == "OrderDocStatus")?.FirstOrDefaultAsyncSafe()).ConfigureAwait(false)).Id;

                    ////ORDH_DOCUMENT_STATUS != NEW 
                    list = list.Where(x => x.StatusId != orderDocId);

                    if (inputModel.UseInvoiceDates)
                    {
                        if (inputModel.InvoiceDateFrom.HasValue && inputModel.InvoiceDateTo.HasValue)
                        {
                            list = list.Where(x => x.InvoiceDate.Value.Date >= inputModel.InvoiceDateFrom.Value.Date && x.InvoiceDate.Value.Date <= inputModel.InvoiceDateTo.Value.Date);
                        }
                        else
                        {
                            throw new BadRequestException(ErrorMessages.OrderInvoiceDateRangeNotSelected.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    else
                    {
                        if (inputModel.OrderPostedDateFrom.HasValue && inputModel.OrderPostedDateTo.HasValue)
                        {
                            //Old system is including time of PostedDate
                            list = list.Where(x => x.PostedDate.Value >= inputModel.OrderPostedDateFrom.Value.Date && x.PostedDate.Value <= inputModel.OrderPostedDateTo.Value.Date);
                        }
                        else
                        {
                            throw new BadRequestException(ErrorMessages.OrderPostedDateRangeNotSelected.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    //if zone then select outlets in zone and apply filter
                    if (inputModel.Zones?.Count > 0)
                    {
                        var zoneRepo = _unitOfWork?.GetRepository<ZoneOutlet>();
                        var outletIds = zoneRepo?.GetAll()?.Where(x => inputModel.Zones.Contains(x.ZoneId))?.Select(x => x.StoreId).ToList();
                        //add to outlets and add distinct
                        if (inputModel.Outlets == null) { inputModel.Outlets = new List<int>(); }
                        inputModel.Outlets.AddRange(outletIds);
                        inputModel.Outlets = inputModel.Outlets.Distinct().ToList();
                    }
                    if (!string.IsNullOrEmpty(inputModel.ZoneIds))
                    {
                        var zoneRepo = _unitOfWork?.GetRepository<ZoneOutlet>();
                        var zones = inputModel.ZoneIds.Split(',');
                        var outletIds = zoneRepo?.GetAll()?.Where(x => zones.Contains(x.ZoneId.ToString()))?.Select(x => x.StoreId).ToList();
                        //add to outlets and add distinct
                        if (inputModel.Outlets == null) { inputModel.Outlets = new List<int>(); }
                        inputModel.Outlets.AddRange(outletIds);
                        inputModel.Outlets = inputModel.Outlets.Distinct().ToList();
                    }
                    if (inputModel.Outlets?.Count > 0)
                    {
                        list = list.Where(x => inputModel.Outlets.Contains(x.OutletId));
                    }
                    if (!string.IsNullOrEmpty(inputModel.StoreIds))
                    {
                        var stores = inputModel.StoreIds.Split(',');
                        list = list.Where(x => stores.Contains(x.OutletId.ToString()));
                    }
                    if (inputModel.Suppliers?.Count > 0)
                    {
                        list = list.Where(x => inputModel.Suppliers.Contains(Convert.ToInt32(x.SupplierId)));
                    }
                    if (!string.IsNullOrEmpty(inputModel.SupplierIds))
                    {
                        var supps = inputModel.SupplierIds.Split(',');
                        list = list.Where(x => supps.Contains(x.SupplierId.ToString()));
                    }
                    list = list.OrderBy(x => x.Supplier.Desc).ThenBy(x => x.Store.Code).ThenBy(x => x.OrderNo);
                }
                else
                {
                    //list = list.OrderByDescending(x => x.UpdatedAt);
                    //select * from ORDHTBL where ORDH_DOCUMENT_STATUS <> 'INVOICE' and ORDH_DOCUMENT_STATUS <> 'CREDIT' order by ORDH_SUPPLIER_NAME
                    var masterListRepo = _unitOfWork?.GetRepository<MasterList>();
                    var masterListItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                    var orderDocStatus = masterListRepo.GetAll().Where(x => x.Code == "OrderDocStatus").FirstOrDefault()?.Id;
                    var invoiceId = masterListItemRepo.GetAll().Where(x => x.ListId == orderDocStatus && x.Code == "INVOICE").FirstOrDefault()?.Id;
                    var creditId = masterListItemRepo.GetAll().Where(x => x.ListId == orderDocStatus && x.Code == "CREDIT").FirstOrDefault()?.Id;
                    list = list.Where(x => x.StatusId != invoiceId && x.StatusId != creditId);
                    list = list.OrderBy(x => x.Supplier.Desc);
                }

                int count = list?.Count() ?? 0;

                if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                    list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                if (!string.IsNullOrEmpty(inputModel.Sorting))
                {
                    switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                    {
                        default:
                            if (string.IsNullOrEmpty(inputModel.Direction))
                                list = list.OrderBy(x => x.UpdatedAt);
                            else
                                list = list.OrderByDescending(x => x.UpdatedAt);
                            break;
                    }
                }

                List<OrderHeaderResponseViewModel> listVM;
                listVM = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();
                return new PagedOutputModel<List<OrderHeaderResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new Exception(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get Active Order Header and Invoice History using SP.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="securityViewModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<OrderHeaderResponseViewModel>>> GetActiveOrderHeaders(OrderInvoiceRequestModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                if (inputModel == null)
                    inputModel = new OrderInvoiceRequestModel();

                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;

                var repository = _unitOfWork?.GetRepository<OrderHeader>();

                bool AccessStoreIds = false;
                var AccessStores = String.Empty;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                {
                    foreach (var storeId in securityViewModel.StoreIds)
                        AccessStores += storeId + ",";
                    AccessStoreIds = true;
                }

                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                     new SqlParameter("@SortColumn", inputModel?.Sorting),
                     new SqlParameter("@SortDirection", inputModel?.Direction),
                     new SqlParameter("@PageNumber", inputModel?.SkipCount),
                     new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                     new SqlParameter("@IsCountRequired",1),
                     new SqlParameter("@SkipCount",inputModel?.SkipCount),
                     new SqlParameter("@MaxResultCount",inputModel?.MaxResultCount),
                     new SqlParameter("@ShowHistory",inputModel?.ShowOrderHistory),
                     new SqlParameter("@UseInvoiceDates",inputModel?.UseInvoiceDates),
                     new SqlParameter("@InvoiceDateFrom",inputModel?.InvoiceDateFrom),
                     new SqlParameter("@InvoiceDateTo",inputModel?.InvoiceDateTo),
                     new SqlParameter("@PostedDateFrom",inputModel?.OrderPostedDateFrom),
                     new SqlParameter("@PostedDateTo",inputModel?.OrderPostedDateTo),
                     new SqlParameter("@AcessStoreIds", (AccessStoreIds == true)?AccessStores:null),
                     new SqlParameter("@StoreIds",inputModel?.StoreIds),
                     new SqlParameter("@SupplierIds",inputModel?.SupplierIds),
                     new SqlParameter("@ZoneIds",inputModel?.ZoneIds),
                     new SqlParameter("@MemberIds", inputModel?.MemberIds),
                     new SqlParameter("@IsLogged", inputModel?.IsLogged),
                     new SqlParameter("@Module","Order"),
                     new SqlParameter("@RoleId",RoleId)
                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllOrderHeaders, dbParams.ToArray()).ConfigureAwait(false);

                var count = 0;
                List<OrderHeaderResponseViewModel> listVM = new List<OrderHeaderResponseViewModel>();
                if (dset.Tables.Count > 0)
                {
                    listVM = MappingHelpers.ConvertDataTable<OrderHeaderResponseViewModel>(dset.Tables[0]);
                }

                if (dset.Tables.Count > 1)
                {
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                }

                return new PagedOutputModel<List<OrderHeaderResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                if (ex is BadRequestException)
                {
                    throw new Exception(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get header and details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OrderDetailResponseViewModel> GetOrderById(long id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OrderHeader>();

                var orderHeader = await repository.GetAll().Include(c => c.Store).Include(c => c.Supplier).Include(c => c.CreationType).Include(c => c.Type).Include(c => c.Status)
                         .Include(d => d.OrderDetail).ThenInclude(pm => pm.OrderType).Include(d => d.OrderDetail).ThenInclude(pm => pm.Product).Include(d => d.OrderDetail).ThenInclude(pm => pm.SupplierProduct)
                         .Include(d => d.OrderDetail).ThenInclude(p => p.Product).ThenInclude(t => t.Tax)
                         .Include(d => d.OrderDetail).ThenInclude(pm => pm.Supplier).Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                //var orderHeader = await repository.GetAll(x => x.Id == id && !x.IsDeleted, include: x => x.Include(c => c.Store).Include(c => c.Supplier).Include(c => c.CreationType).Include(c => c.Type).Include(c => c.Status)
                //                   .Include(d => d.OrderDetail).ThenInclude(pm => pm.OrderType).Include(d => d.OrderDetail).ThenInclude(pm => pm.Product).Include(d => d.OrderDetail).ThenInclude(pm => pm.SupplierProduct)
                //                   .Include(d => d.OrderDetail).ThenInclude(pm => pm.Supplier)).FirstOrDefaultAsync().ConfigureAwait(false);
                if (orderHeader == null)
                    throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));

                var orderVM = MappingHelpers.CreateMap(orderHeader);
                OrderDetailResponseViewModel orderDetailVM = new OrderDetailResponseViewModel
                {
                    OrderHeaders = orderVM,
                    OrderDetails = (orderHeader.OrderDetail.Where(x => !x.IsDeleted).ToList()).Select(MappingHelpers.CreateMap).ToList()
                };

                return orderDetailVM;
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
        /// Add new header and details
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OrderDetailResponseViewModel> Insert(OrderRequestModel viewModel, int userId)
        {
            try
            {
                long resultId = 0;
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<OrderHeader>();
                    var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                    var masterRepo = _unitOfWork?.GetRepository<MasterList>();
                    var storeRepo = _unitOfWork?.GetRepository<Store>();
                    // is OrderCreationType
                    if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == viewModel.CreationTypeId && x.MasterList.Code == MasterListCode.OrderCreationType).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.CreationTypeNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    // is OrderDocType
                    if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == viewModel.TypeId && x.MasterList.Code == MasterListCode.ORDERDOCTYPE).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.TypeNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        var orderType = await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == viewModel.TypeId && x.MasterList.Code == MasterListCode.ORDERDOCTYPE).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                        if (orderType.Name.ToLower() != "delivery" && !string.IsNullOrEmpty(viewModel.DeliveryNo))
                        {
                            if (!string.IsNullOrEmpty(viewModel.DeliveryNo) || viewModel.DeliveryDate != null)
                            {
                                throw new BadRequestException(ErrorMessages.OrderNotDelivery.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                        if (orderType.Name.ToLower() != "invoice" && !string.IsNullOrEmpty(viewModel.InvoiceNo))
                        {
                            if (!string.IsNullOrEmpty(viewModel.InvoiceNo) || viewModel.InvoiceDate != null || viewModel.InvoiceTotal != null)
                            {
                                throw new BadRequestException(ErrorMessages.OrderNotInvoice.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                        /**
                         * ORder Doc Type == Transfer
                         * for this case SupplierID is NULL 
                         * but outlet id is supplier**/
                        if (orderType.Name.ToLower() == "transfer")
                        {
                            if (!viewModel.StoreIdAsSupplier.HasValue)
                            {
                                throw new BadRequestException(ErrorMessages.TransferOrderStoreAsSupplier.ToString(CultureInfo.CurrentCulture));
                            }
                            if (!(await storeRepo.GetAll().Where(x => x.Id == viewModel.StoreIdAsSupplier && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                            {
                                throw new BadRequestException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            viewModel.SupplierId = null;
                        }
                        else
                        {
                            if (!viewModel.SupplierId.HasValue)
                            {
                                throw new BadRequestException(ErrorMessages.OrderSupplierRequired.ToString(CultureInfo.CurrentCulture));
                            }
                            var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
                            if (!(await supplierRepo.GetAll().Where(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                            {
                                throw new BadRequestException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            viewModel.StoreIdAsSupplier = null;
                        }
                    }
                    //is OrderDocStatus
                    if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => x.Id == viewModel.StatusId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    if (!(await storeRepo.GetAll().Where(x => x.Id == viewModel.OutletId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var orderHeader = _iAutoMap.Mapping<OrderRequestModel, OrderHeader>(viewModel);
                    orderHeader.IsDeleted = false;
                    orderHeader.CreatedById = userId;
                    orderHeader.UpdatedById = userId;
                    orderHeader.CreatedAt = DateTime.UtcNow;
                    orderHeader.UpdatedAt = DateTime.UtcNow;

                    if (viewModel.OrderDetails != null && viewModel.OrderDetails.Count > 0)
                    {
                        // Insert child
                        var prodRepo = _unitOfWork?.GetRepository<Product>();
                        var suppProdRepo = _unitOfWork?.GetRepository<SupplierProduct>();
                        var suppRepo = _unitOfWork?.GetRepository<Supplier>();
                        foreach (var childModel in viewModel.OrderDetails)
                        {
                            //is ORDERDETAILTYPE
                            #region  ORDERDETAILTYPE COMMENTS
                            /* As Discussed with Manoj sir 
                             * For Order Creation Type MANUAL 
                             * ORDERDETAILTYPE is always NORMALBUY
                             * 
                             * Commenting below code for above implementation
                             * //To be changed later after discussing with Manoj sir
                            int orderTypeID = await listItemRepo.GetAll().Where(x => x.Id == childModel.OrderTypeId && !x.IsDeleted && x.Code == "NORMALBUY").Select(x => x.Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                            //if (!orderDetailType.HasValue || !(await listItemRepo.GetAll(x => x.Id == childModel.OrderTypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                            //{
                            //    throw new NotFoundException(ErrorMessages.OrderTypeNotFound.ToString(CultureInfo.CurrentCulture));
                            //}*/
                            #endregion
                            if (await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == viewModel.CreationTypeId
                            && x.MasterList.Code == MasterListCode.OrderCreationType && x.Code == "MANUAL").AnyAsyncSafe().ConfigureAwait(false))
                            {
                                int? orderDetailType = await listItemRepo.GetAll().Where(x => x.Code == "NORMALBUY").Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                                if (!orderDetailType.HasValue) { throw new NotFoundException(ErrorMessages.OrderTypeNotFound.ToString(CultureInfo.CurrentCulture)); }
                                childModel.OrderTypeId = orderDetailType ?? 0;
                            }
                            if (!(await prodRepo.GetAll().Where(x => x.Id == childModel.ProductId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                            {
                                throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                            if (childModel.SupplierProductId != null)
                            {
                                if (!(await suppProdRepo.GetAll().Where(x => x.Id == childModel.SupplierProductId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                                {
                                    throw new NotFoundException(ErrorMessages.SupplierProductNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                            }
                            if (childModel.CheaperSupplierId != null)
                            {
                                if (!(await suppRepo.GetAll().Where(x => x.Id == childModel.CheaperSupplierId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                                {
                                    throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                                }
                            }
                            //handling first item 
                            if (orderHeader.OrderDetail == null)
                                orderHeader.OrderDetail = new List<OrderDetail>();
                            var newChild = _iAutoMap.Mapping<OrderDetailRequestModel, OrderDetail>(childModel);
                            newChild.CreatedAt = DateTime.UtcNow;
                            newChild.CreatedById = userId;
                            newChild.UpdatedAt = DateTime.UtcNow;
                            newChild.UpdatedById = userId;
                            newChild.IsDeleted = false;
                            orderHeader.OrderDetail.Add(newChild);
                        }
                    }
                    var result = await repository.InsertAsync(orderHeader).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        resultId = result.Id;
                    }
                }
                return await GetOrderById(resultId).ConfigureAwait(false);
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
        /// Update header and details 
        /// deletes items that are not in request but in DB
        /// updates items that are in request and DB
        /// inserts items that are in request but not in DB
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OrderDetailResponseViewModel> Update(OrderRequestModel viewModel, long id, int userId)
        {
            try
            {
                if (viewModel == null || id == 0)
                {
                    throw new NullReferenceException();
                }

                var repository = _unitOfWork?.GetRepository<OrderHeader>();
                if (await repository.GetAll().Where(x => x.OrderNo == viewModel.OrderNo && x.Id != id && x.OutletId == viewModel.OutletId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
                {
                    throw new AlreadyExistsException(ErrorMessages.OrderHeaderDuplicate.ToString(CultureInfo.CurrentCulture));
                }

                var exists = await repository.GetAll().Include(d => d.OrderDetail).Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (exists == null)
                {
                    throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));
                }

                var listItemRepo = _unitOfWork?.GetRepository<MasterListItems>();
                var masterRepo = _unitOfWork?.GetRepository<MasterList>();
                var storeRepo = _unitOfWork?.GetRepository<Store>();
                // is OrderCreationType
                if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => x.MasterList.Code == MasterListCode.OrderCreationType && x.Id == viewModel.CreationTypeId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                {
                    throw new NotFoundException(ErrorMessages.CreationTypeNotFound.ToString(CultureInfo.CurrentCulture));
                }
                // is OrderDocType
                if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => x.MasterList.Code == MasterListCode.ORDERDOCTYPE && x.Id == viewModel.TypeId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                {
                    throw new NotFoundException(ErrorMessages.TypeNotFound.ToString(CultureInfo.CurrentCulture));
                }
                else
                {
                    var orderType = await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => x.MasterList.Code == MasterListCode.ORDERDOCTYPE && x.Id == viewModel.TypeId && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    if (orderType.Name.ToLower() != "delivery" && !string.IsNullOrEmpty(viewModel.DeliveryNo))
                    {
                        viewModel.DeliveryNo = exists.DeliveryNo;
                        viewModel.DeliveryDate = exists.DeliveryDate;
                    }
                    if (orderType.Name.ToLower() != "invoice" && !string.IsNullOrEmpty(viewModel.InvoiceNo))
                    {
                        viewModel.InvoiceNo = exists.InvoiceNo;
                        viewModel.InvoiceDate = exists.InvoiceDate;
                        viewModel.InvoiceTotal = exists.InvoiceTotal;
                    }
                    /**     Order Doc Type == Transfer  for this case SupplierID is NULL but outlet id is supplier     **/
                    if (orderType.Name.ToLower() == "transfer")
                    {
                        if (!viewModel.StoreIdAsSupplier.HasValue)
                        {
                            throw new BadRequestException(ErrorMessages.TransferOrderStoreAsSupplier.ToString(CultureInfo.CurrentCulture));
                        }
                        if (!(await storeRepo.GetAll().Where(x => x.Id == viewModel.StoreIdAsSupplier && !x.IsDeleted && viewModel.StoreIdAsSupplier != viewModel.OutletId).AnyAsyncSafe().ConfigureAwait(false)))
                        {
                            throw new BadRequestException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        viewModel.SupplierId = null;
                    }
                    else
                    {
                        if (!viewModel.SupplierId.HasValue)
                        {
                            throw new BadRequestException(ErrorMessages.OrderSupplierRequired.ToString(CultureInfo.CurrentCulture));
                        }
                        var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
                        if (!(await supplierRepo.GetAll().Where(x => x.Id == viewModel.SupplierId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        viewModel.StoreIdAsSupplier = null;
                    }
                }
                //is OrderDocStatus
                if (!(await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => x.MasterList.Code == MasterListCode.ORDERDOCSTATUS && x.Id == viewModel.StatusId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                {
                    throw new NotFoundException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));
                }

                if (!(await storeRepo.GetAll().Where(x => x.Id == viewModel.OutletId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                {
                    throw new NotFoundException(ErrorMessages.StoreNotFound.ToString(CultureInfo.CurrentCulture));
                }
                var orderHeader = _iAutoMap.Mapping<OrderRequestModel, OrderHeader>(viewModel);
                orderHeader.Id = id;
                orderHeader.OrderNo = exists.OrderNo;
                orderHeader.CreatedAt = exists.CreatedAt;
                orderHeader.CreatedDate = exists.CreatedDate;
                orderHeader.CreatedById = exists.CreatedById;
                orderHeader.UpdatedAt = DateTime.UtcNow;
                orderHeader.UpdatedById = userId;
                orderHeader.IsDeleted = false;

                if (viewModel.OrderDetails?.Count > 0)
                {
                    var orderDetailRepo = _unitOfWork?.GetRepository<OrderDetail>();
                    #region Delete Children
                    // Delete children
                    if (exists.OrderDetail?.Count > 0)
                    {
                        foreach (var existingChild in exists.OrderDetail.ToList())
                        {
                            var child = viewModel.OrderDetails.FirstOrDefault(c => c.ProductId == existingChild.ProductId && c.SupplierProductId == existingChild.SupplierProductId);
                            if (child == null)
                            {
                                //item is deleted
                                existingChild.UpdatedAt = DateTime.UtcNow;
                                existingChild.UpdatedById = userId;
                                existingChild.IsDeleted = true;
                                orderDetailRepo.Update(existingChild);
                            }
                        }
                    }
                    #endregion

                    #region Update/Insert Children
                    // Update and Insert children
                    var prodRepo = _unitOfWork?.GetRepository<Product>();
                    var suppProdRepo = _unitOfWork?.GetRepository<SupplierProduct>();
                    var suppRepo = _unitOfWork?.GetRepository<Supplier>();
                    foreach (var childModel in viewModel.OrderDetails)
                    {
                        //is ORDERDETAILTYPE
                        #region  ORDERDETAILTYPE COMMENTS
                        /* As Discussed with Manoj sir 
                         * For Order Creation Type MANUAL 
                         * ORDERDETAILTYPE is always NORMALBUY
                         * 
                         * Commenting below code for above implementation
                         * //To be changed later after discussing with Manoj sir
                        int orderTypeID = await listItemRepo.GetAll().Where(x => x.Id == childModel.OrderTypeId && !x.IsDeleted && x.Code == "NORMALBUY").Select(x => x.Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                        //if (!orderDetailType.HasValue || !(await listItemRepo.GetAll(x => x.Id == childModel.OrderTypeId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                        //{
                        //    throw new NotFoundException(ErrorMessages.OrderTypeNotFound.ToString(CultureInfo.CurrentCulture));
                        //}*/
                        #endregion
                        if (await listItemRepo.GetAll().Include(x => x.MasterList).Where(x => !x.IsDeleted && x.Id == viewModel.CreationTypeId
                        && x.MasterList.Code == MasterListCode.OrderCreationType && x.Code == "MANUAL").AnyAsyncSafe().ConfigureAwait(false))
                        {
                            int? orderDetailType = await listItemRepo.GetAll().Where(x => x.Code == "NORMALBUY").Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                            if (!orderDetailType.HasValue) { throw new NotFoundException(ErrorMessages.OrderTypeNotFound.ToString(CultureInfo.CurrentCulture)); }
                            childModel.OrderTypeId = orderDetailType ?? 0;
                        }
                        if (!(await prodRepo.GetAll().Where(x => x.Id == childModel.ProductId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                        {
                            throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (childModel.SupplierProductId != null)
                        {
                            if (!(await suppProdRepo.GetAll().Where(x => x.Id == childModel.SupplierProductId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                            {
                                throw new NotFoundException(ErrorMessages.SupplierProductNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                        if (childModel.CheaperSupplierId != null)
                        {
                            if (!(await suppRepo.GetAll().Where(x => x.Id == childModel.CheaperSupplierId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                            {
                                throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                        long? SupplierProductIdCheck = 0;
                        if (childModel.SupplierProductId != null)
                        {
                            SupplierProductIdCheck = childModel.SupplierProductId;
                        }

                        var existingOrderDetail = exists.OrderDetail?.Where(c => c.ProductId == childModel.ProductId).FirstOrDefault();

                        // && c.SupplierProductId == SupplierProductIdCheck
                        //var existingChild =exists.OrderDetail?.Where(c => c.ProductId == childModel.ProductId && c.SupplierProductId == SupplierProductIdCheck && !c.IsDeleted && !c.IsDeleted).FirstOrDefault();
                        if (existingOrderDetail != null)
                        {
                            // Update child
                            var updItem = _iAutoMap.Mapping<OrderDetailRequestModel, OrderDetail>(childModel);
                            updItem.CreatedAt = existingOrderDetail.CreatedAt;
                            updItem.CreatedById = existingOrderDetail.CreatedById;
                            updItem.UpdatedAt = DateTime.UtcNow;
                            updItem.UpdatedById = userId;
                            updItem.IsDeleted = false;
                            updItem.OrderHeaderId = existingOrderDetail.OrderHeaderId;
                            updItem.Id = existingOrderDetail.Id;
                            orderDetailRepo.DetachLocal(_ => _.Id == existingOrderDetail.Id);
                            orderDetailRepo.Update(updItem);
                        }
                        else
                        {
                            // Insert child
                            //handling first item 
                            if (exists.OrderDetail == null)
                                exists.OrderDetail = new List<OrderDetail>();
                            var newChild = _iAutoMap.Mapping<OrderDetailRequestModel, OrderDetail>(childModel);
                            newChild.CreatedAt = DateTime.UtcNow;
                            newChild.CreatedById = userId;
                            newChild.UpdatedAt = DateTime.UtcNow;
                            newChild.UpdatedById = userId;
                            newChild.IsDeleted = false;
                            newChild.OrderHeaderId = orderHeader.Id;
                            exists.OrderDetail.Add(newChild);
                        }
                    }
                    #endregion
                }
                repository.DetachLocal(_ => _.Id == orderHeader.Id);
                repository.Update(orderHeader);
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetOrderById(id).ConfigureAwait(false);
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

        public async Task EDIForLionSupplier(long orderId, string deliveryInstructions)
        {
            try
            {
                var xmlFile = await GenerateXMLForLionOrder(orderId, deliveryInstructions).ConfigureAwait(false);

                var response = await _sendEmailService.SendMailForEDI(xmlFile).ConfigureAwait(false);

                if (response == System.Net.Mail.DeliveryNotificationOptions.OnSuccess)
                {
                    // Create a FileInfo  
                    FileInfo xml = new FileInfo(xmlFile);
                    // Check if file is there  
                    if (xml.Exists)
                    {
                        // Move file with a new name. Hence renamed.  
                        var newFileName = xmlFile.Replace(lIONOrderSettings.FileFormatInEmail, lIONOrderSettings.FileFormatInFolder);
                        FileInfo sav = new FileInfo(newFileName);
                        if (!sav.Exists)
                        {
                            xml.MoveTo(newFileName);
                        }
                    }
                }
                else
                    throw new BadRequestException(ErrorMessages.EDILIONEmailNotSent.ToString(CultureInfo.CurrentCulture));
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }
        public async Task EDIForCocaColaSupplier(long orderId, string deliveryInstructions)
        {
            try
            {
                var savFile = await GenerateSAVForCocaColaOrder(orderId, deliveryInstructions).ConfigureAwait(false);
                var filePath = savFile?.Split('/');
                var response = _sFTPService.Send(savFile, cocaColaOrderSettings.RemotePath + filePath[filePath.Length - 1], sFTPSettings.CocaColaSupplier);

                if (response != 0)
                    throw new BadRequestException(ErrorMessages.EDICocaColaSFTPNotSent.ToString(CultureInfo.CurrentCulture));
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }
        public async Task EDIForDistributorSupplier(long orderId, string deliveryInstructions)
        {
            try
            {
                var xmlFile = await GenerateXMLForDistributorSupplier(orderId, deliveryInstructions).ConfigureAwait(false);
                var filePath = xmlFile?.Split('/');
                var response = _sFTPService.Send(xmlFile, distributorOrderSettings.RemotePath + filePath[filePath.Length - 1], sFTPSettings.DistributorSupplier);
                if (response == 0)
                {
                    FileInfo xml = new FileInfo(xmlFile);
                    // Check if file is there  
                    if (xml.Exists)
                    {
                        // Move file with a new name. Hence renamed.  
                        var newFileName = xmlFile.Replace(distributorOrderSettings.FileFormatToSend, distributorOrderSettings.FileFormatToSave);
                        if (File.Exists(newFileName))
                        {
                            File.Delete(newFileName);
                        }
                        xml.MoveTo(newFileName);
                    }
                }
                else
                    throw new BadRequestException(ErrorMessages.EDIDistributorSFTPNotSent.ToString(CultureInfo.CurrentCulture));
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message, ex);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }

        }

        /// <summary>
        /// Generation of XML File for Email Purpose
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private async Task<string> GenerateXMLForLionOrder(long orderId, string deliveryInstructions)
        {
            if (!Directory.Exists(lIONOrderSettings.FolderPath))
            {
                throw new NotFoundException(ErrorMessages.OrderFilePathNotFound.ToString(CultureInfo.CurrentCulture));
            }
            var fileName = lIONOrderSettings.FolderPath;
            var repository = _unitOfWork?.GetRepository<OrderHeader>();
            var orderHeader = await repository.GetAll(x => x.Id == orderId && !x.IsDeleted)
                .Include(o => o.Store)
                .Include(o => o.OrderDetail).ThenInclude(od => od.Product)
                .Include(o => o.Supplier).ThenInclude(s => s.OutletSupplierSettings)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (orderHeader == null)
            {
                throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode orderNode = xmlDoc.CreateElement("order");
                xmlDoc.AppendChild(orderNode);

                XmlNode customerNumber = xmlDoc.CreateElement("customerNumber");
                customerNumber.InnerText = orderHeader.Supplier?.OutletSupplierSettings?.Select(s => s.CustomerNumber).FirstOrDefault()?.ToString();
                orderNode.AppendChild(customerNumber);

                XmlNode storeNumber = xmlDoc.CreateElement("storeNumber");
                storeNumber.InnerText = orderHeader.Store?.Code;
                fileName += storeNumber.InnerText + lIONOrderSettings.FileNameSaperator;
                orderNode.AppendChild(storeNumber);

                XmlNode poNumber = xmlDoc.CreateElement("poNumber");
                poNumber.InnerText = orderHeader.OrderNo.ToString();
                fileName += poNumber.InnerText + lIONOrderSettings.FileNameSaperator;
                orderNode.AppendChild(poNumber);

                XmlNode dateSent = xmlDoc.CreateElement("dateSent");
                dateSent.InnerText = orderHeader.CreatedDate.ToString(lIONOrderSettings.XMLFileDateFormat);
                fileName += DateTime.Now.ToString(lIONOrderSettings.DateFormat);
                orderNode.AppendChild(dateSent);

                XmlNode deliveryDate = xmlDoc.CreateElement("deliveryDate");
                deliveryDate.InnerText = DateTime.Now.AddDays(1).ToString(lIONOrderSettings.XMLFileDateFormat);
                orderNode.AppendChild(deliveryDate);

                XmlNode state = xmlDoc.CreateElement("state");
                state.InnerText = ((State)Enum.ToObject(typeof(State), orderHeader.Supplier?.OutletSupplierSettings?.Select(s => s.StateId).FirstOrDefault())).ToString();
                orderNode.AppendChild(state);

                XmlNode email = xmlDoc.CreateElement("emailAddress");
                email.InnerText = orderHeader.Supplier?.Email?.ToString();
                orderNode.AppendChild(email);

                XmlNode delInstuctions = xmlDoc.CreateElement("delInstruct");
                delInstuctions.InnerText = deliveryInstructions;
                orderNode.AppendChild(delInstuctions);

                XmlNode lineCount = xmlDoc.CreateElement("lineCount");
                lineCount.InnerText = orderHeader.OrderDetail?.Count.ToString();
                orderNode.AppendChild(lineCount);

                XmlNode totalPrice = xmlDoc.CreateElement("totalPrice");
                totalPrice.InnerText = orderHeader.InvoiceTotal?.ToString();
                orderNode.AppendChild(totalPrice);

                if (orderHeader.OrderDetail != null && orderHeader.OrderDetail.Count > 0)
                {
                    XmlNode orderItemNode = xmlDoc.CreateElement("orderitem");
                    foreach (var detail in orderHeader.OrderDetail)
                    {
                        XmlNode lineNumber = xmlDoc.CreateElement("lineNum");
                        lineNumber.InnerText = detail.LineNo.ToString();
                        orderItemNode.AppendChild(lineNumber);

                        XmlNode productId = xmlDoc.CreateElement("productID");
                        productId.InnerText = detail.Product?.Number.ToString();
                        orderItemNode.AppendChild(productId);

                        XmlNode gtin = xmlDoc.CreateElement("gtin");
                        orderItemNode.AppendChild(gtin);

                        XmlNode quantity = xmlDoc.CreateElement("quantity");
                        quantity.InnerText = detail.TotalUnits.ToString();
                        orderItemNode.AppendChild(quantity);

                        XmlNode uom = xmlDoc.CreateElement("uom");
                        if (detail.CartonQty > detail.TotalUnits)
                            uom.InnerText = UOM.CT.ToString();
                        else
                            uom.InnerText = UOM.EA.ToString();
                        orderItemNode.AppendChild(uom);

                        XmlNode netUnitPrice = xmlDoc.CreateElement("netUnitPrice");
                        if (detail.CartonCost != null && detail.CartonCost.Value > 0)
                            netUnitPrice.InnerText = (detail.CartonCost / detail.CartonQty)?.ToString();
                        else
                            netUnitPrice.InnerText = "0";
                        orderItemNode.AppendChild(netUnitPrice);

                        XmlNode netValue = xmlDoc.CreateElement("netValue");
                        netValue.InnerText = detail.FinalLineTotal?.ToString();
                        orderItemNode.AppendChild(netValue);
                    }

                    if (orderItemNode.HasChildNodes)
                        orderNode.AppendChild(orderItemNode);
                }
                fileName += lIONOrderSettings.FileExtentionSaperator + lIONOrderSettings.FileFormatInEmail;
                xmlDoc.Save(fileName);
            }
            return fileName;
        }

        /// <summary>
        /// Generation of SAV File for Coca Cola Supplier
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private async Task<string> GenerateSAVForCocaColaOrder(long orderId, string deliveryInstructions)
        {
            if (!Directory.Exists(cocaColaOrderSettings.FolderPath))
            {
                throw new NotFoundException(ErrorMessages.OrderFilePathNotFound.ToString(CultureInfo.CurrentCulture));
            }
            var fileName = cocaColaOrderSettings.FolderPath;
            var stringBuilder = new StringBuilder();
            var repository = _unitOfWork?.GetRepository<OrderHeader>();
            var orderHeader = await repository.GetAll(x => x.Id == orderId && !x.IsDeleted)
                .Include(o => o.Store)
                .Include(o => o.OrderDetail).ThenInclude(od => od.Product)
                .Include(o => o.Supplier).ThenInclude(s => s.OutletSupplierSettings)
                .Include(o => o.OrderDetail).ThenInclude(s => s.SupplierProduct)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (orderHeader == null)
            {
                throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                try
                {
                    //Creating Customer Order Number for file
                    var customerNumber = orderHeader.Supplier?.OutletSupplierSettings?.Select(s => s.CustomerNumber).FirstOrDefault()?.ToString();
                    if (!string.IsNullOrWhiteSpace(customerNumber))
                    {
                        var customerNumberStuffing = cocaColaOrderSettings.CustomerNumberLength - customerNumber.Length;
                        stringBuilder.Append('0', customerNumberStuffing);
                        stringBuilder.Append(customerNumber);
                        customerNumber = stringBuilder.ToString();
                    }
                    else
                        customerNumber = stringBuilder.Append('0', cocaColaOrderSettings.CustomerNumberLength).ToString();

                    var spaceDelimiter = stringBuilder.Clear().Append(' ', cocaColaOrderSettings.SpaceCount).ToString();

                    //Creating Date for File
                    var documentDate = DateTime.UtcNow.ToString(cocaColaOrderSettings.DateFormat);

                    //Creating Order Number for File
                    var orderNumber = orderHeader.OrderNo.ToString();
                    if (!string.IsNullOrWhiteSpace(orderNumber))
                    {
                        var orderNumberStuffing = cocaColaOrderSettings.OrderNumberLength - orderNumber.Length;
                        stringBuilder.Clear();
                        stringBuilder.Append('0', orderNumberStuffing);
                        stringBuilder.Append(orderNumber);
                        orderNumber = stringBuilder.ToString();
                    }
                    else
                        orderNumber = stringBuilder.Append('0', cocaColaOrderSettings.OrderNumberLength).ToString();

                    //Creating Delivery Instructions for File
                    var delInstruct = deliveryInstructions;
                    if (!string.IsNullOrWhiteSpace(delInstruct))
                    {
                        var delInstructStuffing = cocaColaOrderSettings.SpecialInstructionLength - delInstruct.Length;
                        stringBuilder.Clear();
                        stringBuilder.Append('0', delInstructStuffing);
                        stringBuilder.Append(delInstruct);
                        delInstruct = stringBuilder.ToString();
                    }

                    fileName += orderHeader.Store?.Code?.ToString() + cocaColaOrderSettings.FileNameSaperator + orderHeader.Supplier?.Code?.ToString() + cocaColaOrderSettings.FileNameSaperator + orderHeader.OrderNo.ToString() + cocaColaOrderSettings.FileNameSaperator + DateTime.Now.ToString(cocaColaOrderSettings.SAVFileDateFormat) + cocaColaOrderSettings.FileExtentionSaperator + cocaColaOrderSettings.FileFormatInFolder;
                    // Check if file already exists. If yes, delete it.     
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    // Create a new file     
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        int count = 1;
                        sw.WriteLine(cocaColaOrderSettings.HeaderSymbol + customerNumber + spaceDelimiter + documentDate + spaceDelimiter + orderNumber);
                        if (!string.IsNullOrWhiteSpace(delInstruct))
                        {
                            sw.WriteLine(cocaColaOrderSettings.SpecialInstructionSymbol + delInstruct);
                            count++;
                        }
                        if (orderHeader.OrderDetail != null && orderHeader.OrderDetail.Count > 0)
                        {
                            foreach (var detail in orderHeader.OrderDetail)
                            {
                                if (!detail.IsDeleted)
                                {
                                    //Creating Supplier Item for File
                                    var productItem = detail.SupplierProduct?.SupplierItem.ToString();
                                    if (!string.IsNullOrWhiteSpace(productItem))
                                    {
                                        var productItemStuffing = cocaColaOrderSettings.ProductCodeLength - productItem.Length;
                                        stringBuilder.Clear();
                                        stringBuilder.Append('0', productItemStuffing);
                                        stringBuilder.Append(productItem);
                                        productItem = stringBuilder.ToString();
                                    }
                                    else
                                        productItem = stringBuilder.Clear().Append('0', cocaColaOrderSettings.ProductCodeLength).ToString();

                                    //Creating Product Quantity for File
                                    var quantity = detail.TotalUnits.ToString();
                                    if (!string.IsNullOrWhiteSpace(quantity))
                                    {
                                        var unitsStuffing = cocaColaOrderSettings.OrderedQuantityLength - quantity.Length;
                                        stringBuilder.Clear();
                                        stringBuilder.Append('0', unitsStuffing);
                                        stringBuilder.Append(quantity);
                                        quantity = stringBuilder.ToString();
                                    }
                                    else
                                        quantity = stringBuilder.Clear().Append('0', cocaColaOrderSettings.OrderedQuantityLength).ToString();

                                    //Creating Units for File
                                    var unit = 'C';
                                    if (detail.CartonQty > detail.TotalUnits)
                                        unit = 'C';
                                    else
                                        unit = 'U';

                                    sw.WriteLine(productItem + quantity + unit);
                                    count++;
                                }
                            }
                        }
                        count++;
                        var counter = count.ToString();
                        if (!string.IsNullOrWhiteSpace(counter))
                        {
                            var counterStuffing = cocaColaOrderSettings.OrderTrailerLength - counter.Length;
                            stringBuilder.Clear();
                            stringBuilder.Append('0', counterStuffing);
                            stringBuilder.Append(counter);
                            counter = stringBuilder.ToString();
                        }
                        else
                            counter = stringBuilder.Clear().Append('0', cocaColaOrderSettings.OrderTrailerLength).ToString();

                        sw.WriteLine(cocaColaOrderSettings.FileEndSymbol + counter);
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception(Ex.Message);
                }
            }
            return fileName;
        }

        /// <summary>
        /// Generation of XML File for Distributor Supplier
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="deliveryInstructions"></param>
        /// <returns></returns>
        private async Task<string> GenerateXMLForDistributorSupplier(long orderId, string deliveryInstructions)
        {
            if (!Directory.Exists(distributorOrderSettings.FolderPath))
            {
                throw new NotFoundException(ErrorMessages.OrderFilePathNotFound.ToString(CultureInfo.CurrentCulture));
            }
            var fileName = distributorOrderSettings.FolderPath + distributorOrderSettings.FileName;
            var repository = _unitOfWork?.GetRepository<OrderHeader>();
            var orderHeader = await repository.GetAll(x => x.Id == orderId && !x.IsDeleted)
                .Include(o => o.Store)
                .Include(o => o.OrderDetail).ThenInclude(od => od.Product).ThenInclude(p => p.APNProduct)
                .Include(o => o.OrderDetail).ThenInclude(od => od.SupplierProduct)
                .Include(o => o.Supplier).ThenInclude(s => s.OutletSupplierSettings)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (orderHeader == null)
            {
                throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.CreateXmlDeclaration(distributorOrderSettings.Version, distributorOrderSettings.Encoding, null);

                XmlNode cXMLNode = xmlDoc.CreateElement("cXML");
                XmlAttribute payload = xmlDoc.CreateAttribute("payloadID");
                payload.Value = distributorOrderSettings.PayloadID;
                cXMLNode.Attributes.Append(payload);
                XmlAttribute timestamp = xmlDoc.CreateAttribute("timestamp");
                timestamp.Value = DateTime.Now.ToString(distributorOrderSettings.XMLFileDateFormat);
                cXMLNode.Attributes.Append(timestamp);
                XmlAttribute version = xmlDoc.CreateAttribute("version");
                version.Value = DateTime.Now.ToString(distributorOrderSettings.Version);
                cXMLNode.Attributes.Append(version);
                xmlDoc.AppendChild(cXMLNode);

                XmlNode header = xmlDoc.CreateElement("Header");
                cXMLNode.AppendChild(header);

                XmlNode from = xmlDoc.CreateElement("From");
                header.AppendChild(from);

                XmlNode senderCredential = xmlDoc.CreateElement("Credential");
                XmlAttribute sdomain = xmlDoc.CreateAttribute("domain");
                sdomain.Value = distributorOrderSettings.Domain;
                senderCredential.Attributes.Append(sdomain);
                from.AppendChild(senderCredential);

                XmlNode senderIdentity = xmlDoc.CreateElement("Identity");
                senderIdentity.InnerText = orderHeader.Store?.Desc;
                senderCredential.AppendChild(senderIdentity);

                XmlNode to = xmlDoc.CreateElement("To");
                header.AppendChild(to);

                XmlNode credential = xmlDoc.CreateElement("Credential");
                XmlAttribute tdomain = xmlDoc.CreateAttribute("domain");
                tdomain.Value = distributorOrderSettings.Domain;
                credential.Attributes.Append(tdomain);
                to.AppendChild(credential);

                XmlNode identity = xmlDoc.CreateElement("Identity");
                identity.InnerText = orderHeader.Supplier?.Desc;
                credential.AppendChild(identity);


                XmlNode request = xmlDoc.CreateElement("Request");
                XmlAttribute deploymentMode = xmlDoc.CreateAttribute("deploymentMode");
                deploymentMode.Value = distributorOrderSettings.DeploymentMode;
                request.Attributes.Append(deploymentMode);
                cXMLNode.AppendChild(request);

                XmlNode orderRequest = xmlDoc.CreateElement("OrderRequest");
                request.AppendChild(orderRequest);

                XmlNode orderRequestHeader = xmlDoc.CreateElement("OrderRequestHeader");
                XmlAttribute orderDate = xmlDoc.CreateAttribute("orderDate");
                orderDate.Value = DateTime.Now.ToString(distributorOrderSettings.XMLFileDateFormat);
                orderRequestHeader.Attributes.Append(orderDate);
                XmlAttribute orderDeliverBeforeDate = xmlDoc.CreateAttribute("orderDeliverBeforeDate");
                orderDeliverBeforeDate.Value = DateTime.Now.ToString(distributorOrderSettings.XMLFileDateFormat);
                orderRequestHeader.Attributes.Append(orderDeliverBeforeDate);
                XmlAttribute orderID = xmlDoc.CreateAttribute("orderID");
                orderID.Value = orderHeader.OrderNo.ToString();
                orderRequestHeader.Attributes.Append(orderID);
                XmlAttribute orderType = xmlDoc.CreateAttribute("orderType");
                orderType.Value = distributorOrderSettings.OrderType;
                orderRequestHeader.Attributes.Append(orderType);
                XmlAttribute orderVersion = xmlDoc.CreateAttribute("orderVersion");
                orderVersion.Value = distributorOrderSettings.Version;
                orderRequestHeader.Attributes.Append(orderVersion);
                XmlAttribute type = xmlDoc.CreateAttribute("type");
                type.Value = distributorOrderSettings.Type;
                orderRequestHeader.Attributes.Append(type);
                orderRequest.AppendChild(orderRequestHeader);

                XmlNode total = xmlDoc.CreateElement("Total");
                orderRequestHeader.AppendChild(total);

                XmlNode money = xmlDoc.CreateElement("Money");
                XmlAttribute currency = xmlDoc.CreateAttribute("currency");
                currency.Value = distributorOrderSettings.Currency; ;
                money.Attributes.Append(currency);
                total.InnerText = orderHeader.InvoiceTotal?.ToString();
                total.AppendChild(money);

                XmlNode shipTo = xmlDoc.CreateElement("ShipTo");
                orderRequestHeader.AppendChild(shipTo);

                XmlNode branchID = xmlDoc.CreateElement("BranchID");
                branchID.InnerText = orderHeader.Store?.Code?.ToString();
                fileName += branchID.InnerText + distributorOrderSettings.FileNameSaperator + orderID.Value;
                shipTo.AppendChild(branchID);

                XmlNode address = xmlDoc.CreateElement("Address");
                XmlAttribute addressID = xmlDoc.CreateAttribute("addressID");
                addressID.Value = "4000"; //pending from Manoj Side
                address.Attributes.Append(addressID);
                XmlAttribute addressIDDomain = xmlDoc.CreateAttribute("addressIDDomain");
                addressIDDomain.Value = orderHeader.Supplier?.OutletSupplierSettings?.Select(s => s.StateId).FirstOrDefault().ToString();
                address.Attributes.Append(addressIDDomain);
                XmlAttribute isoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                isoCountryCode.Value = distributorOrderSettings.IsoCountryCode;
                address.Attributes.Append(isoCountryCode);
                shipTo.AppendChild(address);

                XmlNode name = xmlDoc.CreateElement("Name");
                XmlAttribute lang = xmlDoc.CreateAttribute("xml:lang");
                lang.Value = distributorOrderSettings.XMLLanguage;
                name.Attributes.Append(lang);
                name.InnerText = orderHeader.Supplier?.OutletSupplierSettings?.Select(s => s.Desc).FirstOrDefault().ToString();
                address.AppendChild(name);

                XmlNode postalAddress = xmlDoc.CreateElement("PostalAddress");
                XmlAttribute addressName = xmlDoc.CreateAttribute("name");
                addressName.Value = distributorOrderSettings.Default;
                postalAddress.Attributes.Append(addressName);
                address.AppendChild(postalAddress);

                XmlNode street = xmlDoc.CreateElement("Street");
                street.InnerText = "John Doe";
                postalAddress.AppendChild(street);

                XmlNode city = xmlDoc.CreateElement("City");
                city.InnerText = "John Doe";
                postalAddress.AppendChild(city);

                XmlNode state = xmlDoc.CreateElement("State");
                state.InnerText = "John Doe";
                postalAddress.AppendChild(state);

                XmlNode postalCode = xmlDoc.CreateElement("PostalCode");
                postalCode.InnerText = "John Doe";
                postalAddress.AppendChild(postalCode);

                XmlNode country = xmlDoc.CreateElement("Country");
                XmlAttribute cisoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                cisoCountryCode.Value = distributorOrderSettings.IsoCountryCode;
                country.Attributes.Append(cisoCountryCode);
                postalAddress.AppendChild(country);

                XmlNode phone = xmlDoc.CreateElement("Phone");
                address.AppendChild(phone);

                XmlNode telephoneNumber = xmlDoc.CreateElement("TelephoneNumber");
                phone.AppendChild(telephoneNumber);

                XmlNode countryCode = xmlDoc.CreateElement("CountryCode");
                XmlAttribute ccisoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                ccisoCountryCode.Value = distributorOrderSettings.IsoCountryCode;
                countryCode.Attributes.Append(ccisoCountryCode);
                countryCode.InnerText = distributorOrderSettings.IsoCountryCode;
                telephoneNumber.AppendChild(countryCode);

                XmlNode areaOrCityCode = xmlDoc.CreateElement("AreaOrCityCode");
                telephoneNumber.AppendChild(areaOrCityCode);

                XmlNode number = xmlDoc.CreateElement("Number");
                number.InnerText = orderHeader.Supplier?.Phone;
                telephoneNumber.AppendChild(number);

                XmlNode email = xmlDoc.CreateElement("Email");
                XmlAttribute eaddressName = xmlDoc.CreateAttribute("name");
                eaddressName.Value = distributorOrderSettings.Default;
                email.InnerText = orderHeader.Supplier?.Email;
                email.Attributes.Append(eaddressName);
                address.AppendChild(email);

                XmlNode billTo = xmlDoc.CreateElement("BillTo");
                orderRequestHeader.AppendChild(billTo);

                XmlNode billingAddress = xmlDoc.CreateElement("Address");
                XmlAttribute billingIsoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                billingIsoCountryCode.Value = distributorOrderSettings.IsoCountryCode;
                billingAddress.Attributes.Append(billingIsoCountryCode);
                billTo.AppendChild(billingAddress);

                XmlNode billingName = xmlDoc.CreateElement("Name");
                XmlAttribute billingLang = xmlDoc.CreateAttribute("xml:lang");
                billingLang.Value = distributorOrderSettings.XMLLanguage;
                billingName.Attributes.Append(billingLang);
                billingName.InnerText = orderHeader.Supplier?.OutletSupplierSettings?.Select(s => s.Desc).FirstOrDefault().ToString();
                billingAddress.AppendChild(billingName);

                XmlNode postalBillingAddress = xmlDoc.CreateElement("PostalAddress");
                XmlAttribute addressBillingName = xmlDoc.CreateAttribute("name");
                addressBillingName.Value = "John Doe";
                postalBillingAddress.Attributes.Append(addressBillingName);
                billingAddress.AppendChild(postalBillingAddress);

                XmlNode billingStreet = xmlDoc.CreateElement("Street");
                billingStreet.InnerText = "John Doe";
                postalBillingAddress.AppendChild(billingStreet);

                XmlNode billingCity = xmlDoc.CreateElement("City");
                billingCity.InnerText = "John Doe";
                postalBillingAddress.AppendChild(billingCity);

                XmlNode billingState = xmlDoc.CreateElement("State");
                billingState.InnerText = "John Doe";
                postalBillingAddress.AppendChild(billingState);

                XmlNode billingPostalCode = xmlDoc.CreateElement("PostalCode");
                billingPostalCode.InnerText = "John Doe";
                postalBillingAddress.AppendChild(billingPostalCode);

                XmlNode billingCountry = xmlDoc.CreateElement("Country");
                XmlAttribute billingisoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                billingisoCountryCode.Value = distributorOrderSettings.IsoCountryCode;
                billingCountry.Attributes.Append(billingisoCountryCode);
                postalBillingAddress.AppendChild(billingCountry);

                XmlNode billingPhone = xmlDoc.CreateElement("Phone");
                billingAddress.AppendChild(billingPhone);

                XmlNode billingTelephoneNumber = xmlDoc.CreateElement("TelephoneNumber");
                billingPhone.AppendChild(billingTelephoneNumber);

                XmlNode billingCountryCode = xmlDoc.CreateElement("CountryCode");
                XmlAttribute billingsisoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                billingsisoCountryCode.Value = "John Doe";
                billingCountryCode.Attributes.Append(billingsisoCountryCode);
                billingCountryCode.InnerText = "John Doe";
                billingTelephoneNumber.AppendChild(billingCountryCode);

                XmlNode billingAreaOrCityCode = xmlDoc.CreateElement("AreaOrCityCode");
                billingTelephoneNumber.AppendChild(billingAreaOrCityCode);

                XmlNode billingNumber = xmlDoc.CreateElement("Number");
                billingNumber.InnerText = "John Doe";
                billingTelephoneNumber.AppendChild(billingNumber);

                XmlNode billingEmail = xmlDoc.CreateElement("Email");
                XmlAttribute billingEAddressName = xmlDoc.CreateAttribute("name");
                billingEAddressName.Value = distributorOrderSettings.Default;
                billingEmail.InnerText = "John Doe";
                billingEmail.Attributes.Append(billingEAddressName);
                billingAddress.AppendChild(billingEmail);

                XmlNode supplierContact = xmlDoc.CreateElement("SupplierContact");
                XmlAttribute supplierAddressID = xmlDoc.CreateAttribute("addressID");
                supplierAddressID.Value = "John Doe";
                supplierContact.Attributes.Append(supplierAddressID);
                XmlAttribute supplierAddressIDDomain = xmlDoc.CreateAttribute("addressIDDomain");
                supplierAddressIDDomain.Value = "John Doe";
                supplierContact.Attributes.Append(supplierAddressIDDomain);
                XmlAttribute role = xmlDoc.CreateAttribute("role");
                role.Value = "supplierCorporate";
                supplierContact.Attributes.Append(role);
                orderRequestHeader.AppendChild(supplierContact);

                XmlNode supplierName = xmlDoc.CreateElement("Name");
                XmlAttribute supplierLang = xmlDoc.CreateAttribute("xml:lang");
                supplierLang.Value = distributorOrderSettings.XMLLanguage;
                supplierName.Attributes.Append(supplierLang);
                supplierName.InnerText = orderHeader.Supplier?.OutletSupplierSettings?.Select(s => s.Desc).FirstOrDefault().ToString();
                supplierContact.AppendChild(supplierName);

                XmlNode postalSupplierAddress = xmlDoc.CreateElement("PostalAddress");
                supplierContact.AppendChild(postalSupplierAddress);

                XmlNode supplierStreet = xmlDoc.CreateElement("Street");
                postalSupplierAddress.AppendChild(supplierStreet);

                XmlNode supplierCity = xmlDoc.CreateElement("City");
                postalSupplierAddress.AppendChild(supplierCity);

                XmlNode supplierState = xmlDoc.CreateElement("State");
                postalSupplierAddress.AppendChild(supplierState);

                XmlNode supplierPostalCode = xmlDoc.CreateElement("PostalCode");
                postalSupplierAddress.AppendChild(supplierPostalCode);

                XmlNode supplierCountry = xmlDoc.CreateElement("Country");
                XmlAttribute supplierisoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                supplierisoCountryCode.Value = distributorOrderSettings.IsoCountryCode;
                supplierCountry.Attributes.Append(supplierisoCountryCode);
                postalSupplierAddress.AppendChild(supplierCountry);

                XmlNode supplierPhone = xmlDoc.CreateElement("Phone");
                supplierContact.AppendChild(supplierPhone);

                XmlNode supplierTelephoneNumber = xmlDoc.CreateElement("TelephoneNumber");
                supplierPhone.AppendChild(supplierTelephoneNumber);

                XmlNode supplierCountryCode = xmlDoc.CreateElement("CountryCode");
                XmlAttribute suppliersisoCountryCode = xmlDoc.CreateAttribute("isoCountryCode");
                suppliersisoCountryCode.Value = distributorOrderSettings.IsoCountryCode;
                supplierCountryCode.Attributes.Append(suppliersisoCountryCode);
                supplierCountryCode.InnerText = "John Doe";
                supplierTelephoneNumber.AppendChild(supplierCountryCode);

                XmlNode supplierAreaOrCityCode = xmlDoc.CreateElement("AreaOrCityCode");
                supplierTelephoneNumber.AppendChild(supplierAreaOrCityCode);

                XmlNode supplierNumber = xmlDoc.CreateElement("Number");
                supplierTelephoneNumber.AppendChild(supplierNumber);

                XmlNode supplierEmail = xmlDoc.CreateElement("Email");
                XmlAttribute supplierEAddressName = xmlDoc.CreateAttribute("name");
                supplierEAddressName.Value = distributorOrderSettings.Default;
                supplierEmail.InnerText = "John Doe";
                supplierEmail.Attributes.Append(supplierEAddressName);
                supplierContact.AppendChild(supplierEmail);

                XmlNode comments = xmlDoc.CreateElement("Comments");
                XmlAttribute clang = xmlDoc.CreateAttribute("xml:lang");
                clang.Value = distributorOrderSettings.XMLLanguage;
                comments.Attributes.Append(clang);
                comments.InnerText = deliveryInstructions;
                orderRequestHeader.AppendChild(comments);

                if (orderHeader.OrderDetail != null && orderHeader.OrderDetail.Count > 0)
                {
                    foreach (var detail in orderHeader.OrderDetail)
                    {
                        XmlNode itemOut = xmlDoc.CreateElement("ItemOut");
                        XmlAttribute lineNumber = xmlDoc.CreateAttribute("lineNumber");
                        lineNumber.Value = detail.LineNo.ToString();
                        itemOut.Attributes.Append(lineNumber);
                        XmlAttribute quantity = xmlDoc.CreateAttribute("quantity");
                        quantity.Value = detail.TotalUnits.ToString();
                        itemOut.Attributes.Append(quantity);
                        orderRequest.AppendChild(itemOut);

                        XmlNode itemID = xmlDoc.CreateElement("ItemID");
                        itemOut.AppendChild(itemID);

                        XmlNode supplierPartID = xmlDoc.CreateElement("SupplierPartID");
                        supplierPartID.InnerText = detail.SupplierProduct?.SupplierItem.ToString();
                        itemID.AppendChild(supplierPartID);

                        XmlNode buyerPartID = xmlDoc.CreateElement("BuyerPartID");
                        buyerPartID.InnerText = detail.Product?.Number.ToString();
                        itemID.AppendChild(buyerPartID);

                        XmlNode itemDetail = xmlDoc.CreateElement("ItemDetail");
                        itemOut.AppendChild(itemDetail);

                        XmlNode unitPrice = xmlDoc.CreateElement("UnitPrice");
                        itemDetail.AppendChild(unitPrice);

                        XmlNode itemMoney = xmlDoc.CreateElement("Money");
                        XmlAttribute itemCurrency = xmlDoc.CreateAttribute("currency");
                        itemCurrency.Value = distributorOrderSettings.Currency;
                        itemMoney.Attributes.Append(itemCurrency);
                        itemMoney.InnerText = (detail.CartonCost / detail.CartonQty)?.ToString();
                        unitPrice.AppendChild(itemMoney);

                        XmlNode description = xmlDoc.CreateElement("Description");
                        XmlAttribute discriptionLang = xmlDoc.CreateAttribute("xml:lang");
                        discriptionLang.Value = distributorOrderSettings.XMLLanguage;
                        description.Attributes.Append(discriptionLang);
                        description.InnerText = detail.Product?.Desc;
                        itemDetail.AppendChild(description);

                        XmlNode unitOfMeasure = xmlDoc.CreateElement("UnitOfMeasure");
                        unitOfMeasure.InnerText = distributorOrderSettings.UnitOfMeasure;
                        itemDetail.AppendChild(unitOfMeasure);

                        XmlNode manufacturerPartID = xmlDoc.CreateElement("ManufacturerPartID");
                        itemDetail.AppendChild(manufacturerPartID);

                        XmlNode manufacturerName = xmlDoc.CreateElement("ManufacturerName");
                        XmlAttribute manufacturerLang = xmlDoc.CreateAttribute("xml:lang");
                        manufacturerLang.Value = distributorOrderSettings.XMLLanguage;
                        manufacturerName.Attributes.Append(manufacturerLang);
                        itemDetail.AppendChild(manufacturerName);

                        XmlNode itemDetailIndustry = xmlDoc.CreateElement("ItemDetailIndustry");
                        itemDetail.AppendChild(itemDetailIndustry);

                        XmlNode itemDetailRetail = xmlDoc.CreateElement("ItemDetailRetail");
                        itemDetailIndustry.AppendChild(itemDetailRetail);

                        XmlNode eEANID = xmlDoc.CreateElement("EANID");
                        eEANID.InnerText = detail.Product?.APNProduct?.Select(p => p.Number).FirstOrDefault().ToString();
                        itemDetailRetail.AppendChild(eEANID);

                        XmlNode itemComments = xmlDoc.CreateElement("Comments");
                        XmlAttribute itemclang = xmlDoc.CreateAttribute("xml:lang");
                        itemclang.Value = distributorOrderSettings.XMLLanguage;
                        itemComments.Attributes.Append(itemclang);
                        itemComments.InnerText = deliveryInstructions;
                        itemOut.AppendChild(itemComments);
                    }
                }

                fileName += lIONOrderSettings.FileExtentionSaperator + lIONOrderSettings.FileFormatInEmail;
                xmlDoc.Save(fileName);
            }
            return fileName;
        }
        //public async Task<long> GetNewOrderNumber(int storeId)
        //{
        //    long newOrderNo = 0;
        //    string newOrder = "";
        //    try
        //    {
        //        var repository = _unitOfWork?.GetRepository<OrderHeader>();

        //        var list = await repository.GetAll().Where(x => x.OutletId == storeId && !x.IsDeleted).OrderBy(x => x.OrderNo).Select(x => x.OrderNo).ToListAsyncSafe().ConfigureAwait(false);

        //        long startNo = Convert.ToInt64(storeId.ToString() + "1");

        //        if (list == null || list.Count <= 0)
        //        {
        //            return startNo;
        //        }

        //        long[] orderNo = new long[list.Count];

        //        int i = 0;
        //        foreach (var no in list)
        //        {
        //            var index = no.ToString().ElementAt(0);

        //            if (index == storeId.ToString().ElementAt(0))
        //            {
        //                orderNo[i] = Convert.ToInt64(no.ToString().Substring(storeId.ToString().Length));
        //                i++;
        //            }
        //        }

        //        long missing = findFirstMissing(orderNo, 0, orderNo.Length - 1);

        //        //Check if missing

        //        long findFirstMissing(long[] array, int start, int end)
        //        {
        //            if (start >= end + 1)
        //                return end + 2;

        //            if (start + 1 != array[start])
        //                return start + 1;

        //            int mid = (start + end) / 2;

        //            // Left half has all elements  
        //            // from 0 to mid 
        //            if (array[mid] == mid + 1)
        //                return findFirstMissing(array, mid + 1, end);

        //            return findFirstMissing(array, start, mid);
        //        }

        //        if (missing != 0)
        //        {
        //            newOrder = storeId.ToString() + missing.ToString();
        //        }
        //        else
        //        {
        //            newOrder = storeId.ToString() + (orderNo.Max() + 1).ToString();
        //        }

        //        newOrderNo = Convert.ToInt64(newOrder);

        //        while (await repository.GetAll().Where(x => x.OrderNo == newOrderNo && x.OutletId == storeId && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false))
        //        {
        //            missing = missing + 1;
        //            newOrder = storeId.ToString() + missing.ToString();
        //            newOrderNo = Convert.ToInt64(newOrder);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArithmeticException(ex.Message);
        //    }

        //    return newOrderNo;
        //}

        public async Task<long> GetNewOrderNumber(int storeId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Product>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@StoreId", storeId)
            };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetOrderNumberNew, dbParams.ToArray()).ConfigureAwait(false);
                return Convert.ToInt32(dset.Tables[0].Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<List<OrderDetailViewModel>> UpdateSupplierProductAsync(List<OrderDetailRequestModel> productList, int SupplierId)
        {
            var supplierRepo = _unitOfWork?.GetRepository<Supplier>();
            if (!(await supplierRepo.GetAll(x => x.Id == SupplierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
            {
                throw new BadRequestException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
            }


            var OrderDetailResponseList = new List<OrderDetailViewModel>();

            var repo = _unitOfWork.GetRepository<SupplierProduct>();
            if (productList != null)
            {
                foreach (var item in productList)
                {
                    if (item.ProductId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
                    }

                    var prodRepo = _unitOfWork?.GetRepository<Product>();
                    if (!(await prodRepo.GetAll(x => x.Id == item.ProductId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new BadRequestException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture) + $" : { item.ProductId}");
                    }

                    var response = MappingHelpers.Mapping<OrderDetailRequestModel, OrderDetailViewModel>(item);

                    var supplierItem = await repo.GetAll(x => x.SupplierId == SupplierId && x.ProductId == item.ProductId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (supplierItem != null)
                    {
                        response.SupplierProductId = supplierItem.Id;
                        response.SupplierProductItem = supplierItem.SupplierItem;
                        response.SupplierProductDesc = supplierItem.Desc;
                    }
                    else
                    {
                        response.SupplierProductId = null;
                        response.SupplierProductItem = "";
                        response.SupplierProductDesc = "";
                    }
                    OrderDetailResponseList.Add(response);
                }
            }
            return OrderDetailResponseList;
        }
        public async Task<OrderDetailResponseViewModel> SendOrder(OrderSendRequestModel requestModel, int userId)
        {
            /** As discussed with Manoj sir
               * following is better handled in new APP
               * if a new order and items are selected and finish button is not clicked then 
               * order is auto saved in OLD APP
               * so When SEND ORDER button is clicked before finish button in OLD APP it works perfectly
               * 
               * New APP will throw error (order not found)
               * **/

            try
            {
                if (requestModel != null)
                {
                    var repoOutletSupplierSetting = _unitOfWork?.GetRepository<OutletSupplierSetting>();
                    var repositorySupplier = _unitOfWork?.GetRepository<Supplier>();
                    var repositoryOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                    var repositoryStatusAll = _unitOfWork?.GetRepository<MasterListItems>().GetAll().Include(x => x.MasterList);

                    var orderHeader = await repositoryOrderHeader.GetAll().Include(x => x.OrderDetail).Where(x => !x.IsDeleted && x.OrderNo == requestModel.OrderNo && x.OutletId == requestModel.OutletId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (orderHeader == null)
                        throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.OutletId == 0)
                        throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.SupplierId == null || requestModel.SupplierId == 0)
                        throw new NullReferenceException(ErrorMessages.SupplierRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.TypeId == 0)
                        throw new NullReferenceException(ErrorMessages.DocumentTypeRequired.ToString(CultureInfo.CurrentCulture));

                    var supplier = await repositorySupplier.GetAll(x => x.Id == requestModel.SupplierId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (supplier == null)
                        throw new BadRequestException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));

                    var listCode = repositoryStatusAll.Where(x => x.Id == requestModel.TypeId && !x.IsDeleted && x.MasterList.Code == "OrderDocType").FirstOrDefault();
                    if (listCode == null)
                        throw new NotFoundException(ErrorMessages.TypeNotFound.ToString(CultureInfo.CurrentCulture));

                    if (listCode.Code != "ORDER")
                        throw new NullReferenceException(ErrorMessages.DocumentOrderTypeNotSelect.ToString(CultureInfo.CurrentCulture));
                    //TODO: As discussed with Manoj sir, Remove Metcash and 1 (metcash) as its implementation is pending 
                    //var supplierCode = new string[] { "BCOKE", "COKE", "DCOKE", "RCOKE", "LION", "DIST", "PFD", "METCASH", "1" };
                    var supplierCode = new string[] { "BCOKE", "COKE", "DCOKE", "RCOKE", "LION", "DIST", "PFD" };
                    if (requestModel.Offline && !supplierCode.Any(x => x == supplier.Code))
                    {
                        await MarkOrderAsSent(requestModel.OrderNo, requestModel.OutletId, userId, "send").ConfigureAwait(false);
                        return await GetOrderById(orderHeader.Id).ConfigureAwait(false);
                    }
                    //GetOutletSupplierDetails               
                    var outletSupplierDetails = repoOutletSupplierSetting.GetAll().Include(x => x.Supplier).Where(x => !x.IsDeleted && x.StoreId == requestModel.OutletId && x.SupplierId == requestModel.SupplierId).FirstOrDefault();
                    if (outletSupplierDetails != null)
                    {
                        if (outletSupplierDetails.QtyDefault == null)
                        {
                            outletSupplierDetails.QtyDefault = "SINGLES";
                        }
                        else if (outletSupplierDetails.DivisionId == 0)
                        {
                            outletSupplierDetails.DivisionId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "D" && x.MasterList.Code == "DIVISION").FirstOrDefault()?.Id;
                        }
                        repoOutletSupplierSetting.Update(outletSupplierDetails);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        throw new NotFoundException(ErrorMessages.NoOutletSupplierSetting.ToString(CultureInfo.CurrentCulture));
                    }
                    if ((!supplierCode.Any(x => x == supplier.Code) || string.IsNullOrEmpty(outletSupplierDetails.CustomerNumber)) && !requestModel.Offline)
                    {
                        throw new NotFoundException(ErrorMessages.MarkOffineMessage.ToString(CultureInfo.CurrentCulture));
                    }
                    else if (supplierCode.Any(x => x == supplier.Code))
                    {
                        if (supplier.Code == "BCOKE" || supplier.Code == "COKE" || supplier.Code == "DCOKE" || supplier.Code == "RCOKE")
                        {
                            await EDIForCocaColaSupplier(orderHeader.Id, requestModel.DeliveryInstructions).ConfigureAwait(false);
                        }
                        if (supplier.Code == "LION")
                        {
                            await EDIForLionSupplier(orderHeader.Id, requestModel.DeliveryInstructions).ConfigureAwait(false);
                        }
                        if (supplier.Code == "DIST")
                        {
                            await EDIForDistributorSupplier(orderHeader.Id, requestModel.DeliveryInstructions).ConfigureAwait(false);
                        }
                        if (supplier.Code == "1")
                        {
                            await EDIForMetcashSupplier(orderHeader.Id, requestModel.DeliveryInstructions).ConfigureAwait(false);
                        }
                        await MarkOrderAsSent(requestModel.OrderNo, requestModel.OutletId, userId, "send").ConfigureAwait(false);
                    }
                    else
                    {
                        throw new NotModifiedException(ErrorMessages.OrderNotSent.ToString(CultureInfo.CurrentCulture));
                    }
                    int? statusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == listCode.Code && x.MasterList.Code == MasterListCode.ORDERSTATUS).FirstOrDefault()?.Id;
                    await PostAuditRecord(requestModel.OrderNo, "BtnSendClick", userId, requestModel.OutletId, requestModel.SupplierId, requestModel.TypeId, orderHeader.StatusId, statusId).ConfigureAwait(false);
                    return await GetOrderById(orderHeader.Id).ConfigureAwait(false);
                }
                else
                {
                    throw new NullReferenceException();
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
                if (ex is NotModifiedException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public static void ChangeLine(ref string line) { line = line + "\n"; }
        public static double Frac(double value) { return value - Math.Truncate(value); }
        public bool AllowsPartCartons(long productId, int supplierId, double ctnQty)
        {
            var repositoryZonePricing = _unitOfWork?.GetRepository<ZonePricing>();
            var repositorySupplier = _unitOfWork?.GetRepository<Supplier>();
            var repositoryCostPriceZones = _unitOfWork?.GetRepository<CostPriceZones>();
            var MinReorder = repositoryZonePricing.GetAll()
                    .Join(repositoryCostPriceZones.GetAll(), x => x.PriceZoneId, y => y.ID, (Zprc, PZon) => new { Zprc, PZon })
                    .Join(repositorySupplier.GetAll(), x => x.PZon.Code, y => y.CostZone, (ZPrc, Supp) => new { ZPrc, Supp })
                    .Where(x => x.ZPrc.Zprc.ProductId == productId && x.Supp.Id == supplierId).Select(x => x.ZPrc.Zprc.MinReorderQty).FirstOrDefault();
            return MinReorder < ctnQty;
        }
        public async Task<string> EDIForMetcashSupplier(long orderId, string deliveryInstructions)
        {
            System.Console.WriteLine(deliveryInstructions);
            int recordCount = 0;
            string orderType = string.Empty;
            string custNo = string.Empty;
            string orderNumber = string.Empty;
            string transmissionHeader = string.Empty;
            string businessPillar = string.Empty;
            string stateInd = string.Empty;
            string mess = string.Empty;
            string date = string.Empty;
            string ordLineText = string.Empty;
            var repositoryOrder = _unitOfWork?.GetRepository<OrderHeader>();
            var repositoryOSS = _unitOfWork?.GetRepository<OutletSupplierSetting>();
            var repositorySuppleir = _unitOfWork?.GetRepository<Supplier>();
            var repositoryMasterListItem = _unitOfWork?.GetRepository<MasterListItems>();
            var repositoryMasterList = _unitOfWork?.GetRepository<MasterList>();
            var order = await repositoryOrder.GetById(orderId).ConfigureAwait(false);
            var supplier = await repositorySuppleir.GetById(Convert.ToInt32(order.SupplierId)).ConfigureAwait(false);
            var outletSupplier = await repositoryOSS.GetAll(x => x.StoreId == order.OutletId).FirstOrDefaultAsync().ConfigureAwait(false);
            var stateId = repositoryMasterList.GetAll(y => y.Code == "STATE").FirstOrDefault().Id;
            var divisionId = repositoryMasterList.GetAll(y => y.Code == "DIVISION").FirstOrDefault().Id;
            var masterlistStateInd = await repositoryMasterListItem.GetAll(x => x.ListId == stateId).ToListAsync().ConfigureAwait(false);
            var masterlistDivision = await repositoryMasterListItem.GetAll(x => x.ListId == divisionId).ToListAsync().ConfigureAwait(false);
            string orderlineTxt = string.Empty;
            if (order != null)
            {
                // default to Grocery QLD
                stateInd = "03";
                orderType = "01";
                businessPillar = "D";

                if (outletSupplier != null)
                {
                    custNo = outletSupplier.CustomerNumber;
                    if (supplier.Code == "1") //METCASH
                    {
                        orderType = "01";
                        businessPillar = "D";
                    }

                    if (outletSupplier.DivisionId == masterlistDivision.FirstOrDefault(x => x.Code == "S")?.Id)
                    {
                        businessPillar = "S";
                    }

                    if (outletSupplier.DivisionId == masterlistDivision.FirstOrDefault(x => x.Code == "C")?.Id)
                    {
                        businessPillar = "C";
                    }

                    if (outletSupplier.StateId == masterlistStateInd.FirstOrDefault(x => x.Code == "ALM")?.Id)
                    {
                        orderType = "05";
                        businessPillar = "A";
                    }

                    if (outletSupplier.StateId == masterlistStateInd.FirstOrDefault(x => x.Code == "NSW")?.Id)
                    {
                        stateInd = "01";
                    }

                    if (outletSupplier.StateId == masterlistStateInd.FirstOrDefault(x => x.Code == "VIC")?.Id)
                    {
                        stateInd = "02";
                    }
                    if (outletSupplier.StateId == masterlistStateInd.FirstOrDefault(x => x.Code == "QLD")?.Id)
                    {
                        stateInd = "03";
                    }
                    if (outletSupplier.StateId == masterlistStateInd.FirstOrDefault(x => x.Code == "WA")?.Id)
                    {
                        stateInd = "04";
                    }
                    if (outletSupplier.StateId == masterlistStateInd.FirstOrDefault(x => x.Code == "SA")?.Id)
                    {
                        stateInd = "05";
                    }
                    if (outletSupplier.StateId == masterlistStateInd.FirstOrDefault(x => x.Code == "TAS")?.Id)
                    {
                        stateInd = "07";
                    }

                    if (custNo.Length < 8)
                    {
                        custNo = custNo.PadLeft(8, '0');
                    }

                    orderNumber = order.OrderNo.ToString().PadLeft(10, '0');

                    transmissionHeader = "X";
                    date = DateTime.UtcNow.ToString("ddMMyyyy");

                    orderlineTxt = $"{transmissionHeader}0000000000 F{orderType} F{custNo} F{date} F{businessPillar}  F{stateInd} F{orderNumber}  F**ORDER HEADER REC**";

                    recordCount = recordCount + 1;
                    ChangeLine(ref orderlineTxt);
                    var repositoryOrderDetails = _unitOfWork?.GetRepository<OrderDetail>();
                    var repositoryProduct = _unitOfWork?.GetRepository<Product>();

                    var orderLineItems = await repositoryOrderDetails.GetAll(x => x.OrderHeaderId == orderId).Include(x => x.SupplierProduct).Join(repositoryProduct.GetAll(), x => x.ProductId, y => y.Id, (Ordl, Prod) => new { Ordl, Prod }).ToListAsync().ConfigureAwait(false);

                    if (orderLineItems != null && orderLineItems.Count > 0)
                    {
                        foreach (var item in orderLineItems)
                        {
                            string itemstring = string.Empty;
                            string qtyDescriptor = string.Empty;
                            string orderQty = string.Empty;
                            double ordCartons = item.Ordl.Cartons;
                            double ordUnits = item.Ordl.Units;
                            double ordTotalUnits = item.Ordl.TotalUnits;
                            double tmpCartons = 0;
                            double itemNumeric;

                            if (ordCartons == 0 && ordUnits == 0) continue;

                            if (order?.Supplier == null || string.IsNullOrWhiteSpace(order?.Supplier.Code?.Trim()))
                                throw new NotFoundException(ErrorMessages.EDIMetcashSupplierCodeMissing.ToString(CultureInfo.CurrentCulture));

                            _ = double.TryParse(item.Ordl.SupplierProduct.SupplierItem, out itemNumeric);

                            if (itemNumeric == 0 || itemNumeric == 999999)
                                throw new NotFoundException(ErrorMessages.EdDIMetcashSupplierItemMissing.ToString(CultureInfo.CurrentCulture));

                            itemstring = itemNumeric.ToString().PadLeft(14, '0');

                            if (!AllowsPartCartons(item.Ordl.ProductId, Convert.ToInt32(order.SupplierId), item.Ordl.CartonQty))
                            {
                                qtyDescriptor = "C";

                                if (ordTotalUnits == ordCartons)
                                {
                                    orderQty = ordCartons.ToString().PadLeft(5, '0');
                                }
                                else
                                {
                                    // convert to cartons
                                    tmpCartons = ordUnits / item.Prod.CartonQty;

                                    if (Frac(tmpCartons) > 0.0)
                                    {
                                        tmpCartons = tmpCartons + 1;
                                    }

                                    tmpCartons = ordCartons + tmpCartons;

                                    orderQty = tmpCartons.ToString().PadLeft(5, '0');

                                    orderlineTxt += $"{itemstring}{orderQty}{qtyDescriptor}D";
                                }
                            }
                            else if (item.Prod.Parent == null)
                            {
                                if (ordUnits == 0)
                                {
                                    qtyDescriptor = "C";
                                    orderQty = ordCartons.ToString().PadLeft(5, '0');
                                }
                                else
                                {
                                    // need to use vOrdTotalUnits and not vOrdUnits because if part cartons can be ordered and say you order 1 carton and 3 units
                                    // then need to send total units and not just the units part
                                    qtyDescriptor = "U";
                                    orderQty = ordTotalUnits.ToString().PadLeft(5, '0');

                                }

                                orderlineTxt += $"{itemstring}{orderQty}{qtyDescriptor}D";

                            }
                            else
                            {
                                if (ordUnits > 0)
                                {
                                    orderQty = (ordCartons + 1).ToString().PadLeft(5, '0');
                                }
                                else
                                {
                                    orderQty = ordCartons.ToString().PadLeft(5, '0');
                                }

                                qtyDescriptor = "C";
                                orderlineTxt += $"{itemstring}{orderQty}{qtyDescriptor}D";
                            }

                            recordCount = recordCount + 1;
                            ChangeLine(ref orderlineTxt);
                        }
                    }

                    //
                    recordCount = recordCount + 1;
                    orderlineTxt += $"Z {recordCount.ToString().PadLeft(5, '0')}";
                }

            }

            //File.WriteAllText(Helpers.Constants.MetOrderPath, orderlineTxt);

            OrderDetailsModel orderDetailsModel = new OrderDetailsModel
            {
                CustomerId = custNo,
                StateCode = stateInd,
                PillarId = businessPillar,
                OrderType = orderType,
                Order = orderlineTxt,
            };
            //var auth = repositoryOSS.GetAll(x => x.SupplierId == order.SupplierId && x.StoreId == order.OutletId).Select(x => new Authentic { B2BAccount = x.UserId, Password = x.Password }).FirstOrDefault();
            PlaceOrderRequestModel placeOrderRequestModel = new PlaceOrderRequestModel
            {
                Authentication =// auth,
                new Authentic()
                {
                    B2BAccount = "MRTBOS01",
                    Password = "PASSWORD"
                },
                operatingSystemModel = new OperatingSystemModel
                {
                    ServicePack = "",
                    SystemName = "",
                    SystemType = "",
                    Version = "",
                },

                vendorDetailsModel = new VendorDetailsModel
                {
                    Vendor = "",
                    Version = "",
                    Software = ""
                },

                //PlaceOrderModel = new PlaceOrderModel
                //{
                //    CustomerId = orderDetailsModel.CustomerId,
                //    fileName = "",
                //    StateCode = orderDetailsModel.StateCode,
                //    PillarId = orderDetailsModel.PillarId,
                //    OrderType = orderDetailsModel.OrderType,
                //    Order = orderDetailsModel.Order
                //}
                PlaceOrderModel = new PlaceOrderModel
                {
                    CustomerId = "61170017",//orderDetailsModel.CustomerId,
                    fileName = "",
                    StateCode = "NSW",//orderDetailsModel.StateCode,
                    PillarId = "IGA",//orderDetailsModel.PillarId,
                    OrderType = "PC01Order",//orderDetailsModel.OrderType,
                    Order = orderDetailsModel.Order
                }
            };

            var PlacedOrder = await EdiMetcashService.PlaceOrder(placeOrderRequestModel).ConfigureAwait(false);
            order.Reference = PlacedOrder.BatchId;
            order.PostedDate = DateTime.UtcNow;
            foreach (var ordl in order.OrderDetail)
            {
                ordl.PostedDate = DateTime.UtcNow;
                //ordl.CartonQty = ordl.Cartons;
                //ordl.OrderUnit = ordl.Units;
                //ordl.OrdelTotalUnits = ordl.TotalUnits;
            }
            repositoryOrder.Update(order);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var ordersummaryModel = new OrderSummaryRequestModel
            {
                Authentication = placeOrderRequestModel.Authentication,
                operatingSystemModel = placeOrderRequestModel.operatingSystemModel,
                vendorDetailsModel = placeOrderRequestModel.vendorDetailsModel,
                OrderSummayModel = new OrderSummayModel
                {
                    BatchId = PlacedOrder.BatchId,
                    CustomerId = placeOrderRequestModel.PlaceOrderModel.CustomerId,
                    PillarId = placeOrderRequestModel.PlaceOrderModel.PillarId,
                    StateCode = placeOrderRequestModel.PlaceOrderModel.StateCode,
                    Timestamp = PlacedOrder.Timestamp
                }
            };
            var OrderSummary = await EdiMetcashService.GetOrderSummary(ordersummaryModel).ConfigureAwait(false);
            return OrderSummary;
        }
        public static string MultilineText(string singleLine)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            var strArray = singleLine.Split('\n');
#pragma warning restore CA1062 // Validate arguments of public methods
            var multi = new StringBuilder();
            foreach (var item in strArray)
            {
                multi.AppendFormat(item).AppendLine();
            }
            return multi.ToString();
        }

        public async Task<OrderDetailResponseViewModel> PostOrder(OrderPostRequestModel requestModel, int userId)
        {
            try
            {
                /** As discussed with Manoj sir
                 * following is better handled in new APP
                 * if a new order and items are selected and finish button is not clicked then 
                 * order is auto saved in OLD APP
                 * so When POST button is clicked before finish button in OLD APP it works perfectly
                 * 
                 * New APP will throw error (order not found)
                 * **/
                if (requestModel != null)
                {
                    var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                    var repoOrderDetail = _unitOfWork?.GetRepository<OrderDetail>();
                    var repoSupplier = _unitOfWork?.GetRepository<Supplier>();
                    var repositoryStatusAll = _unitOfWork?.GetRepository<MasterListItems>().GetAll().Include(x => x.MasterList);
                    if (requestModel.OutletId == 0)
                        throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.SupplierId == null || requestModel.SupplierId == 0)
                        throw new NullReferenceException(ErrorMessages.SupplierRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.TypeId == 0)
                        throw new NullReferenceException(ErrorMessages.DocumentTypeRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.StatusId == 0)
                        throw new NullReferenceException(ErrorMessages.DocumentStatusRequired.ToString(CultureInfo.CurrentCulture));


                    var orderHeader = await repoOrderHeader.GetAll().Include(x => x.OrderDetail).Where(x => !x.IsDeleted && x.OrderNo == requestModel.OrderNo && x.OutletId == requestModel.OutletId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (orderHeader == null)
                        throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));

                    var supplier = repoSupplier.GetAll(x => !x.IsDeleted && x.Id == requestModel.SupplierId).FirstOrDefault();
                    if (supplier == null)
                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));

                    var listCode = repositoryStatusAll.Where(x => x.Id == requestModel.TypeId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERDOCTYPE).FirstOrDefault();
                    if (listCode == null)
                        throw new NotFoundException(ErrorMessages.TypeNotFound.ToString(CultureInfo.CurrentCulture));

                    var listStatus = repositoryStatusAll.Where(x => x.Id == requestModel.StatusId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault();
                    if (listStatus == null)
                        throw new NotFoundException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));

                    if (!await repositoryStatusAll.AnyAsync(x => x.Id == requestModel.CreationTypeId && !x.IsDeleted && x.MasterList.Code == MasterListCode.OrderCreationType).ConfigureAwait(false))
                        throw new AlreadyExistsException(ErrorMessages.CreationTypeNotFound.ToString(CultureInfo.CurrentCulture));

                    if (listCode.Code == "CREDIT")
                        throw new NullReferenceException(ErrorMessages.DocumentTypeCREDIT.ToString(CultureInfo.CurrentCulture));


                    /**
                     * DELPHI CODE
                     *  OrderTotal := OrderLineTotal - fOrderHeaderTbl.ORDH_SUBTOT_DISC.TypedColValue - fOrderHeaderTbl.ORDH_SUBTOT_SUBSIDY.TypedColValue +
    fOrderHeaderTbl.ORDH_SUBTOT_FREIGHT.TypedColValue + fOrderHeaderTbl.ORDH_SUBTOT_ADMIN.TypedColValue +
    fOrderHeaderTbl.ORDH_SUBTOT_TAX.TypedColValue;
                    **/
                    var orderLineTotal = orderHeader.OrderDetail.Sum(x => x.LineTotal);
                    float orderTotal = orderLineTotal - requestModel?.LessDisccount ?? 0 - requestModel?.LessSubsidy ?? 0 + requestModel?.PlusFreight ?? 0 + requestModel?.PlusAdmin ?? 0 + requestModel?.PlusGst ?? 0;

                    if (listCode.Code == "INVOICE" || listCode.Code == "CREDIT")
                    {
                        if (requestModel.InvoiceTotalAmount != orderTotal)
                            throw new NotFoundException(ErrorMessages.DocumentTotalsBalance.ToString(CultureInfo.CurrentCulture));

                        if (string.IsNullOrEmpty(requestModel.InvoiceNo))
                            throw new NotFoundException(ErrorMessages.DocumentNumberRequired.ToString(CultureInfo.CurrentCulture));

                        if (requestModel.InvoiceDate == null || requestModel.InvoiceDate == DateTime.MinValue)
                            throw new NotFoundException(ErrorMessages.DocumentDateRequired.ToString(CultureInfo.CurrentCulture));

                        var checkInvoiceNo = await repoOrderHeader.GetAll().Include(x => x.Supplier)
                            .Where(x => !x.IsDeleted && x.OutletId == requestModel.OutletId &&
                                   x.OrderNo != requestModel.OrderNo && x.SupplierId == requestModel.SupplierId &&
                                   x.InvoiceNo == requestModel.InvoiceNo).ToListAsyncSafe().ConfigureAwait(false);

                        if (checkInvoiceNo.Count > 0)
                            throw new NotFoundException(ErrorMessages.InvoiceNumberAlreadyExists.ToString(CultureInfo.CurrentCulture));


                        orderHeader.InvoiceNo = requestModel.InvoiceNo;
                        orderHeader.InvoiceDate = requestModel.InvoiceDate;
                    }

                    if (listCode.Code == "DELIVERY")
                    {
                        if (string.IsNullOrEmpty(requestModel.DeliveryNo))
                            throw new NotFoundException(ErrorMessages.DocumentNumberRequired.ToString(CultureInfo.CurrentCulture));

                        if (requestModel.DeliveryDate == null || requestModel.DeliveryDate == DateTime.MinValue)
                            throw new NotFoundException(ErrorMessages.DocumentDateRequired.ToString(CultureInfo.CurrentCulture));

                        orderHeader.DeliveryNo = requestModel.DeliveryNo;
                        orderHeader.DeliveryDate = requestModel.DeliveryDate;
                    }

                    //checkStockOnHand
                    if (listCode.Code == "ORDER")
                    {
                        int statusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == listCode.Code && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault().Id;

                        orderHeader.PostedDate = DateTime.UtcNow;

                        await MarkOrderAsSent(requestModel.OrderNo, requestModel.OutletId, userId, "post", requestModel.ReferenceNumber).ConfigureAwait(false);
                        await PostAuditRecord(requestModel.OrderNo, "BtnPostClick", userId, requestModel.OutletId, requestModel.SupplierId, requestModel.TypeId, orderHeader.StatusId, statusId).ConfigureAwait(false);
                    }

                    if (listCode.Code == "DELIVERY" || listCode.Code == "INVOICE" || listCode.Code == "TRANSFER")
                    {
                        int oldStatusId = orderHeader.StatusId;
                        if (listCode.Code == "TRANSFER")
                            orderHeader.StatusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "INVOICE" && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault().Id;
                        else
                            orderHeader.StatusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == listCode.Code && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault().Id;

                        if (requestModel.PlusGst > 0)
                        {
                            orderHeader.GstAmt = requestModel.PlusGst;
                        }
                        else
                        {
                            orderHeader.GstAmt = requestModel.OrderGSGTotal;
                        }
                        var creationType = repositoryStatusAll.Where(x => x.Id == requestModel.CreationTypeId && !x.IsDeleted).FirstOrDefault().Code;
                        if (listCode.Code == "TRANSFER" || (listCode.Code == "INVOICE" && creationType == "TRANSFER"))
                        {
                            orderHeader.InvoiceTotal = requestModel.InvoiceTotalAmount;
                        }
                        orderHeader.Reference = requestModel.ReferenceNumber;
                        orderHeader.CreatedAt = DateTime.UtcNow;
                        orderHeader.CreatedById = userId;
                        repoOrderHeader.Update(orderHeader);

                        foreach (var obj in orderHeader.OrderDetail)
                        {
                            obj.PostedDate = DateTime.UtcNow;
                            obj.CreatedAt = DateTime.UtcNow;
                            obj.CreatedById = userId;
                            repoOrderDetail.Update(obj);
                        }
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                        await PostAuditRecord(requestModel.OrderNo, "BtnPostClick 2nd", userId, requestModel.OutletId, requestModel.SupplierId, requestModel.TypeId, oldStatusId, orderHeader.StatusId).ConfigureAwait(false);
                    }
                    return await GetOrderById(orderHeader.Id).ConfigureAwait(false);
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
        public async Task<OrderDetailResponseViewModel> FinishOrder(OrderFinishRequestModel requestModel, int userId)
        {
            try
            {
                if (requestModel != null)
                {
                    var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                    var repoOrderDetail = _unitOfWork?.GetRepository<OrderDetail>();
                    var repoSupplier = _unitOfWork?.GetRepository<Supplier>();
                    var repositoryStatusAll = _unitOfWork?.GetRepository<MasterListItems>();

                    if (requestModel.OutletId == 0)
                        throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));

                    if ((requestModel.SupplierId == null || requestModel.SupplierId == 0) && !requestModel.IsDelete)
                        throw new NullReferenceException(ErrorMessages.SupplierRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.TypeId == 0)
                        throw new NullReferenceException(ErrorMessages.DocumentTypeRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.StatusId == 0)
                        throw new NullReferenceException(ErrorMessages.DocumentStatusRequired.ToString(CultureInfo.CurrentCulture));

                    var orderHeader = await repoOrderHeader.GetAll().Include(x => x.OrderDetail).Where(x => !x.IsDeleted && x.OrderNo == requestModel.OrderNo && x.OutletId == requestModel.OutletId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (orderHeader == null)
                        throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));

                    var supplier = repoSupplier.GetAll(x => !x.IsDeleted && x.Id == requestModel.SupplierId).FirstOrDefault();
                    if (supplier == null)
                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));

                    var listCode = repositoryStatusAll.GetAll(x => x.Id == requestModel.TypeId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERTYPE).FirstOrDefault();
                    if (listCode == null)
                        throw new NotFoundException(ErrorMessages.TypeNotFound.ToString(CultureInfo.CurrentCulture));

                    var listStatus = repositoryStatusAll.GetAll(x => x.Id == requestModel.StatusId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERSTATUS).FirstOrDefault();
                    if (listStatus == null)
                        throw new NotFoundException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.IsDelete)
                    {
                        orderHeader.IsDeleted = true;
                        orderHeader.UpdatedAt = DateTime.UtcNow;
                        orderHeader.UpdatedById = userId;
                        repoOrderHeader.Update(orderHeader);
                        foreach (var obj in orderHeader.OrderDetail)
                        {
                            obj.IsDeleted = true;
                            obj.UpdatedAt = DateTime.UtcNow;
                            obj.UpdatedById = userId;
                            repoOrderDetail.Update(obj);
                        }
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        if (orderHeader.CreatedAt.Date == DateTime.UtcNow.Date)
                            throw new NotFoundException(ErrorMessages.DocumentHasBeenChanged.ToString(CultureInfo.CurrentCulture));

                        await PostAuditRecord(requestModel.OrderNo, "BtnOKClick", userId, requestModel.OutletId, requestModel.SupplierId, requestModel.TypeId, orderHeader.StatusId, requestModel.StatusId).ConfigureAwait(false);

                        orderHeader.StatusId = requestModel.StatusId;
                        orderHeader.TypeId = requestModel.TypeId;
                        orderHeader.CreatedAt = DateTime.UtcNow;
                        orderHeader.CreatedById = userId;
                        orderHeader.Reference = requestModel.ReferenceNumber;
                        repoOrderHeader.Update(orderHeader);
                        foreach (var obj in orderHeader.OrderDetail)
                        {
                            obj.CreatedAt = DateTime.UtcNow;
                            obj.CreatedById = userId;
                            repoOrderDetail.Update(obj);
                        }
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }
                    return await GetOrderById(orderHeader.Id).ConfigureAwait(false);
                }
                else
                {
                    throw new NullReferenceException();
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
        public async Task<List<OrderDetailViewModel>> RefreshOrder(List<OrderDetailRefreshRequestModel> productList, int outletId, long orderNo, int? supplierId = null)
        {
            var orderDetailResponseList = new List<OrderDetailViewModel>();
            if (productList != null)
            {
                var repoTransaction = _unitOfWork?.GetRepository<Transaction>();
                var repoOutletProduct = _unitOfWork?.GetRepository<OutletProduct>();
                var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                var repoOrderDetail = _unitOfWork?.GetRepository<OrderDetail>();
                var repoProduct = _unitOfWork?.GetRepository<Product>();
                var repoDepartment = _unitOfWork?.GetRepository<Department>();

                foreach (var item in productList)
                {
                    if (item.ProductId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.ProductIdReq.ToString(CultureInfo.CurrentCulture));
                    }

                    var product = await repoProduct.GetAll(x => x.Id == item.ProductId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (product == null)
                    {
                        throw new BadRequestException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture) + $" : { item.ProductId}");
                    }
                    DateTime startDate = DateTime.UtcNow.AddDays(-56).Date;
                    DateTime endDate = DateTime.UtcNow.AddDays(-1).Date;

                    var response = MappingHelpers.Mapping<OrderDetailRefreshRequestModel, OrderDetailViewModel>(item);

                    //response.OnHand = repoOutletProduct.GetAll(x => !x.IsDeleted && x.StoreId == outletId && x.ProductId == item.ProductId &&
                    //  x.SupplierId == supplierId && x.Status == true && x.SkipReorder == false &&
                    //  x.MinOnHand > 0 && x.MinOnHand > x.QtyOnHand)?.FirstOrDefault()?.QtyOnHand ?? 0;
                    ////old system not using supplier check 
                    response.OnHand = repoOutletProduct.GetAll(x => !x.IsDeleted && x.StoreId == outletId && x.ProductId == item.ProductId).Select(x => x.QtyOnHand)
                         ?.FirstOrDefault() ?? 0;

                    response.OnOrder = repoOrderDetail.GetAll().Include(x => x.OrderHeader).Where(x => !x.IsDeleted && !x.OrderHeader.IsDeleted &&
                       x.OrderHeader.OutletId == outletId && x.OrderHeader.OrderNo != orderNo && x.OrderHeader.Type.Code == "ORDER" &&
                       x.ProductId == item.ProductId)?.Sum(x => x.TotalUnits) ?? 0;

                    var tranType = new string[] { "WASTAGE", "ITEMSALE", "CHILDSALE", "ADJUSTMENT" };
                    var transaction = repoTransaction.GetAll(x => !x.IsDeleted && x.OutletId == outletId && x.ProductId == item.ProductId &&
                     x.SupplierId == supplierId && (x.Date >= startDate && x.Date <= endDate) &&
                     tranType.Contains(x.Type));

                    //var deptCode = new string[] { "2", "24", "25" };
                    var department = repoDepartment.GetAll(x => !x.IsDeleted && x.ExcludeWastageOptimalOrdering == true).Select(x => x.Id);

                    float tranQty = 0;
                    if (transaction.Where(x => x.Type == "ITEMSALE").ToList().Count > 0)
                    {
                        tranQty = transaction.Where(x => x.Type == "ITEMSALE").Sum(x => x.Qty);
                    }
                    else if (transaction.Where(x => x.Type == "CHILDSALE").ToList().Count > 0)
                    {
                        if (product.Parent == 0)
                            tranQty = -1 * transaction.Where(x => x.Type == "ITEMSALE").Sum(x => x.StockMovement) ?? 0;
                        else
                            tranQty = (-1 * transaction.Where(x => x.Type == "ITEMSALE").Sum(x => x.StockMovement) ?? 0) * product.UnitQty;
                    }
                    else if (transaction.Where(x => x.Type == "WASTAGE").ToList().Count > 0)
                    {
                        if (transaction.Any(x => department.Contains(x.DepartmentId ?? (int)x.DepartmentId)))
                            tranQty = 0;
                        else if (product.Parent == 0)
                            tranQty = -1 * transaction.Where(x => x.Type == "ITEMSALE").Sum(x => x.StockMovement) ?? 0;
                        else
                            tranQty = (-1 * transaction.Where(x => x.Type == "ITEMSALE").Sum(x => x.StockMovement) ?? 0) * product.UnitQty;
                    }
                    else if (transaction.Where(x => x.Type == "ADJUSTMENT").ToList().Count > 0)
                    {
                        if (transaction.Any(x => department.Contains(x.DepartmentId ?? (int)x.DepartmentId)) && transaction.Where(x => x.Tender == "WASTAGE").ToList().Count > 0)
                            tranQty = 0;
                        else
                            tranQty = -1 * transaction.Where(x => x.Type == "ITEMSALE").Sum(x => x.StockMovement) ?? 0;
                    }
                    var avgDaily = tranQty / Convert.ToSingle((endDate - startDate).TotalDays);

                    //TAutoOrdering.IsANewProduct
                    // Look for 365 days sale transaction instead of 57 days to get whether product is new or not
                    endDate = DateTime.UtcNow.AddDays(-365).Date;
                    if (!(await repoTransaction.GetAll(x => !x.IsDeleted && x.OutletId == outletId && (x.ProductId == item.ProductId || x.ProductId == product.Parent)
                     && (x.Date < endDate) && x.Type == "ITEMSALE").AnyAsync().ConfigureAwait(false)))
                    {
                        // AutoOrdering.GetNewProdAdjustedSales
                        var minDate = await repoTransaction.GetAll(x => !x.IsDeleted && x.OutletId == outletId && x.ProductId == item.ProductId && x.Type == "ITEMSALE")
                            .MinAsync(x => (DateTime?)x.Date).ConfigureAwait(false);
                        if (minDate.HasValue && (minDate.Value.Date - DateTime.UtcNow.Date).TotalDays >= 7)
                        {
                            avgDaily = tranQty / (float)(minDate.Value.Date - DateTime.UtcNow.Date).TotalDays;
                        }
                        else
                        {
                            avgDaily = tranQty / 7;
                        }
                    }

                    // aAvgWeekly := aAvgDaily * 7; //old db saves in ORDL_SUGG_UNITS_AVGWEEKLY, no column in new
                    var orderDetail = await repoOrderDetail.GetAll(x => x.Id == response.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                    orderDetail.OnHand = response?.OnHand ?? 0;
                    orderDetail.OnOrder = response?.OnOrder ?? 0;
                    orderDetail.NonPromoAvgDaily = avgDaily;
                    repoOrderDetail.Update(orderDetail);
                    //
                    response.NonPromoAvgDaily = avgDaily;
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    orderDetailResponseList.Add(response);
                }
            }
            return orderDetailResponseList;
        }
        public async Task<bool> PDELoad(OrderPDERequestModel requestModel)
        {
            try
            {
                if (requestModel != null)
                {
                    var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                    var repoOrderDetail = _unitOfWork?.GetRepository<OrderDetail>();
                    var orderHeader = await repoOrderHeader.GetAll().Include(x => x.OrderDetail).Where(x => !x.IsDeleted && x.OrderNo == requestModel.OrderNo && x.OutletId == requestModel.OutletId).FirstOrDefaultAsync().ConfigureAwait(false);
                    var repositoryStatusAll = _unitOfWork?.GetRepository<MasterListItems>().GetAll().Include(x => x.MasterList);
                    if (requestModel.TypeId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.DocumentTypeRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.StatusId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.DocumentStatusRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    var statusCode = repositoryStatusAll.Where(x => x.Id == requestModel.StatusId && !x.IsDeleted).FirstOrDefault().Code;
                    var typeCode = repositoryStatusAll.Where(x => x.Id == requestModel.TypeId && !x.IsDeleted).FirstOrDefault().Code;
                    if (statusCode.ToLower() != "new")
                    {
                        throw new NullReferenceException(ErrorMessages.DocumentStatusNew.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.OutletId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.SupplierId == null || requestModel.SupplierId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.SupplierRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (orderHeader.OrderDetail.Count == 0)
                    {
                        throw new NotFoundException(ErrorMessages.DocumentProdcutNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    string path = fileDirectorySettings.FolderPath + "PdeFile.txt";
                    if (!File.Exists(path))
                    {
                        throw new NotFoundException(ErrorMessages.PDEFileNotExist.ToString(CultureInfo.CurrentCulture));
                    }
                    return true;
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
        public async Task<List<OrderTabletLoadResponseModel>> TabletLoad(OrderTabletLoadRequestModel requestModel)
        {
            try
            {
                if (requestModel != null)
                {
                    var repoOutletSupplierSetting = _unitOfWork?.GetRepository<OutletSupplierSetting>();
                    var repoOutlet = _unitOfWork?.GetRepository<Store>();
                    var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                    var repoSupplier = _unitOfWork?.GetRepository<Supplier>();
                    var repoBulkOrderFromTablet = _unitOfWork?.GetRepository<BulkOrderFromTablet>();
                    var repositoryStatusAll = _unitOfWork?.GetRepository<MasterListItems>().GetAll().Include(x => x.MasterList);
                    if (requestModel.TypeId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.DocumentTypeRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.StatusId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.DocumentStatusRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.OutletId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.SupplierId == null || requestModel.SupplierId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.SupplierRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    var supplier = repoSupplier.GetAll(x => !x.IsDeleted && x.Id == requestModel.SupplierId).FirstOrDefault();
                    if (supplier == null)
                    {
                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var listCode = repositoryStatusAll.Where(x => x.Id == requestModel.TypeId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERTYPE).FirstOrDefault();
                    if (listCode == null)
                    {
                        throw new NotFoundException(ErrorMessages.TypeNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var listStatus = repositoryStatusAll.Where(x => x.Id == requestModel.StatusId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERSTATUS).FirstOrDefault();
                    if (listStatus == null)
                    {
                        throw new NotFoundException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var statusCode = repositoryStatusAll.Where(x => x.Id == requestModel.StatusId && !x.IsDeleted).FirstOrDefault().Code;
                    var typeCode = repositoryStatusAll.Where(x => x.Id == requestModel.TypeId && !x.IsDeleted).FirstOrDefault().Code;
                    if (statusCode.ToLower() != "new")
                    {
                        throw new NullReferenceException(ErrorMessages.DocumentStatusNew.ToString(CultureInfo.CurrentCulture));
                    }

                    var outletSupplierDetails = repoOutletSupplierSetting.GetAll().Include(x => x.Supplier).Where(x => !x.IsDeleted && x.StoreId == requestModel.OutletId && x.SupplierId == requestModel.SupplierId).FirstOrDefault();
                    if (outletSupplierDetails != null)
                    {
                        if (outletSupplierDetails.QtyDefault == null)
                        {
                            outletSupplierDetails.QtyDefault = "SINGLES";
                        }
                        else if (outletSupplierDetails.DivisionId == 0)
                        {
                            outletSupplierDetails.DivisionId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "D" && x.MasterList.Code == "DIVISION").FirstOrDefault().Id;
                        }
                        repoOutletSupplierSetting.Update(outletSupplierDetails);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }

                    var list = await (from tablet in repoBulkOrderFromTablet.GetAll()
                                      join outlet in repoOutlet.GetAll() on tablet.OutletId equals outlet.Id
                                      where !tablet.IsDeleted && tablet.OutletId == requestModel.OutletId && tablet.Type == CommonMessages.OrderTabletLoad
                                      group new { outlet, tablet } by new
                                      {
                                          OutletId = tablet.OutletId,
                                          Code = outlet.Code,
                                          Desc = outlet.Desc,
                                          OrderBatch = tablet.OrderBatch,
                                          LastImport = tablet.LastImport
                                      }).Select(g => new OrderTabletLoadResponseModel
                                      {
                                          OutletId = g.Key.OutletId,
                                          OutletCode = g.Key.Code,
                                          OutletDesc = g.Key.Desc,
                                          OrderBatch = g.Key.OrderBatch,
                                          LastImport = g.Key.LastImport
                                      }).ToListAsyncSafe().ConfigureAwait(false);

                    return list;
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
        public async Task<bool> UpliftRecalcOrder(OrderUpliftRecalcRequestModel requestModel)
        {
            try
            {
                if (requestModel != null)
                {
                    int dayOfWeek = (int)requestModel.OrderDate.DayOfWeek;
                    //week start from Sunday in DB data and in above line of code week starts on Monday
                    dayOfWeek = dayOfWeek + 1;
                    dayOfWeek = dayOfWeek > 7 ? 0 : dayOfWeek;
                    var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                    var repoSupplier = _unitOfWork?.GetRepository<Supplier>();
                    var repoOutletSupplierSchedule = _unitOfWork?.GetRepository<SupplierOrderingSchedule>();
                    var orderHeader = await repoOrderHeader.GetAll().Include(x => x.OrderDetail).Where(x => !x.IsDeleted && x.OrderNo == requestModel.OrderNo && x.OutletId == requestModel.OutletId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (requestModel.OutletId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.SupplierId == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.SupplierRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    var supplier = repoSupplier.GetAll(x => !x.IsDeleted && x.Id == requestModel.SupplierId).FirstOrDefault();
                    if (supplier == null)
                    {
                        throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.Type != "Recalc" && requestModel.UpliftFactor <= 0 || requestModel.UpliftFactor > 100)
                    {
                        throw new NullReferenceException(ErrorMessages.OrderUpliftFactor.ToString(CultureInfo.CurrentCulture));
                    }
                    var outletSupplierScheduleList = await repoOutletSupplierSchedule.GetAll(x => !x.IsDeleted && x.StoreId == requestModel.OutletId
                    && x.SupplierId == requestModel.SupplierId && x.DOWGenerateOrder == dayOfWeek
                                                               ).ToListAsyncSafe().ConfigureAwait(false);
                    if (outletSupplierScheduleList.Count == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.SupplierScheduleMatching.ToString(CultureInfo.CurrentCulture));
                    }
                    if (requestModel.EndDate == null)
                    {
                        requestModel.EndDate = requestModel.OrderDate.AddDays(-1);
                    }
                    //StartDate and EndDate is mandatory from Front end //Manoj sir
                    //DateTime salesEndDate = requestModel.OrderDate.AddDays(-1);
                    //DateTime salesStartDate;
                    //if (requestModel.Type == "Recalc")
                    //{
                    //    if (requestModel.StartDate == null)
                    //    {
                    //        requestModel.StartDate = requestModel.EndDate?.AddDays(-28); //use 28 instead of 56 //Manoj sir
                    //    }
                    //}
                    //else
                    //{
                    //    requestModel.StartDate = requestModel.EndDate?.AddDays(-112);
                    //}
                    //int coverDays = 0;
                    var outletSupplierSchedule = outletSupplierScheduleList.FirstOrDefault();
                    //if (outletSupplierSchedule.MultipleOrdersInAWeek)
                    //{
                    //    coverDays = outletSupplierSchedule.CoverDays;
                    //}
                    //else
                    //{
                    //    coverDays = 7 + outletSupplierSchedule.ReceiveOrderOffset;
                    //}
                    //if (requestModel.Type != "Recalc")
                    //{
                    //    coverDays = coverDays + Convert.ToInt32(Math.Round(coverDays * (requestModel.UpliftFactor / 100)));
                    //}
                    outletSupplierSchedule.CoverDays = requestModel.CoverDays;
                    outletSupplierSchedule.ReceiveOrderOffset = requestModel.CoverDays - 7;
                    orderHeader.CoverDays = requestModel.CoverDays;
                    //repoOutletSupplierSchedule.Update(outletSupplierSchedule);//not to be updated in DB //Manoj sir
                    orderHeader.CoverDays = requestModel.CoverDays;
                    repoOrderHeader.Update(orderHeader);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                    //TODO: Pendings as shared by Manoj sir
                    /**
                     * // No sales last 28 days OR no orders for 28 days for national range 1 and prod not in order
                        DoNoNatRangeSalesOrder
                        // items about to go on sale promo No sales last 28 days OR no orders for 28 days and prod not in order
                        DoNoSalePromoSalesOrder
                        Pending , we will cover this later
                    **/

                    //AutoOrdering functionalty discussion with Manoj Sir & Vikas Sir
                    //Not

                    /**
                     	exec [CDN_SP__DoOptimalOrder] @vShowOptimalReport=0,
	@aRunDate='2020-11-30',@aSalesStartDate='2020-11-02',@aSalesEndDate='2020-11-29',@aDoRecalc=1,@OSS_Outlet=91,@OSS_MultipleOrdersInAWeek=0,@OSS_DOWGenerateOrder=2
	,@OSS_Supplier=3,@OSS_CoverDays=9,@OSS_InvoiceOrderOffset=1,@OSS_ReceiveOrderOffset=2,@OSS_DiscountThresholdThree=30,@OSS_CoverDaysDiscountThreshold3=84,
	@OSS_DiscountThresholdTwo=20,@OSS_CoverDaysDiscountThreshold2=56,@OSS_DiscountThresholdOne=10,@OSS_CoverDaysDiscountThreshold1=28,@aOrderNum=1,@Debug=1,
	@OSS_Id=1221,@OSS_OrderNonDefaultSupplier=3
                    **/
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@vShowOptimalReport", false),
                    new SqlParameter("@aRunDate", requestModel.OrderDate),
                    new SqlParameter("@aSalesStartDate", requestModel.StartDate),
                    new SqlParameter("@aSalesEndDate", requestModel.EndDate),
                    new SqlParameter("@aDoRecalc", true),
                    new SqlParameter("@OSS_Outlet", requestModel.OutletId),
                    new SqlParameter("@OSS_MultipleOrdersInAWeek", outletSupplierSchedule.MultipleOrdersInAWeek),
                    new SqlParameter("@OSS_DOWGenerateOrder", outletSupplierSchedule.DOWGenerateOrder),
                    new SqlParameter("@OSS_Supplier", requestModel.SupplierId),
                    new SqlParameter("@aOrderNum", requestModel.OrderNo),
                    new SqlParameter("@OSS_CoverDays",requestModel.CoverDays),
                    new SqlParameter("@OSS_InvoiceOrderOffset", outletSupplierSchedule.InvoiceOrderOffset),
                    new SqlParameter("@OSS_ReceiveOrderOffset", outletSupplierSchedule.ReceiveOrderOffset),
                    new SqlParameter("@OSS_DiscountThresholdThree", outletSupplierSchedule.DiscountThresholdThree),
                    new SqlParameter("@OSS_CoverDaysDiscountThreshold3", outletSupplierSchedule.CoverDaysDiscountThreshold3),
                    new SqlParameter("@OSS_DiscountThresholdTwo", outletSupplierSchedule.DiscountThresholdTwo),
                    new SqlParameter("@OSS_CoverDaysDiscountThreshold2", outletSupplierSchedule.CoverDaysDiscountThreshold2),
                    new SqlParameter("@OSS_DiscountThresholdOne", outletSupplierSchedule.DiscountThresholdOne),
                    new SqlParameter("@OSS_CoverDaysDiscountThreshold1",outletSupplierSchedule.CoverDaysDiscountThreshold1),
                    new SqlParameter("@OSS_Id", outletSupplierSchedule.Id),
                    new SqlParameter("@OSS_OrderNonDefaultSupplier", outletSupplierSchedule.OrderNonDefaultSupplier),
                    new SqlParameter("@Debug", 1),

                };

                    var dset = await repoOrderHeader.ExecuteStoredProcedure(StoredProcedures.CDNOptimalOrderCalc, dbParams.ToArray()).ConfigureAwait(false);

                    //return await GetOrderById(orderHeader.Id).ConfigureAwait(false);
                    return true;
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
        private async Task MarkOrderAsSent(long orderNo, int outletId, int userId, string modelName, string referenceNumber = null)
        {
            try
            {
                var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                var repoOrderDetail = _unitOfWork?.GetRepository<OrderDetail>();
                var repositoryStatusAll = _unitOfWork?.GetRepository<MasterListItems>().GetAll().Include(x => x.MasterList);

                var orderHeader = await repoOrderHeader.GetAll().Include(x => x.OrderDetail).Where(x => !x.IsDeleted && x.OrderNo == orderNo && x.OutletId == outletId).FirstOrDefaultAsync().ConfigureAwait(false);
                orderHeader.StatusId = repositoryStatusAll.FirstOrDefault(x => !x.IsDeleted && x.MasterList.Code == "OrderDocStatus" && x.Code == "ORDER").Id;
                orderHeader.PostedDate = DateTime.UtcNow;
                if (modelName == "post")
                {
                    orderHeader.CreatedAt = DateTime.UtcNow;
                    orderHeader.CreatedById = userId;
                    orderHeader.Reference = referenceNumber;
                }
                repoOrderHeader.Update(orderHeader);

                foreach (var obj in orderHeader.OrderDetail)
                {
                    obj.PostedDate = DateTime.UtcNow;
                    obj.DeliverCartons = obj.Cartons;
                    obj.DeliverUnits = obj.Units;
                    obj.DeliverTotalUnits = obj.TotalUnits;
                    if (modelName == "post")
                    {
                        obj.CreatedAt = DateTime.UtcNow;
                        obj.CreatedById = userId;
                    }
                    repoOrderDetail.Update(obj);
                }

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        private async Task PostAuditRecord(long orderNo, string action, int userId, int? outletId = null, int? supplierId = null, int? typeId = null, int? statusId = null, int? newStatusId = null)
        {
            try
            {
                var repoOrderAudit = _unitOfWork?.GetRepository<OrderAudit>();
                var repoSupplier = _unitOfWork?.GetRepository<Supplier>();

                var orderAudit = new OrderAudit();
                orderAudit.OutletId = outletId ?? null;
                orderAudit.OrderNo = orderNo;
                orderAudit.SupplierId = supplierId;
                orderAudit.TypeId = typeId;
                orderAudit.StatusId = statusId;
                orderAudit.NewStatusId = newStatusId;
                orderAudit.Action = action;
                orderAudit.CreatedAt = DateTime.UtcNow;
                orderAudit.UpdatedAt = DateTime.UtcNow;
                orderAudit.CreatedById = userId;
                orderAudit.UpdatedById = userId;
                await repoOrderAudit.InsertAsync(orderAudit).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (BadRequestException br)
            {
                throw new BadRequestException(br.Message, br);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        public async Task<AutomaticOrderResponseModel> AutomaticOrder(AutomaticOrderRequestModel viewModel, int userId)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OrderHeader>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                    new SqlParameter("@StoreId", viewModel?.StoreId),
                    new SqlParameter("@SupplierId", viewModel?.SupplierId),
                    new SqlParameter("@AltDirectSupplierId", viewModel?.AltSupplierId),
                    new SqlParameter("@OrderType", viewModel?.OrderType),
                    new SqlParameter("@IgnoreStockLevel", viewModel?.IngnoreStockLevel),
                    new SqlParameter("@HistoryDays", viewModel?.DaysHistory),
                    new SqlParameter("@ExcludePromo", viewModel?.ExcludePromo),
                    new SqlParameter("@CoverDays", viewModel?.CoverDays),
                    new SqlParameter("@DiscountThreshold", viewModel?.DiscountThreshold),
                    new SqlParameter("@OrderNumber", viewModel?.ExistingOrderNo),
                    new SqlParameter("@NewOrderNumber", await GetNewOrderNumber(viewModel.StoreId).ConfigureAwait(false)),
                    new SqlParameter("@Normal", viewModel?.MetcashNormal),
                    new SqlParameter("@variety", viewModel?.MetcashVariety),
                    new SqlParameter("@SlowMoving", viewModel?.MetcashSlow),
                    new SqlParameter("@CompareDirectSupplier", viewModel?.CompareDirectSuppliers),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@InvestmentBuyDays", viewModel?.InvestmentBuyDays),
                    new SqlParameter("@DepartmentIds", viewModel?.DepartmentIds),
                    new SqlParameter("@ProductId",viewModel?.ProductId)
                };

                var autoOrderResponse = new AutomaticOrderResponseModel();

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.AutomaticOrder, dbParams.ToArray()).ConfigureAwait(false);

                if (dset.Tables.Count > 0)
                {
                    autoOrderResponse.OrderHeaders = MappingHelpers.ConvertDataTable<OrderHeaderResponseViewModel>(dset.Tables[0]).FirstOrDefault();
                }
                if (dset.Tables.Count > 1)
                {
                    List<OrderDetailViewModel> orderDetail = MappingHelpers.ConvertDataTable<OrderDetailViewModel>(dset.Tables[1]);

                    autoOrderResponse.OrderDetails.AddRange(orderDetail);
                }
                if (dset.Tables.Count > 2)
                {
                    List<InvestmentBuyProducts> IncludeProd = MappingHelpers.ConvertDataTable<InvestmentBuyProducts>(dset.Tables[2]);
                    autoOrderResponse.IncludedProducts.AddRange(IncludeProd);
                }
                if (dset.Tables.Count > 3)
                {
                    List<InvestmentBuyProducts> ExcludeProd = MappingHelpers.ConvertDataTable<InvestmentBuyProducts>(dset.Tables[3]);
                    autoOrderResponse.IncludedProducts.AddRange(ExcludeProd);
                }
                //save Auto Order Settings in DB
                if (viewModel.StoreId > 0 && viewModel.SupplierId > 0)
                {
                    var aosRepo = _unitOfWork?.GetRepository<AutoOrderSettings>();
                    var aos = aosRepo.GetAll().Where(x => !x.IsDeleted && x.StoreId == viewModel.StoreId && x.SupplierId == viewModel.SupplierId).FirstOrDefault();
                    if (aos == null)
                    {
                        AutoOrderSettings orderSettings = new AutoOrderSettings
                        {
                            StoreId = viewModel.StoreId,
                            SupplierId = viewModel.SupplierId,
                            CoverDays = viewModel.CoverDays,
                            HistoryDays = viewModel.DaysHistory,
                            InvestmentBuyDays = viewModel.InvestmentBuyDays ?? 0,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            CreatedById = userId,
                            UpdatedById = userId
                        };
                        await aosRepo.InsertAsync(orderSettings).ConfigureAwait(false);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        aos.CoverDays = viewModel.CoverDays;
                        aos.HistoryDays = viewModel.DaysHistory;
                        aos.InvestmentBuyDays = viewModel.InvestmentBuyDays ?? aos.InvestmentBuyDays;
                        aos.IsDeleted = false;
                        aos.UpdatedById = userId;
                        aosRepo.DetachLocal(_ => _.Id == aos.Id);
                        aosRepo.Update(aos);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                return autoOrderResponse;
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

        public async Task<int> GetCoverDaysForOutletSupplier(int outletId, int supplierId)
        {
            try
            {
                var repoOutletSupplierSchedule = _unitOfWork?.GetRepository<SupplierOrderingSchedule>();
                var schedule = await repoOutletSupplierSchedule.GetAll(x => x.StoreId == outletId && x.SupplierId == supplierId && !x.IsDeleted)
                     .FirstOrDefaultAsync().ConfigureAwait(false);
                if (schedule == null)
                {
                    throw new NotFoundException();
                }
                int coverDays = 0;
                if (schedule.MultipleOrdersInAWeek)
                {
                    coverDays = schedule.CoverDays;
                }
                else
                {
                    coverDays = 7 + schedule.ReceiveOrderOffset;
                }
                return coverDays;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.NoOutletSupplierSetting.ToString(CultureInfo.CurrentCulture));
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

        public async Task<OrderDetailResponseViewModel> PostOrderClick(OrderPostRequestModel requestModel, int userId)
        {
            try
            {
                /****/
                if (requestModel == null)
                {
                    throw new NullReferenceException();
                }
                var repoOrderHeader = _unitOfWork?.GetRepository<OrderHeader>();
                var repoOrderDetail = _unitOfWork?.GetRepository<OrderDetail>();
                var repoSupplier = _unitOfWork?.GetRepository<Supplier>();
                var repositoryStatusAll = _unitOfWork?.GetRepository<MasterListItems>().GetAll().Include(x => x.MasterList);
                var repoOP = _unitOfWork?.GetRepository<OutletProduct>();
                #region General Validations
                if (requestModel.OutletId == 0)
                    throw new NullReferenceException(ErrorMessages.OutletRequired.ToString(CultureInfo.CurrentCulture));

                if (requestModel.TypeId == 0)
                    throw new NullReferenceException(ErrorMessages.DocumentTypeRequired.ToString(CultureInfo.CurrentCulture));

                if (requestModel.StatusId == 0)
                    throw new NullReferenceException(ErrorMessages.DocumentStatusRequired.ToString(CultureInfo.CurrentCulture));


                var orderHeader = await repoOrderHeader.GetAll().Include(x => x.OrderDetail).ThenInclude(p => p.Product).ThenInclude(t => t.Tax).Include(s => s.Store).
                    Where(x => !x.IsDeleted && x.OrderNo == requestModel.OrderNo && x.OutletId == requestModel.OutletId).FirstOrDefaultAsync().ConfigureAwait(false);
                if (orderHeader == null)
                    throw new NotFoundException(ErrorMessages.OrderNotFound.ToString(CultureInfo.CurrentCulture));


                var listCode = repositoryStatusAll.Where(x => x.Id == requestModel.TypeId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERDOCTYPE).FirstOrDefault();
                if (listCode == null)
                    throw new NotFoundException(ErrorMessages.TypeNotFound.ToString(CultureInfo.CurrentCulture));



                if (listCode.Code == "TRANSFER" || (listCode.Code == "INVOICE" && repositoryStatusAll.Any(x => x.Id == requestModel.CreationTypeId && !x.IsDeleted &&
                       x.MasterList.Code == MasterListCode.OrderCreationType && x.Code == "TRANSFER")))
                {
                    //No need to check the supplier // as it can be done
                }
                else if ((requestModel.SupplierId == null || requestModel.SupplierId == 0))
                    throw new NullReferenceException(ErrorMessages.SupplierRequired.ToString(CultureInfo.CurrentCulture));

                var supplier = repoSupplier.GetAll(x => !x.IsDeleted && x.Id == requestModel.SupplierId).FirstOrDefault();
                if (listCode.Code == "TRANSFER" || (listCode.Code == "INVOICE" && repositoryStatusAll.Any(x => x.Id == requestModel.CreationTypeId && !x.IsDeleted &&
                      x.MasterList.Code == MasterListCode.OrderCreationType && x.Code == "TRANSFER")))
                {
                    //No need to check the supplier // as it can be done
                }
                else if (supplier == null)
                    throw new NotFoundException(ErrorMessages.SupplierNotFound.ToString(CultureInfo.CurrentCulture));


                var listStatus = repositoryStatusAll.Where(x => x.Id == requestModel.StatusId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault();
                if (listStatus == null)
                    throw new NotFoundException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));

                if (!await repositoryStatusAll.AnyAsync(x => x.Id == requestModel.CreationTypeId && !x.IsDeleted && x.MasterList.Code == MasterListCode.OrderCreationType).ConfigureAwait(false))
                    throw new AlreadyExistsException(ErrorMessages.CreationTypeNotFound.ToString(CultureInfo.CurrentCulture));

                if (listCode.Code == "CREDIT")
                    throw new NullReferenceException(ErrorMessages.DocumentTypeCREDIT.ToString(CultureInfo.CurrentCulture));

                #endregion
                /**
                 * DELPHI CODE
                 *  OrderTotal := OrderLineTotal - fOrderHeaderTbl.ORDH_SUBTOT_DISC.TypedColValue - fOrderHeaderTbl.ORDH_SUBTOT_SUBSIDY.TypedColValue +
                    fOrderHeaderTbl.ORDH_SUBTOT_FREIGHT.TypedColValue + fOrderHeaderTbl.ORDH_SUBTOT_ADMIN.TypedColValue +
                    fOrderHeaderTbl.ORDH_SUBTOT_TAX.TypedColValue;
                **/
                var orderLineTotal = (float)Math.Round(orderHeader.OrderDetail.Where(x => !x.IsDeleted).Sum(x => x.LineTotal), 2);
                float orderTotal = orderLineTotal - requestModel?.LessDisccount ?? 0 - requestModel?.LessSubsidy ?? 0 + requestModel?.PlusFreight ?? 0 + requestModel?.PlusAdmin ?? 0 + requestModel?.PlusGst ?? 0;

                var SuppUpdCostOpt = (supplier?.UpdateCost == "yes");
                if (listCode.Code == "INVOICE" || listCode.Code == "CREDIT")
                {
                    //DocumentTotalsBalance check is handled at front End
                    //if (requestModel.InvoiceTotalAmount != orderTotal)
                    //    throw new NotFoundException(ErrorMessages.DocumentTotalsBalance.ToString(CultureInfo.CurrentCulture));

                    if (string.IsNullOrEmpty(requestModel.InvoiceNo))
                        throw new NotFoundException(ErrorMessages.DocumentNumberRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.InvoiceDate == null || requestModel.InvoiceDate == DateTime.MinValue)
                        throw new NotFoundException(ErrorMessages.DocumentDateRequired.ToString(CultureInfo.CurrentCulture));

                    var checkInvoiceNo = await repoOrderHeader.GetAll().Include(x => x.Supplier)
                        .Where(x => !x.IsDeleted && x.OutletId == requestModel.OutletId &&
                               x.OrderNo != requestModel.OrderNo && x.SupplierId == requestModel.SupplierId &&
                               x.InvoiceNo == requestModel.InvoiceNo).ToListAsyncSafe().ConfigureAwait(false);

                    if (checkInvoiceNo.Count > 0)
                        throw new NotFoundException(ErrorMessages.InvoiceNumberAlreadyExists.ToString(CultureInfo.CurrentCulture));

                    orderHeader.InvoiceNo = requestModel.InvoiceNo;
                    orderHeader.InvoiceDate = requestModel.InvoiceDate;
                }

                if (listCode.Code == "DELIVERY")
                {
                    if (string.IsNullOrEmpty(requestModel.DeliveryNo))
                        throw new NotFoundException(ErrorMessages.DocumentNumberRequired.ToString(CultureInfo.CurrentCulture));

                    if (requestModel.DeliveryDate == null || requestModel.DeliveryDate == DateTime.MinValue)
                        throw new NotFoundException(ErrorMessages.DocumentDateRequired.ToString(CultureInfo.CurrentCulture));

                    orderHeader.DeliveryNo = requestModel.DeliveryNo;
                    orderHeader.DeliveryDate = requestModel.DeliveryDate;
                }

                // old system does entry first here with BtnPostClick and then with BtnPostClick 2nd for invoice 
                await PostAuditRecord(requestModel.OrderNo, "BtnPostClick", userId, requestModel.OutletId, supplier?.Id,
                    requestModel.TypeId, orderHeader.StatusId, null).ConfigureAwait(false);

                //If order type is Transfer then check stock on hand values for the products in StoreAsSupplier and throw error if SOH-OrderValue<=0
                if (listCode.Code == "TRANSFER")
                {
                    var repoStore = _unitOfWork?.GetRepository<Store>();
                    var storeCodeAsSupplier = repoStore.GetAll().Where(x => x.Id == (orderHeader.StoreIdAsSupplier ?? 0) && !x.IsDeleted).FirstOrDefault()?.Code;
                    if (!orderHeader.StoreIdAsSupplier.HasValue || string.IsNullOrEmpty(storeCodeAsSupplier))
                    {
                        throw new BadRequestException(ErrorMessages.TransferOrderStoreAsSupplier.ToString(CultureInfo.CurrentCulture));
                    }


                    foreach (var detail in orderHeader.OrderDetail.Where(x => !x.IsDeleted))
                    {
                        var supplierOutletProduct = await repoOP.GetAll().Where(x => !x.IsDeleted && x.StoreId == orderHeader.StoreIdAsSupplier
                        && x.ProductId == detail.ProductId).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                        if (supplierOutletProduct.QtyOnHand <= 0 || supplierOutletProduct.QtyOnHand <= detail.TotalUnits)
                        {
                            var repoProduct = _unitOfWork?.GetRepository<Product>();
                            throw new BadRequestException(string.Format(ErrorMessages.NotEnoughStockForTransfer.ToString(CultureInfo.CurrentCulture),
                                repoProduct.GetAll().FirstOrDefault(x => !x.IsDeleted && x.Id == detail.ProductId)?.Number.ToString() ?? "",
                                storeCodeAsSupplier, supplierOutletProduct.QtyOnHand));
                        }
                    }
                }

                if (listCode.Code == "ORDER")
                {
                    orderHeader.StatusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == listCode.Code && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault().Id;
                    orderHeader.PostedDate = DateTime.UtcNow;
                    orderHeader.Reference = requestModel.ReferenceNumber;
                    orderHeader.CreatedAt = DateTime.UtcNow;
                    orderHeader.CreatedById = userId;
                    repoOrderHeader.Update(orderHeader);
                    if (!await _unitOfWork.SaveChangesAsync().ConfigureAwait(false))
                    {
                        throw new BadRequestException(ErrorMessages.OrderNotSaved.ToString(CultureInfo.CurrentCulture));
                    }
                }

                if (listCode.Code == "DELIVERY" || listCode.Code == "INVOICE" || listCode.Code == "TRANSFER")
                {
                    int oldStatusId = orderHeader.StatusId;
                    if (listCode.Code == "TRANSFER")
                        orderHeader.StatusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "INVOICE" && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault().Id;
                    else
                        orderHeader.StatusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == listCode.Code && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault().Id;

                    #region Update listStatus.Code to above selected status
                    //
                    listStatus = repositoryStatusAll.Where(x => x.Id == orderHeader.StatusId && !x.IsDeleted && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault();
                    if (listStatus == null)
                        throw new NotFoundException(ErrorMessages.StatusNotFound.ToString(CultureInfo.CurrentCulture));
                    #endregion

                    //Update Document 
                    #region Update Document 
                    float CostVarFact = 1;
                    float CostVarAmt = orderTotal - orderLineTotal;
                    if (orderLineTotal != 0 && CostVarAmt != 0)
                    {
                        CostVarFact = (CostVarAmt / orderLineTotal) + 1;
                    }
                    float GSTTotal = 0;
                    foreach (var detail in orderHeader.OrderDetail.Where(x => !x.IsDeleted))
                    {
                        detail.FinalLineTotal = (float)Math.Round(detail.LineTotal * CostVarFact, 2);
                        detail.FinalCartonCost = (float)Math.Round((double)(detail.CartonCost * CostVarFact), 2);
                        //Final line GST column missing here in model
                        GSTTotal = GSTTotal + ((detail.FinalLineTotal ?? 0) * detail.Product.Tax.Factor) / 100;

                        //UpdateDocumentLine
                        // Decrement stock in case of stock transfer
                        #region UpdateDocumentLine
                        if (!detail.Product.Status) { detail.Product.Status = true; }
                        var outletProduct = repoOP.GetAll().Where(x => !x.IsDeleted && x.StoreId == requestModel.OutletId && x.ProductId == detail.ProductId)
                            .FirstOrDefault();

                        //Activate product if inactive
                        if (outletProduct == null)
                        {
                            //can't add a product in order that is not in the outlet
                            throw new NullReferenceException();
                        }
                        else
                        {
                            outletProduct.Status = true;
                            repoOP.Update(outletProduct);
                        }

                        float onHand = outletProduct.QtyOnHand;
                        float stockMovement = detail.TotalUnits;
                        float newOnHand = 0;
                        if (listStatus.Code == "DELIVERY" || listStatus.Code == "INVOICE")
                        {
                            newOnHand = onHand + stockMovement;
                        }
                        if (listStatus.Code == "CREDIT")
                        {
                            newOnHand = onHand - stockMovement;
                        }

                        //  If you are creating a Transfer and the SOH is ZERO or the SOH after Transfer becomes less than ZERO,
                        //  the Process needs to STOP, this process cant run if the SOH is 0 or less, or will become less than 0.
                        if (listCode.Code == "TRANSFER")
                        {
                            newOnHand = onHand - Math.Abs(stockMovement); //  StockMovement is already -ve value in case of stock transfer
                        }
                        //update stock on hand
                        outletProduct.QtyOnHand = newOnHand;

                        //: LABEL AND SKIP REORDER CODE //insert code here in outlet product table
                        // as non existing product can't be added in order so skipping insertion // discussed with Manoj sir

                        if (detail.FinalCartonCost > 0)
                        {
                            outletProduct.CartonCostInv = detail.FinalCartonCost;
                        }
                        if (outletProduct.CartonCost == 0)
                            outletProduct.CartonCost = detail.FinalCartonCost ?? 0;

                        if (SuppUpdCostOpt && (detail.FinalCartonCost ?? 0) > 0)
                            outletProduct.CartonCost = detail.FinalCartonCost ?? 0;

                        if (detail.Product.Parent != null && detail.Product != null && outletProduct.CartonCost > 0)
                        {
                            #region UpdChildCosts
                            //get parent outletProduct
                            var repoProd = _unitOfWork?.GetRepository<Product>();
                            var childProds = repoProd.GetAll().Where(x => !x.IsDeleted && x.Parent == detail.Product.Number);
                            //Update all child prods in outlet prod  table
                            var childOP = repoOP.GetAll(x => !x.IsDeleted && x.StoreId == requestModel.OutletId && childProds.Select(y => y.Id).Contains(x.ProductId));
                            foreach (var child in childOP)
                            {
                                child.CartonCost = detail.FinalCartonCost ?? 0;
                                repoOP.Update(child);
                            }
                            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                            #endregion
                        }

                        repoOP.Update(outletProduct);
                        if (outletProduct.FifoStock == true && listStatus.Code == "INVOICE")
                        {
                            //TODO: INSERT FIFO INVOICE //NO FIFO INVOICE TABLE
                        }

                        #endregion

                        detail.PostedDate = DateTime.UtcNow;
                        //Master.ORDLTBLORDL_FINAL_GST_AMT.value := LineTotalGst;

                        if (listStatus.Code == "CREDIT")
                        {
                            detail.LineTotal = -1 * detail.LineTotal;
                            detail.TotalUnits = -1 * detail.TotalUnits;
                            detail.FinalLineTotal = -1 * detail.FinalLineTotal;
                            //detail.FinalCartonCost = -1 * detail.FinalCartonCost;
                        }
                        if (listCode.Code == "TRANSFER")
                        {
                            detail.LineTotal = -1 * Math.Abs(detail.LineTotal);
                            detail.TotalUnits = -1 * Math.Abs(detail.TotalUnits);
                            detail.Units = -1 * Math.Abs(detail.Units);
                            detail.FinalLineTotal = -1 * Math.Abs(detail.FinalLineTotal ?? 0);
                        }
                        if (listStatus.Code == "DELIVERY")
                        {
                            detail.DeliverCartons = detail.Cartons;
                            detail.DeliverUnits = detail.Units;
                            detail.DeliverTotalUnits = detail.TotalUnits;
                            detail.FinalLineTotal = detail.FinalLineTotal;
                            detail.FinalCartonCost = detail.FinalCartonCost;
                        }

                    }
                    orderHeader.PostedDate = DateTime.UtcNow;
                    orderHeader.Reference = requestModel.ReferenceNumber;
                    orderHeader.CreatedAt = DateTime.UtcNow;
                    orderHeader.CreatedById = userId;
                    repoOrderHeader.Update(orderHeader);
                    if (!await _unitOfWork.SaveChangesAsync().ConfigureAwait(false))
                    {
                        throw new BadRequestException(ErrorMessages.OrderNotSaved.ToString(CultureInfo.CurrentCulture));
                    }
                    #endregion

                    orderHeader.PostedDate = DateTime.UtcNow;
                    orderHeader.GstAmt = GSTTotal;
                    //Delphi Code // WTF why do this !!!
                    if (orderHeader.SubTotalTax != 0)
                    {
                        orderHeader.GstAmt = orderHeader.SubTotalTax;
                    }

                    if (listStatus.Code == "CREDIT")
                    {
                        orderHeader.InvoiceTotal = -1 * orderHeader.InvoiceTotal;
                        orderHeader.GstAmt = -1 * orderHeader.GstAmt;
                        orderHeader.SubTotalDisc = -1 * orderHeader.SubTotalDisc;
                        orderHeader.SubTotalSubsidy = -1 * orderHeader.SubTotalSubsidy;
                        orderHeader.SubTotalFreight = -1 * orderHeader.SubTotalFreight;
                        orderHeader.SubTotalAdmin = -1 * orderHeader.SubTotalAdmin;
                        //fOrderHeaderTbl.ORDH_SUBTOT_FINANCE.ActualColValue := fOrderHeaderTbl.ORDH_SUBTOT_FINANCE.TypedColValue * -1;
                        orderHeader.SubTotalTax = -1 * orderHeader.SubTotalTax;
                    }

                    orderHeader.Reference = requestModel.ReferenceNumber;
                    orderHeader.CreatedAt = DateTime.UtcNow;
                    orderHeader.CreatedById = userId;
                    repoOrderHeader.Update(orderHeader);

                    foreach (var obj in orderHeader.OrderDetail.Where(x => !x.IsDeleted))
                    {
                        obj.PostedDate = DateTime.UtcNow;
                        obj.CreatedAt = DateTime.UtcNow;
                        obj.CreatedById = userId;
                        repoOrderDetail.Update(obj);
                    }
                    await PostAuditRecord(requestModel.OrderNo, "BtnPostClick 2nd", userId, requestModel.OutletId, supplier?.Id, requestModel.TypeId, oldStatusId, orderHeader.StatusId).ConfigureAwait(false);

                    if (!await _unitOfWork.SaveChangesAsync().ConfigureAwait(false))
                    {
                        throw new BadRequestException(ErrorMessages.OrderNotSaved.ToString(CultureInfo.CurrentCulture));
                    }
                    // if child item on order then need to trx entries to do stock "adjustments"
                    // INVOICE: Inc parent (single) Dec child (carton)
                    // CREDIT: Dec parent (single) Inc child (carton)
                    //line 2400
                    float vSupplierInvoiceAmt = 0;
                    long vRecipientOutletOrderNo = 0;
                    OrderHeader trfOrderHeader = null;
                    foreach (var detail in orderHeader.OrderDetail.Where(x => !x.IsDeleted))
                    {
                        var trxRepo = _unitOfWork?.GetRepository<Transaction>();
                        float lORDL_TOTAL_UNITS, lOrdl_Units = 0;

                        #region DoChildToParentAdjustments
                        //if current product has any parent 
                        if (detail.Product.Parent != null)
                        {
                            // 30/06/2016  OUTP_QTY_ONHAND for child should always be 0 BUT TRX items should still happen
                            // if child item on order then need to trx entries to do stock "adjustments"
                            // INVOICE: Inc parent (single) Dec child (carton)
                            // CREDIT: Dec parent (single) Inc child (carton)

                            // for manually created orders and then invoicing the "ORDL_TOTAL_UNITS" field will be the
                            // converted line unit total i.e. ctn qty * ctns ordered.
                            // when we receive a metcash electronic inv the "ORDL_TOTAL_UNITS" is the number of ctns
                            // hence we need to allow for both

                            lORDL_TOTAL_UNITS = 0; lOrdl_Units = 0;
                            lORDL_TOTAL_UNITS = detail.TotalUnits;
                            lOrdl_Units = detail.Units;
                            // if diff revert back to metcah ele inv way as that's what's been codes for
                            // i.e. lORDL_TOTAL_UNITS to equal cartons ordered.
                            if (lORDL_TOTAL_UNITS != lOrdl_Units)
                            {
                                lORDL_TOTAL_UNITS = detail.Cartons;
                            }

                            // INVOICE: Inc parent (single) Dec child (carton)
                            // child (carton) -- DEC
                            var outletProduct = repoOP.GetAll().Where(x => !x.IsDeleted && x.StoreId == requestModel.OutletId && x.ProductId == detail.ProductId).FirstOrDefault();
                            if (outletProduct == null) { throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture)); }
                            outletProduct.QtyOnHand = 0;
                            repoOP.Update(outletProduct);
                            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                            var trxChild = new Transaction
                            {
                                Date = DateTime.UtcNow,
                                Type = "CHILD_INV",
                                ProductId = detail.ProductId,
                                OutletId = requestModel.OutletId,
                                TillId = null,
                                Sequence = 0,
                                SupplierId = detail.Product.SupplierId,
                                ManufacturerId = detail.Product.ManufacturerId,
                                Group = detail.Product.GroupId,
                                DepartmentId = detail.Product.DepartmentId,
                                CommodityId = detail.Product.CommodityId,
                                CategoryId = detail.Product.CategoryId,
                                SubRange = null,  //As discussed with manoj sir not using subrange in new Application
                                Reference = "ORDER",
                                CreatedById = userId,
                                UpdatedById = userId,
                                Qty = 0,
                                Amt = 0,
                                ExGSTAmt = 0,
                                Cost = 0,
                                ExGSTCost = 0,
                                Discount = 0,
                                Price = 0,
                                PromoSellId = null,
                                PromoBuyId = null,
                                Weekend = DateTime.UtcNow.EndOfWeek(DayOfWeek.Monday),
                                Day = DateTime.UtcNow.ToString("ddd"),
                                NewOnHand = 0,
                                Member = null,
                                Points = 0,
                                CartonQty = detail.Product.CartonQty,
                                UnitQty = detail.Product.UnitQty,
                                //Parent in transaction table is also Product Number
                                Parent = detail.Product.Parent,
                                StockMovement = -1 * lORDL_TOTAL_UNITS,
                                Tender = "",
                                ManualInd = false,
                                GLAccount = "",
                                GLPostedInd = false,
                                PromoSales = 0,
                                PromoSalesGST = 0,

                            };
                            await trxRepo.InsertAsync(trxChild).ConfigureAwait(false);
                            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                            // Parent (single) -- Inc
                            var repoProduct = _unitOfWork?.GetRepository<Product>();
                            //in DB parent field contains 
                            var parentProduct = repoProduct.GetAll().Where(x => !x.IsDeleted && x.Number == detail.Product.Parent).FirstOrDefault();
                            if (parentProduct == null) { throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture)); }
                            var outletParentproduct = repoOP.GetAll().Where(x => !x.IsDeleted && x.StoreId == requestModel.OutletId && x.ProductId == parentProduct.Id).FirstOrDefault();
                            if (outletParentproduct == null) { throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture)); }

                            outletParentproduct.QtyOnHand = outletParentproduct.QtyOnHand + (lORDL_TOTAL_UNITS * parentProduct.CartonQty);
                            repoOP.Update(outletParentproduct);
                            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                            var trxParent = new Transaction
                            {
                                Date = DateTime.UtcNow,
                                Type = "CHILD_INV",
                                ProductId = parentProduct.Id,
                                OutletId = requestModel.OutletId,
                                TillId = null,
                                Sequence = 0,
                                SupplierId = parentProduct.SupplierId,
                                ManufacturerId = parentProduct.ManufacturerId,
                                Group = parentProduct.GroupId,
                                DepartmentId = parentProduct.DepartmentId,
                                CommodityId = parentProduct.CommodityId,
                                CategoryId = parentProduct.CategoryId,
                                SubRange = null,//As discussed with manoj sir not using subrange in new Application
                                Reference = "ORDER",
                                CreatedById = userId,
                                UpdatedById = userId,
                                Qty = 0,
                                Amt = 0,
                                ExGSTAmt = 0,
                                Cost = 0,
                                ExGSTCost = 0,
                                Discount = 0,
                                Price = 0,
                                PromoSellId = null,
                                PromoBuyId = null,
                                Weekend = DateTime.UtcNow.EndOfWeek(DayOfWeek.Monday),
                                Day = DateTime.UtcNow.ToString("ddd"),
                                NewOnHand = outletParentproduct.QtyOnHand,
                                Member = null,
                                Points = 0,
                                CartonQty = detail.Product.CartonQty,
                                UnitQty = detail.Product.UnitQty,
                                Parent = detail.Product.Parent,
                                StockMovement = lORDL_TOTAL_UNITS * parentProduct.CartonQty,
                                Tender = "",
                                ManualInd = false,
                                GLAccount = "",
                                GLPostedInd = false,
                                PromoSales = 0,
                                PromoSalesGST = 0,

                            };
                            await trxRepo.InsertAsync(trxParent).ConfigureAwait(false);
                            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                        }
                        #endregion

                        // Stock Transfer
                        #region DoStockTransferAdjustments
                        lORDL_TOTAL_UNITS = 0; lOrdl_Units = 0;
                        lORDL_TOTAL_UNITS = detail.TotalUnits;
                        lOrdl_Units = detail.Units;
                        // if diff revert back to metcah ele inv way as that's what's been codes for
                        if (lORDL_TOTAL_UNITS != lOrdl_Units)
                        {
                            lORDL_TOTAL_UNITS = -1 * Math.Abs(detail.Cartons);
                        }
                        var outletProductTrf = repoOP.GetAll().Where(x => !x.IsDeleted && x.StoreId == requestModel.OutletId && x.ProductId == detail.ProductId).FirstOrDefault();
                        if (outletProductTrf == null) { throw new NotFoundException(ErrorMessages.ProductNotFound.ToString(CultureInfo.CurrentCulture)); }

                        var trxs = trxRepo.GetAll().Where(x => !x.IsDeleted && x.OutletId == requestModel.OutletId && x.ProductId == detail.ProductId &&
                        x.Type.ToUpper() == "TRANSFER" && (x.TillId ?? 0) == 0 && x.Date.Date == DateTime.UtcNow.Date);
                        int trxSeq = 0;
                        if (trxs != null && trxs.Any())
                        {
                            trxSeq = trxs?.Max(x => x.Sequence) ?? 0;
                        }
                        trxSeq = trxSeq + 1;
                        //TRANSFER-OutletCode
                        var trxTrf = new Transaction
                        {
                            Date = DateTime.UtcNow,
                            Type = "TRANSFER",
                            ProductId = detail.ProductId,
                            OutletId = requestModel.OutletId,
                            TillId = null,
                            Sequence = trxSeq,
                            SupplierId = detail.Product.SupplierId,
                            ManufacturerId = detail.Product.ManufacturerId,
                            Group = detail.Product.GroupId,
                            DepartmentId = detail.Product.DepartmentId,
                            CommodityId = detail.Product.CommodityId,
                            CategoryId = detail.Product.CategoryId,
                            SubRange = null,  //As discussed with manoj sir not using subrange in new Application
                            Reference = "TRANSFER",
                            CreatedById = userId,
                            UpdatedById = userId,
                            Qty = lORDL_TOTAL_UNITS,
                            Amt = 0,
                            ExGSTAmt = 0,
                            Cost = detail.FinalLineTotal ?? 0,
                            ExGSTCost = GSTTotal,
                            Discount = 0,
                            Price = 0,
                            PromoSellId = null,
                            PromoBuyId = null,
                            Weekend = DateTime.UtcNow.EndOfWeek(DayOfWeek.Monday),
                            Day = DateTime.UtcNow.ToString("ddd"),
                            NewOnHand = outletProductTrf.QtyOnHand,
                            Member = null,
                            Points = 0,
                            CartonQty = detail.Product.CartonQty,
                            UnitQty = detail.Product.UnitQty,
                            Parent = detail.Product.Parent,
                            StockMovement = lORDL_TOTAL_UNITS,
                            Tender = "TRANSFER-" + orderHeader.Store.Code,// TRANSFER-792
                            ManualInd = false,
                            GLAccount = "",
                            GLPostedInd = false,
                            PromoSales = 0,
                            PromoSalesGST = 0,

                        };
                        // Make values +ve in case of invoice oder which is created by stock transfer
                        if (listCode.Code == "INVOICE" && repositoryStatusAll.Any(x => x.Id == requestModel.CreationTypeId && !x.IsDeleted &&
                          x.MasterList.Code == MasterListCode.OrderCreationType && x.Code == "TRANSFER"))
                        {
                            trxTrf.Qty = Math.Abs(trxTrf.Qty);
                            trxTrf.Cost = Math.Abs(trxTrf.Cost);
                            trxTrf.ExGSTCost = Math.Abs(trxTrf.ExGSTCost);
                            trxTrf.StockMovement = Math.Abs(trxTrf.StockMovement ?? 0);
                            trxTrf.Tender = "TRANSFER+" + orderHeader.Store.Code; // TRANSFER+188
                        }
                        await trxRepo.InsertAsync(trxTrf).ConfigureAwait(false);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                        #endregion

                        if (listCode.Code == "TRANSFER")
                        {
                            // hit post, this creates Invoice Order using the existing items and quantities in the order from Store A for Store B.

                            int lineno = 0;
                            #region   InsertOrderLine
                            // InsertOrderLine 
                            if (vRecipientOutletOrderNo > 0)
                            {
                                #region OpenExistingOrderBatch
                                //get max line no for the order
                                trfOrderHeader = repoOrderHeader.GetAll().Include(d => d.OrderDetail.OrderByDescending(y => y.LineNo)).Where(x => !x.IsDeleted && x.OutletId ==
                              (orderHeader.StoreIdAsSupplier ?? 0) && x.OrderNo == vRecipientOutletOrderNo)?.FirstOrDefault();
                                lineno = trfOrderHeader?.OrderDetail?.FirstOrDefault()?.LineNo ?? 0;

                                #endregion
                            }
                            else
                            {
                                #region CreateAutoOrderBatch
                                // Get the Highest Order Number in Outlet
                                vRecipientOutletOrderNo = repoOrderHeader.GetAll().Where(x => x.OutletId == orderHeader.StoreIdAsSupplier && x.OrderNo < 1000000)
                                   .OrderByDescending(x => x.OrderNo).Select(x => x.OrderNo)?.FirstOrDefault() ?? 0;
                                vRecipientOutletOrderNo = vRecipientOutletOrderNo + 1;

                                //create new order Header
                                trfOrderHeader = new OrderHeader
                                {
                                    OutletId = orderHeader.StoreIdAsSupplier ?? 0,
                                    OrderNo = vRecipientOutletOrderNo,
                                    CreatedDate = DateTime.UtcNow,
                                    TypeId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "INVOICE" && x.MasterList.Code == MasterListCode.ORDERDOCTYPE).FirstOrDefault().Id,
                                    StatusId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "ORDER" && x.MasterList.Code == MasterListCode.ORDERDOCSTATUS).FirstOrDefault().Id,
                                    StoreIdAsSupplier = orderHeader.OutletId,
                                    Reference = orderHeader.OrderNo.ToString(),
                                    InvoiceDate = orderHeader.CreatedDate,
                                    CreationTypeId = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "TRANSFER" && x.MasterList.Code == MasterListCode.OrderCreationType).FirstOrDefault().Id,
                                    CreatedAt = DateTime.UtcNow,
                                    CreatedById = userId,
                                    UpdatedAt = DateTime.UtcNow,
                                    UpdatedById = userId
                                };

                                #endregion
                            }

                            if (vRecipientOutletOrderNo == 0)
                            {
                                throw new Exception();
                            }

                            //Add invoice Order for transfer outlet
                            //add this product in order detail
                            lineno = lineno + 1;
                            var orderTypeid = repositoryStatusAll.Where(x => !x.IsDeleted && x.Code == "NORMALBUY" && x.MasterList.Code == "ORDERDETAILTYPE").FirstOrDefault().Id;
                            if (trfOrderHeader.OrderDetail == null)
                            {
                                trfOrderHeader.OrderDetail = new List<OrderDetail>();
                            }
                            trfOrderHeader.OrderDetail.Add(new OrderDetail
                            {
                                LineNo = lineno,
                                OrderTypeId = orderTypeid,
                                ProductId = detail.ProductId,
                                CartonQty = detail.CartonQty,
                                CartonCost = detail.CartonCost,
                                Cartons = detail.Cartons,
                                Units = Math.Abs(detail.Units),
                                TotalUnits = Math.Abs(detail.TotalUnits),
                                LineTotal = Math.Abs(detail.LineTotal),
                                FinalLineTotal = Math.Abs(detail.FinalLineTotal ?? 0),
                                FinalCartonCost = Math.Abs(detail.FinalCartonCost ?? 0),
                                SupplierItem = detail.SupplierItem,
                                CreatedAt = DateTime.UtcNow,
                                CreatedById = userId,
                                UpdatedAt = DateTime.UtcNow,
                                UpdatedById = userId
                            });

                            vSupplierInvoiceAmt = vSupplierInvoiceAmt + Math.Abs(detail.LineTotal);

                            #endregion
                            //// Update Invoice total on receieving outlet
                            //var ReceivingOutletOrder = repoOrderHeader.GetAll().Where(x => !x.IsDeleted && x.OutletId == orderHeader.StoreIdAsSupplier &&
                            //x.OrderNo == vRecipientOutletOrderNo).FirstOrDefault();
                            //if (ReceivingOutletOrder == null) { throw new NullReferenceException(); }
                            //ReceivingOutletOrder.InvoiceTotal = vSupplierInvoiceAmt;
                            //repoOrderHeader.Update(ReceivingOutletOrder);
                            //await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                            //// Update Supplier details to NULL for sending outlet if it is TRANSFER ansfer order because it stock transfer betwwen two store
                            ////NEED to confirm implementation

                        }
                    }

                    if (trfOrderHeader != null)
                    {
                        await repoOrderHeader.InsertAsync(trfOrderHeader).ConfigureAwait(false);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }
                    if (listCode.Code == "TRANSFER")
                    {
                        // Update Invoice total on receieving outlet
                        var ReceivingOutletOrder = repoOrderHeader.GetAll().Where(x => !x.IsDeleted && x.OutletId == orderHeader.StoreIdAsSupplier &&
                        x.OrderNo == vRecipientOutletOrderNo).FirstOrDefault();
                        if (ReceivingOutletOrder == null) { throw new NullReferenceException(); }
                        ReceivingOutletOrder.InvoiceTotal = vSupplierInvoiceAmt;
                        repoOrderHeader.Update(ReceivingOutletOrder);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    }
                }


                return await GetOrderById(orderHeader.Id).ConfigureAwait(false);
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

        public async Task<AutoOrderDefaultSettingsResponseViewModel> GetDefaultSettingsForOutlet(int outletId, int supplierId)
        {
            try
            {
                var storeRepo = _unitOfWork?.GetRepository<Store>();
                var store = await storeRepo.GetAll().Where(x => !x.IsDeleted && x.Id == outletId).FirstOrDefaultAsync().ConfigureAwait(false);
                if (store != null)
                {
                    if (supplierId == 0)
                    {
                        var wRepo = _unitOfWork?.GetRepository<Warehouse>();
                        var defaultSupplier = await wRepo.GetAll().Where(x => !x.IsDeleted && x.Id == store.WarehouseId).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (defaultSupplier != null)
                        {
                            supplierId = defaultSupplier.SupplierId;
                        }
                        else
                        {
                            throw new NotFoundException(ErrorMessages.WarehouseNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                    }

                    //Fetch data related to SAV_AUTO_ORDER using outletid and supplierId
                    var aosRepo = _unitOfWork?.GetRepository<AutoOrderSettings>();
                    var aos = await aosRepo.GetAll().Where(x => !x.IsDeleted && x.StoreId == outletId && x.SupplierId == supplierId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (aos != null)
                    {
                        return new AutoOrderDefaultSettingsResponseViewModel
                        { SupplierId = supplierId, CoverDays = aos.CoverDays, HistoryDays = aos.HistoryDays, InvestmentBuyDays = aos.InvestmentBuyDays };
                    }
                    //TODO: Default values for Cover days, history days and IvestmentBuyDays //to be provided by Manoj sir
                    return new AutoOrderDefaultSettingsResponseViewModel
                    { SupplierId = supplierId, CoverDays = 0, HistoryDays = 0, InvestmentBuyDays = 0 };

                }
                else
                {
                    throw new NotFoundException(ErrorMessages.OutletNotFound.ToString(CultureInfo.CurrentCulture));
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
    }
}
