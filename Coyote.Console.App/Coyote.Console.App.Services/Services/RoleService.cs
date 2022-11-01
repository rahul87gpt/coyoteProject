using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using Coyote.Console.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Coyote.Console.App.Services
{
    public class RoleService : IRoleService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;
        private readonly IAccessDepartmentServices _iAccessDepartmentService = null;
        /// <summary>
        /// RoleService Constructor, initilize CoyoteAppDBContext,IAutoMappingServices
        /// </summary>
        /// <param name="coyoteConsoleAppDBContext"></param>
        /// <param name="iAutoMapperService"></param>
        public RoleService(IUnitOfWork context, IAutoMappingServices iAutoMapperService, ILoggerManager iLoggerManager, IAccessDepartmentServices iAccessDepartmentService)
        {
            _unitOfWork = context;
            _iAutoMapper = iAutoMapperService;
            _iLoggerManager = iLoggerManager;
            _iAccessDepartmentService = iAccessDepartmentService;
        }

        /// <summary>
        /// return true or false when role deletion is success or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool- true or false</returns>
        public async Task<bool> DeleteRole(int id, int userId)
        {
            try
            {
                if (id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Roles>();
                    var exists = await repository.GetById(id).ConfigureAwait(false);
                    if (exists != null)
                    {
                        if (!exists.IsDeleted)
                        {
                            exists.UpdatedById = userId;
                            exists.IsDeleted = true;
                         //   exists.Code = (exists.Code + "~" + exists.Id);
                           // exists.Name = (exists.Name + "~" + exists.Id);
                            repository?.Update(exists);
                            await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                            return true;
                        }
                    }
                    throw new NotFoundException(ErrorMessages.RoleNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.RoleIdNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// returns all roles with Is_Deleted is false
        /// </summary>
        /// <returns>List of Role View model</returns>
        public async Task<PagedOutputModel<List<RolesViewModel>>> GetAllActiveRoles(PagedInputModel inputModel)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Roles>();
                var roles = repository.GetAll(x => !x.IsDeleted && x.Type != "SuperAdmin");


                int count = 0;
                if (inputModel != null)
                {
                    if (!string.IsNullOrEmpty((inputModel?.GlobalFilter)))
                        roles = roles.Where(x => x.Name.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Code.ToLower().Contains(inputModel.GlobalFilter.ToLower()) || x.Type.ToLower().Contains(inputModel.GlobalFilter.ToLower()));

                    if (!string.IsNullOrEmpty((inputModel?.Status)))
                        roles = roles.Where(x => x.Status);

                    roles = roles.OrderByDescending(x => x.UpdatedAt);
                    count = roles.Count();

                    if (inputModel.MaxResultCount.HasValue && inputModel.SkipCount.HasValue)
                        roles = roles.Skip(inputModel.SkipCount.Value).Take(inputModel.MaxResultCount.Value);

                    if (!string.IsNullOrEmpty(inputModel.Sorting))
                    {
                        switch (inputModel.Sorting.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "name":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    roles = roles.OrderBy(x => x.Name);
                                else
                                    roles = roles.OrderByDescending(x => x.Name);
                                break;
                            case "type":
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    roles = roles.OrderBy(x => x.Type);
                                else
                                    roles = roles.OrderByDescending(x => x.Type);
                                break;
                            default:
                                if (string.IsNullOrEmpty(inputModel.Direction))
                                    roles = roles.OrderBy(x => x.Code);
                                else
                                    roles = roles.OrderByDescending(x => x.Code);
                                break;
                        }
                    }
                }
                List<RolesViewModel> roleViewModel;
                roleViewModel = (await roles.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<Roles, RolesViewModel>).ToList();
                return new PagedOutputModel<List<RolesViewModel>>(roleViewModel, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.RoleNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get User View Model by Role_Id
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <returns></returns>
        public async Task<RolesViewModel> GetRoleById(int roleId)
        {
            try
            {
                if (roleId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Roles>();
                    var role = await repository.GetById(roleId).ConfigureAwait(false);
                    if (role == null || role.IsDeleted)
                    {
                        throw new NotFoundException(ErrorMessages.RoleNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    RolesViewModel roleViewModel;
                    roleViewModel = _iAutoMapper.Mapping<Roles, RolesViewModel>(role);
                    return roleViewModel;
                }
                throw new NullReferenceException(ErrorMessages.RoleNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> Update(RolesViewModel viewModel, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.Id == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.RoleIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    var repository = _unitOfWork?.GetRepository<Roles>();
                    if (await repository.GetAll(x => x.Name == viewModel.Name && x.Id != viewModel.Id && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new NullReferenceException(ErrorMessages.RoleNameDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    if (await repository.GetAll(x => x.Code == viewModel.Code && x.Id != viewModel.Id && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new NullReferenceException(ErrorMessages.RoleCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    var exists = await repository.GetById(viewModel.Id).ConfigureAwait(false);
                    //var accessDept = false;
                    if (exists != null)
                    {
                         
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.RoleNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        if (viewModel.PermissionSet != "*")
                        {
                            var repoRoleDefaultPermission = _unitOfWork?.GetRepository<RolesDefaultPermissions>();
                            //string defaultPermissions = string.Empty;
                            string defaultPermissions = CommonMessages.DefaultRolePermission;
                            //Add default permission from list of permissions
                            foreach (var permission in viewModel.PermissionSet.Split(",", StringSplitOptions.RemoveEmptyEntries))
                            {
                                var dp = repoRoleDefaultPermission.GetAll(x => !x.IsDeleted && permission == (x.ModuleName + "." + x.HttpVerb)).FirstOrDefault()?.DefaultRolePermissionss;
                                if (dp != null)
                                {
                                    defaultPermissions += "," + dp;
                                }
                            }
                            viewModel.PermissionSet += "," + defaultPermissions.Trim(',');
                        }
                        if(true) // set value according to condition 
                        {
                            var repoAccessDept = _unitOfWork?.GetRepository<AccessDepartment>();

                            var repoAccessDeptList = repoAccessDept.GetAll(x => !x.IsDeleted && x.RoleId == viewModel.Id).ToList();

                            if(repoAccessDeptList.Count>0)
                            {

                                foreach(var res in repoAccessDeptList) 
                                {
                                    await _iAccessDepartmentService.DeleteAccessDepartment(res.Id, userId).ConfigureAwait(false);
                                }
                            }


                            if (viewModel.PermissionDeptSet != "*" && !String.IsNullOrEmpty(viewModel.PermissionDeptSet))
                            {
                                foreach (var permissionDept in viewModel.PermissionDeptSet.Split(",", StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int pd = Convert.ToInt32(permissionDept);
                                    var dp = repoAccessDept.GetAll(x => !x.IsDeleted && x.RoleId == viewModel.Id && x.DepartmentId == pd).FirstOrDefault();
                                    if (dp == null)
                                    {
                                        AccessDepartmentViewModel adv = new AccessDepartmentViewModel();
                                        adv.DepartmentId = pd;
                                        adv.RoleId = viewModel.Id;
                                        adv.IsDeleted = false;

                                        await _iAccessDepartmentService.Insert(adv, userId).ConfigureAwait(false);

                                    }
                                }
                            }
                       
                        
                        }

                        var comm = _iAutoMapper.Mapping<RolesViewModel, Roles>(viewModel);
                        comm.IsDeleted = false;
                        comm.UpdatedById = userId;
                        comm.CreatedAt = exists.CreatedAt;
                        comm.CreatedById = exists.CreatedById;
                        //Detaching tracked entry - exists
                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NotFoundException(ErrorMessages.RoleNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException();
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

        public async Task<int> Insert(RolesViewModel viewModel, int userId)
        {
            int resultId = 0;
            try
            {
                if (viewModel != null)
                {

                    var repository = _unitOfWork?.GetRepository<Roles>();
                    var repoRoleDefaultPermission = _unitOfWork?.GetRepository<RolesDefaultPermissions>();
                    if (await repository.GetAll(x => x.Name == viewModel.Name && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.RoleNameDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    if (await repository.GetAll(x => x.Code == viewModel.Code && !x.IsDeleted ).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.RoleCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    }

                    //var accessDept = false;
                    if (viewModel.PermissionSet != "*")
                    {
                        string defaultPermissions = CommonMessages.DefaultRolePermission;
                        //string defaultPermissions = string.Empty;
                        //Add default permission from list of permissions
                        foreach (var permission in viewModel.PermissionSet.Split(",", StringSplitOptions.RemoveEmptyEntries))
                        {
                            var dp = repoRoleDefaultPermission.GetAll(x => !x.IsDeleted && permission == (x.ModuleName + "." + x.HttpVerb)).FirstOrDefault()?.DefaultRolePermissionss;
                            if (dp != null)
                            {
                                defaultPermissions += "," + dp;
                            }
                        }
                        viewModel.PermissionSet += "," + defaultPermissions.Trim(',');
                    }



                    var comm = _iAutoMapper.Mapping<RolesViewModel, Roles>(viewModel);
                    

                    comm.IsDeleted = false;
                    comm.CreatedById = userId;
                    comm.UpdatedById = userId;
                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        resultId = result.Id;
                    }

                    if (true) // set value according to condition 
                    {
                        var repoAccessDept = _unitOfWork?.GetRepository<AccessDepartment>();

                        var repoAccessDeptList = repoAccessDept.GetAll(x => !x.IsDeleted && x.RoleId == viewModel.Id).ToList();

                        if (repoAccessDeptList.Count > 0)
                        {

                            foreach (var res in repoAccessDeptList)
                            {
                                await _iAccessDepartmentService.DeleteAccessDepartment(res.Id, userId).ConfigureAwait(false);
                            }
                        }

                        if (viewModel.PermissionDeptSet != "*" && !String.IsNullOrEmpty(viewModel.PermissionDeptSet))
                        {
                            foreach (var permissionDept in viewModel.PermissionDeptSet.Split(",", StringSplitOptions.RemoveEmptyEntries))
                            {
                                int pd = Convert.ToInt32(permissionDept);
                                var dp = repoAccessDept.GetAll(x => !x.IsDeleted && x.RoleId == viewModel.Id && x.DepartmentId == pd).FirstOrDefault();
                                if (dp == null)
                                {
                                    AccessDepartmentViewModel adv = new AccessDepartmentViewModel();
                                    adv.DepartmentId = pd;
                                    adv.RoleId = resultId;
                                    adv.IsDeleted = false;

                                    await _iAccessDepartmentService.Insert(adv, userId).ConfigureAwait(false);

                                }
                            }
                        }


                    }
                }
                return resultId;
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
