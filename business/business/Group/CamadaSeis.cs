using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.Group
{
    public class CamadaSeis : BaseModel
    {
        [Key, ForeignKey("Filtro")]
        public new Int64 Id { get; set; }
        public string? Nome { get; set; }
        public virtual List<Pagina> Pagina { get; set; }
        public virtual List<CamadaSete>? CamadaSete { get; set; }
         public Int64 SubSubGrupoId { get; set; }
        public virtual SubSubGrupo? SubSubGrupo { get; set; }
        public virtual Filtro? Filtro { get; set; }
    }
}