using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserBook10 : BaseModel
    {
        public string? user { get; set; }

        public long LivroId { get; set; }

        public virtual Livro? Livro { get; set; }

        public int Capitulo { get; set; }
    }
}
