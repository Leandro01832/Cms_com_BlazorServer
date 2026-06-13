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
        public int vendas { get; set; }
        public virtual List<UserModelTime> usuarios { get; set; }
        public virtual RelogioParede RelogioParede { get; set; }


    }
}
