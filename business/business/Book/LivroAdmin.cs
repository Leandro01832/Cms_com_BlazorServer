using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Book
{
    public class LivroAdmin : Livro
    {
        public long InstanteAdminId { get; set; }
        public virtual InstanteAdmin InstanteAdmin { get; set; }
    }
}
