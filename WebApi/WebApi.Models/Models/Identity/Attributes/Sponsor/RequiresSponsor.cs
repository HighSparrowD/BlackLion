using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Enums.Enums.Authentication;
using WebApi.Models.Utilities;

namespace WebApi.Models.Models.Identity.Attributes.Sponsor
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequiresSponsor : BaseRoleAttribute
    {
        public override void OnAuthorization(AuthorizationFilterContext context)
        {
            base.OnAuthorization(context);
            
            var user = context.HttpContext.User;
            if (!user.IsInRole(Role.Sponsor.ToLowerString()))
            {
                Forbid(context);
            }
        }
    }
}
