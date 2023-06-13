using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace business.Group
{

    public class Grupo : BaseModel
    {
            [Key, ForeignKey("Filtro")]
             public new Int64 Id { get; set; }
        public string? Nome { get; set; }
        public virtual List<Pagina>? Pagina { get; set; }
             public virtual List<SubGrupo>? SubGrupo { get; set; }
            public Int64 SubStoryId { get; set; }
            public virtual SubStory? SubStory { get; set; }
            public virtual Filtro? Filtro { get; set; }

    }

}