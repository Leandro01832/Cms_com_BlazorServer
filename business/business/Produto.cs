using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace 
    business
{
    public class Produto : BaseModel
    {
            [Key, ForeignKey("Pagina")]
            public new Int64 Id { get; set; }
            public string? Descricao { get; set; }     
            public string? Nome { get; set; }     
            public decimal Preco { get; set; }
            public int QuantEstoque { get; set; }
             public virtual List<ItemPedido>? Itens { get; set; }   
             public virtual Pagina? Pagina { get; set; }   
             public virtual List<ImagemProduto>? Imagem { get; set; }   
    }
}