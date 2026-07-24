
using business.business.conteudo;
using business.business.Group;

namespace business.business.sistema
{
    public class Relogio : BaseModel
    {
        public Relogio()
        {
            
        }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public long SubFiltroId { get; set; }
        public virtual SubFiltro SubFiltro {get;set;}
        public long ContentId { get; set; }    
        public virtual Content Content { get; set; }
        public long? HashtagId { get; set; }    
        public virtual Hashtag Hashtag { get; set; }
        public string UserModelId { get; set; }
        public virtual UserModel UserModel {get;set;}
    }
    
}
