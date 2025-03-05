using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using business;
using business.business;

namespace business.Group
{
    public class CamadaDez : PastaCompartilhada
    {   
        public Int64 CamadaNoveId { get; set; }
        public virtual CamadaNove? CamadaNove { get; set; }
    }
}