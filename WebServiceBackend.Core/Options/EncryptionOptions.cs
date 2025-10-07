using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceBackend.Core.Options
{
    public class EncryptionOptions
    {
        public const string SectionName = "Encryption";
        public string Key { get; set; } = string.Empty;
        public string IV { get; set; } = string.Empty;
    }
}
