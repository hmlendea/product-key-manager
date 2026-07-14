using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciDAL.Repositories;

using NuciLog;
using NuciLog.Core;

using ProductKeyManager.Configuration;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Service;

namespace ProductKeyManager
{
    public static class ServiceCollectionExtensions
    {
        private static DataStoreSettings dataStoreSettings;
        private static SecuritySettings securitySettings;

        public static IServiceCollection AddConfigurations(
            this IServiceCollection services,
            IConfiguration configuration)
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

        public static IServiceCollection AddCustomServices(
            this IServiceCollection services) => services
            .AddSingleton<IFileRepository<ProductKeyDataObject>>(
                serviceProvider => new XmlRepository<ProductKeyDataObject>(
                    dataStoreSettings.ProductKeysStorePath))
            .AddSingleton<IProductKeyService, ProductKeyService>()
            .AddSingleton<ILogger, NuciLogger>();
    }
}
