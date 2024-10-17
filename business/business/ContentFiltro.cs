using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class ContentFiltro
    {
        public long FiltroId { get; set; }
        public long ContentId { get; set; }
        public virtual Content? Content { get; set; }
        public virtual Filtro? Filtro { get; set; }
    }
}
