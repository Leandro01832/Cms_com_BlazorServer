using business.business.conteudo;
using business.business.sistema;

namespace business.business.relacionamento
{
    public class UserModelContent
    {
        public string UserModelId { get; set; }
        public long ContentId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Content Content { get; set; }
    }
}
