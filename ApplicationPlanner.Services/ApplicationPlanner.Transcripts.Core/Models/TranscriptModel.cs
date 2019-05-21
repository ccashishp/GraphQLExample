using System;

namespace ApplicationPlanner.Transcripts.Core.Models
{
    // ApplicationPlanner.Transcript Table
    public class TranscriptBaseModel
    {
        public int TranscriptId { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string DateOfBirth { get; set; }
        public DateTime ReceivedDateUtc { get; set; }
    }

    public class TranscriptModel: TranscriptBaseModel
    {
        public int SchoolId { get; set; }
        public string EmailAddress { get; set; }
        public int PortfolioId { get; set; }
        public DateTime? LinkApprovedDateUTC { get; set; }
        public int EducatorId { get; set; }
        public bool IsAutoLink { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsArchived { get; set; }
    }
}
