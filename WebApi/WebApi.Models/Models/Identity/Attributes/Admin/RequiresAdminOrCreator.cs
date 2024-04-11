using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Enums.Enums.Authentication;
using WebApi.Models.Utilities;

namespace WebApi.Models.Models.Identity.Attributes.Admin
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequiresAdminOrCreator : BaseRoleAttribute
    {
        public override void OnAuthorization(AuthorizationFilterContext context)
        {
            base.OnAuthorization(context);
            
            var user = context.HttpContext.User;
            if (!user.IsInRole(Role.Admin.ToLowerString()) && !user.IsInRole(Role.Creator.ToLowerString()))
            {
                Forbid(context);
            }
        }
    }
}
