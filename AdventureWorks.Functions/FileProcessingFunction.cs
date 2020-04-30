using System.IO;
using AdventureWorks.Services.Documents;
using AdventureWorks.Services.FileNotifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AdventureWorks.Functions
{
    public class FileProcessingFunction
    {
        private readonly IFileNotificationSerializer _fileNotificationSerializer;
        private readonly IDocumentService _documentService;

        public FileProcessingFunction(
            IFileNotificationSerializer fileNotificationSerializer,
            IDocumentService documentService)
        {
            _fileNotificationSerializer = fileNotificationSerializer;
            _documentService = documentService;
        }

        [FunctionName("FileProcessingFunction")]
        public void Run(
            [QueueTrigger("notifications", Connection = "AzureWebJobsStorage")]
            string myQueueItem,
            ILogger log)
        {
            var notification = _fileNotificationSerializer.Deserialize(myQueueItem);
            var parameters = new DocumentCreateParameters
            {
                FileName = notification.FileName,
                DocumentSummary = notification.DocumentSummary,
                //DocumentData = ReadBlob(blob)
            };

            _documentService.Create(parameters);

            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }

        private byte[] ReadBlob(Stream blob)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                blob.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}