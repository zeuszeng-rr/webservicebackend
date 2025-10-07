using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceBackend.Core.Options
{
    public class LoggingOptions
    {
        public bool WriteToFile { get; set; }
        public bool WriteToDatabase { get; set; }
        public string LogFilePath { get; set; } = null!;
    }

    public class AppSettingOptions
    {
        public const string SectionName = "AppSettings";
        public string TimeZoneId { get; set; } = null!;
        public List<string> IncludePaths { get; set; } = new();
        public LoggingOptions LoggingOptions { get; set; } = new();
    }
}
