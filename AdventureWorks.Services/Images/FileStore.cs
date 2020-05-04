using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using AdventureWorks.Services.FileNotifications;

namespace AdventureWorks.Services.Images
{
    public class FileStore : IFileStore
    {
        private readonly CloudStorageAccount _account;
        private readonly FileStoreSettings _settings;

        public FileStore(
            CloudStorageAccount account,
            FileStoreSettings settings)
        {
            _account = account;
            _settings = settings;
        }

        public async Task<string> Save(string fileName, Stream fileStream)
        {
            var blobClient = _account.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference(_settings.BlobContainerName.ToLower());
            await cloudBlobContainer.CreateIfNotExistsAsync();

            var queueClient = _account.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_settings.QueueName.ToLower());
            await queue.CreateIfNotExistsAsync();

            var fileBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            await fileBlob.UploadFromStreamAsync(fileStream);
            var azureFileName = $"{blobClient.BaseUri}{cloudBlobContainer.Name}/{fileName}";

            var notification = FileNotificationFactory.Create(fileName, azureFileName);

            var serializer = new JsonSerializer();
            var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, notification);

            CloudQueueMessage message = new CloudQueueMessage(stringWriter.ToString());
            await queue.AddMessageAsync(message);

            return azureFileName;
        }

        public async Task<byte[]> DownloadFile(string fileName)
        {
            var blobClient = _account.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference(_settings.BlobContainerName.ToLower());
            await cloudBlobContainer.CreateIfNotExistsAsync();

            var fileBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            using (var ms = new MemoryStream())
            {
                await fileBlob.DownloadToStreamAsync(ms);

                return ms.ToArray();
            }
        }
    }
}