
using business.business.conteudo;

namespace business.business.sistema
{
    public class MarcacaoVideoFilter : BaseModel
    {
        public long ContentId { get; set; }
        public virtual Content Content { get; set; }
        public int Segundos { get; set; }
    }
    
}
