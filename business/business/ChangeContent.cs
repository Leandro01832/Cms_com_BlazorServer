using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using business.Group;

namespace business.business
{
    public class ChangeContent : Pagina
    {
        public ChangeContent() : base()
        {
        }

        public ChangeContent(Story story) : base(story)
        {
        }

        public ChangeContent(int count) : base(count)
        {
        }

        public virtual MudancaEstado MudancaEstado { get; set; }
    }
}
