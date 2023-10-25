using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;

namespace business.Group
{
    public class CamadaNove : Filtro
    {
        public virtual List<CamadaDez>? CamadaDez { get; set; }
         public Int64 CamadaOitoId { get; set; }
        public virtual CamadaOito? CamadaOito { get; set; }
    }
}