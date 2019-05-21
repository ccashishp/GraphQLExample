using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace ApplicationPlanner.Transcripts.Web.Filters
{
    public class QAAuthorizationFilter : ActionFilterAttribute
    {
        private readonly TokenConfig _tokenConfig;
        private IList<string> _allowedEnvironments { get; } = new List<string> { "dev", "test", "uat" };

        public QAAuthorizationFilter(TokenConfig tokenConfig)
        {
            _tokenConfig = tokenConfig;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!CheckEnvironmentAccess())
            {
                context.Result = new ContentResult
                {
                    Content = "Forbidden",
                    ContentType = "application/json; charset=utf-8",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            base.OnActionExecuting(context);
        }

        private bool CheckEnvironmentAccess()
        {
            var environment = _tokenConfig.CcEnvironment;
            if (_allowedEnvironments.Contains(environment.ToLower()))
                return true;

            return false;
        }
    }
}
