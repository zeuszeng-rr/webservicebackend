using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceBackend.Core.Options
{
    namespace WebServiceBackend.Core.Options
    {
        public class ConnectionDatabaseOptions
        {
            public const string SectionName = "ConnectionStrings";

            public string DatabaseProvider { get; set; } = string.Empty;
            public string DefaultConnection { get; set; } = string.Empty;
            public string PostgresConnection { get; set; } = string.Empty;
            public string HangfireConnection { get; set; } = string.Empty;
            
        }
    }
}
