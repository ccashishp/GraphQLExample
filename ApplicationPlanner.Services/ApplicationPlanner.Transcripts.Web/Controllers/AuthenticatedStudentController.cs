using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Services;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Controllers
{
    [Route("transcripts/[controller]")]
    public class AuthenticatedStudentController : BaseController
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentService _studentService;

        public AuthenticatedStudentController(IStudentRepository studentRepository, IStudentService studentService)
        {
            _studentRepository = studentRepository;
            _studentService = studentService;
        }

        // GET api/transcripts/authenticatedstudent
        [HttpGet]
        public async Task<IActionResult> GetAuthenticatedStudent()
        {
            var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
            var studentGeneralInfo = await _studentRepository.StudentGeneralInfoGetByPortfolioIdAsync(portfolioId);
            if (studentGeneralInfo == null)
                return NotFound();
            var authenticatedStudent = _studentService.AuthenticatedStudentGetByStudentGeneralInfo(studentGeneralInfo);
            return Ok(authenticatedStudent);
        }
    }
}
