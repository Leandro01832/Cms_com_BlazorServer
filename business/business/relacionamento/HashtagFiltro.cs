using business.business.Group;
using business.business.sistema;

namespace business.business.relacionamento
{

        public class HashtagFiltro 
        {
            public virtual Hashtag Hashtag { get; set; }
            public long HashtagId { get; set; }
            public virtual Filtro Filtro { get; set; }
            public long FiltroId { get; set; }
            
        }

}