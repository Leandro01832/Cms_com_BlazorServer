using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;

namespace business.Group
{
    public class CamadaNove : BaseModel
    {
        [Key, ForeignKey("Filtro")]
        public new Int64 Id { get; set; }
        public string? Nome { get; set; }
        public virtual List<Pagina>? Pagina { get; set; }
        public virtual List<CamadaDez>? CamadaDez { get; set; }
         public Int64 CamadaOitoId { get; set; }
        public virtual CamadaOito? CamadaOito { get; set; }
        public virtual Filtro? Filtro { get; set; }
    }
}