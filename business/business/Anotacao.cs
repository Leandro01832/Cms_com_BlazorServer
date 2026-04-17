using business;
using business.business;
using business.business.Book;

public class Anotacao : BaseModel
{
    // Anota versiculos que foram pesquisados com mais criterio,
    //  ou seja, que o usuario quer guardar para ler depois, ou compartilhar com amigos
    public DateTime DataCriacao { get; set; }
    public long? LivroId { get; set; }
    public virtual Livro Livro { get; set; }
    public int Capitulo { get; set; }
    public string Query { get; set; }

    public string UserModelId { get; set; }

    public virtual UserModel UserModel { get; set; }

    public virtual List<AnotacaoVersiculo> AnotacaoVersiculos { get; set; }

        public void FazerAnotacao(Filtro filtro)
            {
                if (this.AnotacaoVersiculos == null) this.AnotacaoVersiculos = new List<AnotacaoVersiculo>();
                this.AnotacaoVersiculos!.Add(new AnotacaoVersiculo { Anotacao = this, Filtro = filtro });
            }
    
           


}

