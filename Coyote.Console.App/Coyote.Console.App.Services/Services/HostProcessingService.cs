using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
    public class HostProcessingService : IHostProcessing
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly IProductService _productService = null;


        //   string BuyBatchDoneYN = "";

        public HostProcessingService(IUnitOfWork unitOfWork, IAutoMappingServices autoMapping, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMapping;
            _productService = productService;
        }

        /// <summary>
        /// Get all HostProcessings
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<PagedOutputModel<List<HostProcessingResponseModel>>> GetAllHostProcessing(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<HostProcessing>();
                var hostProcessing = repository.GetAll().Where(x => x.IsActive == Status.Active);
                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                    {
                        hostProcessing = hostProcessing.Where(x => x.Description.ToLower().Contains(inputModel.GlobalFilter.ToLower()));
                    }

                    hostProcessing = hostProcessing.OrderByDescending(x => x.TimeStamp);
                    count = hostProcessing.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        hostProcessing = hostProcessing.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    hostProcessing = hostProcessing.OrderBy(x => x.Code);
                                else
                                    hostProcessing = hostProcessing.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    hostProcessing = hostProcessing.OrderBy(x => x.Description);
                                else
                                    hostProcessing = hostProcessing.OrderByDescending(x => x.Description);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    hostProcessing = hostProcessing.OrderBy(x => x.ID);
                                else
                                    hostProcessing = hostProcessing.OrderByDescending(x => x.ID);
                                break;
                        }
                    }

                }

                List<HostProcessingResponseModel> hostResponses = (await hostProcessing.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<HostProcessing, HostProcessingResponseModel>).ToList();

                return new PagedOutputModel<List<HostProcessingResponseModel>>(hostResponses, count);
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
        /// Get HostProcessings
        /// </summary>
        /// <returns>List<SupplierViewModel></returns>
        public async Task<HostProcessingResponseModel> GetHostProcessingById(int Id)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<HostProcessing>();
                var hostProcessing = await repository.GetAll().Where(x => x.ID == Id && x.IsActive == Status.Active).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                if (hostProcessing == null)
                    throw new NotFoundException(ErrorMessages.HostProcessingNotFond.ToString(CultureInfo.CurrentCulture));
                HostProcessingResponseModel hostResponses = _iAutoMapper.Mapping<HostProcessing, HostProcessingResponseModel>(hostProcessing);

                return hostResponses;
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
        /// Insert HostSystemControls
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="hostSettingsiD"></param>
        /// <param name="userId"></param>
        /// <returns>List</returns>
        public async Task<HostProcessingResponseModel> AddUpdateHostProcessing(HostProcessingRequestModel viewModel, int userId)
        {

            try
            {
                if (viewModel == null)
                    throw new NullReferenceException();

                var repository = _unitOfWork?.GetRepository<HostProcessing>();

                //Insert HostUPD
                var comm = new HostProcessing();
                comm.Code = "HOSTUPD";
                comm.Description = "Run " + DateTime.UtcNow.ToString();
                comm.Number = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                comm.TimeStamp =(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
                comm.IsActive = Status.Active;
                comm.CreatedBy = userId;
                comm.CreatedDate = DateTime.UtcNow;
                comm.ModifiedBy = userId;
                comm.ModifiedDate = DateTime.UtcNow;
                var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                //string BuyStart = "";
                //string BuyEnd = "";
               // string LastBuyStart = "";
              //  string LastBuyEnd = "";

                var reposHosting = _unitOfWork?.GetRepository<HostSettings>();
                if (viewModel.Hosts != null)
                {
                    foreach (var host in viewModel.Hosts)
                    {
                        var hostSettings = await reposHosting.GetAll().Include(c => c.Path).Include(c => c.Supplier).Include(c => c.Warehouse).Include(c => c.HostFormatWareHouse).Where(x => x.ID == host.HostSettingID && x.IsActive != Status.Deleted && x.Path.IsActive != Status.Deleted).FirstOrDefaultAsync().ConfigureAwait(false);
                        if (hostSettings == null)
                            throw new NotFoundException(ErrorMessages.HostSettingsNotFound.ToString(CultureInfo.CurrentCulture));

                        HostSettingsResponseModel Response = MappingHelpers.CreateHostSettingsMap(hostSettings);

                        if ((Response.WareHouse == CommonMessages.WarehouseCode1) || (Response.WareHouse == CommonMessages.WarehouseCode2) || (Response.WareHouse == CommonMessages.WarehouseCode3))
                        {

                            var FileName = host.HostType == HostType.Weekly ? Response.WeeklyFile : Response.InitialLoadFileWeekly;
                            var WarehouseId = Response.WareHouseID;

                            string filePath = Path.Combine(Response.FilePath);

                            if (!File.Exists(filePath))
                            {
                                throw new NotFoundException(ErrorMessages.FilePathNotFond.ToString(CultureInfo.CurrentCulture));
                            }

                            using FileStream fs = File.OpenRead(filePath);
                            using var sr = new StreamReader(fs);
                            string line;
                            var rslt = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (string.IsNullOrEmpty(line))
                                {
                                    break;
                                }
                                var recType = line.Substring(0, 2);

                                switch (recType)
                                {
                                    case "10":
                                        rslt = await GroceryRecType10(line, userId).ConfigureAwait(false);
                                        break;
                                    case "25":
                                        rslt = await GroceryRecType10(line, userId).ConfigureAwait(false);
                                        break;
                                    case "41":
                                        rslt = await GroceryRecType41(line, Response, userId, viewModel.HoldGDP).ConfigureAwait(false);
                                        break;
                                    case "63":
                                        rslt = await GroceryRecType63(line, Response, userId, viewModel.HoldGDP).ConfigureAwait(false);
                                        break;
                                    case "66":
                                        rslt = await GroceryRecType66(line, Response, userId, viewModel.HoldGDP).ConfigureAwait(false);
                                        break;
                                    case "67":
                                        rslt = await GroceryRecType67(line, Response, userId, viewModel.HoldGDP).ConfigureAwait(false);
                                        break;
                                    case "59":
                                        rslt = await GroceryRecType59(line, Response, userId, viewModel.HoldGDP).ConfigureAwait(false);
                                        break;
                                    case "80":
                                        break;
                                    case "82":
                                        break;

                                    case "51":

                                        break;
                                    default:
                                        break;


                                }

                            }
                        }
                    }
                }

                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return await GetHostProcessingById(result.ID).ConfigureAwait(false);
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

        //public Task<int> ProcessTypeSparGrocery(string SData)
        //{
        //    string S = SData;
        //    string[] GUnstr = new string[100];

        //    for (int i = 0; i < 100; i++)
        //    {
        //        GUnstr[i] = "";
        //    }

        //    int j = 1;
        //    for (int i = 0; i < S.Length; i++)
        //    {
        //        if (S[i] != ",")
        //        {
        //            if (S != '"')
        //            {
        //                GUnstr[j] = GUnstr + S[i];
        //            }
        //        }
        //        else
        //        {
        //            j = j + 1;
        //        }
        //    }

        //    return 0;
        //}

        public async Task<int> GroceryRecType10(string data, int userId)
        {
            try
            {
                if (data == null)
                    throw new NullReferenceException();

                var code = data.Substring(2, 1);
                var result = 0;

                // if (code == "1")
                {
                    var viewModel = new CommodityRequestModel();
                    var depCode = data.Substring(3, 2);
                    viewModel.Code = code.Trim();
                    var desc = data.Substring(6, 41);
                    viewModel.Desc = desc.Trim();
                    var repoDep = _unitOfWork?.GetRepository<Department>();
                    var department = await repoDep.GetAll(x => !x.IsDeleted && x.Code == depCode.Trim()).Select(x => x.Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                    if (department > 0)
                        viewModel.DepartmentId = department;
                    var repository = _unitOfWork?.GetRepository<Commodity>();
                    if ((await repository.GetAll().Where(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.CommodityCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var comm = MappingHelpers.Mapping<CommodityRequestModel, Commodity>(viewModel);
                    comm.IsDeleted = false;
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    comm.Status = true;
                    var response = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    result = response.Id;
                }

                return result;
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

        public async Task<int> GroceryRecType25(string data, int userId)
        {
            try
            {
                if (data == null)
                    throw new NullReferenceException();

                var result = 0;

                var code = data.Substring(2, 5);

                if (code != "000")
                {
                    var viewModel = new SupplierRequestModel();

                    viewModel.Code = code;
                    var desc = data.Substring(7, 30);
                    viewModel.Desc = desc.Trim();
                    var repository = _unitOfWork?.GetRepository<Supplier>();
                    if ((await repository.GetAll().Where(x => x.Code == viewModel.Code && !x.IsDeleted).AnyAsyncSafe().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.SupplierDuplicateCode.ToString(CultureInfo.CurrentCulture));
                    }
                    var comm = _iAutoMapper.Mapping<SupplierRequestModel, Supplier>(viewModel);
                    comm.IsDeleted = false;
                    comm.CreatedAt = DateTime.UtcNow;
                    comm.UpdatedAt = DateTime.UtcNow;
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    var res = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    result = res.Id;
                }

                return result;
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

        //public async Task<int> GroceryRecType41(string data, int userId)
        //{
        //    try
        //    {
        //        if (data == null)
        //            throw new NullReferenceException();

        //        var HostItemNo = data.Substring(3, 5);

        //        var Product = new ProductRequestModel();

        //        var number = Convert.ToInt64(HostItemNo);

        //        if (number > 0)
        //        {
        //            var repository = _unitOfWork?.GetRepository<Product>();

        //            var proNumber = repository.GetAll().Where(p => p.Number == number).Select(x => x.Number).FirstOrDefault();

        //            if (proNumber == 0)
        //            {
        //                var numberDetail = await _productService.GetNewProductNumber().ConfigureAwait(false);
        //                Product.Number = numberDetail.Number;
        //            }
        //            else
        //            {
        //                Product.Number = proNumber;
        //            }

        //            var desc = data.Substring(8, 37).Trim();                   
        //            var Apn = data.Substring(50, 62).Trim();
        //            var shrtDesc = data.Substring(38, 49);                   
        //            Product.Desc = desc;                   
        //            Product.APNNumber.Add(Convert.ToInt64(Apn.Trim()));
        //            Product.PosDesc = shrtDesc;

        //            if (WareHouse == CommonMessages.WarehouseCode1)
        //            {
        //                Product.HostNumber = HostItemNo;                        
        //            }
        //            if (WareHouse == CommonMessages.WarehouseCode1)
        //            {
        //                Product.HostNumber = HostItemNo;
        //               // Product.HostItemType = Type;
        //            }
        //            if (WareHouse == CommonMessages.WarehouseCode2)
        //            {
        //                Product.HostNumber = HostItemNo;
        //              //  Product.HostItemType = Type;
        //            }
        //            if (WareHouse == CommonMessages.WarehouseCode3)
        //            {
        //                Product.HostNumber = HostItemNo;
        //             //   Product.HostItemType = Type;
        //            }

        //        }


        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is BadRequestException)
        //        {
        //            throw new BadRequestException(ex.Message);
        //        }
        //        if (ex is AlreadyExistsException)
        //        {
        //            throw new AlreadyExistsException(ex.Message);
        //        }
        //        if (ex is NullReferenceException)
        //        {
        //            throw new NullReferenceException(ex.Message);
        //        }
        //        throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
        //    }
        //}

        public async Task<int> GroceryRecType41(string data, HostSettingsResponseModel host, int userId, bool HoldgDP) //, int userId, 
        {
            try
            {
                var result = 0;
                if (data == null)
                    throw new NullReferenceException();

                var HostItemNo = data.Substring(2, 6);

                var ViewModel = new ProductRequestModel();

                var number = Convert.ToInt64(HostItemNo);

                if (number > 0)
                {
                    var repoSuppPro = _unitOfWork?.GetRepository<SupplierProduct>();

                    var SuppProduct = await repoSuppPro.GetAll().Where(p => p.SupplierItem == HostItemNo.Trim() && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (SuppProduct != null)
                    {
                        var BeforeCtnQty = 0;
                        var AfterCtnQty = 0;
                        var repository = _unitOfWork?.GetRepository<Product>();

                        var Product = await repository.GetAll().Where(p => p.Id == SuppProduct.ProductId && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);

                        if (Product != null)
                        {
                            BeforeCtnQty = Product.CartonQty;
                            var recType = data.Substring(0, 1).ToString();
                            var desc = data.Substring(8, 30).Trim();
                            var Apn = data.Substring(50, 13).Trim();
                            var shrtDesc = data.Substring(38, 12);

                            ViewModel = _iAutoMapper.Mapping<Product, ProductRequestModel>(Product);

                            ViewModel.Desc = desc;
                            ViewModel.CartonQty = Product.CartonQty;
                            AfterCtnQty = Product.CartonQty;
                            ViewModel.APNNumber.Add(Convert.ToInt64(Apn.Trim()));
                            ViewModel.PosDesc = shrtDesc;

                            if (host?.WareHouse == CommonMessages.WarehouseCode1)
                            {
                                ViewModel.HostNumber = HostItemNo;
                            }
                            if (host.WareHouse == CommonMessages.WarehouseCode1)
                            {
                                ViewModel.HostNumber = HostItemNo;
                            }
                            if (host.WareHouse == CommonMessages.WarehouseCode2)
                            {
                                ViewModel.HostNumber = HostItemNo;
                            }
                            if (host.WareHouse == CommonMessages.WarehouseCode3)
                            {
                                ViewModel.HostNumber = HostItemNo;
                            }

                            var productRequestModelDataTable = MappingHelpers.ToDataTable(ViewModel);

                            HostUpdChgRequestModels hostUpdChgRequest = new HostUpdChgRequestModels();

                            hostUpdChgRequest.Action = "Change";

                            hostUpdChgRequest.Desc = Product.Desc;
                            hostUpdChgRequest.Number = Product.Number;
#pragma warning disable CA1062 // Validate arguments of public methods
                            hostUpdChgRequest.HostSettingId = host.ID;
#pragma warning restore CA1062 // Validate arguments of public methods
                            hostUpdChgRequest.BeforeCtnQty = BeforeCtnQty;
                            hostUpdChgRequest.AfterCtnQty = AfterCtnQty;
                            string APNMumbers = null;

                            var hostUpdChgRequestTable = MappingHelpers.ToDataTable(hostUpdChgRequest);

                            foreach (var apn in ViewModel.APNNumber)
                            {
                                if (APNMumbers == null)
                                {
                                    APNMumbers = "";
                                }
                                else
                                {
                                    APNMumbers += ",";
                                }
                                APNMumbers += apn;
                            }
                            if (productRequestModelDataTable.Rows.Count > 0)
                            {
                                productRequestModelDataTable.Rows[0]["APNNumber"] = APNMumbers;
                            }

                            hostUpdChgRequestTable.Columns.Add("RowIndex").SetOrdinal(0);

                            if (hostUpdChgRequestTable?.Rows?.Count > 0)
                            {
                                for (int i = 0; i < hostUpdChgRequestTable?.Rows?.Count; i++)
                                {
                                    hostUpdChgRequestTable.Rows[i]["RowIndex"] = i + 1;
                                }
                            }

                            hostUpdChgRequestTable.AcceptChanges();

                            List<SqlParameter> dbParams = new List<SqlParameter>{
                            new SqlParameter("@ProductId", 0),
                            new SqlParameter("@UserId", userId),
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@productRequestModelDataTable",
                              TypeName ="dbo.ProductRequestType",
                              Value = productRequestModelDataTable,
                              SqlDbType = SqlDbType.Structured
                            },
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@HostUpdChgRequestTypeDataTable",
                              TypeName ="dbo.HostUpdChgRequestType",
                              Value = hostUpdChgRequestTable,
                              SqlDbType = SqlDbType.Structured
                            },
                              new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString()),
                              new SqlParameter("@RecType",recType),
                              new SqlParameter("@IsDeleted",Product.IsDeleted),
                              new SqlParameter("@HostSettingId",host?.ID),
                              new SqlParameter("@UniTShell",0),
                              new SqlParameter("@MinOrdQty",0),
                              new SqlParameter("@HoldGDP",HoldgDP),
                              new SqlParameter("@HostItemNo",HostItemNo)
                            };


                            var dset = await repository.ExecuteStoredProcedure(StoredProcedures.HostProcessingAddUpdateProduct, dbParams.ToArray()).ConfigureAwait(false);
                            if (dset?.Tables?.Count > 0)
                            {
                                if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                                {
                                    throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                                }
                                else
                                {
                                    result = 1;
                                }

                            }
                        }
                    }
                }


                return result;
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
        public async Task<int> GroceryRecType59(string data, HostSettingsResponseModel host, int userId, bool HoldgDP) //, 
        {
            try
            {
                var result = 0;
                if (data == null)
                    throw new NullReferenceException();

                var HostItemNo = data.Substring(2, 6);

                var ViewModel = new ProductRequestModel();

                long ProdId = 0;

                if (Convert.ToInt32(HostItemNo) > 0)
                {
                    var repoSuppPro = _unitOfWork?.GetRepository<SupplierProduct>();

                    var SuppProduct = await repoSuppPro.GetAll().Where(p => p.SupplierItem == HostItemNo.Trim() && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (SuppProduct != null)
                    {
                        ProdId = SuppProduct.ProductId;
                    }

                    var repository = _unitOfWork?.GetRepository<Product>();

                    var Product = await repository.GetAll().Where(p => p.Id == ProdId).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (Product != null)
                    {
                        Product.IsDeleted = true;
                        ViewModel = _iAutoMapper.Mapping<Product, ProductRequestModel>(Product);
                        var recType = data.Substring(0, 1).ToString();

                        if (host?.WareHouse == CommonMessages.WarehouseCode1)
                        {
                            ViewModel.HostNumber = "DELETE";
                        }
                        if (host?.WareHouse == CommonMessages.WarehouseCode1)
                        {
                            ViewModel.HostNumber2 = "DELETE";
                        }
                        if (host?.WareHouse == CommonMessages.WarehouseCode2)
                        {
                            ViewModel.HostNumber3 = "DELETE";
                        }
                        if (host?.WareHouse == CommonMessages.WarehouseCode3)
                        {
                            ViewModel.HostNumber4 = "DELETE";
                        }
                        var productRequestModelDataTable = MappingHelpers.ToDataTable(ViewModel);

                        HostUpdChgRequestModels hostUpdChgRequest = new HostUpdChgRequestModels();

                        hostUpdChgRequest.Action = "Delete";

                        hostUpdChgRequest.Desc = Product.Desc;
                        hostUpdChgRequest.Number = Product.Number;
#pragma warning disable CA1062 // Validate arguments of public methods
                        hostUpdChgRequest.HostSettingId = host.ID;
#pragma warning restore CA1062 // Validate arguments of public methods
                        hostUpdChgRequest.BeforeCtnQty = 0;
                        hostUpdChgRequest.AfterCtnQty = 0;
                        string APNMumbers = null;
                        foreach (var apn in ViewModel.APNNumber)
                        {
                            if (APNMumbers == null)
                            {
                                APNMumbers = "";
                            }
                            else
                            {
                                APNMumbers += ",";
                            }
                            APNMumbers += apn;
                        }
                        if (productRequestModelDataTable.Rows.Count > 0)
                        {
                            productRequestModelDataTable.Rows[0]["APNNumber"] = APNMumbers;
                        }
                        var hostUpdChgRequestTable = MappingHelpers.ToDataTable(hostUpdChgRequest);

                        List<SqlParameter> dbParams = new List<SqlParameter>{
                            new SqlParameter("@ProductId", 0),
                            new SqlParameter("@UserId", userId),
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@productRequestModelDataTable",
                              TypeName ="dbo.ProductRequestType",
                              Value = productRequestModelDataTable,
                              SqlDbType = SqlDbType.Structured
                            },
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@HostUpdChgRequestTypeDataTable",
                              TypeName ="dbo.HostUpdChgRequestType",
                              Value = hostUpdChgRequestTable,
                              SqlDbType = SqlDbType.Structured
                            },
                              new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString()),
                              new SqlParameter("@RecType",recType),
                              new SqlParameter("@IsDeleted",true),
                              new SqlParameter("@HostSettingId",host?.ID),
                              new SqlParameter("@UniTShell",0),
                              new SqlParameter("@MinOrdQty",0),
                              new SqlParameter("@HoldGDP",HoldgDP),
                              new SqlParameter("@HostItemNo",HostItemNo)
                            };


                        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.HostProcessingAddUpdateProduct, dbParams.ToArray()).ConfigureAwait(false);
                        if (dset?.Tables?.Count > 0)
                        {
                            if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                            {
                                throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                            }
                            else
                            {
                                result = 1;
                            }

                        }
                    }
                }


                return result;
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
        public async Task<int> GroceryRecType66(string data, HostSettingsResponseModel host, int userId, bool HoldgDP) //, int userId, 
        {
            try
            {
                if (data == null)
                    throw new NullReferenceException();

                var Result = 0;
                var HostItemNo = data.Substring(2, 6);
                var BeforeCtnQty = 0;
                var AfterCtnQty = 0;

                var Product = new ProductRequestModel();

                var number = Convert.ToInt64(HostItemNo);

                if (number > 0)
                {
                    var repoSuppPro = _unitOfWork?.GetRepository<SupplierProduct>();
                    var repository = _unitOfWork?.GetRepository<Product>();
                    var ProductNU = "N";
                    var SuppProduct = await repoSuppPro.GetAll().Where(p => p.SupplierItem == HostItemNo.Trim() && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);


                    if (SuppProduct != null)
                    {
                        var Pro = await repository.GetAll().Where(p => p.Id == SuppProduct.ProductId && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);
                        Product = _iAutoMapper.Mapping<Product, ProductRequestModel>(Pro);
                        ProductNU = "U";
                        BeforeCtnQty = Product.CartonQty;

                    }

                    if (ProductNU == "N")
                    {
                        var numberDetail = await _productService.GetNewProductNumber().ConfigureAwait(false);
                        Product.Number = numberDetail.Number;
                        Product.Status = false;
                        Product.UnitQty = 1;
                        Product.SupplierId = 1;
                    }

                    var cartQty = data.Substring(8, 4);
                    var minOrdQty = data.Substring(12, 4);
                    var ManufacturerId = data.Substring(19, 5);
                    var Replicate = data.Substring(28, 5).Trim();

                    Product.CartonQty = Convert.ToInt32(cartQty);
                    if (Product.CartonQty == 0)
                    {
                        Product.CartonQty = 1;
                    }
                    if (Product.CartonCost != 0)
                    {
                        Product.CartonCost = Product.CartonCost / 100;
                    }

                    AfterCtnQty = Product.CartonQty;

                    Product.ManufacturerId = Convert.ToInt32(ManufacturerId);

                    var vs = "";

                    vs = data.Substring(16, 3).Trim();
                    var vhCom = (float)Convert.ToDouble(vs);
                    if (vhCom < 1000)
                    {
                        var CommoDept = _unitOfWork.GetRepository<Commodity>();
                        var dept = CommoDept.GetAll().Where(x => x.Code == vs).Select(x => x.DepartmentId).FirstOrDefault();
                        Product.DepartmentId = dept;
                    }


                    Product.GroupId = 1;
                    Product.CategoryId = 2;
                    Product.CommodityId = Convert.ToInt32(vhCom);

                    if (Convert.ToInt32(Replicate) > 0)
                    {
                        Product.Replicate = Replicate;
                    }

                    Product.NationalRangeId = Convert.ToInt32(number);

                    vs = data.Substring(36, 4).Trim();
                    var Gst = Convert.ToInt32(vs);
                    Product.TaxId = 2; // for FRE
                    if (Gst > 0)
                    {
                        Product.TaxId = 17; // for GST
                        Gst = Gst / 100;
                    }

                    Product.SlowMovingInd = false;
                    vs = data.Substring(34, 1).Trim();
                    if (vs == "Z" || !string.IsNullOrEmpty(vs))
                    {
                        Product.SlowMovingInd = true;
                    }
                    Product.VarietyInd = false;
                    vs = data.Substring(35, 1);
                    if (vs == "V" || !string.IsNullOrEmpty(vs))
                    {
                        Product.VarietyInd = true;
                    }

                    var masterListRepo = _unitOfWork.GetRepository<MasterList>();
                    var masterRepo = _unitOfWork.GetRepository<MasterListItems>();

                    vs = data.Substring(24, 1).Trim();
                    var Type = vs;
                    var TypeCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.ProductType).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (vs == "D")
                    {
                        var typeId = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == TypeCode && x.Code == "Direct").Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                        Product.TypeId = typeId;
                    }
                    else
                    {
                        Type = "W";
                        var typeId = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == TypeCode && x.Code == "Host").Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                        Product.TypeId = typeId;
                    }
                    if (host?.WareHouse == CommonMessages.WarehouseCode1)
                    {
                        Product.HostNumber = HostItemNo;
                        Product.HostItemType = Type;
                    }
                    if (host?.WareHouse == CommonMessages.WarehouseCode2)
                    {
                        Product.HostNumber = HostItemNo;
                        Product.HostItemType = Type;
                    }
                    if (host?.WareHouse == CommonMessages.WarehouseCode3)
                    {
                        Product.HostNumber = HostItemNo;
                        Product.HostItemType = Type;
                    }

                    var unitShell = 0;

                    var recType = data.Substring(0, 2).ToString();

                    var productRequestModelDataTable = MappingHelpers.ToDataTable(Product);
                    bool IsDeleted = false;

                    HostUpdChgRequestModels hostUpdChgRequest = new HostUpdChgRequestModels();
                    hostUpdChgRequest.Action = "Add";
                    if (ProductNU == "U")
                        hostUpdChgRequest.Action = "Change";

                    hostUpdChgRequest.Desc = Product.Desc;
                    hostUpdChgRequest.Number = Product.Number;
#pragma warning disable CA1062 // Validate arguments of public methods
                    hostUpdChgRequest.HostSettingId = host.ID;
#pragma warning restore CA1062 // Validate arguments of public methods
                    hostUpdChgRequest.BeforeCtnQty = BeforeCtnQty;
                    hostUpdChgRequest.AfterCtnQty = AfterCtnQty;
                    string APNMumbers = null;

                    var hostUpdChgRequestTable = MappingHelpers.ToDataTable(hostUpdChgRequest);

                    foreach (var apn in Product.APNNumber)
                    {
                        if (APNMumbers == null)
                        {
                            APNMumbers = "";
                        }
                        else
                        {
                            APNMumbers += ",";
                        }
                        APNMumbers += apn;
                    }
                    if (productRequestModelDataTable.Rows.Count > 0)
                    {
                        productRequestModelDataTable.Rows[0]["APNNumber"] = APNMumbers;
                    }

                    hostUpdChgRequestTable.Columns.Add("RowIndex").SetOrdinal(0);

                    if (hostUpdChgRequestTable?.Rows?.Count > 0)
                    {
                        for (int i = 0; i < hostUpdChgRequestTable?.Rows?.Count; i++)
                        {
                            hostUpdChgRequestTable.Rows[i]["RowIndex"] = i + 1;
                        }
                    }

                    hostUpdChgRequestTable.AcceptChanges();


                    List<SqlParameter> dbParams = new List<SqlParameter>{
                            new SqlParameter("@ProductId", 0),
                            new SqlParameter("@UserId", userId),
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@productRequestModelDataTable",
                              TypeName ="dbo.ProductRequestType",
                              Value = productRequestModelDataTable,
                              SqlDbType = SqlDbType.Structured
                            },
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@HostUpdChgRequestTypeDataTable",
                              TypeName ="dbo.HostUpdChgRequestType",
                              Value = hostUpdChgRequestTable,
                              SqlDbType = SqlDbType.Structured
                            },
                              new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString()),
                              new SqlParameter("@RecType",recType),
                              new SqlParameter("@IsDeleted",IsDeleted),
                              new SqlParameter("@HostSettingId",host?.ID),
                              new SqlParameter("@UniTShell",(float)Convert.ToDouble(unitShell)),
                              new SqlParameter("@MinOrdQty",(float)Convert.ToDouble(minOrdQty)),
                              new SqlParameter("@HoldGDP",HoldgDP),
                              new SqlParameter("@HostItemNo",HostItemNo)
                            };


                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.HostProcessingAddUpdateProduct, dbParams.ToArray()).ConfigureAwait(false);
                    if (dset?.Tables?.Count > 0)
                    {
                        if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                        {
                            throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                        }
                        else
                        {
                            Result = 1;
                        }

                    }

                }


                return Result;
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
        public async Task<int> GroceryRecType67(string data, HostSettingsResponseModel host, int userId, bool HoldgDP) //, int userId, 
        {
            try
            {
                var result = 0;
                if (data == null)
                    throw new NullReferenceException();
                var BeforeCtnQty = 0;
                var AfterCtnQty = 0;
                var HostItemNo = data.Substring(3, 3);

                var Product = new ProductRequestModel();
                HostUpdChgRequestModels updChgRequestModels = new HostUpdChgRequestModels();

                var number = Convert.ToInt64(HostItemNo);

                if (number > 0)
                {
                    var repoSuppPro = _unitOfWork?.GetRepository<SupplierProduct>();
                    var repository = _unitOfWork?.GetRepository<Product>();
                    var ProductNU = "N";
                    var SuppProduct = await repoSuppPro.GetAll().Where(p => p.SupplierItem == HostItemNo.Trim() && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);


                    if (SuppProduct != null)
                    {
                        var Pro = await repository.GetAll().Where(p => p.Id == SuppProduct.ProductId && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);
                        Product = _iAutoMapper.Mapping<Product, ProductRequestModel>(Pro);
                        ProductNU = "U";
                        BeforeCtnQty = Product.CartonQty;

                    }

                    if (ProductNU == "N")
                    {
                        var numberDetail = await _productService.GetNewProductNumber().ConfigureAwait(false);
                        Product.Number = numberDetail.Number;
                        Product.Status = false;
                        Product.UnitQty = 1;
                        Product.SupplierId = 1;
                    }

                    var desc = data.Substring(8, 30).Trim();
                    var cartQty = data.Substring(38, 4);
                    var minOrdQty = data.Substring(42, 4);
                    var Apn = data.Substring(46, 13);
                    var shrtDesc = data.Substring(59, 12);
                    var totalCost = data.Substring(71, 6);
                    var UnitQty = data.Substring(77, 6);
                    var ManufacturerId = data.Substring(91, 5);
                    var Replicate = data.Substring(108, 5).Trim();

                    Product.Desc = desc;
                    Product.CartonQty = Convert.ToInt32(cartQty);
                    AfterCtnQty = Product.CartonQty;
                    if (Product.CartonQty == 0)
                    {
                        Product.CartonQty = 1;
                    }

                    if (Product.APNNumber == null)
                    {
                        Product.APNNumber = new List<long>();
                    }

                    Product.APNNumber.Add(Convert.ToInt64(Apn.Trim()));
                    Product.PosDesc = shrtDesc;
                    Product.CartonCost = (float)Convert.ToDouble(totalCost);
                    if (Product.CartonCost != 0)
                    {
                        Product.CartonCost = Product.CartonCost / 100;
                    }
                    var unitShell = (float)Convert.ToDouble(UnitQty);

                    if (unitShell != 0)
                    {
                        unitShell = unitShell / 100;
                    }

                    Product.ManufacturerId = Convert.ToInt32(ManufacturerId);

                    var vs = "";

                    vs = data.Substring(88, 3).Trim();
                    var Departmentno = MapDepartment(data.Substring(98, 1).Trim());
                    var vhCom = (float)Convert.ToDouble(vs);
                    if (vhCom < 1000)
                    {
                        var deptRepo = _unitOfWork.GetRepository<Department>();
                        var dept = deptRepo.GetAll().Where(x => x.Code == vs && !x.IsDeleted).Select(x => x.Id).FirstOrDefault();
                        Product.DepartmentId = dept;
                        if (dept == 0)
                        {
                            var deptComm = _unitOfWork.GetRepository<Commodity>();
                            dept = deptComm.GetAll().Where(x => x.Code == vs && !x.IsDeleted).Select(x => x.DepartmentId).FirstOrDefault();
                        }
                        Product.DepartmentId = dept;
                    }
                    Product.GroupId = 1;
                    Product.CategoryId = 2;
                    Product.CommodityId = Convert.ToInt32(vhCom);

                    if (Convert.ToInt32(Replicate) > 0)
                    {
                        Product.Replicate = Replicate;
                    }

                    Product.NationalRangeId = Convert.ToInt32(number);

                    vs = data.Substring(128, 4).Trim();
                    var Gst = Convert.ToInt32(vs);
                    Product.TaxId = 17; // for FRE
                    if (Gst > 0)
                    {
                        Product.TaxId = 17; // for GST
                        Gst = Gst / 100;
                    }

                    Product.SlowMovingInd = false;
                    vs = data.Substring(138, 1).Trim();
                    if (vs == "Z" || !string.IsNullOrEmpty(vs))
                    {
                        Product.SlowMovingInd = true;
                    }
                    Product.VarietyInd = false;
                    vs = data.Substring(139, 1);
                    if (vs == "V" || !string.IsNullOrEmpty(vs))
                    {
                        Product.VarietyInd = true;
                    }

                    var masterListRepo = _unitOfWork.GetRepository<MasterList>();
                    var masterRepo = _unitOfWork.GetRepository<MasterListItems>();

                    vs = data.Substring(96, 1).Trim();
                    var Type = vs;
                    var TypeCode = await masterListRepo.GetAll(x => !x.IsDeleted && x.Code == MasterListCode.ProductType).Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (vs == "D")
                    {
                        int code = TypeCode;
                        var typeId = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == TypeCode && x.Code == "Direct").Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                        Product.TypeId = typeId;
                    }
                    else
                    {
                        Type = "W";
                        int code = TypeCode;
                        var typeId = await masterRepo.GetAll(x => !x.IsDeleted && x.ListId == code && x.Code == "Host").Select(x => x.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                        Product.TypeId = typeId;
                    }
                    if (host?.WareHouse == CommonMessages.WarehouseCode1)
                    {
                        Product.HostNumber = HostItemNo;
                        Product.HostItemType = Type;
                    }
                    if (host?.WareHouse == CommonMessages.WarehouseCode1)
                    {
                        Product.HostNumber = HostItemNo;
                        Product.HostItemType = Type;
                    }
                    if (host?.WareHouse == CommonMessages.WarehouseCode2)
                    {
                        Product.HostNumber = HostItemNo;
                        Product.HostItemType = Type;
                    }
                    if (host?.WareHouse == CommonMessages.WarehouseCode3)
                    {
                        Product.HostNumber = HostItemNo;
                        Product.HostItemType = Type;
                    }
                    var recType = data.Substring(0, 2).ToString();

                    var productRequestModelDataTable = MappingHelpers.ToDataTable(Product);
                    bool IsDeleted = false;

                    HostUpdChgRequestModels hostUpdChgRequest = new HostUpdChgRequestModels();
                    hostUpdChgRequest.Action = "Add";
                    if (ProductNU == "U")
                        hostUpdChgRequest.Action = "Change";

                    hostUpdChgRequest.Desc = Product.Desc;
                    hostUpdChgRequest.Number = Product.Number;
#pragma warning disable CA1062 // Validate arguments of public methods
                    hostUpdChgRequest.HostSettingId = host.ID;
#pragma warning restore CA1062 // Validate arguments of public methods
                    hostUpdChgRequest.BeforeCtnQty = BeforeCtnQty;
                    hostUpdChgRequest.AfterCtnQty = AfterCtnQty;
                    string APNMumbers = null;

                    var hostUpdChgRequestTable = MappingHelpers.ToDataTable(hostUpdChgRequest);

                    foreach (var apn in Product.APNNumber)
                    {
                        if (APNMumbers == null)
                        {
                            APNMumbers = "";
                        }
                        else
                        {
                            APNMumbers += ",";
                        }
                        APNMumbers += apn;
                    }
                    if (productRequestModelDataTable.Rows.Count > 0)
                    {
                        productRequestModelDataTable.Rows[0]["APNNumber"] = APNMumbers;
                    }

                    hostUpdChgRequestTable.Columns.Add("RowIndex").SetOrdinal(0);

                    if (hostUpdChgRequestTable?.Rows?.Count > 0)
                    {
                        for (int i = 0; i < hostUpdChgRequestTable?.Rows?.Count; i++)
                        {
                            hostUpdChgRequestTable.Rows[i]["RowIndex"] = i + 1;
                        }
                    }

                    hostUpdChgRequestTable.AcceptChanges();


                    List<SqlParameter> dbParams = new List<SqlParameter>{
                            new SqlParameter("@ProductId", 0),
                            new SqlParameter("@UserId", userId),
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@productRequestModelDataTable",
                              TypeName ="dbo.ProductRequestType",
                              Value = productRequestModelDataTable,
                              SqlDbType = SqlDbType.Structured
                            },
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@HostUpdChgRequestTypeDataTable",
                              TypeName ="dbo.HostUpdChgRequestType",
                              Value = hostUpdChgRequestTable,
                              SqlDbType = SqlDbType.Structured
                            },
                              new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString()),
                              new SqlParameter("@RecType",recType),
                              new SqlParameter("@IsDeleted",IsDeleted),
                              new SqlParameter("@HostSettingId",host?.ID),
                              new SqlParameter("@UniTShell",(float)Convert.ToDouble(2)),
                              new SqlParameter("@MinOrdQty",(float)Convert.ToDouble(minOrdQty)),
                              new SqlParameter("@HoldGDP",HoldgDP),
                              new SqlParameter("@HostItemNo",HostItemNo)
                            };


                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.HostProcessingAddUpdateProduct, dbParams.ToArray()).ConfigureAwait(false);
                    if (dset?.Tables?.Count > 0)
                    {
                        if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                        {
                            throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                        }
                        else
                        {
                            result = 1;
                        }

                    }
                }


                return result;
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
        public async Task<int> GroceryRecType63(string data, HostSettingsResponseModel host, int userId, bool HoldgDP) //, int userId, 
        {
            try
            {
                var result = 0;
                if (data == null)
                    throw new NullReferenceException();

                var HostItemNo = data.Substring(2, 6);
                var BeforeCtnQty = 0;
                var AfterCtnQty = 0;

                var ViewModel = new ProductRequestModel();

                var number = Convert.ToInt64(HostItemNo);

                if (number > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Product>();
                    var repoSuppPro = _unitOfWork?.GetRepository<SupplierProduct>();
                    var SuppProduct = await repoSuppPro.GetAll().Where(p => p.SupplierItem == HostItemNo.Trim() && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);


                    if (SuppProduct != null)
                    {
                        var Product = await repository.GetAll().Where(p => p.Id == SuppProduct.ProductId && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);

                        var totalCost = data.Substring(8, 6).Trim();
                        Product.CartonCost = (float)Convert.ToDouble(totalCost);
                        if (Product.CartonCost != 0)
                        {
                            Product.CartonCost = Product.CartonCost / 100;
                        }
                        BeforeCtnQty = Product.CartonQty;
                        var unitQty = data.Substring(14, 6).Trim();
                        var unitshell = (float)Convert.ToDouble(unitQty);
                        if (unitshell != 0)
                        {
                            unitshell = Product.UnitQty / 100;
                        }

                        ViewModel = _iAutoMapper.Mapping<Product, ProductRequestModel>(Product);


                        if (host?.WareHouse == CommonMessages.WarehouseCode1)
                        {
                            ViewModel.HostNumber = HostItemNo;
                        }
                        if (host?.WareHouse == CommonMessages.WarehouseCode2)
                        {
                            ViewModel.HostNumber = HostItemNo;
                        }
                        if (host?.WareHouse == CommonMessages.WarehouseCode3)
                        {
                            ViewModel.HostNumber = HostItemNo;
                        }

                        var productRequestModelDataTable = MappingHelpers.ToDataTable(ViewModel);
                        HostUpdChgRequestModels hostUpdChgRequest = new HostUpdChgRequestModels();
                        hostUpdChgRequest.Action = "Change";

                        hostUpdChgRequest.Desc = Product.Desc;
                        hostUpdChgRequest.Number = Product.Number;
#pragma warning disable CA1062 // Validate arguments of public methods
                        hostUpdChgRequest.HostSettingId = host.ID;
#pragma warning restore CA1062 // Validate arguments of public methods
                        hostUpdChgRequest.BeforeCtnQty = BeforeCtnQty;
                        hostUpdChgRequest.AfterCtnQty = AfterCtnQty;
                        string APNMumbers = null;
                        var recType = data.Substring(0, 2).ToString();
                        var hostUpdChgRequestTable = MappingHelpers.ToDataTable(hostUpdChgRequest);
                        AfterCtnQty = Product.CartonQty;
                        foreach (var apn in ViewModel.APNNumber)
                        {
                            if (APNMumbers == null)
                            {
                                APNMumbers = "";
                            }
                            else
                            {
                                APNMumbers += ",";
                            }
                            APNMumbers += apn;
                        }
                        if (productRequestModelDataTable.Rows.Count > 0)
                        {
                            productRequestModelDataTable.Rows[0]["APNNumber"] = APNMumbers;
                        }

                        hostUpdChgRequestTable.Columns.Add("RowIndex").SetOrdinal(0);

                        if (hostUpdChgRequestTable?.Rows?.Count > 0)
                        {
                            for (int i = 0; i < hostUpdChgRequestTable?.Rows?.Count; i++)
                            {
                                hostUpdChgRequestTable.Rows[i]["RowIndex"] = i + 1;
                            }
                        }

                        hostUpdChgRequestTable.AcceptChanges();


                        List<SqlParameter> dbParams = new List<SqlParameter>{
                            new SqlParameter("@ProductId", 0),
                            new SqlParameter("@UserId", userId),
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@productRequestModelDataTable",
                              TypeName ="dbo.ProductRequestType",
                              Value = productRequestModelDataTable,
                              SqlDbType = SqlDbType.Structured
                            },
                            new SqlParameter{
                              Direction = ParameterDirection.Input,
                              ParameterName = "@HostUpdChgRequestTypeDataTable",
                              TypeName ="dbo.HostUpdChgRequestType",
                              Value = hostUpdChgRequestTable,
                              SqlDbType = SqlDbType.Structured
                            },
                              new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString()),
                              new SqlParameter("@RecType",recType),
                              new SqlParameter("@IsDeleted",Product.IsDeleted),
                              new SqlParameter("@HostSettingId",host?.ID),
                              new SqlParameter("@UniTShell",(float)Convert.ToDouble(unitshell)),
                              new SqlParameter("@MinOrdQty",(float)Convert.ToDouble(1)),
                              new SqlParameter("@HoldGDP",HoldgDP),
                              new SqlParameter("@HostItemNo",HostItemNo)
                            };


                        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.HostProcessingAddUpdateProduct, dbParams.ToArray()).ConfigureAwait(false);
                        if (dset?.Tables?.Count > 0)
                        {
                            if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                            {
                                throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                            }
                            else
                            {
                                result = 1;
                            }

                        }

                    }
                }


                return result;
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

        //public async Task<bool> GroceryRecType80(string data, string WareHouse, HostSettingsResponseModel host) //, int userId, 
        //{
        //    try
        //    {
        //        if (data == null)
        //            throw new NullReferenceException();

        //        var Week = data.Substring(11, 2).Trim();
        //        var Desc = data.Substring(2, 9).Trim();

        //        var SellStart = data.Substring(14, 2) + '/' + data.Substring(16, 2) + '/' + data.Substring(18, 4);
        //        var SellEnd = data.Substring(22, 2) + '/' + data.Substring(24, 2) + '/' + data.Substring(26, 4);



        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is BadRequestException)
        //        {
        //            throw new BadRequestException(ex.Message);
        //        }
        //        if (ex is AlreadyExistsException)
        //        {
        //            throw new AlreadyExistsException(ex.Message);
        //        }
        //        if (ex is NullReferenceException)
        //        {
        //            throw new NullReferenceException(ex.Message);
        //        }
        //        throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
        //    }
        //}

        public async Task<bool> GroceryRecType82(string data, HostSettingsResponseModel host, string LastBuystart, string LastBuyEnd) //, int userId, 
        {
            try
            {
                if (data == null)
                    throw new NullReferenceException();

                var HostItemNo = data.Substring(2, 6);

                var ViewModel = new ProductRequestModel();

                long ProdId = 0;

                if (Convert.ToInt32(HostItemNo) > 0)
                {
                    var repoSuppPro = _unitOfWork?.GetRepository<SupplierProduct>();

                    var SuppProduct = await repoSuppPro.GetAll().Where(p => p.SupplierItem == HostItemNo.Trim() && p.SupplierId == host.SupplierID).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (SuppProduct != null)
                    {
                        ProdId = SuppProduct.ProductId;
                    }

                    var repository = _unitOfWork?.GetRepository<Product>();

                    var Product = await repository.GetAll().Where(p => p.Id == ProdId).Select(x => x.Number).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (Product > 0)
                    {
                        var HostPromotyp = data.Substring(21, 2).Trim();
                        var cartQty = data.Substring(23, 4).Trim();
                        if (Convert.ToInt32(cartQty) == 0)
                            cartQty = "0";
                        var minOrdQty = data.Substring(12, 4).Trim();
                        //var Apn = data.Substring(46, 58);
                        var Desc = data.Substring(8, 13);
                        var totalCost = data.Substring(27, 6);
                        //var UnitQty = data.Substring(77, 82);
                        var ManufacturerId = data.Substring(19, 5);
                        var Replicate = data.Substring(28, 5).Trim();

                        var unitShell = (float)Convert.ToDouble(totalCost);
                        if (unitShell != 0)
                        {
                            unitShell = unitShell / 100;
                        }

                        var unitQty = Convert.ToInt32(data.Substring(34, 6).Trim());
                        //Product.CartonCost = (float)Convert.ToDouble(unitQty);
                        if (Convert.ToInt32(unitQty) != 0)
                        {
                            unitQty = Convert.ToInt32(unitQty) / 100;
                        }

                        var BuyStart = data.Substring(41, 2) + '/' + data.Substring(43, 2) + '/' + data.Substring(45, 4);
                        var BuyEnd = data.Substring(49, 2) + '/' + data.Substring(51, 2) + '/' + data.Substring(53, 4);
#pragma warning disable CS0219 // Variable is assigned but its value is never used
                        string BuyBatchDoneYN = "Y";
                        string ValidBuyPromoItemYN = "N";
                        string ValidBuyDates = "Y";
                      //  string ValidSellPromoItemYN:= 'N';
#pragma warning restore CS0219 // Variable is assigned but its value is never used
                        DateTime StartDate;

                        if ((BuyStart != LastBuystart) || (BuyEnd != LastBuyEnd))
                        {
                             BuyBatchDoneYN = "N";
                        }

                        if (!DateTime.TryParse(BuyStart, out StartDate))
                        {
                            ValidBuyDates = "N";
                        }

                        if (ValidBuyDates == "Y" && Convert.ToInt32(cartQty) > 0)
                            ValidBuyPromoItemYN = "Y";

                      
                    }
                }


                return true;
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

        private async Task<string> MapDepartment(string DepartType)
        {
            var Department = "0";

            var RepoDept = _unitOfWork.GetRepository<Department>();

            var Depart = RepoDept.GetAll(x => !x.IsDeleted, includes: new Expression<Func<Department, object>>[] { c => c.MapTypeMasterListItems });


            switch (DepartType)
            {
                case "G":
                    Department = await Depart.Where(x => x.Desc == "GROCERY").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "A":
                    Department = await Depart.Where(x => x.Desc == "DELI").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "N":
                    Department = await Depart.Where(x => x.Desc == "DELI").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;

                case "O":
                    Department = await Depart.Where(x => x.Desc == "DELI").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "B":
                    Department = await Depart.Where(x => x.Desc == "MEAT").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;

                case "M":
                    Department = await Depart.Where(x => x.Desc == "MEAT").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;

                case "D":
                    Department = await Depart.Where(x => x.Desc == "DAIRY").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;

                case "F":
                    Department = await Depart.Where(x => x.Desc == "FROZEN").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;

                case "C":
                    Department = await Depart.Where(x => x.Desc == "CONFECTIONARY").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;

                case "V":
                    Department = await Depart.Where(x => x.Desc == "VARIETY").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "T":
                    Department = await Depart.Where(x => x.Desc == "TOBACCO").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "S":
                    Department = await Depart.Where(x => x.Desc == "SOFTDRINKS").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "K":
                    Department = await Depart.Where(x => x.Desc == "BAKERY").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "P":
                    Department = await Depart.Where(x => x.Desc == "FRUIT&VEG").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                case "X":
                    Department = await Depart.Where(x => x.Desc == "MISC").Select(x => x.Code).FirstOrDefaultAsync().ConfigureAwait(false);
                    break;
                default:
                    break;
            }

            return Department;

        }

        public async Task<HOSTUPDUserActivityResponseModel> HostChangesSheet(long HostSettingId)
        {
            try
            {
                var HostUPD = await GetHOSTUPDUserLog<HOSTUPDDetails>(HostSettingId).ConfigureAwait(false);
                var response = new HOSTUPDUserActivityResponseModel();
                response.HostUPD = HostUPD;

                return response;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.NotAcceptedOrCreated.ToString(CultureInfo.CurrentCulture));
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

        public async Task<PagedOutputModel<List<UserLogResponseModel<T>>>> GetHOSTUPDUserLog<T>(long? Id) where T : class
        {
            try
            {
                var responseModel = new PagedOutputModel<List<UserLogResponseModel<T>>>();

                if (Id != null)
                {
                    var repository = _unitOfWork?.GetRepository<UserLog>();

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@TableId", Id)                       
                    };
                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetHOSTUPD, dbParams.ToArray()).ConfigureAwait(false);
                    var userAuditLogVM = MappingHelpers.ConvertDataTable<UserAuditLogResponseModel>(dset.Tables[0]).ToList();
                    List<UserLogResponseModel<T>> usrLogVM = new List<UserLogResponseModel<T>>();
                    foreach (var userAuditLogResponseModel in userAuditLogVM)
                    {
                        UserLogResponseModel<T> userLogObject = new UserLogResponseModel<T>();
                        userLogObject.Id = userAuditLogResponseModel.Id;
                        userLogObject.Table = userAuditLogResponseModel.Table;
                        userLogObject.TableId = userAuditLogResponseModel.TableId;
                        userLogObject.Action = userAuditLogResponseModel.Action;
                        userLogObject.ActionAt = userAuditLogResponseModel.ActionAt;
                        userLogObject.ActionBy = userAuditLogResponseModel.ActionBy;
                        userLogObject.UserName = userAuditLogResponseModel.UserName;
                        userLogObject.UserNumber = userAuditLogResponseModel.UserNumber;
                        userLogObject.DataLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<DataLog<T>>(userAuditLogResponseModel?.DataLog);
                        usrLogVM.Add(userLogObject);
                    }

                    responseModel = new PagedOutputModel<List<UserLogResponseModel<T>>>(usrLogVM, userAuditLogVM.Count);
                }
                return responseModel;
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


    }
}
