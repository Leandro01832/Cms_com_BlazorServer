using System.ComponentModel.DataAnnotations;

namespace business.business.conteudo
{
    public class ProductContent : Pagina
    {
        public ProductContent() : base()
        {
        }


        public ProductContent(int count) : base(count)
        {
            
        }

        [Url]
        public string Afilado { get; set; }

        public override string ToString()
        {
            return "Produtos";
        }
    }
}
