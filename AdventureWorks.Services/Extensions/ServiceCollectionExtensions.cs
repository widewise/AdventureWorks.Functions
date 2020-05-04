﻿using System;
using AdventureWorks.Services.Documents;
using AdventureWorks.Services.FileNotifications;
using AdventureWorks.Services.Images;
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
            services.AddSingleton(_ =>
            {
                string azureWebJobsStorage = Environment.GetEnvironmentVariable("FileStoreSettings:AzureWebJobsStorage", EnvironmentVariableTarget.Process);
                string blobContainerName = Environment.GetEnvironmentVariable("FileStoreSettings:BlobContainerName", EnvironmentVariableTarget.Process);
                string queueName = Environment.GetEnvironmentVariable("FileStoreSettings:QueueName", EnvironmentVariableTarget.Process);

                return new FileStoreSettings(azureWebJobsStorage, blobContainerName, queueName);
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