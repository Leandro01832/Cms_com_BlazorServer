
using System;
using business.Elementos.texto;
using business.div;

namespace business.Join
{
    public class DivElemento
    {
        public Int64? ElementoId { get; set; }
        public Int64? DivId { get; set; }
        public virtual Div? Div { get; set; }
        public virtual Texto? Elemento { get; set; }
    }
}
