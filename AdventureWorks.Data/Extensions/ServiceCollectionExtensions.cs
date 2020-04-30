using System.Data;
using System.Data.SqlClient;
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
                });

            services.AddScoped<IDbConnection>(_ =>
            {
                var settings = _.GetService<DatabaseSettings>();

                return new SqlConnection(settings.DbConnectionStringName);
            });

            services.AddTransient<IDocumentRepository, DocumentRepository>();

            return services;
        }
    }
}