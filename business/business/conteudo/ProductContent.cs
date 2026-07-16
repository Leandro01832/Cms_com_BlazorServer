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

        public override string ToString()
        {
            return "Produtos";
        }
    }
}
