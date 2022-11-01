using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Coyote.Console.App.WebAPI.Helper
{
    /// <summary>
    /// PermissionAuthorizeAttribute
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Permission
        /// </summary>
        public PermissionEnum Permission { get; set; } //Permission string to get from controller
        /// <summary>
        /// OnAuthorization
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context != null)
            {
                var expTime = Convert.ToDateTime(context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Expiration).Select(x => x.Value).FirstOrDefault());
                if (expTime <= DateTime.UtcNow)
                {
                    context.Result = new CustomUnauthorizedResult(ErrorMessages.TokenExpired.ToString(CultureInfo.CurrentCulture) + expTime);
                }

                var email = context.HttpContext.User.Claims.Where(x => x.Type == CustomClaimTypes.UserEmail).Select(x => x.Value).FirstOrDefault();
                var role = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();

                if (role == DBRoles.Admin || role == DBRoles.SuperAdmin)  // Admin and Super Admin will have all permision i.e. that they are authorized.
                    return;


                var userService = (IUserServices)context.HttpContext.RequestServices.GetService(typeof(IUserServices));
                List<string> permission = userService.GetUserPermission(email);

                var controllerDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (controllerDescriptor != null)
                {
                    var controllerName = controllerDescriptor.ControllerName.ToLower(CultureInfo.CurrentCulture);
                    var actionName = controllerDescriptor.ActionName.ToLower(CultureInfo.CurrentCulture);
                    var methodInvocationType = context.HttpContext.Request.Method.ToLower(CultureInfo.CurrentCulture);
                    if (permission.Any(p => p.ToLower(CultureInfo.CurrentCulture) == "*" || p.ToLower(CultureInfo.CurrentCulture) == controllerName + ".*" || p.ToLower(CultureInfo.CurrentCulture) == controllerName + "." + methodInvocationType))
                    {
                        return;
                    }
                }
            }
            //context.Result = new UnauthorizedResult();
            context.Result = new CustomPermissionDeniedResult(ErrorMessages.UnAuthorizeAccess.ToString(CultureInfo.CurrentCulture));
            return;
        }
    }

    /// <summary>
    /// CustomUnauthorizedResult
    /// </summary>
    public class CustomUnauthorizedResult : JsonResult
    {
        /// <summary>
        /// CustomUnauthorizedResult
        /// </summary>
        /// <param name="message"></param>
        public CustomUnauthorizedResult(string message)
            : base(APIReponseBuilder.HandleResponse(message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

    /// <summary>
    /// CustomPermissionDeniedResult
    /// </summary>
    public class CustomPermissionDeniedResult : JsonResult
    {
        /// <summary>
        /// CustomPermissionDeniedResult
        /// </summary>
        /// <param name="message"></param>
        public CustomPermissionDeniedResult(string message)
            : base(APIReponseBuilder.HandleResponse(message))
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }

}

