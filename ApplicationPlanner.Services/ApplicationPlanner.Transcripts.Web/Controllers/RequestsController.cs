using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Controllers
{
    [Route("transcripts/[controller]")]
    public class RequestsController : BaseController
    {
        private readonly ITranscriptRequestRepository _transcriptRequestRepository;
        private readonly ITranscriptRequestService _transcriptRequestService;
        private readonly IInstitutionRepository _institutionRepository;
        private readonly ILinqWrapperService _linqWrapperService;
        private readonly ITranscriptRepository _transcriptRepository;
        private readonly ITranscriptService _transcriptService;
        private readonly ITranscriptProviderService _transcriptProviderService;

        public RequestsController(
            ITranscriptRequestRepository transcriptRequestRepository,
            ITranscriptRequestService transcriptRequestService,
            IInstitutionRepository institutionRepository,
            ILinqWrapperService linqWrapperService,
            ITranscriptRepository transcriptRepository,
            ITranscriptService transcriptService,
            ITranscriptProviderService transcriptProviderService
            )
        {
            _transcriptRequestRepository = transcriptRequestRepository;
            _transcriptRequestService = transcriptRequestService;
            _institutionRepository = institutionRepository;
            _linqWrapperService = linqWrapperService;
            _transcriptRepository = transcriptRepository;
            _transcriptService = transcriptService;
            _transcriptProviderService = transcriptProviderService;
        }

        // POST transcripts/requests => Used in the student-side OR educator-side, send on behalf of student
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RequestTranscriptRequestModel model)
        {
            // Validation
            if (model == null ||
                String.IsNullOrEmpty(model.InunId) ||
                String.IsNullOrEmpty(model.ReceivingInstitutionName) ||
                String.IsNullOrEmpty(model.ReceivingInstitutionCity) ||
                String.IsNullOrEmpty(model.ReceivingInstitutionStateCode))
                return BadRequest();

            var userType = GetClaim<string>(CcClaimType.UserType);
            var portfolioId = userType == UserType.Student.ToString() 
                ? GetClaim<int>(CcClaimType.StudentPortfolioId)
                : model.PortfolioId ?? 0;

            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);

            try
            {
                var id = await _transcriptRequestRepository.CreateTranscriptRequestAsync(
                    portfolioId: portfolioId,
                    inunId: model.InunId,
                    transcriptRequestTypeId: (int)model.TranscriptRequestTypeId,
                    receivingInstitutionCode: model.ReceivingInstitutionCode,
                    receivingInstitutionName: model.ReceivingInstitutionName,
                    receivingInstitutionCity: model.ReceivingInstitutionCity,
                    receivingInstitutionStateCode: model.ReceivingInstitutionStateCode,
                    createdById: userAccountId);
                return Ok(id);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // GET transcripts/requests
        // When used in the student-side, portfolioId = current authenticated student's
        // When used in the educator-side, portfolioId = selected student's => We check to make sure the authenticated user is an educator
        [HttpGet]
        public async Task<IActionResult> Get(int portfolioId = 0)
        {
            if (GetClaim<string>(CcClaimType.UserType) == UserType.Student.ToString())
                portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);
            var transcriptRequests = await _transcriptRequestService.GetTranscriptRequestByPortfolioIdAsync(portfolioId, userAccountId);
            return Ok(transcriptRequests);
        }

        // GET api/transcripts/requests/timeline
        [HttpGet("timeline")]
        public async Task<IActionResult> GetTimeline()
        {
            var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);
            var transcriptRequestsTimeline = await _transcriptRequestService.GetTranscriptRequestsTimelineByPortfolioIdAsync(portfolioId: portfolioId, userAccountId: userAccountId, translationLanguageId: 2);
            return Ok(transcriptRequestsTimeline);
        }

        // GET api/transcripts/requests/history
        [HttpGet("timeline/history")]
        public async Task<IActionResult> GetTimelineV2()
        {
            var portfolioId = GetClaim<int>(CcClaimType.StudentPortfolioId);
            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);
            var transcriptRequestsTimeline = await _transcriptRequestService.GetAllHistoryByPortfolioIdAsync(portfolioId: portfolioId, userAccountId: userAccountId, translationLanguageId: 2);
            return Ok(transcriptRequestsTimeline);
        }

        // GET api/transcripts/requests/school?query=123&filterByProperty=transcriptRequestType&filterByValue=1&orderBy=requestedDate&sortOrder=ASC/DESC&skip=0&take=25
        [HttpGet("school")]
        public async Task<IActionResult> GetTranscriptRequestForCurrentSchool(string query = "", string filterByProperty = "", string filterByValue = "", string orderBy = "requestedDate", SortOrder sortOrder = SortOrder.ASC, int skip = 0, int take = int.MaxValue)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            // @TODO: The max number of records per school is 1k so It seems reasonable to assume the 1k records will fit in memory
            // and do all the manipulation (query, filter, sorting, skip, take) with LINQ (this will be cheaper and faster) instead of SQL.
            // Cache the list in Redis and make sure we update the cache when a new transcript request is added or deleted
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);

            var transcriptRequestStudentCompleteList = await _transcriptRequestRepository.GetTranscriptRequestBySchoolIdAsync(currentSchool.Id);

            var latestRequestsSearchPredicate = _transcriptRequestService.GetLatestRequestsSearchPredicate(query);
            var transcriptRequestStudentLinqedList = _linqWrapperService.GetLinqedList(transcriptRequestStudentCompleteList, latestRequestsSearchPredicate, filterByProperty, filterByValue, orderBy, sortOrder, skip, take);

            var result = new ItemsCountModel<TranscriptRequestStudentResponseModel>
            {
                Items = await _transcriptRequestService.GetTranscriptRequestStudentResponseModelAsync(transcriptRequestStudentLinqedList.Items, currentSchool.Id),
                Count = transcriptRequestStudentLinqedList.Count
            };
            return Ok(result);
        }

        // DELETE api/transcripts/requests/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var result = await _transcriptRequestRepository.DeleteByIdAsync(id, deletedById: educatorId);
            return Ok(result);
        }

        // PUT api/transcripts/requests/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUndo(int id)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            var result = await _transcriptRequestRepository.DeleteUndoByIdAsync(id);
            return Ok(result);
        }

        // GET api/transcripts/requests/progress/school => used in the Educator-side - Sent Transcripts Tab
        [HttpGet("progress/school")]
        public async Task<IActionResult> GetTranscriptRequestProgressForCurrentSchool(string query = "", string filterByProperty = "", string filterByValue = "", string orderBy = "requestedDate", SortOrder sortOrder = SortOrder.ASC, int skip = 0, int take = int.MaxValue)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            // @TODO: The max number of records per school is 1k so It seems reasonable to assume the 1k records will fit in memory
            // and do all the manipulation (query, filter, sorting, skip, take) with LINQ (this will be cheaper and faster) instead of SQL.
            // Cache the list in Redis and make sure we update the cache when a new transcript is sent
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);

            var transcriptRequestProgressCompleteList = await _transcriptRequestRepository.GetTranscriptRequestProgressBySchoolIdAsync(currentSchool.Id);

            var transcriptRequestProgressSearchPredicate = _transcriptRequestService.GetTranscriptRequestProgressSearchPredicate(query);
            var transcriptRequestProgressLinqedList = _linqWrapperService.GetLinqedList(transcriptRequestProgressCompleteList, transcriptRequestProgressSearchPredicate, filterByProperty, filterByValue, orderBy, sortOrder, skip, take);

            var result = new ItemsCountModel<TranscriptRequestProgressResponseModel>
            {
                Items = await _transcriptRequestService.GetTranscriptRequestProgressResponseModelAsync(transcriptRequestProgressLinqedList.Items, currentSchool.Id),
                Count = transcriptRequestProgressLinqedList.Count
            };
            return Ok(result);
        }

        // GET api/transcripts/requests/progress/students/{id} => used in the Educator-side - Sent Transcripts Tab => Track Progress. id = portfolioId
        [HttpGet("progress/students/{id}")]
        public async Task<IActionResult> GetTranscriptRequestProgressForStudent(int id)
        {
            var studentTranscript = await _transcriptRepository.GetStudentTranscriptByPortfolioIdAsync(id);
            var result = new StudentRequestProgressResponseModel
            {
                Student = await _transcriptService.GetStudentTranscriptResponseModelAsync(studentTranscript, id),
                Progress = await _transcriptRequestService.GetTranscriptRequestsTimelineByPortfolioIdAsync(portfolioId: id, userAccountId: studentTranscript.UserAccountId, translationLanguageId: 2, includeRequested: false)
            };
            return Ok(result);
        }

        // POST api/transcripts/requests/{id}/status/submitted => used in the Educator-side - Latest Requests Tab (Send to institution) OR Send on behalf of student. id = TranscriptRequestId
        [HttpPost("{id}/status/submitted")]
        public async Task<IActionResult> SendTranscriptRequest(int id, [FromBody]TranscriptSubmitRequestModel submitData)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);

            var sendTranscriptInfos = await _transcriptRequestRepository.GetSendTranscriptByTranscriptRequestIdAsync(id);
            if (!await _transcriptRequestService.IsSendTranscriptRequestInputValidAsync(sendTranscriptInfos, submitData))
                return BadRequest();

            switch (submitData.TranscriptRequestType)
            {
                case TranscriptRequestType.Mail:
                    var dateSentByMailUtc = await _transcriptRequestService.GetDateSentByMailUtcAsync(sendTranscriptInfos.TranscriptRequestId, submitData.DateSentByMail ?? DateTime.Now, submitData.SkipValidationForDateSentByMail);
                    await _transcriptRequestService.SubmitOutOfNetworkAsync(id, TranscriptRequestType.Mail, dateSentByMailUtc: dateSentByMailUtc);
                    await _transcriptRequestRepository.AppendHistoryAsync(id, TranscriptRequestStatus.Submitted, userAccountId);
                    break;
                case TranscriptRequestType.Email:
                    await _transcriptProviderService.SendTranscriptRequestAsync(sendTranscriptInfos.TranscriptRequestId, sendTranscriptInfos.StudentId, sendTranscriptInfos.TranscriptId, sendTranscriptInfos.SchoolId, sendTranscriptInfos.ReceivingInstitutionCode, sendTranscriptInfos.ReceivingInstitutionName, userAccountId, submitData?.ReceivingInstitutionEmail);
                    await _transcriptRequestService.SubmitOutOfNetworkAsync(id, TranscriptRequestType.Email, receivingInstitutionEmail : submitData.ReceivingInstitutionEmail);
                    break;
                default:
                    await _transcriptProviderService.SendTranscriptRequestAsync(sendTranscriptInfos.TranscriptRequestId, sendTranscriptInfos.StudentId, sendTranscriptInfos.TranscriptId, sendTranscriptInfos.SchoolId, sendTranscriptInfos.ReceivingInstitutionCode, sendTranscriptInfos.ReceivingInstitutionName, userAccountId);
                    break;
            }
            return Ok();
        }

        // POST api/transcripts/requests/status/submitted => used in the Educator-side - Latest Requests Tab => Bulk send to institution
        [HttpPost("status/submitted")]
        public async Task<IActionResult> BulkSendTranscriptRequest([FromBody]List<int> idList)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            var userAccountId = GetClaim<int>(CcClaimType.UserAccountId);
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);

            var sendTranscriptInfos = await _transcriptRequestRepository.GetSendTranscriptByTranscriptRequestIdListAsync(idList);
            if (sendTranscriptInfos == null || sendTranscriptInfos.Count() == 0)
                return BadRequest();

            await _transcriptProviderService.BulkSendTranscriptRequestAsync(sendTranscriptInfos, currentSchool.Id, userAccountId);
            return Ok();
        }
    }
}
