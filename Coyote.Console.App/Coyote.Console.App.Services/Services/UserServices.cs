using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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

namespace Coyote.Console.App.Services
{
    public class UserServices : IUserServices
    {
        private ISendMailService _SendEmailService = null;
        private IUnitOfWork _unitOfWork = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;
        private readonly IUserRepository _repository = null;
        /// <summary>
        /// UserServices Constructor, initilize CoyoteAppDBContext,IAutoMappingServices
        /// </summary>
        /// <param name="IUserRepository"></param>
        /// <param name="iAutoMapper"></param>
        public UserServices(ISendMailService iSendEmailService, IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager, IUnitOfWork unitOfWork, IUserRepository repository)
        {
            _unitOfWork = unitOfWork;
            _SendEmailService = iSendEmailService;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
            _repository = repository;
        }

        /// <summary>
        /// returns all users with Is_Deleted is false
        /// </summary>
        /// <returns>List of User View model</returns>
        public async Task<PagedOutputModel<List<UserSavedResponseViewModel>>> GetAllActiveUsers(PagedInputModel filter = null)
        {
            try
            {
                var repository = _unitOfWork?.GetRepository<Users>();
                var list = repository.GetAll(user => user.IsDeleted == false);

                int count = 0;
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                        list = list.Where(x => x.Email.ToLower().Contains(filter.GlobalFilter.ToLower()) || x.FirstName.ToLower().Contains(filter.GlobalFilter.ToLower()) || x.LastName.ToLower().Contains(filter.GlobalFilter.ToLower()));

                    count = list.Count();
                    if (filter.MaxResultCount.HasValue && filter.SkipCount.HasValue)
                        list = list.Skip(filter.SkipCount.Value).Take(filter.MaxResultCount.Value);
                    switch (filter.Sorting?.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "email":
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.Email);
                            else
                                list = list.OrderByDescending(x => x.Email);
                            break;
                        case "firstname":
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderBy(x => x.FirstName);
                            else
                                list = list.OrderByDescending(x => x.FirstName);
                            break;
                        default:
                            if (string.IsNullOrEmpty(filter.Direction))
                                list = list.OrderByDescending(x => x.CreatedAt);
                            else
                                list = list.OrderBy(x => x.CreatedAt);
                            break;
                    }
                }
                var listViewModel = (await list.ToListAsyncSafe().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<Users, UserSavedResponseViewModel>).ToList();

                foreach (var user in listViewModel)
                {

                    if (!string.IsNullOrEmpty(user.ZoneIds))
                    {
                        var zoneRepo = _unitOfWork?.GetRepository<MasterListItems>();
                        var zoneIds = user.ZoneIds.Split(',').ToList();
                        foreach (var zoneId in zoneIds)
                        {
                            var zoneDset =await zoneRepo.GetAll(x => !x.IsDeleted && x.Code == zoneId).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                            user.ZoneList.Add(MappingHelpers.Mapping<MasterListItems,MasterListItemResponseViewModel>(zoneDset));

                            //user.ZoneIdList.Add(Convert.ToInt64(zoneId));
                        }
                    }
                    if (!string.IsNullOrEmpty(user.StoreIds))
                    {
                        var storeRepo = _unitOfWork?.GetRepository<Store>();
                        var storeIds = user.StoreIds.Split(',').ToList();
                        foreach (var storeId in storeIds)
                        {
                            var storeDset = await storeRepo.GetAll(x => !x.IsDeleted && x.Id == Convert.ToInt32(storeId)).FirstOrDefaultAsyncSafe().ConfigureAwait(false);

                            user.StoreList.Add(MappingHelpers.Mapping<Store, StoreResponseModel>(storeDset));
                        }
                    }

                }


                return new PagedOutputModel<List<UserSavedResponseViewModel>>(listViewModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.UserNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    throw new NotFoundException(ex.Message);
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// return true or false when user deletion is success or not
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>bool- true or false</returns>
        public async Task<bool> DeleteUser(int Id, int userId)
        {
            try
            {
                if (Id > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Users>();
                    var exists = await repository.GetById(Id).ConfigureAwait(false);
                    if (exists != null && !exists.IsDeleted)
                    {
                        exists.UpdatedById = userId;
                        exists.IsDeleted = true;
                        repository?.Update(exists);
                        await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                        return true;
                    }
                    throw new NullReferenceException(ErrorMessages.UserNotFound.ToString(CultureInfo.CurrentCulture));
                }
                throw new NullReferenceException(ErrorMessages.UserNotFound.ToString(CultureInfo.CurrentCulture));
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
        /// Get User View Model by User_Id
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns>Object of User_Role View Model</returns>
        public async Task<UserSavedResponseViewModel> GetUserById(int userId)
        {
            try
            {
                if (userId > 0)
                {
                    var repository = _unitOfWork?.GetRepository<Users>();
                    var user = await repository.GetAll(user => user.Id == userId && user.IsDeleted == false).Include(x => x.UserRolesCreated).ThenInclude(x => x.Roles).SingleOrDefaultAsync().ConfigureAwait(false);
                    if (user == null)
                    {
                        throw new NotFoundException(ErrorMessages.UserIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }

                    var response = _iAutoMapper.Mapping<Users, UserSavedResponseViewModel>(user);
                    response.Password = user.PlainPassword;

                    if (user.UserRolesCreated != null)
                    {
                        var roles = _repository.GetUserRoles(user.Id);
                        if (roles != null)
                        {
                            response.UserRolesList.AddRange(roles);
                        }
                        response.DefaultRoleId = user.UserRolesCreated.Where(x => !x.IsDeleted && x.IsDefault).Select(x => x.RoleId).FirstOrDefault();


                    }
                    if (!string.IsNullOrEmpty(user.ImagePath))
                    {
                        Byte[] imageBytes;
                        string imageFolderPath = Directory.GetCurrentDirectory() + user.ImagePath;
                        if (File.Exists(imageFolderPath))
                        {
                            imageBytes = System.IO.File.ReadAllBytes(imageFolderPath);
                            response.Image = imageBytes;
                        }
                    }
                    if (!string.IsNullOrEmpty(user.ZoneIds))
                    {
                        var zoneIds = user.ZoneIds.Split(',').ToList();
                        foreach (var zoneId in zoneIds)
                        {
                            response.ZoneIdList.Add(Convert.ToInt64(zoneId));
                        }
                    }
                    if (!string.IsNullOrEmpty(user.StoreIds))
                    {
                        var storeIds = user.StoreIds.Split(',').ToList();
                        foreach (var storeId in storeIds)
                        {
                            response.StoreIdList.Add(Convert.ToInt32(storeId));
                        }
                    }

                    return response;


                }
                throw new NotFoundException(ErrorMessages.UserIdNotFound.ToString(CultureInfo.CurrentCulture));
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

        public async Task<bool> Update(UserRequestModel viewModel, int userId, string imagePath = null)
        {
            try
            {
                if (viewModel != null)
                {

                    if (viewModel.DateOfBirth.HasValue)
                    {
                        if (viewModel.DateOfBirth.Value.CompareTo(DateTime.Now) >= 0)
                        {
                            throw new NullReferenceException(ErrorMessages.UserDOBInvalid.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    if (viewModel.DefaultRoleId <= 0)
                        throw new NullReferenceException(ErrorMessages.UserRoleDefaultRoleRequired.ToString(CultureInfo.CurrentCulture));

                    var repository = _unitOfWork?.GetRepository<Users>();
                    var exists = await repository.GetAll(x => x.Id == viewModel.Id && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (exists == null)
                    {
                        throw new NotFoundException(ErrorMessages.UserNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                    if (exists.Email != viewModel.Email && await repository.GetAll(x => x.Email == viewModel.Email && x.Id != viewModel.Id).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.UserEmailDuplidate.ToString(CultureInfo.CurrentCulture));
                    }
                    if (exists.UserName != viewModel.UserName && await repository.GetAll(x => x.UserName == viewModel.UserName && x.Id != viewModel.Id && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new AlreadyExistsException(ErrorMessages.UserNameDuplicate.ToString(CultureInfo.CurrentCulture));
                    }
                    if (viewModel.StoreIdList == null || viewModel.StoreIdList.Count == 0)
                    {
                        viewModel.StoreIds = null;
                    }
                    else
                    {
                        foreach (var storeId in viewModel.StoreIdList)
                        {
                            viewModel.StoreIds += storeId + ",";
                        }
                        if (!string.IsNullOrEmpty(viewModel.StoreIds))
                            viewModel.StoreIds = viewModel.StoreIds.Substring(0, viewModel.StoreIds.LastIndexOf(",", StringComparison.OrdinalIgnoreCase));
                    }
                    if (viewModel.ZoneIdList == null || viewModel.ZoneIdList.Count == 0)
                    {
                        viewModel.ZoneIds = null;
                    }
                    else
                    {
                        foreach (var zoneId in viewModel.ZoneIdList)
                        {
                            viewModel.ZoneIds += zoneId + ",";
                        }
                        if (!string.IsNullOrEmpty(viewModel.ZoneIds))
                            viewModel.ZoneIds = viewModel.ZoneIds.Substring(0, viewModel.ZoneIds.LastIndexOf(",", StringComparison.OrdinalIgnoreCase));
                    }
                    viewModel.Id = exists.Id;
                    var comm = _iAutoMapper.Mapping<UserRequestModel, Users>(viewModel);
                    comm.ImagePath = imagePath;

                    if (exists.PlainPassword == viewModel.Password)
                    {
                        comm.LastLogin = exists.LastLogin;
                    }
                    if (viewModel.Password == null)
                    { comm.PlainPassword = exists.Password; }
                    else
                    { comm.PlainPassword = viewModel.Password; }

                    comm.Password = EncryptDecryptAlgorithm.EncryptString(comm.PlainPassword);
                    comm.CreatedAt = exists.CreatedAt;
                    comm.CreatedById = exists.CreatedById;
                    comm.UpdatedAt = DateTime.UtcNow;
                    comm.UpdatedById = userId;
                    comm.IsDeleted = false;
                    repository.DetachLocal(_ => _.Id == comm.Id);
                    repository.Update(comm);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                    //updating user roles
                    var rolesModel = new UserRolesPutRequestModel
                    {
                        DefaultRoleId = viewModel.DefaultRoleId,
                        Roles = viewModel.RoleIdList
                    };
                    var roleResponse = await this.UpdateUserRoles(rolesModel, viewModel.Id, userId).ConfigureAwait(false);
                    if (!roleResponse)
                    {
                        throw new Exception(ErrorMessages.UserRoleUpdateFailure.ToString(CultureInfo.CurrentCulture));
                    }
                    return true;
                }
                throw new NullReferenceException(ErrorMessages.UserNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    throw new NullReferenceException(ex.Message);
                }
                if (ex is AlreadyExistsException)
                {
                    throw new AlreadyExistsException(ex.Message);
                }
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="id">user id to assign roles to</param>
        /// <param name="userId">logged in user id</param>
        /// <returns></returns>
        public async Task<bool> UpdateUserRoles(UserRolesPutRequestModel viewModel, int userId, int loggedUserId)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new NullReferenceException(ErrorMessages.UserRoleIdRequired.ToString(CultureInfo.CurrentCulture));
                }
                var roleRepo = _unitOfWork?.GetRepository<Roles>();
                if (viewModel.Roles == null)
                {
                    viewModel.Roles = new List<int>();
                }
                foreach (var role in viewModel.Roles)
                {
                    if (!await roleRepo.GetAll(x => x.Id == role && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        throw new NullReferenceException(ErrorMessages.UserRoleIdNotFound.ToString(CultureInfo.CurrentCulture));
                    }
                }

                /**
                 * Remove Roles that are not in List
                 * Add new Roles from List
                 * Mark Role Default from default role id
                 * **/
                //get current roles for the user
                var userRoleRepo = _unitOfWork?.GetRepository<UserRoles>();
                var userRoles = await userRoleRepo.GetAll(x => x.UserId == userId && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);
                //get roles to be deleted
                var deleteRoles = (from prevRole in userRoles
                                   join currRole in viewModel.Roles on prevRole.RoleId equals currRole into prev
                                   from newRole in prev.DefaultIfEmpty()
                                   where newRole == 0
                                   select new
                                   {
                                       prevRole
                                   }).ToList();
                //delete these Roles 
                foreach (var delRol in deleteRoles)
                {
                    var role = delRol.prevRole;
                    role.IsDeleted = true;
                    role.UpdatedAt = DateTime.UtcNow;
                    role.UpdatedById = loggedUserId;
                    userRoleRepo.Update(role);
                }
                //Assign new Roles
                List<int> newRoles = (from currRole in viewModel.Roles
                                      join prevRole in userRoles on currRole equals prevRole.RoleId into nr
                                      from newRole in nr.DefaultIfEmpty()
                                      where newRole is null
                                      select currRole).ToList();
                //Add these Roles 
                foreach (int role in newRoles)
                {
                    UserRoles ur = new UserRoles
                    {
                        UserId = userId,
                        RoleId = role,
                        IsDeleted = false,
                        IsDefault = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedById = loggedUserId,
                        UpdatedById = loggedUserId,
                    };
                    await userRoleRepo.InsertAsync(ur).ConfigureAwait(false);
                }

                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

                //Assign default role //if default role is 0 then assign any other 
                UserRoles defaultRole = null;
                if (viewModel.DefaultRoleId > 0)
                {
                    //update prev default role as not default and update default role to DefaultRoleId
                    var roles = await userRoleRepo.GetAll(x => x.UserId == userId && x.IsDefault && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);
                    foreach (var role in roles)
                    {
                        role.IsDefault = false;
                        userRoleRepo.Update(role);
                    }
                    defaultRole = await userRoleRepo.GetAll(x => x.UserId == userId && x.RoleId == viewModel.DefaultRoleId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                }
                else
                {
                    //if default role exists then don't update else update any role as default
                    if (!await userRoleRepo.GetAll(x => x.UserId == userId && x.IsDefault && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        defaultRole = await userRoleRepo.GetAll(x => x.UserId == userId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    }
                }
                if (defaultRole != null)
                {
                    defaultRole.IsDefault = true;
                    defaultRole.UpdatedAt = DateTime.UtcNow;
                    defaultRole.UpdatedById = loggedUserId;
                    userRoleRepo.Update(defaultRole);
                }
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                return true;
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

        public async Task<int> Insert(UserRequestModel userViewModel, int userId)
        {
            int resultId = 0;
            try
            {
                if (userViewModel != null)
                {
                    // If Image to be saved, throw error before saving
                    if (!string.IsNullOrEmpty(userViewModel.ImageName))
                    {
                        var postedFileExtension = Path.GetExtension(userViewModel.ImageName);
                        if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    var repository = _unitOfWork?.GetRepository<Users>();
                    if (await repository.GetAll(x => x.Email == userViewModel.Email).AnyAsync().ConfigureAwait(false))
                        throw new AlreadyExistsException(ErrorMessages.UserEmailDuplidate.ToString(CultureInfo.CurrentCulture));
                    if (string.IsNullOrEmpty(userViewModel.UserName))
                        throw new NullReferenceException(ErrorMessages.UserNameRequired.ToString(CultureInfo.CurrentCulture));
                    if (await repository.GetAll(x => x.UserName == userViewModel.UserName && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                        throw new AlreadyExistsException(ErrorMessages.UserNameDuplicate.ToString(CultureInfo.CurrentCulture));

                    if (userViewModel.DefaultRoleId <= 0)
                        throw new NullReferenceException(ErrorMessages.UserRoleDefaultRoleRequired.ToString(CultureInfo.CurrentCulture));

                    if (!userViewModel.RoleIdList.Contains(userViewModel.DefaultRoleId))
                        throw new BadRequestException(ErrorMessages.DefaultRoleInvalid.ToString(CultureInfo.CurrentCulture));

                    if (userViewModel.StoreIdList == null || userViewModel.StoreIdList.Count == 0)
                        userViewModel.StoreIds = null;
                    else
                    {
                        foreach (var storeId in userViewModel.StoreIdList)
                            userViewModel.StoreIds += storeId + ",";

                        if (!string.IsNullOrEmpty(userViewModel.StoreIds))
                            userViewModel.StoreIds = userViewModel.StoreIds.Substring(0, userViewModel.StoreIds.LastIndexOf(",", StringComparison.OrdinalIgnoreCase));
                    }

                    if (userViewModel.ZoneIdList == null || userViewModel.ZoneIdList.Count == 0)
                        userViewModel.ZoneIds = null;
                    else
                    {
                        foreach (var zoneId in userViewModel.ZoneIdList)
                            userViewModel.ZoneIds += zoneId + ",";

                        if (!string.IsNullOrEmpty(userViewModel.StoreIds))
                            userViewModel.ZoneIds = userViewModel.ZoneIds.Substring(0, userViewModel.ZoneIds.LastIndexOf(",", StringComparison.OrdinalIgnoreCase));
                    }

                    if (userViewModel.DateOfBirth.HasValue)
                    {
                        if (userViewModel.DateOfBirth.Value.CompareTo(DateTime.Now) >= 0)
                            throw new BadRequestException(ErrorMessages.UserDOBInvalid.ToString(CultureInfo.CurrentCulture));
                    }
                    var userTempPassword = Guid.NewGuid().ToString();
                    var user = _iAutoMapper.Mapping<UserRequestModel, Users>(userViewModel);

                    user.CreatedById = userId;
                    user.UpdatedById = userId;
                    user.CreatedAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                    user.IsResetPassword = true;
                    user.TemporaryPassword = userTempPassword;
                    Random newPassword = new Random();
                    user.PlainPassword = userViewModel.Password;
                    // user.PlainPassword = GenerateRandomString(6, newPassword);
                    user.Password = EncryptDecryptAlgorithm.EncryptString(user.PlainPassword);

                    var result = await repository.InsertAsync(user).ConfigureAwait(false);
                    await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);
                    if (result != null)
                    {
                        var rolesModel = new UserRolesPutRequestModel
                        {
                            DefaultRoleId = userViewModel.DefaultRoleId,
                            Roles = userViewModel.RoleIdList
                        };
                        var roleResponse = await this.UpdateUserRoles(rolesModel, result.Id, userId).ConfigureAwait(false);
                        if (roleResponse)
                        {
                            resultId = result.Id;
                            bool isForgotPassword = false;
                            await _SendEmailService.SendMail(user, isForgotPassword).ConfigureAwait(false);
                        }
                        else
                            throw new Exception(ErrorMessages.InternalErrorUserAdded.ToString(CultureInfo.CurrentCulture));
                    }
                }
                else
                {
                    throw new BadRequestException(ErrorMessages.UserFirstNameRequired.ToString(CultureInfo.CurrentCulture));
                }
                return resultId;

            }
            catch (Exception ex)
            {
                _iLoggerManager.WriteErrorLog(ex);
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
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        public SecurityViewModel GetUserAllowedStoresId(string email)
        {
            var userRepository = _unitOfWork?.GetRepository<Users>();
            var zoneOutletRepository = _unitOfWork?.GetRepository<ZoneOutlet>();
            var securityViewModel = new SecurityViewModel();
            var storeIds = new List<int>();
            var user = userRepository.GetAll(u => u.Email == email && !u.IsDeleted && u.Status).FirstOrDefault();

            var zones = user?.ZoneIds?.Split(',').Select(Int64.Parse).ToList();

            var stores = user?.StoreIds?.Split(',').Select(int.Parse).ToList();

            if ((zones == null || zones.Count == 0) && stores != null && stores.Count > 0)
            {
                storeIds = stores;
            }
            else if (zones != null && zones.Count > 0 && (stores == null || stores.Count == 0))
            {
                storeIds = zoneOutletRepository.GetAll(zo => zo.IsDeleted == false && zones.Contains(zo.ZoneId))
                    .Select(x => x.StoreId).Distinct().ToList();
            }
            else if (zones != null && zones.Count > 0 && stores != null && stores.Count > 0)
            {
                storeIds = stores;
            }
            securityViewModel.StoreIds = storeIds;
            securityViewModel.ZoneIds = zones;
            return securityViewModel;
        }

        public List<string> GetUserPermission(string email)
        {
            var repository = _unitOfWork?.GetRepository<Users>();
            var permissions = repository.GetAll(u => u.Email == email && u.IsDeleted == false && u.Status && u.RolesList.Any(ur => ur.IsDeleted == false && !ur.Roles.IsDeleted && ur.Roles.Status))
                .Select(u => u.RolesList.Where(ur => ur.IsDeleted == false && !ur.Roles.IsDeleted && ur.Roles.Status && ur.IsDefault).Select(ur => ur.Roles.PermissionSet).ToList())?.FirstOrDefault();

            List<string> pmList = new List<string>();

            if (permissions != null)
            {
                foreach (var pm in permissions)
                {
                    pmList.AddRange(pm.Split(",", StringSplitOptions.RemoveEmptyEntries));
                }
            }
            return pmList.Distinct().ToList();
        }

        public static string GenerateRandomString(int length, Random random)
        {

            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            if (random != null)
            {
                for (int i = 0; i < length; i++)
                {
                    result.Append(characters[random.Next(characters.Length)]);
                }
            }
            return result.ToString();
        }

        public async Task SwitchDefaultRole(int roleId, int userId, int loggedUserId)
        {
            try
            {
                if (roleId <= 0)
                {
                    throw new NullReferenceException(ErrorMessages.UserRoleIdRequired.ToString(CultureInfo.CurrentCulture));
                }
                var userRoleRepo = _unitOfWork?.GetRepository<UserRoles>();
                //Assign default role //if default role is 0 then assign any other 
                UserRoles defaultRole = null;
                if (roleId > 0)
                {
                    //update prev default role as not default and update default role to DefaultRoleId
                    var roles = await userRoleRepo.GetAll(x => x.UserId == userId && x.IsDefault && !x.IsDeleted).ToListAsyncSafe().ConfigureAwait(false);
                    foreach (var role in roles)
                    {
                        role.IsDefault = false;
                        userRoleRepo.Update(role);
                    }
                    defaultRole = await userRoleRepo.GetAll(x => x.UserId == userId && x.RoleId == roleId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                }
                else
                {
                    //if default role exists then don't update else update any role as default
                    if (!await userRoleRepo.GetAll(x => x.UserId == userId && x.IsDefault && !x.IsDeleted).AnyAsync().ConfigureAwait(false))
                    {
                        defaultRole = await userRoleRepo.GetAll(x => x.UserId == userId && !x.IsDeleted).FirstOrDefaultAsync().ConfigureAwait(false);
                    }
                }
                if (defaultRole != null)
                {
                    defaultRole.IsDefault = true;
                    defaultRole.UpdatedAt = DateTime.UtcNow;
                    defaultRole.UpdatedById = loggedUserId;
                    userRoleRepo.Update(defaultRole);
                }
                await (_unitOfWork?.SaveChangesAsync()).ConfigureAwait(false);

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


        public async Task<PagedOutputModel<List<UserSavedResponseViewModel>>> GetUserByAccessStores(PagedInputModel inputModel, int userId)
        {

            try
            {
                var repository = _unitOfWork?.GetRepository<Users>();
                List<SqlParameter> dbParams = new List<SqlParameter>{
                     new SqlParameter("@UserId", userId),
                     new SqlParameter("@GlobalFilter", inputModel?.GlobalFilter),
                     new SqlParameter("@SortColumn", inputModel?.Sorting),
                     new SqlParameter("@SortDirection", inputModel?.Direction),
                     new SqlParameter("@PageNumber", inputModel?.SkipCount),
                     new SqlParameter("@PageSize", inputModel?.MaxResultCount)
                };

                var dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetUserByAccessStore, dbParams.ToArray()).ConfigureAwait(false);
                List<UserSavedResponseViewModel> responseModel =
                    MappingHelpers.ConvertDataTable<UserSavedResponseViewModel>(dset.Tables[0]);
                var count = 0;
                count = Convert.ToInt32(dset.Tables[1].Rows[0]["TotalRecordCount"]);
                return new PagedOutputModel<List<UserSavedResponseViewModel>>(responseModel, count);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.UserNotFound.ToString(CultureInfo.CurrentCulture));
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
