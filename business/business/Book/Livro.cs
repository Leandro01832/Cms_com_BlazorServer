using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.business.Book
{
    public class Livro : BaseModel
    {
        public Livro()
        {
            
        }

        public Livro(List<Livro> livros)
        {
            BookNumber = livros.Count + 1;
        }

        public string Nome { get; set; }

        public int BookNumber { get; set; }

        public int StandardChapter { get; set; }

        public string? url { get; set; }

        public virtual List<UserModelLivro> Users { get; set; }
        public virtual List<Content> Content { get; set; }
        public virtual List<Filtro> Filtro { get; set; }
    }
}