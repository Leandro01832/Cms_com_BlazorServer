using business.business.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserModelTime
    {
        public string UserModelId { get; set; }
        public long TimeId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Time Time { get; set; }

    }
}
