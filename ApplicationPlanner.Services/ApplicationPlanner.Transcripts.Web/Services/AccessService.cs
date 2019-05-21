using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using CC.Common.Enum;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface IAccessService
    {
        /// <summary>
        /// Check if a student has a ccess to Transcripts
        /// </summary>
        /// <param name="schoolId">The school the student is at</param>
        /// <param name="countryType">The country of the school the student is at</param>
        /// <param name="gradeNumber">The grade the student is in</param>
        /// <returns>yes/no</returns>
        Task<bool> StudentHasAccessToTranscriptsAsync(int schoolId, CountryType countryType, int gradeNumber);

        /// <summary>
        /// Check if an educator has a ccess to Transcripts
        /// </summary>
        /// <param name="institutionId">The id of the institution, the educator is currently in</param>
        /// <param name="institutionType">The type of that institution</param>
        /// <param name="countryType">The country of that institution</param>
        /// <param name="availableGrades">All the grades avaialble at that institution</param>
        /// <returns>yes/no</returns>
        Task<bool> EducatorHasAccessToTranscriptsAsync(int institutionId, InstitutionTypeEnum institutionType, CountryType countryType, int[] availableGrades);
    }

    public class AccessService : IAccessService
    {
        private readonly ISchoolSettingRepository _schoolSettingRepo;
        private readonly int[] _gradesWithAccessToTranscripts;
        private readonly CountryType[] _countriesWithAccessToTranscripts;

        public AccessService(ISchoolSettingRepository schoolSettingRepo)
        {
            _schoolSettingRepo = schoolSettingRepo;
            _gradesWithAccessToTranscripts = new[] { 11, 12 };
            _countriesWithAccessToTranscripts = new[] { CountryType.US };
        }

        public async Task<bool> StudentHasAccessToTranscriptsAsync(int schoolId, CountryType countryType, int gradeNumber)
        {
            return await CheckAccessAsync(schoolId, countryType, new[] { gradeNumber });
        }

        public async Task<bool> EducatorHasAccessToTranscriptsAsync(int institutionId, InstitutionTypeEnum institutionType, CountryType countryType, int[] availableGrades)
        {
            return institutionType == InstitutionTypeEnum.School
                && await CheckAccessAsync(institutionId, countryType, availableGrades);
        }

        public async Task<bool> CheckAccessAsync(int schoolId, CountryType countryType, int[] grades)
        {
            var schoolSetting = await _schoolSettingRepo.GetBySchoolIdAsync(schoolId);
            var isTranscriptsSetupComplete = schoolSetting == null ? false : true;

            return isTranscriptsSetupComplete
                && schoolSetting.IsTranscriptEnabled
                && _countriesWithAccessToTranscripts.Contains(countryType)
                && grades.Any(g => _gradesWithAccessToTranscripts.Contains(g));
        }
    }
}
