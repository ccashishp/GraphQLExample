using CC.Common.Enum;
using System;

namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class TranscriptImportedViewModel : IAvatarDetail
    {
        public int Id { get; set; } // Student's PortfolioId
        public int UserAccountId { get; set; }
        public string AvatarFileName { get; set; }
        public CountryType SchoolCountryType { get; set; }
        public string StudentName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GradeId { get; set; }
        public string GradeKey { get; set; }
        public string StudentId { get; set; }
        public int TranscriptId { get; set; }
        public DateTime ImportedDate { get; set; }
    }
}
