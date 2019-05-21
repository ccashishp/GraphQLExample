using ApplicationPlanner.Transcripts.Web.Filters;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ApplicationPlanner.Transcripts.Web.Controllers
{
    [ServiceFilter(typeof(ApplicationPlannerAuthorizationFilter))]
    public class BaseController : Controller
    {
        protected T GetClaim<T>(CcClaimType type)
        {
            var claim = User.Claims.Single(c => c.Type == type.ToString());
            return (T)Convert.ChangeType(claim.Value, typeof(T));
        }
    }
}