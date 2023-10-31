using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class PageLiked : BaseModel
    {
        public string user { get; set; }
        public int capitulo { get; set; }
        public int indice { get; set; }
        public int verso { get; set; }
        public int substory { get; set; }
        public int? grupo { get; set; }
        public int? subgrupo { get; set; }
        public int? subsubgrupo { get; set; }
        public int? camadaSeis { get; set; }
        public int? camadaSete { get; set; }
        public int? camadaOito { get; set; }
        public int? camadaNove { get; set; }
        public int? camadaDez { get; set; }

    }
}
