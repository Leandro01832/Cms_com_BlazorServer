using business.business;
using business.Group;
using System;

namespace business
{
    public  class Filtro : BaseModel
    {
        public string? Nome { get; set; }
        public string? Rotas { get; set; }
        public string? user { get; set; }
        public virtual List<Content> Content { get; set; }
        public virtual List<UserModel> UserModel { get; set; }
        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }

        public virtual List<FiltroPagina>? Pagina { get; set; }

        public virtual List<UserModelFiltro> usuarios { get; set; }

        public void IncluiPagina(Pagina pag)
        {
            if (this.Pagina == null) this.Pagina = new List<FiltroPagina>();
            this.Pagina!.Add(new FiltroPagina { Pagina = pag, Filtro = this });
        }
    }
}