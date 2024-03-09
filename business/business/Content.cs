using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Content : BaseModel
    {
        public long highlighterId { get; set; }
        public highlighter highlighter { get; set; }
        public string Html { get; set; }
    }
}
