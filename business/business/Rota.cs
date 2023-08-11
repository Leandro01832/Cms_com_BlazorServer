using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Rota : BaseModel
    {
        public string? Nome { get; set; }
        public bool Registrado { get; set; }
    }
}
