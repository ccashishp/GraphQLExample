using CC3.AuthServices.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Filters
{
    public class ApplicationPlannerAuthorizationFilter : ActionFilterAttribute
    {
        /*TODO: we may need to refactor this filter.
         *      1. on token validation fail, we send 403 to F/E but should be 401
         *      2. because this filter is attached to CC3.API or CAMS3.BE it has 
         *         its authorizationfilter, that runs ahead of all other, and it does
         *         token validation here - so i think we could reuse the User from context.
         */      

        private readonly ITokenValidator _tokenValidator;

        public ApplicationPlannerAuthorizationFilter(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            var isAuth = await CheckAuthorization(actionContext);
            if (!isAuth)
            {
                actionContext.Result = new ContentResult
                {
                    Content = "Forbidden",
                    ContentType = "application/json; charset=utf-8",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            await base.OnActionExecutionAsync(actionContext, next);
        }


        private async Task<bool> CheckAuthorization(ActionContext actionContext)
        {
            try
            {
                var authHeader = actionContext.HttpContext.Request.Headers["Authorization"].ToString();
                var token = await _tokenValidator.Validate(authHeader);
                var claims = token.CcClaims.Select(c => new Claim(c.ClaimType.ToString(), c.Value, c.ValueType.ToString(), c.Issuer));
                var identity = new ClaimsIdentity(claims);
                actionContext.HttpContext.User = new ClaimsPrincipal(identity);
                actionContext.HttpContext.Items.Add("CcValidatedToken", token);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
