using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Book
{
    public class Instante : BaseModel
    {
        public string? Descricao { get; set; }
        public virtual List<Livro> livros { get; set; }
    }
}
