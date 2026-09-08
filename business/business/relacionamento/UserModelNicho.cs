using business.business.Group;
using business.business.sistema;

namespace business.business.relacionamento
{
    public class UserModelNicho
    {
        public string UserModelId { get; set; }
        public long NichoId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Nicho Nicho { get; set; }

        // Define qual o nicho/categoria principal do estabelecimento
        public bool IsPrincipal { get; set; }
    }    
}
