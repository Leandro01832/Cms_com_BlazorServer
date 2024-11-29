using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserModelContent
    {
        public string UserModelId { get; set; }
        public long ContentId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Content Content { get; set; }
    }
}
