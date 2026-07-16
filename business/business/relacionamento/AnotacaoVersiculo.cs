using business.business.Group;
using business.business.sistema;

namespace business.business.relacionamento
{
    public class AnotacaoVersiculo : BaseModel
    {
        public long FiltroId { get; set; }
        public long AnotacaoId { get; set; }
        public virtual Anotacao Anotacao { get; set; }
        public virtual Filtro Filtro { get; set; }
    }
    
}
