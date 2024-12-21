using System.ComponentModel.DataAnnotations;
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
        public string? Titulo { get; set; }

        [Display(Name = "Story")]
        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }

        public virtual List<FiltroContent>? Filtro { get; set; }

        public string? Rotas { get; set; }


        public Int64? ProdutoId { get; set; }

        public virtual Produto? Produto { get; set; }

        public virtual List<UserModelContent> usuarios { get; set; }
        public virtual List<Comment> Comentario { get; set; }

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

        public void IncluiFiltro(Filtro fil)
        {
            if (this.Filtro == null) this.Filtro = new List<FiltroContent>();

            if (this.Filtro.FirstOrDefault(f => f.FiltroId == fil.Id) == null)
            {
                if (fil is SubStory)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is SubStory).ToList().Count + 1
                    });

                if (fil is Grupo)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is Grupo).ToList().Count + 1
                    });

                if (fil is SubGrupo)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is SubGrupo).ToList().Count + 1
                    });

                if (fil is SubSubGrupo)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is SubSubGrupo).ToList().Count + 1
                    });

                if (fil is CamadaSeis)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaSeis).ToList().Count + 1
                    });

                if (fil is CamadaSete)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaSete).ToList().Count + 1
                    });

                if (fil is CamadaOito)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaOito).ToList().Count + 1
                    });

                if (fil is CamadaNove)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaNove).ToList().Count + 1
                    });

                if (fil is CamadaDez)
                    this.Filtro!.Add(new FiltroContent
                    {
                        Filtro = fil,
                        Content = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaDez).ToList().Count + 1
                    });

            }


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
