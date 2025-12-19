using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
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

        public int Posicao { get; set; }

        public virtual Content Content { get; set; }
    }
}
