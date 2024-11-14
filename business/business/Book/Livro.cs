using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.business.Book
{
    public class Livro : BaseModel
    {
        public bool IsBook { get; set; }
        
        public string? url { get; set; }

        public virtual List<UserModelLivro> Users { get; set; }
    }
}