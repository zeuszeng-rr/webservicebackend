using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebServiceBackend.Core.Entities;
using WebServiceBackend.Core.Options;

namespace WebServiceBackend.Infrastructure.Interceptors
{
    public class AuditTrailInterceptor : SaveChangesInterceptor
    {
        private readonly AuditTrailOptions _options;

        public AuditTrailInterceptor(IOptions<AuditTrailOptions> options)
        {
            _options = options.Value;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context;

            if (context == null || !_options.Enabled)
                return base.SavingChanges(eventData, result);

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Where(e => !_options.ExcludeTables.Contains(e.Metadata.GetTableName() ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                .ToList();

            var auditEntries = new List<MstAuditTrailEntity>();

            foreach (var entry in entries)
            {
                var audit = new MstAuditTrailEntity
                {
                    TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                    RecordId = GetPrimaryKeyValue(entry),
                    Action = entry.State.ToString().ToUpper(),
                    PerformedAt = DateTime.UtcNow,
                    PerformedBy = null, // ไว้มาเติม
                    IpAddress = null, // ไว้มาเติม
                    Source = "AppService", // ไว้มาเติม
                };

                if (entry.State == EntityState.Modified)
                {
                    audit.OldValues = SerializeProperties(entry.OriginalValues);
                    audit.NewValues = SerializeProperties(entry.CurrentValues);
                }
                else if (entry.State == EntityState.Added)
                {
                    audit.NewValues = SerializeProperties(entry.CurrentValues);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    audit.OldValues = SerializeProperties(entry.OriginalValues);
                }

                auditEntries.Add(audit);
            }

            if (auditEntries.Any())
            {
                context.Set<MstAuditTrailEntity>().AddRange(auditEntries);
            }

            return base.SavingChanges(eventData, result);
        }

        private string? GetPrimaryKeyValue(EntityEntry entry)
        {
            var key = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            return key?.CurrentValue?.ToString();
        }

        private string SerializeProperties(PropertyValues values)
        {
            var dict = values.Properties.ToDictionary(p => p.Name, p => values[p.Name]);
            return JsonSerializer.Serialize(dict);
        }
    }
}
