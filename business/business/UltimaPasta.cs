using business;

namespace business.business
{

        public class UltimaPasta : Filtro
        {
            public Int64 FiltroId { get; set; }
            public virtual Filtro? Filtro { get; set; }
        }


}