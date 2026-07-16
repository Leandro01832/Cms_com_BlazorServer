using business.business.conteudo;

namespace business.business.Contrato
{
    public interface IMudancaEstado
    {
        Pagina MudarEstado( UserContent m, long curtidas, long compartilhamentos);
    }
}
