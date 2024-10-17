using business.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Content : BaseModel
    {
        private DateTime data = DateTime.Now;

        public virtual List<ContentFiltro> Filtro { get; set; }
        public string Html { get; set; }

        public DateTime Data { get { return data; } set { data = value; } }

        public void IncluiFiltro(Filtro fil)
        {
            if (this.Filtro == null) this.Filtro = new List<ContentFiltro>();

            
                    this.Filtro!.Add(new ContentFiltro
                    {
                        Filtro = fil,
                        Content = this,
                    });

                

            


        }
    }
}
