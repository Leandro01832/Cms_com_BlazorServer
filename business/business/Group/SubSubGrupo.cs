using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace business.Group
{
    public class SubSubGrupo : BaseModel
    {
        [Key, ForeignKey("Filtro")]
        public new Int64 Id { get; set; }
        public string? Nome { get; set; }
        public virtual List<Pagina>? Pagina { get; set; }
        public virtual List<CamadaSeis>? CamadaSeis { get; set; }

         public Int64 SubGrupoId { get; set; }
        public virtual SubGrupo? SubGrupo { get; set; }
        public virtual Filtro? Filtro { get; set; }

    }
}