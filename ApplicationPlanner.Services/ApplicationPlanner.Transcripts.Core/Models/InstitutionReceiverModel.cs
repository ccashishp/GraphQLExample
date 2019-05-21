using System.Collections.Generic;
namespace ApplicationPlanner.Transcripts.Core.Models
{
    // matches the data fetched via the Credentails API: https://www.credentials-inc.com/cgi-bin/RCVRLSTCGI.pgm?XMLorJSON=JSON&gradLevel=ALL 
    public class InstitutionReceiverModel
    {
        public string Name { get; set; }
        public string CruzId { get; set; }
        public string Fice { get; set; }
        public string Duns { get; set; }
        public string Sduns { get; set; }
        public string EssId { get; set; }
        public string Ceeb { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Accepts_Pdf { get; set; }
        public string Accepts_Xml { get; set; }
        public string Accepts_Edi { get; set; }
        public string Mail_Only { get; set; }
        public List<string> Emails { get; set; }
    }
    public class CredentialsReceiversDto
    {
        public CredentialsReceiverListDto ReceiverList { get; set; }
    }
    public class CredentialsReceiverListDto
    {
        public IEnumerable<InstitutionReceiverModel> Receiver { get; set; }
    }
}