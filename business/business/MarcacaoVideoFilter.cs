
using business;

public class MarcacaoVideoFilter : BaseModel
{
    public long VideoFilterId { get; set; }
    public virtual VideoFilter VideoFilter { get; set; }
    public int Segundos { get; set; }
}