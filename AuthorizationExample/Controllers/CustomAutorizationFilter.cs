using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationExample.Controllers
{
    public class CustomAuthorizationAttribute : TypeFilterAttribute
    {
        public CustomAuthorizationAttribute(string claimType, string claimValue) 
            : base(typeof(CustomAuthorizationFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public CustomAuthorizationFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(
                c => c.Type == _claim.Type && c.Value.StartsWith(_claim.Value));
            if (!hasClaim)
            {
                context.Result = new ViewResult() { ViewName = "AccessDenied" };
            }
        }
    }
}
