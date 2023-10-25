using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace business.Group
{

    public class Grupo : Filtro
    {
             public virtual List<SubGrupo>? SubGrupo { get; set; }
            public Int64 SubStoryId { get; set; }
            public virtual SubStory? SubStory { get; set; }

    }

}