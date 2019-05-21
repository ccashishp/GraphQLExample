using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ApplicationPlanner.Transcripts.Web.Helpers
{
    public class JwtHelper
    {
        public static CcValidatedToken GetToken(HttpRequest request)
        {
            var hasToken = request.HttpContext.Items.TryGetValue("CcValidatedToken", out object tokenObj);
            return hasToken ? (CcValidatedToken)tokenObj : null;
        }
        public static string GetUserType(CcValidatedToken validatedToken) => validatedToken.CcClaims.First(a => a.ClaimType == CcClaimType.UserType).Value;
        public static int GetEducatorId(CcValidatedToken validatedToken) => int.Parse(validatedToken.CcClaims.First(a => a.ClaimType == CcClaimType.EducatorId).Value);
        public static int GetPortfolioId(CcValidatedToken validatedToken) => int.Parse(validatedToken.CcClaims.First(a => a.ClaimType == CcClaimType.StudentPortfolioId).Value);
        public static bool IsEducator(CcValidatedToken validatedToken) => validatedToken.CcClaims.First(a => a.ClaimType == CcClaimType.UserType).Value == "Educator";
        public static bool IsStudent(CcValidatedToken validatedToken) => validatedToken.CcClaims.First(a => a.ClaimType == CcClaimType.UserType).Value == "Student";
    }
}
