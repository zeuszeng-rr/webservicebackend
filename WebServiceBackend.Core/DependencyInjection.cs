using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebServiceBackend.Core.Options;
using WebServiceBackend.Core.Options.WebServiceBackend.Core.Options;

namespace WebServiceBackend.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CorsOptions>(configuration.GetSection(CorsOptions.SectionName));
            services.Configure<JwtConfigurationOptions>(configuration.GetSection(JwtConfigurationOptions.SectionName));
            services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));
            services.Configure<ConnectionDatabaseOptions>(configuration.GetSection(ConnectionDatabaseOptions.SectionName));
            services.Configure<AppSettingOptions>(configuration.GetSection(AppSettingOptions.SectionName));
            services.Configure<EncryptionOptions>(configuration.GetSection(EncryptionOptions.SectionName));

            return services;
        }
    }
}
