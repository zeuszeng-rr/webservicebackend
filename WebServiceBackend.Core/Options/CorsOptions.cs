using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceBackend.Core.Options
{
    public class CorsOptions
    {
        public const string SectionName = "Cors";
        public string[] AllowedOrigins { get; set; } = [];
    }
}
