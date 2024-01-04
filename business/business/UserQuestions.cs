using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserQuestions : BaseModel
    {
        public string user { get; set; }
        public int capitulo { get; set; }
        public int pasta { get; set; }

        public virtual List<Pergunta>? Pergunta { get; set; }
    }
}
