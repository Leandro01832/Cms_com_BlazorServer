using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace business.Group
{
    public class SubSubGrupo : Filtro
    {
        public virtual List<CamadaSeis>? CamadaSeis { get; set; }
         public Int64 SubGrupoId { get; set; }
        public virtual SubGrupo? SubGrupo { get; set; }

    }
}