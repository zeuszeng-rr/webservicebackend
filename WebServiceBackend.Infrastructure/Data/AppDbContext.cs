using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceBackend.Core.Entities;
using WebServiceBackend.Infrastructure.Helpers;
using WebServiceBackend.Infrastructure.Interceptors;

namespace WebServiceBackend.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly EncryptionHelper _encryptionHelper;
        private readonly AuditTrailInterceptor _auditTrailInterceptor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            EncryptionHelper encryptionHelper,
            AuditTrailInterceptor auditTrailInterceptor
        ) : base(options)
        {
            _encryptionHelper = encryptionHelper;
            _auditTrailInterceptor = auditTrailInterceptor;
        }

        public DbSet<TrnLoggingInfoEntity> TrnLoggingInfoEntity { get; set; }
        public DbSet<MstAuditTrailEntity> MstAuditTrailEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

    }
}
