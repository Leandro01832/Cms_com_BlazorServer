using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class FiltroPagina
    {
        public int QuantidadePorType { get; set; }
        public long PaginaId { get; set; }
        public long FiltroId { get; set; }
        public virtual Filtro? Filtro { get; set; }
        public virtual Pagina? Pagina { get; set; }
    }
}
