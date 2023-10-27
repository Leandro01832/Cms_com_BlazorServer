using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Book
{
    public class InstanteUser : Instante
    {
        public virtual List<LivroUser> Livros { get; set; }
    }
}
