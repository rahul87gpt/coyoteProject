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
    public class UserRoleService : IUserRoleService
    {
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;

        /// <summary>
        /// User_RoleService Constructor, initilize CoyoteAppDBContext,IAutoMappingServices
        /// </summary>
        /// <param name="userRoleRepo"></param>
        /// <param name="iAutoMapper"></param>
        public UserRoleService(IUnitOfWork userRoleRepo, IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager)
        {
            _unitOfWork = userRoleRepo;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }

        /// <summary>
        /// returns all user_roles with Is_Deleted is false
        /// </summary>
        /// <returns>List of UserRole View model</returns>
        public async Task<PagedOutputModel<List<UserRoleResponseModel>>> GetAllActiveUserRoles(PagedInputModel filter = null, int? UserId = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<UserRoles>();
                var list = repository.GetAll(x => (!UserId.HasValue || x.UserId == UserId) && !x.IsDeleted && /*!x.Stores.IsDeleted &&*/ !x.Roles.IsDeleted && !x.UserRoleList.IsDeleted, include: x => x.Include(z => z.UserRoleList).Include(z => z.Roles)/*.Include(z => z.Stores)*/);
                int count = 0;
                if (filter != null)
                {

                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);
                    switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "roleid":
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.RoleId);
                            else
                                list = list.OrderByDescending(x => x.RoleId);
                            break;
                        case "userid":
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.UserId);
                            else
                                list = list.OrderByDescending(x => x.UserId);
                            break;
                        default:
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.Id);
                            else
                                list = list.OrderByDescending(x => x.Id);
                            break;
                    }
                }
                list = list.OrderByDescending(x => x.UpdatedAt);
                count = list.Count();
                var t = (await list.ToListAsyncSafe().ConfigureAwait(false));
                var listViewModel = t.Select(_iAutoMapper.Mapping<UserRoles, UserRoleResponseModel>).ToList();
                foreach (var x in listViewModel)
                {
                    x.Role = t.Where(y => y.RoleId == x.RoleId && y.UserId == x.UserId/* && y.StoreId == x.StoreId*/).Select(y =>
                    new RoleResponseViewModel
                    {
                        Code = y.Roles.Code,
                        Id = y.Roles.Id,
                        Name = y.Roles.Name,
                        IsDefualt = y.IsDefault
                    }).FirstOrDefault();
                    // x.Store = t.Where(y => y.RoleId == x.RoleId && y.UserId == x.UserId && y.StoreId == x.StoreId).Select(y =>
                    //new StoreResponseViewModel
                    //{
                    //    Code = y.Stores.Code,
                    //    Id = y.Stores.Id,
                    //    Name = y.Stores.Desc
                    //}).FirstOrDefault();
                    x.User = t.Where(y => y.RoleId == x.RoleId && y.UserId == x.UserId /*&& y.StoreId == x.StoreId*/).Select(y =>
                   new UserDetailResponseModel()
                   {
                       FirstName = y.UserRoleList.FirstName,
                       Id = y.UserRoleList.Id,
                       LastName = y.UserRoleList.LastName
                   }).FirstOrDefault();
                }

                return new PagedOutputModel<List<UserRoleResponseModel>>(listViewModel, count);

            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.UserRoleIdNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    throw new NotFoundException(ex.Message);
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }
        /// <summary>
        /// return true or false when User_role deletion is success or not
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>bool- true or false</returns>
        public async Task<bool> DeleteUserRole(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<UserRoles>();
                    var exists = await repository.GetById(Id).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        exists.UpdatedById = userId;
                        exists.IsDeleted = true;
                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.UserRoleNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.UserRoleNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get User_Role View Model by User_RoleId
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns>Object of User_Role View Model</returns>
        public async Task<UserRoleViewModel> GetUserRoleById(int userRoleId)
        {
            try
            {
                if (userRoleId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<UserRoles>();
                    var urole = await repository.GetById(userRoleId).ConfigureAwait(false);
                    if (urole == null)
                    {
                        throw new NotFoundException(ErrorMessages.UserRoleNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    return _iAutoMapper.Mapping<UserRoles, UserRoleViewModel>(urole);
                }
                throw new NullReferenceException(ErrorMessages.UserRoleIdNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> Update(UserRoleViewModel viewModel, int userId)
        {
            try
            {
                if (viewModel != null)
                {
                    viewModel.UpdatedById = userId;
                    if (viewModel.Id == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.UserIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.UpdatedById == 0)
                    {
                        throw new NullReferenceException(ErrorMessages.UpdatedByRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    var repository = _unitOfWork?.GetRepository<UserRoles>();
                    var exists = await repository.GetById(viewModel.Id).ConfigureAwait(false);

                    if (exists != null)
                    {
                        //if (exists.Code != viewModel.Code && await repository.GetAll(x => x.Code == viewModel.Code).AnyAsync().ConfigureAwait(false))
                        //{
                        //    throw new BadRequestException(ErrorMessages.UserRoleCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                        //}
                        //if (exists.Name != viewModel.Name && await repository.GetAll(x => x.Name == viewModel.Name).AnyAsync().ConfigureAwait(false))
                        //{
                        //    throw new BadRequestException(ErrorMessages.UserRoleNameDuplicate.ToString(CultureInfo.CurrentCulture));
                        //}
                        if (exists.IsDeleted == true)
                        {
                            throw new NotFoundException(ErrorMessages.UserRoleNotFound.ToString(CultureInfo.CurrentCulture));
                        }
                        viewModel.Id = exists.Id;
                        viewModel.CreatedAt = exists.CreatedAt;
                        viewModel.CreatedById = exists.CreatedById;
                        viewModel.UpdatedAt = DateTime.UtcNow;
                        var comm = _iAutoMapper.Mapping<UserRoleViewModel, UserRoles>(viewModel);
                        comm.IsDeleted = false;
                        repository.DetachLocal(_ => _.Id == comm.Id);
                        repository.Update(comm);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NotFoundException(ErrorMessages.UserRoleIdNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<int> Insert(UserRoleViewModel viewModel, int userId)
        {
            int resultId = 0;
            try
            {
                if (viewModel != null)
                {
                    viewModel.CreatedById = userId;
                    viewModel.UpdatedById = userId;
                    var repository = _unitOfWork?.GetRepository<UserRoles>();
                    //if (await repository.GetAll(x => x.Code == viewModel.Code).AnyAsync().ConfigureAwait(false))
                    //{
                    //    throw new BadRequestException(ErrorMessages.UserRoleCodeDuplicate.ToString(CultureInfo.CurrentCulture));
                    //}
                    //if (await repository.GetAll(x => x.Name == viewModel.Name).AnyAsync().ConfigureAwait(false))
                    //{
                    //    throw new BadRequestException(ErrorMessages.UserRoleNameDuplicate.ToString(CultureInfo.CurrentCulture));
                    //}
                    if (viewModel.CreatedById == 0 || viewModel.UpdatedById == 0)
                    {
                        throw new BadRequestException(ErrorMessages.CreatedUpdatedRequired.ToString(CultureInfo.CurrentCulture));
                    }
                    var comm = _iAutoMapper.Mapping<UserRoleViewModel, UserRoles>(viewModel);
                    comm.IsDeleted = false;
                    comm.CreatedAt = DateTime.UtcNow;
                    comm.UpdatedAt = DateTime.UtcNow;
                    var result = await repository.InsertAsync(comm).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        resultId = result.Id;
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
