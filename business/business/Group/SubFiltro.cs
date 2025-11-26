namespace business.Group
{
    public class SubFiltro : Filtro
    {
        
        public Int64 FiltroId { get; set; }
        public virtual Filtro? Filtro { get; set; }
    }
}