using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using business;
using business.business.Book;

public class Criterio : BaseModel
{   

    public virtual List<Filtro> Filtro { get; set; }
    public long CamadaId { get; set; }
    public virtual Camada Camada { get; set; }
    public string Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public Int64? LivroId { get; set; } = null;
        public virtual Livro? Livro { get; set; }
    public bool Ativo { get; set; }

    public Criterio()
    {
        DataCriacao = DateTime.Now;
        Ativo = true;
    }
}