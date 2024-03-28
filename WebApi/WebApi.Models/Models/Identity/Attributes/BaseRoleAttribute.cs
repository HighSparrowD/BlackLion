using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Models.Models.Identity.Attributes;

public abstract class BaseRoleAttribute : Attribute, IAuthorizationFilter
{
    public virtual void OnAuthorization(AuthorizationFilterContext context)
    {
        var tokenId = context.HttpContext.User.Claims.SingleOrDefault(c => c.Type == IdentityData.JwtIdClaimName)?.Value;
        var userId = context.HttpContext.User.Claims.SingleOrDefault(c => c.Type == IdentityData.UserIdClaimName)?.Value;

        if (tokenId == null)
        {
            Forbid(context);
            return;
        }

        if(!JwtStorage.IsTokenValid(tokenId, userId))
        {
            Forbid(context);
            return;
        }
    }

    public virtual void Forbid(AuthorizationFilterContext context)
    {
        context.Result = new ForbidResult();
    }
}
