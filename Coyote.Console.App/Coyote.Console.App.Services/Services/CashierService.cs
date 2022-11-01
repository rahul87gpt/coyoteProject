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
using Microsoft.Extensions.Options;

namespace Coyote.Console.App.Services.Services
{
    public class CashierService : ICashierServices
    {
        private IUnitOfWork _unitOfWork = null;
        private IAutoMappingServices _iAutoMapper;
        // private readonly IImageUploadHelper _iImageUploader = null;

        public CashierService(IOptions<AppSettings> appsetting, IUnitOfWork unitOfWork, IAutoMappingServices autoMappingServices)
        {
            _appsetting = appsetting?.Value;
            _unitOfWork = unitOfWork;
            _iAutoMapper = autoMappingServices;
        }

        private AppSettings _appsetting
        {
            get; set;
        }

        /// <summary>
        /// Add new Cashier
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CashierResponseModel> Insert(CashierRequestModel viewModel, int userId)
        {
            CashierResponseModel responseModel = new CashierResponseModel();
            try
            {
                if (viewModel != null)
                {
                    if (string.IsNullOrEmpty(viewModel.Dispname))
                    {
                        throw new BadRequestException(ErrorMessages.CashierDispname.ToString(CultureInfo.CurrentCulture));
                    }
                    if (string.IsNullOrEmpty(viewModel.Password))
                    {
                        throw new BadRequestException(ErrorMessages.CashierPassword.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.StoreGroupId == 0 || viewModel.StoreGroupId == null)
                    {
                        viewModel.OutletId = null;
                        viewModel.StoreGroupId = null;
                        //viewModel.ZoneId = null;
                    }

                    if (String.IsNullOrEmpty(viewModel.LeftHandTillInd))
                    {
                        viewModel.LeftHandTillInd = "Rgt";
                    }

                    var repository = _unitOfWork?.GetRepository<Cashier>();

                    var cashierRequestDataTable = MappingHelpers.ToDataTable(viewModel);
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@CashierId", 0),
                        new SqlParameter("@UserId", userId),
                        new SqlParameter{
                            Direction = ParameterDirection.Input,
                            ParameterName = "@CashierRequest",
                            TypeName ="dbo.CashierRequestType",
                            Value = cashierRequestDataTable,
                            SqlDbType = SqlDbType.Structured
                        },

                        new SqlParameter("@ActionPerformed",ActionPerformed.Create.ToString()),
                    };
                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.ADDUpdateCashier, dbParams.ToArray()).ConfigureAwait(false);
                    if (dset?.Tables?.Count > 0)
                    {
                        if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                        {
                            throw new BadRequestException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                        }
                        else
                        {
                            responseModel = await GetCashierById(Convert.ToInt32(dset.Tables[0].Rows[0]["CashierId"])).ConfigureAwait(false);
                        }
                    }
                    return responseModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.DeptId.ToString(CultureInfo.CurrentCulture));
                }
            }
            catch (Exception ex)
            {
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
        /// Update Existing cashier
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CashierResponseModel> Update(CashierRequestModel viewModel, int cashierId, int userId, string imagePath = null)
        {
            try
            {
                CashierResponseModel responseModel = new CashierResponseModel();
                var repository = _unitOfWork?.GetRepository<Cashier>();
                if (!await (repository.GetAll(x => x.Id == cashierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                {
                    throw new NotFoundException(ErrorMessages.CashierNotFound.ToString(CultureInfo.CurrentCulture));
                }

                if (viewModel != null)
                {
                    if (cashierId <= 0)
                    {
                        throw new BadRequestException(ErrorMessages.CashierIdRequired.ToString(CultureInfo.CurrentCulture));
                    }

                    if (string.IsNullOrEmpty(viewModel.Dispname))
                    {
                        throw new BadRequestException(ErrorMessages.CashierDispname.ToString(CultureInfo.CurrentCulture));
                    }
                    if (string.IsNullOrEmpty(viewModel.Password))
                    {
                        throw new BadRequestException(ErrorMessages.CashierPassword.ToString(CultureInfo.CurrentCulture));
                    }

                    if (viewModel.StoreGroupId == 0 || viewModel.StoreGroupId == null)
                    {
                        viewModel.OutletId = null;
                        viewModel.StoreGroupId = null;
                        //viewModel.ZoneId = null;
                    }

                    var storeRepository = _unitOfWork?.GetRepository<Store>();
                    var masterListRepostiory = _unitOfWork?.GetRepository<MasterList>();
                    var masterRepository = _unitOfWork?.GetRepository<MasterListItems>();


                    if (String.IsNullOrEmpty(viewModel.LeftHandTillInd))
                    {
                        viewModel.LeftHandTillInd = "Rgt";
                    }

                    if (!String.IsNullOrEmpty(imagePath))
                    {
                        viewModel.ImagePath = imagePath;
                    }

                    //if ((await repository.GetAll(x => x.Number == viewModel.Number && x.Id != cashierId && !x.IsDeleted).AnyAsync().ConfigureAwait(false)))
                    //{
                    //    throw new AlreadyExistsException(ErrorMessages.CashierAlreadyExist.ToString(CultureInfo.CurrentCulture));
                    //}


                    var cashierExist = await repository.GetAll(x => x.Id == cashierId && !x.IsDeleted).FirstAsync().ConfigureAwait(false);
                    if (cashierExist != null)
                    {
                        var cashierRequestDataTable = MappingHelpers.ToDataTable(viewModel);

                        List<SqlParameter> dbParams = new List<SqlParameter>{

                        new SqlParameter("@CashierId", cashierId),

                        new SqlParameter("@UserId", userId),

                        new SqlParameter{

                            Direction = ParameterDirection.Input,

                            ParameterName = "@CashierRequest",

                            TypeName ="dbo.CashierRequestType",

                            Value = cashierRequestDataTable,

                            SqlDbType = SqlDbType.Structured

                        },

                        new SqlParameter("@ActionPerformed",ActionPerformed.Update.ToString()),


                    };

                        var dset = await repository.ExecuteStoredProcedure(StoredProcedures.ADDUpdateCashier, dbParams.ToArray()).ConfigureAwait(false);

                        if (dset?.Tables?.Count > 0)
                        {
                            if (Convert.ToBoolean(dset.Tables[0].Rows[0]["IsErrorFound"]))
                            {
                                throw new NotFoundException(Convert.ToString(dset.Tables[0].Rows[0]["ErrorMessage"]));
                            }
                            else
                            {
                                responseModel = await GetCashierById(Convert.ToInt32(dset.Tables[0].Rows[0]["CashierId"])).ConfigureAwait(false);
                            }


                        }
                    }
                }
                return responseModel;
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
        /// Get cashier by Id
        /// </summary>
        /// <param name="cashierId"></param>
        /// <returns></returns>
        public async Task<CashierResponseModel> GetCashierById(long cashierId)
        {
            try
            {
                if (cashierId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Cashier>();
                    List<SqlParameter> dbParams = new List<SqlParameter>{
                      new SqlParameter("@Id", cashierId)
                    };

                    var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllCashier, dbParams.ToArray()).ConfigureAwait(false);
                    List<CashierResponseModel> cashierViewModel = MappingHelpers.ConvertDataTable<CashierResponseModel>(dset.Tables[0]);
                    var count = 0;
                    count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                    var cashier = await repository.GetAll(x => x.Id == cashierId && !x.IsDeleted).Include(c => c.Zone).Include(c => c.StoreGroup).Include(c => c.Type).Include(c => c.AccessLevel).Include(c => c.Outlet).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (cashierViewModel == null || cashierViewModel.Count == 0)
                    {
                        throw new NotFoundException(ErrorMessages.CashierNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    CashierResponseModel cashierModel = cashierViewModel[0];

                    if (!string.IsNullOrEmpty(cashier.ImagePath))
                    {
                        Byte[] imageBytes;
                        string imageFolderPath = Directory.GetCurrentDirectory() + cashier.ImagePath;
                        if (File.Exists(imageFolderPath))
                        {
                            imageBytes = System.IO.File.ReadAllBytes(imageFolderPath);
                            cashierModel.ImageBytes = imageBytes;
                        }
                    }

                    return cashierModel;
                }
                else
                {
                    throw new NullReferenceCustomException(ErrorMessages.OutletProductId.ToString(CultureInfo.CurrentCulture));
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
                throw new Exception(ErrorMessages.OutletProductNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Get all active cashiers
        /// Along with seraching , sorting and paging.
        /// </summary>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<CashierResponseModel>>> GetAllActiveCashier(PagedInputModel inputModel, SecurityViewModel securityViewModel)
        {
            try
            {
                int RoleId = securityViewModel != null ? securityViewModel.RoleId : 0;
                var repository = _unitOfWork?.GetRepository<Cashier>();

                List<SqlParameter> dbParams = new List<SqlParameter>{
                      new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                      new SqlParameter("@PageNumber", inputModel?.SkipCount),
                      new SqlParameter("@PageSize", inputModel?.MaxResultCount),
                      new SqlParameter("@SortColumn", inputModel?.Sorting),
                      new SqlParameter("@SortDirection", inputModel?.Direction),
                      new SqlParameter("@Status", inputModel?.Status),
                      new SqlParameter("@IsLogged", inputModel?.IsLogged),
                      new SqlParameter("@Module", "Cashier"),
                      new SqlParameter("@RoleId",RoleId)
                };
                // var cashier = repository.GetAll(x => !x.IsDeleted);
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllCashier, dbParams.ToArray()).ConfigureAwait(false);
                List<CashierResponseModel> cashierViewModel = MappingHelpers.ConvertDataTable<CashierResponseModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);

                return new PagedOutputModel<List<CashierResponseModel>>(cashierViewModel, count);

                //var cashier = repository.GetAll().Include(c=>c.StoreGroup).Include(c=>c.Outlet).Include(c => c.Zone).Include(c => c.Type).Include( c => c.AccessLevel).Where(x => !x.IsDeleted);
                //int count = 0;
                //if (inputModel != null)
                //{
                //    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                //        cashier = cashier.Where(x => x.FirstName.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Surname.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Number.ToString().ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                //    if (securityViewModel != null && !securityViewModel.IsAdminUser && securityViewModel.StoreIds != null && securityViewModel.StoreIds.Count > 0)
                //        cashier = cashier.Where(x => securityViewModel.StoreIds.Contains(x.OutletId.Value));

                //    if (!string.IsNullOrEmpty((inputModel?.Status)))
                //        cashier = cashier.Where(x => x.Status);

                //    cashier = cashier.OrderByDescending(x => x.UpdatedAt);

                //    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                //        cashier = cashier.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                //    if (!string.IsNullOrEmpty(inputModel.Sorting))
                //    {
                //        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                //        {
                //            case "number":
                //                if (string.IsNullOrEmpty(inputModel.Direction))
                //                    cashier = cashier.OrderBy(x => x.Number);
                //                else
                //                    cashier = cashier.OrderByDescending(x => x.Number);
                //                break;
                //            case "name":
                //                if (string.IsNullOrEmpty(inputModel.Direction))
                //                    cashier = cashier.OrderBy(x => x.FirstName);
                //                else
                //                    cashier = cashier.OrderByDescending(x => x.FirstName);
                //                break;
                //            default:
                //                if (string.IsNullOrEmpty(inputModel.Direction))
                //                    cashier = cashier.OrderBy(x => x.UpdatedAt);
                //                else
                //                    cashier = cashier.OrderByDescending(x => x.UpdatedAt);
                //                break;
                //        }
                //    }

                //}
                //count = cashier.Count();
                //List<CashierResponseModel> cashierModels;
                //cashierModels = (await cashier.ToListAsyncSafe().ConfigureAwait(false)).Select(MappingHelpers.CreateMap).ToList();

                //return new PagedOutputModel<List<CashierResponseModel>>(cashierModels, count);
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
        /// Delete cashier
        /// </summary>
        /// <param name="cashierId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(long cashierId, int userId)
        {
            try
            {
                if (cashierId > 0)
                {
                    var _repository = _unitOfWork?.GetRepository<Cashier>();
                    var cashierExists = await _repository.GetAll(x => x.Id == cashierId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (cashierExists != null)
                    {
                        if (!cashierExists.IsDeleted)
                        {
                            cashierExists.UpdatedById = userId;
                            cashierExists.IsDeleted = true;
                            _repository?.Update(cashierExists);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                        throw new NullReferenceCustomException(ErrorMessages.CashierNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }
                throw new NullReferenceCustomException(ErrorMessages.CashierIdRequired.ToString(CultureInfo.CurrentCulture));
            }
            catch (NullReferenceCustomException nre)
            {
                throw new NullReferenceCustomException(nre.Message, nre);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessages.CashierNotFound.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
    }
}
