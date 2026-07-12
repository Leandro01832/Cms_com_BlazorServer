
using business;
using business.business;

public class MarcacaoVideoFilter : BaseModel
{
    public long ContentId { get; set; }
    public virtual Content Content { get; set; }
    public int Segundos { get; set; }
}