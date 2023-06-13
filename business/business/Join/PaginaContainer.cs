using business.div;
using System;

namespace business.Join
{

   public class PaginaContainer 
   {
         public Int64? ContainerId { get; set; }
        public Int64? PaginaId { get; set; }
        public virtual Pagina? Pagina { get; set; }
        public virtual Container? Container { get; set; }
   }


}