using System.ComponentModel.DataAnnotations.Schema;

namespace business
{
    public class Livro : BaseModel
    {
        public string? url { get; set; }
        
        public int Capitulo { get; set; }

        public bool Compartilhando { get; set; }
    }
}