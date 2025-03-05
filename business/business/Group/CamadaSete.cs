using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;
using business.business;

namespace business.Group
{
    public class CamadaSete : PastaCompartilhada
    {
        public virtual List<CamadaOito>? CamadaOito { get; set; }
         public Int64 CamadaSeisId { get; set; }
        public virtual CamadaSeis? CamadaSeis { get; set; }
    }

}