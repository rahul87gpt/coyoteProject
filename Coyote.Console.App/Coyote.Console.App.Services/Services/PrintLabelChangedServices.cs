using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Services
{
    public class PrintLabelChangedServices : IPrintChangedLabelServices
    {
        private readonly IUnitOfWork _unitOfWork = null;
        private IConfiguration _configuration;
        private IReportService _reportService;

        /// <summary>
        /// Get List of Available Outlet Products available for Print
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="configuration"></param>
        /// <param name="reportService"></param>
        public PrintLabelChangedServices(IUnitOfWork unitOfWork, IConfiguration configuration, IReportService reportService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _reportService = reportService;
        }

        /// <summary>
        /// Get list of Outlets and counts of labels available for Print
        /// </summary>
        /// <returns></returns>
        public async Task<List<PrintChangeLabelResponseModel>> GetPrintLabelChangedAsync()
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                var list = await repository.GetAll()
                    .Include(c => c.Store).ThenInclude(c => c.LabelTypePromo)
                    .Where(x => !x.IsDeleted && !x.Store.IsDeleted && x.ChangeLabelInd == true && x.Status)
                    .ToListAsyncSafe().ConfigureAwait(false);

                var response = list.GroupBy(x => x.Store.Id).Select((x, y) => new PrintChangeLabelResponseModel
                {
                    LabelQty = x.Sum(x => x.LabelQty) ?? 0,
                    StoreCode =(x.FirstOrDefault().Store.Code),
                    StoreDesc = x.FirstOrDefault().Store.Desc,
                    DefaultLabelId = x.FirstOrDefault().Store?.LabelTypeShelfId,
                    DefaultLabelType = x.FirstOrDefault().Store?.LabelTypeShelf?.Code,
                    DefaultLabelTypeDesc = x.FirstOrDefault().Store?.LabelTypeShelf?.Desc,
                    StoreId = x.Key
                }).OrderBy(x=>x.StoreCode).ToList();

                return response;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of Outlets and counts of labels available for Print
        /// </summary>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<PrintChangeLabelResponseModel>>> GetPrintChangedLabel(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                var count = 0;
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter",inputModel?.GlobalFilter),
                        new SqlParameter("@PageNumber",inputModel?.SkipCount),
                        new SqlParameter("@PageSize",inputModel.MaxResultCount),
                        new SqlParameter("@SortColumn",inputModel.Sorting),
                        new SqlParameter("@SortDirection",inputModel.Direction)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetPrintChangedLabels, dbParams.ToArray()).ConfigureAwait(false);
                var response = MappingHelpers.ConvertDataTable<PrintChangeLabelResponseModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<PrintChangeLabelResponseModel>>(response, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of Labels that has already been printed.
        /// </summary>
        /// <returns></returns>
        public async Task<List<RePrintChangeLabelResponseModel>> GetRePrintLabelChanged()
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                var list = await repository.GetAll().Include(c => c.Store).ThenInclude(c => c.LabelTypeShelf).Where(x => !x.IsDeleted && !x.Store.IsDeleted && x.ChangeLabelInd == false && x.LabelQty > 0 && x.ChangeLabelPrinted != null && x.Status).ToListAsyncSafe().ConfigureAwait(false);


                var response = list.GroupBy(x => new { x.Store.Id, x.ChangeLabelPrinted }).Select((x, y) => new RePrintChangeLabelResponseModel
                {
                    LabelQty = x.Sum(x => x.LabelQty) ?? 0,
                    StoreCode = x.FirstOrDefault().Store.Code,
                    StoreDesc = x.FirstOrDefault().Store.Desc,
                    DefaultLabelId = x.FirstOrDefault().Store?.LabelTypeShelfId,
                    DefaultLabelType = x.FirstOrDefault().Store?.LabelTypeShelf?.Code,
                    DefaultLabelTypeDesc = x.FirstOrDefault().Store?.LabelTypeShelf?.Desc,
                    StoreId = x.Key.Id,
                    ChangeLabelPrinted = x.Key.ChangeLabelPrinted

                }).ToList();

                return response;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of Labels that has already been printed, Using SP.
        /// </summary>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<RePrintChangeLabelResponseModel>>> GetRePrintChangedLabel(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                var count = 0;
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter",inputModel?.GlobalFilter),
                        new SqlParameter("@PageNumber",inputModel?.SkipCount),
                        new SqlParameter("@PageSize",inputModel.MaxResultCount),
                        new SqlParameter("@SortColumn",inputModel.Sorting),
                        new SqlParameter("@SortDirection",inputModel.Direction)
                    };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetReprintChangedLabel, dbParams.ToArray()).ConfigureAwait(false);
                var response = MappingHelpers.ConvertDataTable<RePrintChangeLabelResponseModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<RePrintChangeLabelResponseModel>>(response, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of Labels to be print for Special Price
        /// </summary>
        /// <returns></returns>
        public async Task<List<SpecPrintChangeLabelResponseModel>> GetSpecPrintLabelChanged()
        {
            try
            {
                var testdate = DateTime.Now.AddDays(-1);
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                var list = await repository.GetAll().Include(c => c.Store).ThenInclude(c => c.LabelTypeShelf).Where(x => !x.IsDeleted && !x.Store.IsDeleted && x.ChangeLabelInd == true && x.Status && x.SpecPrice > 0 && x.SpecTo > DateTime.Now.AddDays(-1)).ToListAsyncSafe().ConfigureAwait(false);

                var response = list.GroupBy(x => x.Store.Id).Select((x, y) => new SpecPrintChangeLabelResponseModel
                {
                    SpecPrice = x.Sum(x => x.SpecPrice) ?? 0,
                    SpecTo = x.FirstOrDefault().SpecTo,
                    SpecFrom = x.FirstOrDefault().SpecFrom,
                    StoreCode = x.FirstOrDefault().Store.Code,
                    StoreDesc = x.FirstOrDefault().Store.Desc,
                    DefaultLabelId = x.FirstOrDefault().Store?.LabelTypeShelfId,
                    DefaultLabelType = x.FirstOrDefault().Store.LabelTypeShelf?.Code,
                    DefaultLabelTypeDesc = x.FirstOrDefault().Store.LabelTypeShelf?.Desc,
                    StoreId = x.Key
                }).ToList();


                return response;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of Labels to be print for Special Price
        /// </summary>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<SpecPrintChangeLabelResponseModel>>> GetSpecPrintLabelChanged(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<OutletProduct>();
                var count = 0;
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter",inputModel?.GlobalFilter),
                        new SqlParameter("@PageNumber",inputModel?.SkipCount),
                        new SqlParameter("@PageSize",inputModel.MaxResultCount),
                        new SqlParameter("@SortColumn",inputModel.Sorting),
                        new SqlParameter("@SortDirection",inputModel.Direction)
                    };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetSpecLabel, dbParams.ToArray()).ConfigureAwait(false);
                var response = MappingHelpers.ConvertDataTable<SpecPrintChangeLabelResponseModel>(dset.Tables[0]);

                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<SpecPrintChangeLabelResponseModel>>(response, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of Labels to be printed for Products in Promotion
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="securityViewModel"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetPromotionLabelChanged(PromotionFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {

                var repository = _unitOfWork?.GetRepository<Promotion>();
                var list = repository.GetAll(x => !x.IsDeleted, null, includes: new Expression<Func<Promotion, object>>[] { c => c.PromotionFrequency, c => c.PromotionSource, c => c.PromotionType, c => c.PromotionZone });

                list = list.Where(x => x.PromotionType.Code.ToUpper() == "MIXMATCH" || x.PromotionType.Code.ToUpper() == "OFFER" || x.PromotionType.Code.ToUpper() == "SELLING");

                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        list = list.Where(x => x.Desc.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()));
                    if (inputModel.PromotionStartDate.HasValue)
                        list = list.Where(x => x.Start >= inputModel.PromotionStartDate.Value || x.End >= inputModel.PromotionStartDate.Value);
                    if (inputModel.PromotionEndDate.HasValue)
                        list = list.Where(x => x.End >= inputModel.PromotionEndDate.Value || x.End <= inputModel.PromotionEndDate.Value);

                    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.ZoneIds != null && securityViewModel.ZoneIds.Count > 0)
                        list = list.Where(x => securityViewModel.ZoneIds.Contains(x.ZoneId));

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    count = list.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        list = list.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "code":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Code);
                                else
                                    list = list.OrderByDescending(x => x.Code);
                                break;
                            case "desc":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Desc);
                                else
                                    list = list.OrderByDescending(x => x.Desc);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    list = list.OrderBy(x => x.Id);
                                else
                                    list = list.OrderByDescending(x => x.Id);
                                break;
                        }
                    }
                }
                List<PromotionResponseViewModel> listVM = new List<PromotionResponseViewModel>();
                var promos = (await list.ToListAsyncSafe().ConfigureAwait(false));

                listVM = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                count = list.Count();
                return new PagedOutputModel<List<PromotionResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<PagedOutputModel<List<PromotionResponseViewModel>>> GetPromotionPrintLabel(PromotionFilter inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Promotion>();
                int count = 0;
                bool zoneIds = false;
                var AccessZones = String.Empty;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.ZoneIds != null && securityViewModel.ZoneIds.Count > 0)
                {
                    foreach (var zoneId in securityViewModel.ZoneIds)
                        AccessZones += zoneId + ",";
                    zoneIds = true;
                }
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@PromotionStartDate", inputModel?.PromotionStartDate),
                        new SqlParameter("@PromotionEndDate", inputModel?.PromotionEndDate),
                        new SqlParameter("@PrintPromotion",true),
                        new SqlParameter("@Status", inputModel?.Status),
                        new SqlParameter("@ZoneIds", (zoneIds == true)?AccessZones:null),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@IsLogged", inputModel?.IsLogged),
                        new SqlParameter("@Module","PromotionR"),
                        new SqlParameter("@RoleId",RoleId)

                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetActivePromotionBatches, dbParams.ToArray()).ConfigureAwait(false);
                List<PromotionResponseViewModel> listVM = MappingHelpers.ConvertDataTable<PromotionResponseViewModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["RecordCount"]);
                return new PagedOutputModel<List<PromotionResponseViewModel>>(listVM, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.PromotionNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get list of Labels to be printed from Tablet Load
        /// </summary>
        /// <param name="securityViewModel"></param>
        /// <returns></returns>
        public async Task<List<PrintLabelFromTableResponseModel>> GetPrintLabelFromTablet(SecurityViewModel securityViewModel)
        {
            try
            {
                var storeRepository = _unitOfWork?.GetRepository<Store>();

                List<PrintLabelFromTableResponseModel> response = new List<PrintLabelFromTableResponseModel>();

                bool StoreIds = false;
                var AccessStores = String.Empty;
                if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                {
                    foreach (var storeId in securityViewModel.StoreIds)
                        AccessStores += storeId + ",";
                    StoreIds = true;
                }

                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@AccessOutletIds", (StoreIds == true)?AccessStores:null)
                    };

                var dset = await storeRepository.ExecuteStoredProcedure(StoredProcedures.GetPrintLabelsFromTablet, dbParams.ToArray()).ConfigureAwait(false);
                response = MappingHelpers.ConvertDataTable<PrintLabelFromTableResponseModel>(dset.Tables[0]);
                return response;
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);

            }
        }

        /// <summary>
        /// Get list of Labels to be printed from PDE Load
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public async Task<List<PrintLabelFromTabletPDEMode>> GetPrintLabelFromPDE(int StoreId)
        {
            try
            {
                var storeRepository = _unitOfWork?.GetRepository<Store>();

                List<PrintLabelFromTabletPDEMode> response = new List<PrintLabelFromTabletPDEMode>();


                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@OutletId",StoreId)
                    };

                var dset = await storeRepository.ExecuteStoredProcedure(StoredProcedures.GetPrintLabelsFromTabletPDE, dbParams.ToArray()).ConfigureAwait(false);
                response = MappingHelpers.ConvertDataTable<PrintLabelFromTabletPDEMode>(dset.Tables[0]);
                return response;
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);

            }
        }

        /// <summary>
        /// Get list of products to be printed from imported PDE File.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="mime"></param>
        /// <returns></returns>
        public async Task<byte[]> GetPrintLabelPDEImport(PrintLabelRequestModel requestModel, string mime)
        {
            try
            {
                if (requestModel == null)
                {
                    throw new BadRequestException(ErrorMessages.ReportRequestModelIsEmpty.ToString(CultureInfo.CurrentCulture));
                }
                if (requestModel.PrintType.ToLower() != "pdeimport" && requestModel.PrintType.ToLower() != "pricecheck")
                {
                    throw new BadRequestException(ErrorMessages.PrintTypeUnavailable.ToString(CultureInfo.CurrentCulture));
                }
                if (requestModel.StoreId <= 0)
                {
                    throw new BadRequestException(ErrorMessages.StoreId.ToString(CultureInfo.CurrentCulture));
                }
                List<PDEImportModel> pdeImport = new List<PDEImportModel>();

                var pdePath = Directory.GetCurrentDirectory() + requestModel.PDEFilePath;

                var lineCount = File.ReadLines(pdePath).Count();

                try
                {
                    using (var reader = new StreamReader(pdePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            var pdeVal = new PDEImportModel();

                            var line = reader.ReadLine();
                            var values = line.Split(',');

                            //If value is Date, ignore that line.
                            if (values[0].ToLower() != "date")
                            {
                                pdeVal.RecordType = values[0];

                                pdeVal.ProductNumber = string.IsNullOrEmpty(values[1]) ? 0 : Convert.ToInt64(values[1]);

                                pdeVal.Price = string.IsNullOrEmpty(values[2]) ? 0 : Convert.ToInt64(values[2]);
                                pdeImport.Add(pdeVal);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw new BadRequestException(ErrorMessages.InvalidPDEImport.ToString(CultureInfo.CurrentCulture));
                }

                long ProdId = 0;
                int PdeActiveItemsCount = 0;
                var sysControllRepo = _unitOfWork?.GetRepository<SystemControls>();
                var hostItemNumber = 0;// await sysControllRepo.GetAll().Where(x => !x.IsDeleted).Select(x=>x.HostUpdatePricing).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                var apnRepo = _unitOfWork?.GetRepository<APN>();
                var prodRepo = _unitOfWork?.GetRepository<Product>();
                var outProRepo = _unitOfWork?.GetRepository<OutletProduct>();

                foreach (var prod in pdeImport)
                {
                    if (requestModel.PrintType.ToLower() == "pdepricecheck" && prod.Price > 0)
                    {
                        if (prod.Price % 1 == 0)
                            prod.Price = (prod.Price) / 100;
                    }

                    if (prod.RecordType.ToLower() == "hostitem")
                    {
                        prod.ProductNumber = prod.ProductNumber + hostItemNumber;
                    }

                    if (prod.RecordType.ToLower() == "apn")
                    {

                        ProdId = await apnRepo.GetAll().Include(x => x.Product)
                            .Where(x => !x.IsDeleted && x.Number == prod.ProductNumber && !x.Product.IsDeleted && x.Product.Status &&
                              (x.Product.OutletProductProduct.Where(x => !x.IsDeleted && x.Status && x.StoreId == requestModel.StoreId).Any()))
                            .Select(x => x.ProductId).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                    }
                    else
                    {
                        ProdId = await prodRepo.GetAll().Include(c => c.OutletProductProduct).Where(x => !x.IsDeleted && x.Status && x.Number == prod.ProductNumber && (x.OutletProductProduct.Where(x => !x.IsDeleted && x.Status && x.StoreId == requestModel.StoreId).Any()))
                            .Select(x => x.Id).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                        if (ProdId == 0)
                        {
                            ProdId = await apnRepo.GetAll().Include(x => x.Product).ThenInclude(c => c.OutletProductProduct)
                                 .Where(x => !x.IsDeleted && x.Number == prod.ProductNumber && !x.Product.IsDeleted && x.Product.Status &&
                                 (x.Product.OutletProductProduct.Where(x => !x.IsDeleted && x.Status && x.StoreId == requestModel.StoreId).Any()))
                                .Select(x => x.ProductId).FirstOrDefaultAsyncSafe().ConfigureAwait(false);
                        }
                    }
                    if (ProdId != 0)
                    {
                        PdeActiveItemsCount++;

                        var outProd = await outProRepo.GetAll().Where(x => x.ProductId == ProdId && x.StoreId == requestModel.StoreId && x.Status && !x.IsDeleted).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                        outProd.ChangeLabelInd = true;
                        outProRepo.DetachLocal(x => x.Id == outProd.Id);
                        outProRepo.Update(outProd);
                    }
                }

                if (PdeActiveItemsCount > 0 && requestModel.PrintType.ToLower() == "pricecheck")
                {
                    throw new AlreadyExistsException($" {PdeActiveItemsCount} {ErrorMessages.PDEPriceCheckFound.ToString(CultureInfo.CurrentCulture)}");
                }

                if (PdeActiveItemsCount < 0 && requestModel.PrintType.ToLower() == "pricecheck")
                {
                    throw new NotFoundException($" {PdeActiveItemsCount} {ErrorMessages.PDEPriceCheckNotFound.ToString(CultureInfo.CurrentCulture)}");
                }


                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                requestModel.PrintType = "change";
                return _reportService.GetPrintLabel(requestModel, mime);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

    }
}
