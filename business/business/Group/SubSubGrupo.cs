using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business.business;

namespace business.Group
{
    public class SubSubGrupo : PastaCompartilhada
    {
        public virtual List<CamadaSeis>? CamadaSeis { get; set; }
         public Int64 SubGrupoId { get; set; }
        public virtual SubGrupo? SubGrupo { get; set; }

    }
}