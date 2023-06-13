using business;
using business.div;
using System;

namespace business.Join
{
    public class DivPagina
    {
        public Int64? PaginaId { get; set; }
        public Int64? DivId { get; set; }
        public virtual Div? Div { get; set; }
        public virtual Pagina? Pagina { get; set; }
    }
}
