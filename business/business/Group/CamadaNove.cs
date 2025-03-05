using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;
using business.business;

namespace business.Group
{
    public class CamadaNove : PastaCompartilhada
    {
        public virtual List<CamadaDez>? CamadaDez { get; set; }
         public Int64 CamadaOitoId { get; set; }
        public virtual CamadaOito? CamadaOito { get; set; }
    }
}