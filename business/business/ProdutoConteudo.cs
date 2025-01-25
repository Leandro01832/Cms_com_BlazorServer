using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class ProdutoConteudo : BaseModel
    {
        public long ProdutoId { get; set; }
        public long ContentId { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Content Content { get; set; }

    }
}
