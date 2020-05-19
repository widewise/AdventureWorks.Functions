namespace AdventureWorks.Services
{
    public class SearchIndexSettings
    {
        public SearchIndexSettings(
            string serviceName,
            string sqlIndexName,
            string blobIndexName,
            string serviceApiKey)
        {
            ServiceName = serviceName;
            SqlIndexName = sqlIndexName;
            BlobIndexName = blobIndexName;
            ServiceApiKey = serviceApiKey;
        }

        public string ServiceName { get; }
        public string SqlIndexName { get; }
        public string BlobIndexName { get; }
        public string ServiceApiKey { get; }
    }
}