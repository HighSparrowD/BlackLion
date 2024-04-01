using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Enums.Enums.Authentication;
using WebApi.Models.Utilities;

namespace WebApi.Models.Models.Identity.Attributes.Machine;

public class RequiresMachine : BaseRoleAttribute
{
    public override void OnAuthorization(AuthorizationFilterContext context)
    {
        base.OnAuthorization(context);

        var user = context.HttpContext.User;
        if (!user.IsInRole(Role.Machine.ToLowerString()))
        {
            Forbid(context);
        }
    }
}
