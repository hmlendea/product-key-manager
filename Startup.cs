using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductKeyManager.Configuration;

namespace ProductKeyManager
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration => configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddConfigurations(Configuration)
                .AddCustomServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Ensure the log store exists
            var dataStoreSettings = app.ApplicationServices.GetRequiredService<DataStoreSettings>();
            var directory = Path.GetDirectoryName(dataStoreSettings.ProductKeysStorePath);

            if (!string.IsNullOrWhiteSpace(directory) &&
                !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!string.IsNullOrWhiteSpace(dataStoreSettings.ProductKeysStorePath) &&
                !File.Exists(dataStoreSettings.ProductKeysStorePath))
            {
                File.WriteAllText(dataStoreSettings.ProductKeysStorePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><ArrayOfProductKeyEntity xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"></ArrayOfProductKeyEntity>");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
