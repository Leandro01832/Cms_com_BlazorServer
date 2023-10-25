
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.Group
{
        public class SubStory : Filtro
        {
            public virtual List<Grupo>? Grupo { get; set; }


    }

}