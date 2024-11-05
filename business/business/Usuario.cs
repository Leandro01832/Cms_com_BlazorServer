using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Usuario : BaseModel
    {
        public Usuario() { }
        public string user { get; set; }
        public virtual List<UsuarioTime> times { get; set; }

        public void IncluiTime(Time time)
        {
            if (this.times == null) this.times = new List<UsuarioTime>();
            this.times!.Add(new UsuarioTime { Time = time, Usuario = this });
        }
    }
}
