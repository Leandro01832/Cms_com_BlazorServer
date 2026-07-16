
using business.business;
using business.business.Book;
using business.business.Group;

namespace business.business.sistema
{
    public class Camada : BaseModel
    {
        public int Numero { get; set; }
        public virtual List<Criterio> Criterio { get; set; }
        public virtual List<Filtro> Filtro { get; set; }

        public long? LivroId { get; set; }
        public virtual Livro Livro { get; set; }


    }
    
}
