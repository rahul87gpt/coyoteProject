using Coyote.Console.App.EntityFrameworkCore;
using Coyote.Console.App.Models;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.Repository.Repository
{
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        private CoyoteAppDBContext _context = null;
        private readonly IAutoMappingServices _iAutoMapper = null;
        private ILoggerManager _iLoggerManager = null;

        public UserRepository(CoyoteAppDBContext coyoteConsoleAppDBContext, IAutoMappingServices iAutoMapper, ILoggerManager iLoggerManager) : base(coyoteConsoleAppDBContext, iLoggerManager)
        {
            _context = coyoteConsoleAppDBContext;
            _iAutoMapper = iAutoMapper;
            _iLoggerManager = iLoggerManager;
        }

        /// <summary>
        /// Check UserRole by email addrees exists or not in the system
        /// </summary>
        /// <param name="emailAddress">Email Address</param>
        /// <returns>bool - true or false </returns>
        public bool UserExistsCheckByUserEmail(string emailAddress)
        {
            int isUserExists = 0;
            isUserExists = _context.Users.Where(x => x.Email == emailAddress).Count();
            return (isUserExists == 0 ? false : true);
        }
        /// <summary>
        /// returns all users with Is_Deleted is false
        /// </summary>
        /// <returns>List of User View model</returns>
        public async Task<PagedOutputModel<List<UserSavedResponseViewModel>>> GetAllActiveUsers(PagedInputModel filter = null)
        {
            var list = _context.Users.Where(user => user.IsDeleted == false);

            int count = 0;
            if (filter != null)
            {
                if (!string.IsNullOrEmpty((filter?.GlobalFilter)))
                    list = list.Where(x => x.Email.ToLower(CultureInfo.CurrentCulture).Contains(filter.GlobalFilter.ToLower(CultureInfo.CurrentCulture)) || x.FirstName.ToLower(CultureInfo.CurrentCulture).Contains(filter.GlobalFilter.ToLower(CultureInfo.CurrentCulture)) || x.LastName.ToLower(CultureInfo.CurrentCulture).Contains(filter.GlobalFilter.ToLower(CultureInfo.CurrentCulture)));

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
                            list = list.OrderBy(x => x.LastName);
                        else
                            list = list.OrderByDescending(x => x.LastName);
                        break;
                }
            }
            count = list.Count();
            var listViewModel = (await list.ToListAsync().ConfigureAwait(false)).Select(_iAutoMapper.Mapping<Users, UserSavedResponseViewModel>).ToList();
            return new PagedOutputModel<List<UserSavedResponseViewModel>>(listViewModel, count);

        }

        /// <summary>
        /// return true or false when user deletion is success or not
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>bool- true or false</returns>
        public bool DeleteUser(int Id)
        {
            int UserRoleDeleted = 0;
            var users = _context.Users.Where(x => x.Id == Id).FirstOrDefault();
            users.IsDeleted = true;
            UserRoleDeleted = _context.SaveChanges();

            return (UserRoleDeleted == 0 ? false : true);
        }

        /// <summary>
        /// Get User View Model by User_Id
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <returns>Object of User_Role View Model</returns>
        public async Task<UserSavedResponseViewModel> GetUserById(int userId)
        {
            var users = await _context.Users.Where(user => user.Id == userId && user.IsDeleted == false).SingleOrDefaultAsync().ConfigureAwait(false);

            UserSavedResponseViewModel userViewModel;

            userViewModel = _iAutoMapper.Mapping<Users, UserSavedResponseViewModel>(users);
            return userViewModel;
        }

        public string GetUserRole(string email)
        {

            var roles = (from u in _context.Users.Where(x => !x.IsDeleted)
                         join ur in _context.UserRoles.Where(x => !x.IsDeleted)
                         on u.Id equals ur.UserId
                         where u.Email == email
                         join r in _context.Roles.Where(x => !x.IsDeleted)
                    on ur.RoleId equals r.Id
                         select new
                         {
                             r.Name,
                         }).Distinct().ToList();

            string role = null;
            foreach (var x in roles)
            {
                role += x.Name + ",";
            }
            if (!string.IsNullOrEmpty(role))
                role = role.Substring(0, role.LastIndexOf(",", StringComparison.OrdinalIgnoreCase));
            return role;
        }

        public List<RoleResponseViewModel> GetUserRoles(int userId)
        {
            return (from ur in _context.UserRoles.Where(x => !x.IsDeleted && x.UserId == userId)
                    join r in _context.Roles.Where(x => !x.IsDeleted) on ur.RoleId equals r.Id
                    select new RoleResponseViewModel
                    {
                        Id = r.Id,
                        Code = r.Code,
                        Name = r.Name,
                        IsDefualt = ur.IsDefault,
                        Permissions = r.PermissionSet,
                        //PermissionsDept=r.PermissionDeptSet
                        
                    }).ToList();

        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
