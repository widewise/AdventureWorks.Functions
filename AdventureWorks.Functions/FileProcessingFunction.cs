using System;
using System.Threading.Tasks;
using AdventureWorks.Services.Documents;
using AdventureWorks.Services.Images;
using AdventureWorks.Services.FileNotifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AdventureWorks.Functions
{
    public class FileProcessingFunction
    {
        private readonly IFileNotificationSerializer _fileNotificationSerializer;
        private readonly IDocumentService _documentService;
        private readonly IFileStore _fileStore;

        public FileProcessingFunction(
            IFileNotificationSerializer fileNotificationSerializer,
            IFileStore fileStore,
            IDocumentService documentService)
        {
            _fileStore = fileStore;
            _fileNotificationSerializer = fileNotificationSerializer;
            _documentService = documentService;
        }

        [FunctionName("FileProcessingFunction")]
        public async Task Run(
            [QueueTrigger("notifications", Connection = "AzureWebJobsStorage")]
            string myQueueItem,
            ILogger log)
        {
            FileNotification notification = null;
            try
            {
                notification = _fileNotificationSerializer.Deserialize(myQueueItem);
                var parameters = new DocumentCreateParameters
                {
                    FileName = notification.FileName,
                    DocumentSummary = notification.DocumentSummary,
                    DocumentData = await _fileStore.DownloadFile(notification.FileName)
                };

                _documentService.Create(parameters);

                log.LogInformation($"File '{notification.FileName}' is processed. Info: {myQueueItem}");
            }
            catch(Exception e)
            {
                log.LogError($"File '{notification?.FileName ?? string.Empty}' processing error. Info: {myQueueItem}", e);
            }
        }
    }
}