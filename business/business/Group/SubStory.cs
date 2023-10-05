
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.Group
{
        public class SubStory : BaseModel
        {
        [Key, ForeignKey("Filtro")]
        public new Int64 Id { get; set; }
        public string? Nome { get; set; }
        public virtual List<Pagina> Pagina { get; set; }
        public virtual List<Grupo>? Grupo { get; set; }

        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }
        public virtual Filtro? Filtro { get; set; }


    }

}