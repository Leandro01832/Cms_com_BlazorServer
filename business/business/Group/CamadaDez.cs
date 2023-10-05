using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;

namespace business.Group
{
    public class CamadaDez : BaseModel
    {
        [Key, ForeignKey("Filtro")]
        public new Int64 Id { get; set; }
        public virtual List<Pagina> Pagina { get; set; }
        public string? Nome { get; set; }
        public Int64 CamadaNoveId { get; set; }
        public virtual CamadaNove? CamadaNove { get; set; }
        public virtual Filtro? Filtro { get; set; }
    }
}