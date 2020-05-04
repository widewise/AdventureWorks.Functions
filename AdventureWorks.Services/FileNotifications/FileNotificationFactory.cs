namespace AdventureWorks.Services.FileNotifications
{
    public static class FileNotificationFactory
    {
        public static FileNotification Create(string fileName, string azureFileName)
        {
            return new FileNotification
            {
                FileName = fileName,
                AzureBlobFileName = azureFileName,
                DocumentSummary = "This is test Azure Functions."
            };
        }
    }
}
