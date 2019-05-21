using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using CC3.AuthServices.Token.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Controllers
{
    [Route("transcripts")]
    public class TranscriptsController : BaseController
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly ITranscriptProviderService _transcriptProviderService;
        private readonly ITranscriptRepository _transcriptRepository;
        private readonly ITranscriptService _transcriptService;
        private readonly ILinqWrapperService _linqWrapperService;

        public TranscriptsController(IInstitutionRepository institutionRepository, ITranscriptProviderService transcriptProviderService, ITranscriptRepository transcriptRepository, ITranscriptService transcriptService, ILinqWrapperService linqWrapperService)
        {
            _institutionRepository = institutionRepository;
            _transcriptProviderService = transcriptProviderService;
            _transcriptRepository = transcriptRepository;
            _transcriptService = transcriptService;
            _linqWrapperService = linqWrapperService;
        }

        // POST api/transcripts
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);
            try
            {
                //  Validate the type of the file
                var extension = Path.GetExtension(file.FileName).Substring(1);
                if (Enum.IsDefined(typeof(CredentialsAcceptedDocumentTypeEnum), extension))
                {
                    await _transcriptProviderService.ImportTranscriptAsync(currentSchool.Id, educatorId, extension, file.OpenReadStream());
                    return Ok();
                }
                return BadRequest(); //  Not supported file type
            }
            catch (Exception)
            {
                return BadRequest(); // Not supported file format, Credentails error...
            }
        }

        // GET api/transcripts/imported/school
        [HttpGet("imported/school")]
        public async Task<IActionResult> GetTranscriptImportedForCurrentSchool(string query = "", string filterByProperty = "", string filterByValue = "", string orderBy = "importedDate", SortOrder sortOrder = SortOrder.ASC, int skip = 0, int take = int.MaxValue)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            // @TODO: The max number of records per school is 1k so It seems reasonable to assume the 1k records will fit in memory
            // and do all the manipulation (query, filter, sorting, skip, take) with LINQ (this will be cheaper and faster) instead of SQL.
            // Cache the list in Redis and make sure we update the cache when a new transcript is imported or deleted
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);

            var transcriptImportedCompleteList = await _transcriptRepository.GetTranscriptImportedBySchoolIdAsync(currentSchool.Id);

            var transcriptImportedSearchPredicate = _transcriptService.GetTranscriptImportedSearchPredicate(query);
            var transcriptImportedLinqedList = _linqWrapperService.GetLinqedList(transcriptImportedCompleteList, transcriptImportedSearchPredicate, filterByProperty, filterByValue, orderBy, sortOrder, skip, take);

            var result = new ItemsCountModel<TranscriptImportedResponseModel>
            {
                Items = await _transcriptService.GetTranscriptImportedResponseModelAsync(transcriptImportedLinqedList.Items, currentSchool.Id),
                Count = transcriptImportedLinqedList.Count
            };
            return Ok(result);
        }

        // GET api/transcripts/unmatched/school
        [HttpGet("unmatched/school")]
        public async Task<IActionResult> GetTranscriptUnmatchedForCurrentSchool(string query = "", string filterByProperty = "", string filterByValue = "", string orderBy = "receivedDateUtc", SortOrder sortOrder = SortOrder.ASC, int skip = 0, int take = int.MaxValue)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            // @TODO: The max number of records per school is 1k so It seems reasonable to assume the 1k records will fit in memory
            // and do all the manipulation (query, filter, sorting, skip, take) with LINQ (this will be cheaper and faster) instead of SQL.
            // Cache the list in Redis and make sure we update the cache when a new transcript is imported or deleted
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);

            var transcriptUnmatchedCompleteList = await _transcriptRepository.GetTranscriptUnmatchedBySchoolIdAsync(currentSchool.Id);

            var transcriptUnmatchedSearchPredicate = _transcriptService.GetTranscriptUnmatchedSearchPredicate(query);
            var transcriptUnmatchedLinqedList = _linqWrapperService.GetLinqedList(transcriptUnmatchedCompleteList, transcriptUnmatchedSearchPredicate, filterByProperty, filterByValue, orderBy, sortOrder, skip, take);
            var result = new ItemsCountModel<TranscriptBaseModel>
            {
                Items = await _transcriptService.GetTranscriptUnmatchedResponseModelAsync(transcriptUnmatchedLinqedList.Items, currentSchool.Id),
                Count = transcriptUnmatchedLinqedList.Count
            };
            return Ok(result);
        }

        // DELETE transcripts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            await _transcriptRepository.DeleteByIdAsync(id);
            return Ok();
        }

        // PUT transcripts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUndo(int id)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            await _transcriptRepository.DeleteUndoByIdAsync(id);
            return Ok();
        }

        // GET api/transcripts/students
        [HttpGet("students")]
        public async Task<IActionResult> GetStudentTranscriptForCurrentSchool(string query = "", string filterByProperty = "", string filterByValue = "", string orderBy = "", SortOrder sortOrder = SortOrder.ASC, int skip = 0, int take = int.MaxValue)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            // @TODO: The max number of records per school is 1k so It seems reasonable to assume the 1k records will fit in memory
            // and do all the manipulation (query, filter, sorting, skip, take) with LINQ (this will be cheaper and faster) instead of SQL.
            // Cache the list in Redis and make sure we update the cache when a new transcript is imported or deleted
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);

            var studentTranscriptCompleteList = await _transcriptRepository.GetStudentTranscriptBySchoolIdAsync(currentSchool.Id);

            var studentTranscriptSearchPredicate = _transcriptService.GetStudentTranscriptSearchPredicate(query);
            var studentTranscriptLinqedList = _linqWrapperService.GetLinqedList(studentTranscriptCompleteList, studentTranscriptSearchPredicate, filterByProperty, filterByValue, orderBy, sortOrder, skip, take);

            var result = new ItemsCountModel<StudentTranscriptResponseModel>
            {
                Items = await _transcriptService.GetStudentTranscriptResponseModelAsync(studentTranscriptLinqedList.Items),
                Count = studentTranscriptLinqedList.Count
            };
            return Ok(result);
        }

        // POST api/transcripts/{tid}/students/{pid}
        [HttpPost("{tid}/students/{pid}")]
        public async Task<IActionResult> MatchTranscriptToStudent(int tid, int pid)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            await _transcriptRepository.MatchTranscriptToStudentAsync(tid, pid, educatorId);
            return Ok();
        }

        // GET transcripts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // @TODO: Check if the current user is an educator and has permission to view Transcripts => THERE IS ALREADY A CHECK IN FE, THIS IS NECESSARY ONLY TO PROTECT THE API FROM PEOPLE USING SOFTWARE LIKE POSTMAN
            var educatorId = GetClaim<int>(CcClaimType.EducatorId);
            var currentSchool = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(educatorId);

            var transcript = await _transcriptRepository.GetByIdAsync(id);
            var result = await _transcriptService.GetTranscriptResponseModelAsync(transcript, currentSchool.Id);
 
            return Ok(result);
        }
    }
}
