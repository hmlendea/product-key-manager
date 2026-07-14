using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NuciAPI.Middleware.ExceptionHandling;
using NuciAPI.Middleware.Logging;
using NuciAPI.Middleware.Security;

using ProductKeyManager.Configuration;

namespace ProductKeyManager
{
    public class Startup(IConfiguration configuration)
    {
        private static string EmptyProductKeysStoreContent
            => "<?xml version=\"1.0\" encoding=\"utf-8\"?><ArrayOfProductKeyDataObject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"></ArrayOfProductKeyDataObject>";

        public IConfiguration Configuration => configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddConfigurations(Configuration)
                .AddNuciApiScannerProtection()
                .AddNuciApiReplayProtection()
                .AddCustomServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Ensure the product keys store exists
            DataStoreSettings dataStoreSettings = app.ApplicationServices
                .GetRequiredService<DataStoreSettings>();
            string directory = Path.GetDirectoryName(
                dataStoreSettings.ProductKeysStorePath);

            if (!string.IsNullOrWhiteSpace(directory) &&
                !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!string.IsNullOrWhiteSpace(dataStoreSettings.ProductKeysStorePath) &&
                !File.Exists(dataStoreSettings.ProductKeysStorePath))
            {
                File.WriteAllText(dataStoreSettings.ProductKeysStorePath, EmptyProductKeysStoreContent);
            }

            app.UseNuciApiExceptionHandling();
            app.UseNuciApiScannerProtection();
            app.UseNuciApiRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseNuciApiHeaderValidation();
            app.UseNuciApiReplayProtection();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
