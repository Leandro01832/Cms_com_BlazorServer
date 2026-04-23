using business;

namespace business.business
{

public class HashtagFiltro 
{
    public virtual Hashtag Hashtag { get; set; }
    public long HashtagId { get; set; }
    public virtual Filtro Filtro { get; set; }
    public long FiltroId { get; set; }
}

}