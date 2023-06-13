using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace business.Group
{

        public class SubGrupo : BaseModel
        {
                [Key, ForeignKey("Filtro")]
                public new Int64 Id { get; set; }
                public string? Nome { get; set; }
                public virtual List<Pagina>? Pagina { get; set; }
                 public virtual List<SubSubGrupo>? SubSubGrupo { get; set; }                    
                    public Int64 GrupoId { get; set; }
                    public virtual Grupo? Grupo { get; set; }
                    public virtual Filtro? Filtro { get; set; }
    }

}