using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Filters;
using CC.Common.Enum;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Controllers
{
    [ServiceFilter(typeof(QAAuthorizationFilter))]
    [Route("transcripts/qa")]
    public class QAController : BaseController
    {
        private readonly IQARepository _qaRepository;

        public QAController(IQARepository qaRepository)
        {
            _qaRepository = qaRepository;
        }

        /// <summary>
        /// Reset HasSeenCartTooltipForTranscriptsInSavedSchoolsMode flag for the logged-in user (student or educator)
        /// </summary>
        /// <returns></returns>
        // DELETE api/transcripts/qa/seen-cart-tt-saved-schools
        [Route("seen-cart-tt-saved-schools")]
        [HttpDelete]
        public async Task<IActionResult> ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsMode()
        {
            var userType = GetClaim<string>(CcClaimType.UserType);
            if (userType == UserType.Student.ToString())
            {
                var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
                var result = await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByPortfolioIdAsync(portfolioId);
                return Ok(result);
            }
            if (userType == UserType.Educator.ToString())
            {
                var educatorId = GetClaim<int>(CcClaimType.EducatorId);
                var result = await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSavedSchoolsModeByEducatorIdAsync(educatorId);
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// Reset HasSeenCartTooltipForTranscriptsInSearchModeMode flag for the logged-in user (student or educator)
        /// </summary>
        /// <returns></returns>
        // DELETE api/transcripts/qa/seen-cart-tt-search
        [Route("seen-cart-tt-search")]
        [HttpDelete]
        public async Task<IActionResult> ResetHasSeenCartTooltipForTranscriptsInSearchModeMode()
        {
            var userType = GetClaim<string>(CcClaimType.UserType);
            if (userType == UserType.Student.ToString())
            {
                var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
                var result = await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSearchModeByPortfolioIdAsync(portfolioId);
                return Ok(result);
            }
            if (userType == UserType.Educator.ToString())
            {
                var educatorId = GetClaim<int>(CcClaimType.EducatorId);
                var result = await _qaRepository.ResetHasSeenCartTooltipForTranscriptsInSearchModeByEducatorIdAsync(educatorId);
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// Delete all the transcript requests for the logged-in student
        /// </summary>
        /// <returns></returns>
        // DELETE api/transcripts/qa/request
        [Route("request")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTranscriptRequests()
        {
            var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
            await _qaRepository.DeleteTranscriptRequestAsync(portfolioId);
            return Ok();
        }

        /// <summary>
        /// Update the status date of transcript request
        /// </summary>
        /// <returns></returns>
        // UPDATE api/transcripts/qa/request/{id}/{transcriptStatusId}/{minusDays}
        [Route("request/{id}/{transcriptStatusId}/{minusDays}")]
        [HttpPut]
        public async Task<IActionResult> UpdateStatusDate(int id, TranscriptRequestStatus transcriptStatusId, int minusDays)
        {
            var resetTime = DateTime.UtcNow.AddDays(-minusDays);
            TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);           
            resetTime = resetTime.AddHours(-resetTime.Hour - utcOffset.Hours);
            resetTime = resetTime.AddMinutes(-resetTime.Minute);
            resetTime = resetTime.AddSeconds(-resetTime.Second);
            await _qaRepository.UpdateStatusDateUtcAsync(id, transcriptStatusId, resetTime);
            return Ok();
        }

        /// <summary>
        /// Simulate adding a transcript in our db after the Credentails has parsed the pdf and call our webservice to add transcript
        /// </summary>
        /// <returns></returns>
        // POST api/transcripts/qa/import
        [Route("import")]
        [HttpPost]
        public async Task<IActionResult> ImportTranscript([FromBody]ImportTranscriptModel importTranscript)
        {
            var studentInSchool = true;

            // make sure the student is in the schoolId
            if (importTranscript.PortfolioId != null)
            {
                var studentModel = await _qaRepository.GetStudentByPortfolioIdAsync(importTranscript.PortfolioId ?? 0);
                studentInSchool = studentModel != null && studentModel.SchoolId == importTranscript.SchoolId;
            }

            if(studentInSchool)
                await _qaRepository.ImportTranscriptAsync(importTranscript.PortfolioId ?? 0, importTranscript.SchoolId, importTranscript.StudentNumber, importTranscript.StudentName, importTranscript.DateOfBirth, importTranscript.EmailAddress);

            return Ok();
        }

        /// <summary>
        /// Simulate updating a transcript in our db after the Credentials has parsed the pdf and call our webservice to update transcript
        /// </summary>
        /// <returns></returns>
        // PUT api/transcripts/qa/import/{id}
        [Route("import/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateTranscript(int id)
        {
            await _qaRepository.UpdateTranscriptAsync(id);

            return Ok();
        }

        /// <summary>
        /// Simulate receiving a transcript in our db after the Credentails has submitted it and the receiving institution has acknowledged receiving it and call our webservice to update
        /// </summary>
        /// <returns></returns>
        // POST api/transcripts/qa/requests/{id}/status/received
        [Route("requests/{id}/status/received")]
        [HttpPost]
        public async Task<IActionResult> SetReceivedTranscriptAsync(int id)
        {
            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);

            await _qaRepository.ReceivedTranscriptAsync(id, userAccountId);
            return Ok();
        }

        /// <summary>
        /// Simulate expiring sent transcript in our db after the Credentails has sent it and the receiving institution hasn't open it in time and call our webservice to update
        /// </summary>
        /// <returns></returns>
        // POST api/transcripts/qa/requests/{id}/status/expired
        [Route("requests/{id}/status/expired")]
        [HttpPost]
        public async Task<IActionResult> SetExpiredTranscriptAsync(int id)
        {
            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);

            await _qaRepository.ExpiredTranscriptAsync(id, userAccountId);
            return Ok();
        }
    }
}
