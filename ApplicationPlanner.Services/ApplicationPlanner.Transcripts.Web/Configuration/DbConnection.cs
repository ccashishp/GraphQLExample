using CC.Data;

namespace ApplicationPlanner.Transcripts.Web.Configuration
{
    public class DbConnection : ISqlConnectionString
    {
        private string _connString;

        public DbConnection(string connString)
        {
            _connString = connString;
        }

        public string GetConnectionString()
        {
            return _connString;
        }
    }
}
