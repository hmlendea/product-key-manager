using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciDAL.Repositories;
using NuciLog;
using NuciLog.Configuration;
using NuciLog.Core;
using NuciSecurity.HMAC;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Configuration;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Security;
using ProductKeyManager.Service;

namespace ProductKeyManager
{
    public static class ServiceCollectionExtensions
    {
        static DataStoreSettings dataStoreSettings;
        static SecuritySettings securitySettings;
        static NuciLoggerSettings logSettings;

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            dataStoreSettings = new DataStoreSettings();
            securitySettings = new SecuritySettings();
            logSettings = new NuciLoggerSettings();
            
            configuration.Bind(nameof(DataStoreSettings), dataStoreSettings);
            configuration.Bind(nameof(SecuritySettings), securitySettings);
            configuration.Bind(nameof(NuciLoggerSettings), logSettings);
            
            services.AddSingleton(dataStoreSettings);
            services.AddSingleton(securitySettings);
            services.AddSingleton(logSettings);

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IRepository<ProductKeyEntity>>(x => new XmlRepository<ProductKeyEntity>(dataStoreSettings.ProductKeysStorePath))
                .AddSingleton<IHmacEncoder<GetProductKeyRequest>, GetProductKeyRequestHmacEncoder>()
                .AddSingleton<IHmacEncoder<StoreProductKeyRequest>, StoreProductKeyRequestHmacEncoder>()
                .AddSingleton<IHmacEncoder<UpdateProductKeyRequest>, UpdateProductKeyRequestHmacEncoder>()
                .AddSingleton<IHmacEncoder<ProductKeyResponse>, ProductKeyResponseHmacEncoder>()
                .AddSingleton<IProductKeyService, ProductKeyService>()
                .AddScoped<ILogger, NuciLogger>();
        }
    }
}
