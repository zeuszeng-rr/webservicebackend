using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceBackend.Core.Options
{
    public class HangfireOptions
    {
        public const string SectionName = "Hangfire";

        public bool EnableHangfire { get; set; } = true;
        public bool EnableDashboard { get; set; } = true;

        /// <summary>
        /// Supported: "SQL Server", "Postgres"
        /// </summary>
        public string Database { get; set; } = "SQL Server";
    }
}
