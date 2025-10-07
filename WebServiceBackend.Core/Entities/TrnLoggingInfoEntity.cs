using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceBackend.Core.Entities
{
    public class TrnLoggingInfoEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
        public DateTime ResponseTimestamp { get; set; } = DateTime.UtcNow;

        public string LogLevel { get; set; } = "Information";

        public string Method { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string? QueryString { get; set; }

        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }

        public int StatusCode { get; set; }
        public long ElapsedMilliseconds { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CreatedBy { get; set; }

    }
}
