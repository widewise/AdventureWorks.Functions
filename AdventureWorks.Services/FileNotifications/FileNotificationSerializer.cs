using System.IO;
using Newtonsoft.Json;

namespace AdventureWorks.Services.FileNotifications
{
    public class FileNotificationSerializer : IFileNotificationSerializer
    {
        public FileNotification Deserialize(string jsonString)
        {
            var serializer = new JsonSerializer();
            using (var streamReader = new StringReader(jsonString))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return serializer.Deserialize<FileNotification>(jsonTextReader);
                }
            }
        }
    }
}