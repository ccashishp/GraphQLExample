using System;
using System.Collections.Generic;

namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class TranscriptRequestModel
    {
        public int Id { get; set; }
        public string InunId { get; set; }
        public int ReceivingInstitutionCode { get; set; }
        public TranscriptRequestHistoryModel LatestHistory { get; set; }
    }

    public class TranscriptRequestInstitutionModel : TranscriptInstitutionModel
    {
        public int TranscriptRequestId { get; set; }
    }

    public class TranscriptRequestHistoryModel
    {
        public int TranscriptRequestId { get; set; }
        public int ModifiedById { get; set; }
        public int TranscriptStatusId { get; set; }
        public DateTime? StatusDateUTC { get; set; }
        public int TranscriptRequestTypeId { get; set; }
    }

    #region MultiQuery DTOs
    public class TranscriptRequestTimelineDto
    {
        public IEnumerable<TranscriptRequestInstitutionModel> TranscriptRequestInstitutionList { get; set; }
        public IEnumerable<TranscriptRequestHistoryModel> TranscriptRequestHistoryList { get; set; }
    }
    #endregion
}
