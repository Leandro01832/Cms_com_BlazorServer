using business;

public class Camada : BaseModel
{
    public int Numero { get; set; }
    public virtual List<Criterio> Criterio { get; set; }


}