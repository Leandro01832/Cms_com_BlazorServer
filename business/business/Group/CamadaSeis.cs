using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.Group
{
    public class CamadaSeis : Filtro
    {
        public virtual List<CamadaSete>? CamadaSete { get; set; }
         public Int64 SubSubGrupoId { get; set; }
        public virtual SubSubGrupo? SubSubGrupo { get; set; }
    }
}