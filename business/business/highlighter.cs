using business.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class highlighter : BaseModel
    {
        public string? user { get; set; }
        public string? Nome { get; set; }
        public int capitulo { get; set; }
        public int pasta { get; set; }

        public virtual List<MarcadorPagina> Pagina { get; set; }
        public virtual List<Content> Content { get; set; }

        public void IncluiPagina(Pagina fil)
        {
            this.Pagina!.Add(new MarcadorPagina
            {
                highlighter = this,
                Pagina = fil
            });

        }
    }
}
