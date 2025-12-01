using business.business;
using business.business.Book;
using business.Group;
using System;

namespace business
{
    public  class Filtro : BaseModel
    {
        public Filtro()
        {
            PastaSalva = new PastaSalva();
        }
        public string? Nome { get; set; }
        public string? Rotas { get; set; }

      //  public int Camada { get; set; }

      public long? CamadaId { get; set; }
    public virtual Camada Camada { get; set; }

        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }

        public Int64? LivroId { get; set; } = null;
        public virtual Livro? Livro { get; set; }

        public virtual List<FiltroContent>? Pagina { get; set; }
        public virtual List<SubFiltro>? SubFiltro { get; set; }
        

        public virtual List<UserModelFiltro> usuarios { get; set; }

        public virtual PastaSalva PastaSalva { get; set; }

        public Int64? CriterioId { get; set; }
        public virtual Criterio Criterio { get; set; }

        public void IncluiPagina(Pagina pag)
        {
            if (this.Pagina == null) this.Pagina = new List<FiltroContent>();
            this.Pagina!.Add(new FiltroContent { Content = pag, Filtro = this });
        }
    }
}