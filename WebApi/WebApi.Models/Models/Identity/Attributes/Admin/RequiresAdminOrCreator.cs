using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Enums.Enums.User;
using WebApi.Models.Utilities;

namespace WebApi.Models.Models.Identity.Attributes.Admin
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequiresAdminOrCreator : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.IsInRole(UserRole.Admin.ToLowerString()) && !user.IsInRole(UserRole.Creator.ToLowerString()))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
