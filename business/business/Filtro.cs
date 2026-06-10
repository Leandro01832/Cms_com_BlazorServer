using business.business;
using business.business.Book;
using business.business.Group;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace business
{
    public class Filtro : BaseModel
    {
        public Filtro()
        {
            
        }
        public Filtro(List<FiltroContent> pagina)
        {
            if (pagina == null || pagina.Count < 10)
                throw new ArgumentException("A lista deve conter pelo menos 10 conteudos.");
            this.pagina = pagina;
            
        }

        public string? Nome { get; set; }
        public string? Rotas { get; set; }

        public long? CamadaId { get; set; }
        public virtual Camada Camada { get; set; }

        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }

        public Int64? LivroId { get; set; } = null;
        public virtual Livro? Livro { get; set; }
        
        private List<FiltroContent> pagina;

        public virtual List<FiltroContent> Pagina
        {
            get => pagina;
            set
            {
                if (value == null || value.Count < 1)
                    throw new ArgumentException("A lista deve conter pelo menos 10 conteudos.");

                pagina = value;
            }
        }



        public virtual List<SubFiltro>? SubFiltro { get; set; }  

        public virtual List<UserModelFiltro> usuariosDecorando { get; set; }

        public virtual List<AnotacaoVersiculo> AnotacaoVersiculos { get; set; }
        public Int64? CriterioId { get; set; }
        public virtual Criterio Criterio { get; set; }

        public byte[]? VetorEmbedding { get; set; }

        public void FazerAnotacao(Anotacao anotacao)
        {
            if (this.AnotacaoVersiculos == null) this.AnotacaoVersiculos = new List<AnotacaoVersiculo>();
            this.AnotacaoVersiculos!.Add(new AnotacaoVersiculo { Anotacao = anotacao, Filtro = this });
        }

        public void IncluiPagina(Content pag)
        {
            if (this.Pagina == null) this.Pagina = new List<FiltroContent>();
            if (!this.Pagina.Any(fc => fc.ContentId == pag.Id && fc.FiltroId == this.Id))
            {
                this.Pagina.Add(new FiltroContent
                {
                    ContentId = pag.Id,
                    FiltroId = this.Id
                });
            }
        }
    }
}