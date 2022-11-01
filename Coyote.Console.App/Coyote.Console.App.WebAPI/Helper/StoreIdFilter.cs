using Coyote.Console.App.Services.IServices;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace Coyote.Console.App.WebAPI.Helper
{
    /// <summary>
    /// StoreIdFilterAttribute
    /// </summary>
    public class StoreIdFilterAttribute : ActionFilterAttribute, IActionFilter
    {
        /// <summary>
        /// OnActionExecuted
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context != null)
            {
                var email = context.HttpContext.User.Claims.Where(x => x.Type == CustomClaimTypes.UserEmail).Select(x => x.Value).FirstOrDefault();
                if (email != null)
                {
                    var userService = (IUserServices)context.HttpContext.RequestServices.GetService(typeof(IUserServices));
                    var role = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();
                    var roleId = Convert.ToInt32(context.HttpContext.User.Claims.Where(x => x.Type == CustomClaimTypes.RoleId).Select(x => x.Value).FirstOrDefault());
                    var securityViewModel = userService.GetUserAllowedStoresId(email);
                    securityViewModel.UserId = Convert.ToInt64(context.HttpContext.User.Claims.Where(x => x.Type == CustomClaimTypes.Id).Select(x => x.Value).FirstOrDefault());
                    securityViewModel.RoleId = roleId;
                    securityViewModel.AddUnlockProduct = Convert.ToBoolean(context.HttpContext.User.Claims.Where(x => x.Type == CustomClaimTypes.AddUnlockProduct).Select(x => x.Value).FirstOrDefault());

                    securityViewModel.UserRole = role;

                    if (context.HttpContext.Items["SecurityViewModel"] != null)
                        context.HttpContext.Items.Remove("SecurityViewModel");
                    if (!string.IsNullOrWhiteSpace(role) && (role == DBRoles.Admin || role == DBRoles.SuperAdmin))
                        securityViewModel.IsAdminUser = true;
                    context.HttpContext.Items.Add("SecurityViewModel", securityViewModel);
                }
            }
            base.OnActionExecuting(context);
        }
    }
}