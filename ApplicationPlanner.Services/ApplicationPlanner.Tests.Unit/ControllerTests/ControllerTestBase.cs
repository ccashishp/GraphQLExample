using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ApplicationPlanner.Tests.Unit.ControllerTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ControllerTestBase
    {
        protected T Result<T>(IActionResult actionResult)
        {
            var value = (actionResult as ObjectResult).Value;
            return (T)value;
        }
    }

    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(params Claim[] claims) : base(new TestIdentity(claims))
        {
        }
    }

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims)
        {
        }
    }
}
