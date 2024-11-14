using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserModelPageLiked
    {
        public string UserModelId { get; set; }
        public long PageLikedId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual PageLiked PageLiked { get; set; }
    }
}
