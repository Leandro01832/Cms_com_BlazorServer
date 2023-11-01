using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Book
{
    public class LivroAdmin : Livro
    {
        private string? urlBook;
        public long InstanteAdminId { get; set; }
        public virtual InstanteAdmin InstanteAdmin { get; set; }

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
