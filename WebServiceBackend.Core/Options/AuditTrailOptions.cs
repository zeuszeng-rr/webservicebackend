using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceBackend.Core.Options
{
    public class AuditTrailOptions
    {
        public const string SectionName = "AppSettings";
        public bool Enabled { get; set; } = true;
        public List<string> ExcludeTables { get; set; } = new List<string>();
    }
}
