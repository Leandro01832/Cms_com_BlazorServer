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

        [Key, ForeignKey("Assinatura")]
        public new Int64 Id { get; set; }

        public virtual Assinatura? Assinatura { get; set; }

        public string Nome { get; set; }
        public string Capa { get; set; }

        public int BookNumber { get; set; }

        public int StandardChapter { get; set; }



        public string? url { get; set; }

        public virtual List<UserModelLivro> Users { get; set; }
        public virtual List<Content> Content { get; set; }
        public virtual List<Filtro> Filtro { get; set; }
        public virtual List<Camada> Camada { get; set; }
        public virtual List<Criterio> Criterio { get; set; }
    }
}