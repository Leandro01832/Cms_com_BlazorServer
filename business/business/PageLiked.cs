using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class PageLiked : BaseModel
    {
       
        public long? PaginaId { get; set; }
        public virtual Pagina Pagina { get; set; }

        public long? ContentId { get; set; }
        public virtual Content Content { get; set; }

        public virtual List<UserModelPageLiked> usuarios { get; set; }

    }
}
