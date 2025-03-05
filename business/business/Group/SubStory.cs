
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using business.business;

namespace business.Group
{
        public class SubStory : PastaCompartilhada
        {
            public virtual List<Grupo>? Grupo { get; set; }


    }

}