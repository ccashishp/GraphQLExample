using ApplicationPlanner.Transcripts.Core.Models;
using System;
using System.Collections.Generic;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class TranscriptRequestTimelineResponseModel
    {
        public int Id { get; set; } 
        public SavedSchool InstitutionCard { get; set; }
        //this property holds latest request only
        public IEnumerable<TranscriptRequestHistoryEvent> History { get; set; }
        //All requests for the institution 
        public IEnumerable<TranscriptRequestHistoryV2> AllRequests { get; set; }
    }
  
}
