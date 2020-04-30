namespace AdventureWorks.Services.Documents
{
    public class DocumentCreateParameters
    {
        public string FileName { get; set; }

        public string DocumentSummary { get; set; }

        public byte[] DocumentData { get; set; }
    }
}