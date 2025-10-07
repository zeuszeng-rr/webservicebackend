using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebServiceBackend.Core.Interfaces;
using WebServiceBackend.Core.Options.WebServiceBackend.Core.Options;
using WebServiceBackend.Infrastructure.Data;
using WebServiceBackend.Infrastructure.Interceptors;
using WebServiceBackend.Infrastructure.Repositories;

namespace WebServiceBackend.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                var config = provider.GetRequiredService<IOptionsSnapshot<ConnectionDatabaseOptions>>().Value;
                var interceptor = provider.GetRequiredService<AuditTrailInterceptor>();
                if (config.DatabaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseSqlServer(config.DefaultConnection);
                    options.AddInterceptors(interceptor);
                }
                else if (config.DatabaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseNpgsql(config.PostgresConnection);
                    options.AddInterceptors(interceptor);
                }
                else
                {
                    throw new Exception("Unsupported database provider");
                }
            });
            services.AddScoped<ITrnLoggingInfoRepository, TrnLoggingInfoRepository>();

            return services;
        }
    }
}
