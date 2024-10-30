using business.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Content : BaseModel
    {
        private DateTime data = DateTime.Now;

        public long? FiltroId { get; set; }

        public virtual  Filtro Filtro { get; set; }
        public string Html { get; set; }

        public DateTime Data { get { return data; } set { data = value; } }

        public string? user { get; set; }
    }
}
