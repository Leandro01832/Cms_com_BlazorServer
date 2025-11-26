using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using business;

public class Criterio : BaseModel
{
    [Key, ForeignKey("Filtro")]
    public new Int64 Id { get; set; }

    public virtual Filtro Filtro { get; set; }
    public int Camada { get; set; }
    public string Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }

    public Criterio()
    {
        DataCriacao = DateTime.Now;
        Ativo = true;
    }
}