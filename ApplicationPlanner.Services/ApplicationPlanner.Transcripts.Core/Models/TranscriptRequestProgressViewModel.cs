using CC.Common.Enum;
using System;
namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class TranscriptRequestProgressViewModel : TranscriptRequestModel, IAvatarDetail
    {
        public int UserAccountId { get; set; }
        public string AvatarFileName { get; set; }
        public CountryType SchoolCountryType { get; set; }
        public string StudentName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GradeId { get; set; }
        public string GradeKey { get; set; }
        public string StudentId { get; set; }
        public int TranscriptRequestId { get; set; }
        public string ReceivingInstitutionName { get; set; }
        public string ReceivingInstitutionCity { get; set; }
        public string ReceivingInstitutionStateCode { get; set; }
        public DateTime RequestedDate { get; set; }
        public TranscriptRequestStatus TranscriptStatus { get; set; }
        public string TranscriptStatusKey { get; set; }
        public DateTime TranscriptStatusDate { get; set; }
    }
}