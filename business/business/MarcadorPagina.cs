using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class MarcadorPagina
    {
        public long PaginaId { get; set; }
        public long highlighterId { get; set; }
        public virtual highlighter? highlighter { get; set; }
        public virtual Pagina? Pagina { get; set; }
    }
}
