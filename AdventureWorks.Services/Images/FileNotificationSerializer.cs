using System.IO;

namespace AdventureWorks.Services.Images
{
    public static class FileNotificationFactory
    {
        public static FileNotification Create(string fileName, string azureFileName)
        {
            return new FileNotification
            {
                FileName = fileName,
                AzureBlobFileName = azureFileName,
                FileExtension = Path.GetExtension(fileName)
            };
        }
    }
}
