namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class TranscriptInstitutionModel
    {
        public string InunId { get; set; } /* INUN_ID (standard United States identifier for post-secondary institutions) */
        public int ReceivingInstitutionCode { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string City { get; set; }
        public string StateProvCode { get; set; }
        public string StateProvName { get; set; }
    }
}
