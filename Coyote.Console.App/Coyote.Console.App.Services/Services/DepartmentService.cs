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

namespace Coyote.Console.App.Services
{
    public class DepartmentService : IDepartmentService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private readonly ILoggerManager _iLoggerManager = null;
        public DepartmentService(IUnitOfWork iUnitOfWork, IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager)
        {
            _unitOfWork = iUnitOfWork;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }

        public async Task<PagedOutputModel<List<DepartmentResponseModel>>> GetAllActiveDepartments(PagedInputModel filter = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Department>();
                var list = repository.GetAll(x => !x.IsDeleted, includes: new Expression<Func<Department, object>>[] { c => c.MapTypeMasterListItems });
                int count = 0;
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        list = list.Where(x => x.Code.ToLower().Contains(filter.GlobalFilter.ToLower()) || x.Desc.ToLower().Contains(filter.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((filter?.Status)))
                        list = list.Where(x => x.Status);

                    list = list.OrderByDescending(x => x.UpdatedAt);
                    count = list.Count();

                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);
                    switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
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
                                list = list.OrderByDescending(x => x.UpdatedAt);
                            else
                                list = list.OrderBy(x => x.UpdatedAt);
                            break;
                    }

                }
                
                var listViewModel = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<Department, DepartmentResponseModel>).ToList();
                foreach (var item in listViewModel)
                {
                    item.MapType = list.Where(x => x.Id == item.Id).Select(x => x.MapTypeMasterListItems.Name).FirstOrDefault();
                }
                return new PagedOutputModel<List<DepartmentResponseModel>>(listViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get Depaertments list using SP
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PagedOutputModel<List<DepartmentResponseModel>>> GetActiveDepartments(PagedInputModel inputModel = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Department>();
                int count = 0;
                List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                        new SqlParameter("@SkipCount", inputModel?.SkipCount),
                        new SqlParameter("@MaxResultCount", inputModel?.MaxResultCount),
                        new SqlParameter("@SortColumn", inputModel?.Sorting),
                        new SqlParameter("@SortDirection", inputModel?.Direction),
                        new SqlParameter("@IsCountRequired", 1)
                    };
                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetAllDepartments, dbParams.ToArray()).ConfigureAwait(false);
                List<DepartmentResponseModel> listVM = MappingHelpers.ConvertDataTable<DepartmentResponseModel>(dset.Tables[0]);
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["RecordCount"]);
                return new PagedOutputModel<List<DepartmentResponseModel>>(listVM, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<DepartmentResponseModel> GetDepartmentById(int deptId)
        {
            try
            {
                if (deptId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Department>();
                    var dept = await repository.GetAll(x=>x.Id==deptId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (dept == null)
                    {
                        throw new NotFoundException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (!dept.IsDeleted)
                    {
                        DepartmentResponseModel deptViewModel;
                        deptViewModel = _iAutoMapper.Mapping<Department, DepartmentResponseModel>(dept);
                        if(deptViewModel != null)
                        {
                            deptViewModel.BudgetGrowthFactor = dept.BudgetGroethFactor;
                        }
                        return deptViewModel;
                    }

                    throw new NotFoundException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> DeleteDepartment(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Department>();
                    var exists = await repository.GetById(Id).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        exists.UpdatedById = userId;
                        exists.IsDeleted = true;
                      //  exists.Code = (exists.Code + "~" + exists.Id);
                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.DepartmentIdNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<DepartmentResponseModel> Update(DepartmentRequestModel viewModel, int deptId, int user)
        {
            try
            {
                DepartmentResponseModel responseModel = new DepartmentResponseModel();
                if (viewModel != null)
                {
                    if (deptId <= 0)
                    {
                        throw new NullReferenceException(ErrorMessages.DepartmentIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var repository = _unitOfWork?.GetRepository<Department>();
                    var exists = await repository.GetById(deptId).ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (exists.Code != viewModel.Code && (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted && x.Id !=deptId).AnyAsync().ConfigureAwait(false)))
                        {
                            throw new AlreadyExistsException(ErrorMessages.DepartmentCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        }
                        if (exists.IsDeleted == true)
                        {
                            throw new AlreadyExistsException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        var dept = _iAutoMapper.Mapping<DepartmentRequestModel, Department>(viewModel);

                        dept.BudgetGroethFactor = viewModel.BudgetGrowthFactor;
                        dept.IsDeleted = false;
                        dept.Id = deptId;
                        dept.CreatedAt = exists.CreatedAt;
                        dept.CreatedById = exists.CreatedById;
                        dept.UpdatedById = user;
                        dept.Status = true;
                        //Detaching tracked entry - exists
                        repository.DetachLocal(_ => _.Id == dept.Id);
                        //_iDeptRepo.Update(dept); 
                        repository.Update(dept);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        responseModel = await GetDepartmentById(dept.Id).ConfigureAwait(false);

                        return responseModel;
                    }
                    throw new NotFoundException(ErrorMessages.DepartmentNotFound.ToString(CultureInfo.CurrentCulture));

                }
                throw new NullReferenceException();
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

        public async Task<DepartmentResponseModel> Insert(DepartmentRequestModel viewModel, int userId)
        {
            DepartmentResponseModel responseModel = new DepartmentResponseModel();
            try
            {
                if (viewModel != null)
                {
                    var repository = _unitOfWork?.GetRepository<Department>();
                    if ((await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted ).AnyAsync().ConfigureAwait(false)))
                    {
                        throw new AlreadyExistsException(ErrorMessages.DepartmentCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }

                    var dept = _iAutoMapper.Mapping<DepartmentRequestModel, Department>(viewModel);

                    dept.BudgetGroethFactor = viewModel.BudgetGrowthFactor;
                    dept.IsDeleted = false;
                    dept.CreatedById = userId;
                    dept.UpdatedById = userId;
                    dept.CreatedAt = DateTime.UtcNow;
                    dept.UpdatedAt = DateTime.UtcNow;
                    dept.CreatedById = userId;
                    dept.UpdatedById = userId;
                    dept.Status = true;
                    var result = await repository.InsertAsync(dept).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        responseModel = await GetDepartmentById(result.Id).ConfigureAwait(false);
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
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

    }
}
