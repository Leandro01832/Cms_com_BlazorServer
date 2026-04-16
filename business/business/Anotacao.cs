using business;
using business.business.Book;

public class Anotacao : BaseModel
{
    // Anota versiculos que foram pesquisados com mais criterio,
    //  ou seja, que o usuario quer guardar para ler depois, ou compartilhar com amigos
    public DateTime DataCriacao { get; set; }
    public long LivroId { get; set; }
    public virtual Livro Livro { get; set; }
    public int Capitulo { get; set; }
    public string Compartilhar { get; set; }
}