using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class EmailSettings
    {
        public string? PrimaryDomaim { get; set; }
        public int PrimaryPort { get; set; }
        public string? UsernameEmail { get; set; }
        public string? UserNamePassword { get; set; }
        public string? FromEmail { get; set; }
        public string? ToEmail { get; set; }
        public string? CcEmail { get; set; }
    }
}
