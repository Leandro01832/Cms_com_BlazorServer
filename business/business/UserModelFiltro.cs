using business.business.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserModelFiltro
    {
        public string UserModelId { get; set; }
        public long FiltroId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Filtro Filtro { get; set; }

    }
}
