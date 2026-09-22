using System;
using System.Collections.Generic;
using System.Net.Http;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ProductKeyManager.Configuration;

namespace ProductKeyManager.IntegrationTests.Infrastructure
{
    public sealed class ProductKeyManagerApiFactory : WebApplicationFactory<ProductKeyManager.Program>
    {
        public static string ApiKey => "Nucilandia-Integration-Test-Key";

        public ProductKeyManagerApiFactory(string storePath)
        {
            StorePath = storePath;
        }

        public string StorePath { get; }

        public HttpClient CreateHttpsClient()
        {
            WebApplicationFactoryClientOptions options = new()
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            };
            HttpClient client = CreateClient(options);
            client.DefaultRequestHeaders.Add("X-Forwarded-For", "127.0.0.1");

            return client;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTesting");
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                IEnumerable<KeyValuePair<string, string?>> configurationValues =
                [
                    new("securitySettings:sharedSecretKey", ApiKey),
                    new("dataStoreSettings:productKeysStorePath", StorePath),
                    new("nuciLoggerSettings:isFileOutputEnabled", bool.FalseString)
                ];

                configurationBuilder.AddInMemoryCollection(configurationValues);
            });
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DataStoreSettings>();
                services.AddSingleton(new DataStoreSettings { ProductKeysStorePath = StorePath });
            });
        }

    }
}