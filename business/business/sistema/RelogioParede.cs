using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using business.business.conteudo;
using business.business.Group;

namespace business.business.sistema
{
    public class RelogioParede : BaseModel
    { 
        [Key, ForeignKey("Time")]
        public new long Id { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public virtual Time Time { get; set; }
        public long SubFiltroId { get; set; }
        public virtual SubFiltro SubFiltro {get;set;}
        public long ContentId { get; set; }    
        public virtual Content Content { get; set; }

        //responsável
        public string UserModelId { get; set; }
        public virtual UserModel UserModel {get;set;}
        
    }
    
}
