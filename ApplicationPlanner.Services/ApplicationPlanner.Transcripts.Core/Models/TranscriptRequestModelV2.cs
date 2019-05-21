using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationPlanner.Transcripts.Core.Models
{
 
    public class TranscriptRequestHistoryModelV2
    {
        public int TranscriptRequestId { get; set; }
        public string InunId { get; set; }
        public int ReceivingInstitutionCode { get; set; }
        public int ModifiedById { get; set; }
        public int TranscriptStatusId { get; set; }
        public DateTime? StatusDateUTC { get; set; }
        public int TranscriptRequestTypeId { get; set; }
    }

    public class TranscriptRequestTimelineDtoV2
    {
        public IEnumerable<TranscriptInstitutionModel> TranscriptRequestInstitutionList { get; set; }
        public IEnumerable<TranscriptRequestHistoryModelV2> TranscriptRequestHistoryList { get; set; }
    }
}


