using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;
using business.business;

namespace business.Group
{
    public class CamadaOito : Filtro
    {
        public virtual List<CamadaNove>? CamadaNove { get; set; }
         public Int64 CamadaSeteId { get; set; }
        public virtual CamadaSete? CamadaSete { get; set; }
    }
}