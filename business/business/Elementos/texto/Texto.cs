
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace business.Elementos.texto
{
    public class Texto : BaseModel
    {
        [Display(Name = "Texto")]
        public string? PalavrasTexto { get; set; }
    }
}
