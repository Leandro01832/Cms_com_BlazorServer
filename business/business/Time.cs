using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Time : BaseModel
    {
        public string? nome { get; set; }
        public string? user { get; set; }

        // Clicou no botão comprar
        public int vendas { get; set; }
        public virtual List<UsuarioTime> usuarios { get; set; }
    }
}
