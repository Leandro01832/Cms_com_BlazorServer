using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.business.Book
{
    public class Livro : BaseModel
    {
        private string? urlNoBook;

        public bool IsBook { get; set; }
        public string? UrlNoBook
        {
            get { return urlNoBook; }
            set
            {
                urlNoBook = value;
                if (string.IsNullOrEmpty(value)) IsBook = true;
                else IsBook = false;
            }
        }
        public string? url { get; set; }

        public string? user { get; set; }

        public long? InstanteId { get; set; }
        public virtual Instante Instante { get; set; }

        public int capitulo { get; set; }
        public int pasta { get; set; }
        public int versiculo { get; set; }

    }
}