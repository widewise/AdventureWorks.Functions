using AdventureWorks.Services.Documents;
using AdventureWorks.Services.FileNotifications;
using AdventureWorks.Services.Images;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace AdventureWorks.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services to the container.
        /// </summary>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddOptions<FileStoreSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("FileStoreSettings").Bind(settings);
                });

            services.AddTransient<IFileNotificationSerializer, FileNotificationSerializer>();
            services.AddTransient<IDocumentFactory, DocumentFactory>();
            services.AddTransient<IDocumentService, DocumentService>();

            services.AddScoped<IFileStore>(
                c =>
                {
                    var settings = c.GetService<FileStoreSettings>();

                    var account = CloudStorageAccount.Parse(settings.AzureWebJobsStorage);

                    //var settings = new FileStoreSettings(blobContainerName, queueName);

                    return new FileStore(account, settings);
                });

            return services;
        }
    }
}