using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Book
{
    public class LivroUser : Livro
    {
        public long InstanteUserId { get; set; }
        public virtual InstanteUser InstanteUser { get; set; }
        public string? user { get; set; }
        public int Pasta { get; set; }

    }
}
