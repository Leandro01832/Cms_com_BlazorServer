using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business.business.conteudo
{
    public class Chave : Pagina
    {        
        public Chave() : base()
        {
        }


        public Chave(int count) : base(count)
        {
        }

        [Key, ForeignKey("Content")]
        public new long Id { get; set; }
        
        public virtual Content Content { get; set; }

        public override string ToString()
        {
            return "Chaves";
        }
    }
}
