using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Pergunta : BaseModel
    {
        public long FiltroId { get; set; }
        public virtual Filtro Filtro { get; set; }
        public string? Questao { get; set; }
        public string? ResponseChatGpt { get; set; }
    }
}
