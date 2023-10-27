using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Book
{
    public class InstanteAdmin : Instante
    {
        public virtual List<LivroAdmin> livros { get; set; }
    }
}
