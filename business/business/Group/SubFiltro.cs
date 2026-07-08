using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace business.business.Group
{
    public class SubFiltro : Filtro
    {
        public long? ComCriterio { get; set; }
        public Int64? FiltroId { get; set; }
        public virtual Filtro? Filtro { get; set; }
        public bool UltimaPasta { get; set; }   

        // Permitir compartilhar itens se for false && não exclusão destes itens    
        public bool Embaralhar { get; set; }       

        public virtual List<Relogio> Relogio {get;set;}
        public virtual List<RelogioParede> RelogioParede {get;set;}
    }
}