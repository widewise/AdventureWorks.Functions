namespace AdventureWorks.Services.Images
{
    public class FileStoreSettings
    {
        public string AzureWebJobsStorage { get; }
        public string BlobContainerName { get; }
        public string QueueName { get; }

        public FileStoreSettings(
            string azureWebJobsStorage,
            string blobContainerName,
            string queueName)
        {
            AzureWebJobsStorage = azureWebJobsStorage;
            BlobContainerName = blobContainerName;
            QueueName = queueName;
        }
    }
}