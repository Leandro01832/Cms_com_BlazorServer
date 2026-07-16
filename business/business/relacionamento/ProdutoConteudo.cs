using business.business.conteudo;
using business.business.ecommerce;

namespace business.business.relacionamento
{
    public class ProdutoConteudo 
    {
        public long ProdutoId { get; set; }
        public long ContentId { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Content Content { get; set; }

    }
}
