using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciDAL.Repositories;
using NuciLog;
using NuciLog.Configuration;
using NuciLog.Core;

using ProductKeyManager.Configuration;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Service;

namespace ProductKeyManager
{
    public static class ServiceCollectionExtensions
    {
        static DataStoreSettings dataStoreSettings;
        static SecuritySettings securitySettings;

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            dataStoreSettings = new DataStoreSettings();
            securitySettings = new SecuritySettings();

            configuration.Bind(nameof(DataStoreSettings), dataStoreSettings);
            configuration.Bind(nameof(SecuritySettings), securitySettings);

            return services
                .AddSingleton(dataStoreSettings)
                .AddSingleton(securitySettings)
                .AddNuciLoggerSettings(configuration);
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services) => services
            .AddSingleton<IFileRepository<ProductKeyEntity>>(x => new XmlRepository<ProductKeyEntity>(dataStoreSettings.ProductKeysStorePath))
            .AddSingleton<IProductKeyService, ProductKeyService>()
            .AddScoped<ILogger, NuciLogger>();
    }
}
