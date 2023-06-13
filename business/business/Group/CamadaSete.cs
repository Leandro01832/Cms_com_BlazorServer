using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;

namespace business.Group
{
    public class CamadaSete : BaseModel
    {
        [Key, ForeignKey("Filtro")]
        public new Int64 Id { get; set; }
        public string? Nome { get; set; }
        public virtual List<Pagina>? Pagina { get; set; }
        public virtual List<CamadaOito>? CamadaOito { get; set; }

         public Int64 CamadaSeisId { get; set; }
        public virtual CamadaSeis? CamadaSeis { get; set; }
        public virtual Filtro? Filtro { get; set; }
    }

}