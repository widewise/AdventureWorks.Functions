using System;
using System.Data;
using Microsoft.Data.SqlClient;
using AdventureWorks.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdventureWorks.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Data services to the container.
        /// </summary>
        public static IServiceCollection AddDataServices(
            this IServiceCollection services)
        {
            services.AddOptions<DatabaseSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("DatabaseSettings").Bind(settings);
/*
                    var databaseConnectionString = configuration.GetConnectionString("DatabaseConnectionString");

                    settings = new DatabaseSettings
                    {
                        DbConnectionStringName = databaseConnectionString
                    };
*/
                });

            services.AddScoped<IDbConnection>(_ =>
            {
                string cs = Environment.GetEnvironmentVariable("DatabaseSettings:DatabaseConnectionString", EnvironmentVariableTarget.Process);

                return new SqlConnection(cs);
            });

            services.AddTransient<IDocumentRepository, DocumentRepository>();

            return services;
        }
    }
}