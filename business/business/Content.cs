using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using business.business.Book;
using business.business.Contrato;
using business.Group;

namespace business.business
{
    public class Content : BaseModel, IMudancaEstado
    {
        public Content()
        {

        }

        public Content(MudancaEstado m)
        {
            mudanca = m;
                
        }



        public MudancaEstado mudanca;
        private string html;
        public DateTime Data { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O titulo é necessário")]
        [Display(Name = "Titulo da pagina")]
        public string? Titulo { get; set; } = "Conteudo";

        [Display(Name = "Story")]
        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }

        public Int64? LivroId { get; set; } = null;
        public virtual Livro? Livro { get; set; }

        public virtual List<FiltroContent>? Filtro { get; set; }

        

        public string? Rotas { get; set; }



        public virtual List<ProdutoConteudo> Produto { get; set; }

        public virtual List<UserModelContent> usuarios { get; set; }

        public virtual List<Comment> Comentario { get; set; }

        public int QuantLiked { get; set; } = 0;
        public int QuantShared { get; set; } = 0;

        public string? Html
        {
            get { return html; }
            set
            {
                html = value;
                if (value != null)
                    Data = DateTime.Now;
            }
        }

        private int ordenar = 0;
        [NotMapped]
        public int Ordenar
         { 
            get{
                if(ordenar == 0)
                    return (int)Id;
                else
                return ordenar;
                }
            set{ordenar = value;}
        }

        public void IncluiFiltro(Filtro fil)
        {
            if (this.Filtro == null) this.Filtro = new List<FiltroContent>();

            if (this.Filtro.FirstOrDefault(f => f.FiltroId == fil.Id) == null)
            {
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this
                    });

                

            }


        }

        public void IncluiProduto(Produto produto)
        {
            if (this.Produto == null) this.Produto = new List<ProdutoConteudo>();

            this.Produto!.Add(new ProdutoConteudo
            {
                Produto = produto,
                Content = this,
            });
        }


        public Pagina MudarEstado( UserContent m, long curtidas, long compartilhamentos)
        {
          return  mudanca.MudarEstado( m, curtidas, compartilhamentos);
        }

        public Pagina MudarEstado2(Comment m, long curtidas, long compartilhamentos)
        {
            throw new NotImplementedException();
        }
    }
}
