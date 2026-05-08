using business;

public class VideoFilter : Pagina
{
    public virtual List<MarcacaoVideoFilter> Marcacao { get; set; }

    public override string ToString()
    {
        return "Video em filtros";
    }
}