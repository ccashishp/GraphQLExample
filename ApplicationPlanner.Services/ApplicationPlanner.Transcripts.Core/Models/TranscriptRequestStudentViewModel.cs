using CC.Common.Enum;
using System;

namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class TranscriptRequestStudentViewModel : TranscriptRequestModel, IAvatarDetail
    {
        public int UserAccountId { get; set; }
        public string AvatarFileName { get; set; }
        public CountryType SchoolCountryType { get; set; }
        public string StudentName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GenderId { get; set; }
        public string GenderKey { get; set; }
        public int GradeId { get; set; }
        public string GradeKey { get; set; }
        public string StudentId { get; set; }
        public int TranscriptRequestId { get; set; }
        public string ReceivingInstitutionName { get; set; }
        public string ReceivingInstitutionCity { get; set; }
        public string ReceivingInstitutionStateCode { get; set; }
        public string InstitutionCity { get; set; }
        public TranscriptRequestType TranscriptRequestType { get; set; }
        public string TranscriptRequestTypeKey { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ImportedDate { get; set; }
    }
}
