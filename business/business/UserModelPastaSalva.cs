using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserModelPastaSalva
    {
        public string UserModelId { get; set; }
        public long PastaSalvaId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual PastaSalva PastaSalva { get; set; }
    }
}
