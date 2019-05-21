namespace ApplicationPlanner.Transcripts.Core.Models
{
    public class InstitutionModel
    {
        public int Id { get; set; }
        public string InstitutionName { get; set; }
        public InstitutionTypeEnum InstitutionType { get; set; }
    }

    public enum InstitutionTypeEnum
    {
        School = 0,
        Region = 1
    }
}
