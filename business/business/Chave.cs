using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Chave : Pagina
    {
        // Nunca será removido da pasta e a pasta sempre terá uma chave
        
        public Chave() : base()
        {
        }


        public Chave(int count) : base(count)
        {
        }
    }
}
