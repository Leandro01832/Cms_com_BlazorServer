using business.business.Group;
using business.business.sistema;

namespace business.business.relacionamento
{
    public class UserModelFiltro
    {
        public string UserModelId { get; set; }
        public long FiltroId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Filtro Filtro { get; set; }

    }
}
