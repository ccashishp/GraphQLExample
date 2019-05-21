using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Web.Models;
using CC.Common.Enum;
using System.Linq;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface IStudentService
    {
        AuthenticatedStudentModel AuthenticatedStudentGetByStudentGeneralInfo(StudentGeneralInfoModel studentGeneralInfo);
    }

    public class StudentService : IStudentService
    {
        private readonly IAvatarService _avatarService;
        private readonly int[] _gradesWithAccessToTranscripts;

        public StudentService(IAvatarService avatarService)
        {
            _avatarService = avatarService;
            _gradesWithAccessToTranscripts = new[] { 11, 12 }; // Transcripts is accessible for students in grade 11 and 12 only for now
        }

        public AuthenticatedStudentModel AuthenticatedStudentGetByStudentGeneralInfo(StudentGeneralInfoModel studentGeneralInfo)
        {
            return new AuthenticatedStudentModel
            {
                Id = studentGeneralInfo.Id,
                FirstName = studentGeneralInfo.FirstName,
                AvatarUrl = string.IsNullOrWhiteSpace(studentGeneralInfo.AvatarFileName) ? string.Empty : _avatarService.GetStudentAvatarUrl(studentGeneralInfo),
                HasAccessToTranscripts = _gradesWithAccessToTranscripts.Contains(studentGeneralInfo.GradeNumber)
                && studentGeneralInfo.SchoolCountryType == CountryType.US,
                globalSetting = new GlobalSettingModel
                {
                    HasSeenCartTooltipForTranscriptsInSavedSchoolsMode = studentGeneralInfo.HasSeenCartTooltipForTranscriptsInSavedSchoolsMode,
                    HasSeenCartTooltipForTranscriptsInSearchMode = studentGeneralInfo.HasSeenCartTooltipForTranscriptsInSearchMode
                }
            };
        } 
    }
}
