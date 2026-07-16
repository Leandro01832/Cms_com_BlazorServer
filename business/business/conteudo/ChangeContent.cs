namespace business.business.conteudo
{
    public class ChangeContent : Pagina
    {
        public ChangeContent() : base()
        {
        }


        public ChangeContent(int count) : base(count)
        {
        }

        public virtual MudancaEstado MudancaEstado { get; set; }

        public override string ToString()
        {
            return "Mudanças de Conteúdo";
        }
    }
}
