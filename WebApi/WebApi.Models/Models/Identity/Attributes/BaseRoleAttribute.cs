using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Models.Models.Identity.Attributes;

public abstract class BaseRoleAttribute : Attribute, IAuthorizationFilter
{
    public virtual void OnAuthorization(AuthorizationFilterContext context)
    {
        var tokenId = context.HttpContext.User.Claims.Where(c => c.Type == IdentityData.JwtIdClaimName)
            .Select(c => c.Value)
            .SingleOrDefault();

        if (tokenId == null)
        {
            Forbid(context);
            return;
        }

        if(!JwtStorage.IsTokenValid(tokenId))
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
