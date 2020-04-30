using System.IO;

namespace AdventureWorks.Services.FileNotifications
{
    public interface IFileNotificationSerializer
    {
        FileNotification Deserialize(string jsonString);
    }
}