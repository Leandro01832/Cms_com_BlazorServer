using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business
{
    public class Livro : BaseModel
    {
        public string? url { get; set; }
        [Range(1, 500, ErrorMessage ="O capitulo deve ser maior ou igual a 1")]
        public int Capitulo { get; set; }

        public bool Compartilhando { get; set; }
    }
}