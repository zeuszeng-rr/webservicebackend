using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IO;
using WebServiceBackend.Core.Options;
using WebServiceBackend.Core.Options.WebServiceBackend.Core.Options;
using WebServiceBackend.Infrastructure.Data;
using WebServiceBackend.Infrastructure.Helpers;
using WebServiceBackend.Infrastructure.Interceptors;

namespace WebServiceBackend.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOptions = new ConnectionDatabaseOptions();
            config.GetSection("ConnectionStrings").Bind(dbOptions);

            var provider = config["ConnectionStrings:DatabaseProvider"];
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlServer(dbOptions.DefaultConnection);
            }
            else if (string.Equals(provider, "Postgres", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseNpgsql(dbOptions.PostgresConnection);
            }
            else
            {
                throw new Exception($"Unsupported database provider: {provider}");
            }

            var encryptionOptions = new EncryptionOptions();
            config.GetSection(EncryptionOptions.SectionName).Bind(encryptionOptions);
            var encryptionHelper = new EncryptionHelper(Options.Create(encryptionOptions));

            var auditTrailOptions = new AuditTrailOptions();
            config.GetSection(AuditTrailOptions.SectionName).Bind(auditTrailOptions);
            var auditTrailInterceptor = new AuditTrailInterceptor(Options.Create(auditTrailOptions));

            return new AppDbContext(optionsBuilder.Options, encryptionHelper, auditTrailInterceptor);
        }
    }
}
