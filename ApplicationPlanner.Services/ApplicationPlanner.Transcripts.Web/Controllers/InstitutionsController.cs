using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Services;
using ApplicationPlanner.Transcripts.Web.Models;
using CC.Common.Enum;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Controllers
{
    [Route("transcripts/[controller]")]
    public class InstitutionsController : BaseController
    {
        private readonly IInstitutionRepository _institutionsRepository;
        private readonly ITranscriptProviderService _transcriptProviderService;

        public InstitutionsController(IInstitutionRepository institutionsRepository, ITranscriptProviderService transcriptProviderService)
        {
            _institutionsRepository = institutionsRepository;
            _transcriptProviderService = transcriptProviderService;
        }

        // GET api/transcripts/institutions/innetwork => used in the Student-side - Saved Schools & Search
        [HttpGet("innetwork")]
        public IActionResult GetInNetworkReceivers()
        {
            var transcriptInNetworkInstitutionList = _transcriptProviderService.GetInstitutionReceiverResponseModel(_transcriptProviderService.GetTranscriptInNetworkReceiverList());
            return Ok(transcriptInNetworkInstitutionList);
        }

        // GET api/transcripts/institutions
        // When used in the student-side, portfolioId = current authenticated student's
        // When used in the educator-side, portfolioId = selected student's => We check to make sure the authenticated user is an educator
        [HttpGet]
        public async Task<IActionResult> GetSavedSchools(int portfolioId = 0)
        {
            if(GetClaim<string>(CcClaimType.UserType) == UserType.Student.ToString())
                portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
            var institutions = await _institutionsRepository.SavedSchoolsByPortfolioIdAsync(portfolioId: portfolioId, translationLanguageId: 2);
            var viewModel = institutions.Select(i => new SavedSchool
            {
                Institution = new TranscriptInstitutionModel
                {
                    InunId = i.InunId,
                    Name = i.Name,
                    ImageName = i.ImageName,
                    City = i.City,
                    StateProvCode = i.StateProvCode,
                    StateProvName = i.StateProvName
                },
                IsSavedSchool = true
            });
            return Ok(viewModel.ToList());
        }
    }
}
