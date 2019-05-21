namespace ApplicationPlanner.Transcripts.Core.Models
{
    public enum TranscriptRequestType
    {
        InNetwork = 1,
        Email = 2,
        Mail = 3
    }

    // STATUS 2 AND 5 are the same for the user but Credentials treat them differently
    public enum TranscriptRequestStatus
    {
        Requested = 1, // You made a transcript request to your high School -> Your high school is processing your transcript
        Submitted = 2, // transcript has been submitted to the TranscriptProvider (Credentials) in order to be sent to the receiving institution - Your high school sent your transcript
        Received = 3,// OR Read - receiving institution has confirmed receipt of your transcript
        Expired = 4, // Your transcript expired => updated by credentials
        Sent = 5 // Your transcript was sent by the TranscriptProvider (Credentials) to the receiving institution
    }
}
