using business.business.Book;
using business.business.sistema;

namespace business.business.relacionamento
{
    public class UserModelLivro
    {
        public string UserModelId { get; set; }
        public long LivroId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Livro Livro { get; set; }
    }
}
