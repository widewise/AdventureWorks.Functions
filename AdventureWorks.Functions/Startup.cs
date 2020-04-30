using AdventureWorks.Data.Extensions;
using AdventureWorks.Services.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AdventureWorks.Functions.Startup))]
namespace AdventureWorks.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddServices();
            builder.Services.AddDataServices();
        }
    }
}
