using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.business.Book
{
    public class Livro : BaseModel
    {
        private string? urlBook;
        public string? url { get; set; }
        [Range(1, 999, ErrorMessage = "O capitulo deve ser maior ou igual a 1")]
        public int Capitulo { get; set; }
        public bool Compartilhando { get; set; }
        public bool IsBook { get; set; }
        public string? UrlNoBook 
        {
            get { return urlBook; }
            set
            {
                urlBook = value;
                if (string.IsNullOrEmpty(value)) IsBook = true;
                else IsBook = false;
            }
        }
    }
}