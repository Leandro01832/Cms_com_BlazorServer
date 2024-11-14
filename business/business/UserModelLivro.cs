using business.business.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserModelLivro
    {
        public string UserModelId { get; set; }
        public long LivroId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Livro Livro { get; set; }
    }
}
