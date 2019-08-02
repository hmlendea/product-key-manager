using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciLog;
using NuciLog.Core;

using ProductKeyManager.Configuration;

namespace ProductKeyManager
{
    public static class ServiceCollectionExtensions
    {
        static DataStoreSettings dataStoreSettings;

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            dataStoreSettings = new DataStoreSettings();
            configuration.Bind(nameof(DataStoreSettings), dataStoreSettings);
            services.AddSingleton(dataStoreSettings);

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            return services
                .AddScoped<ILogger, NuciLogger>();
        }
    }
}
