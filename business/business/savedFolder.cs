using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class savedFolder : BaseModel
    {
        public string? user { get; set; }

        public long FiltroId { get; set; }

        public virtual Filtro Filtro { get; set; }
    }
}
