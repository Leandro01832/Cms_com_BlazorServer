using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Contrato
{
    public interface IMudancaEstado
    {
        Pagina MudarEstado( UserContent m, long curtidas, long compartilhamentos);
        Pagina MudarEstado2( Comment m, long curtidas, long compartilhamentos);
    }
}
