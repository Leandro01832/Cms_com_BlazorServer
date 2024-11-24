using business.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class ContentUser : Content
    {

        public string UserModelId { get; set; }
        public virtual UserModel UserModel { get; set; }
    }
}
