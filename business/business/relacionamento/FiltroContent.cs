using business.business.conteudo;
using business.business.Group;

namespace business.business
{
    public class FiltroContent
    {
        public long ContentId { get; set; }
        public long FiltroId { get; set; }
        public virtual Filtro? Filtro { get; set; }
        public virtual Content? Content { get; set; }
    }
}
