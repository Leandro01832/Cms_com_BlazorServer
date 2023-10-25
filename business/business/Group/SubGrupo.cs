using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace business.Group
{

        public class SubGrupo : Filtro
        {
                 public virtual List<SubSubGrupo>? SubSubGrupo { get; set; }                    
                    public Int64 GrupoId { get; set; }
                    public virtual Grupo? Grupo { get; set; }
    }

}