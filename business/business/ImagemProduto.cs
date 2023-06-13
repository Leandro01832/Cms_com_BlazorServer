using System.ComponentModel.DataAnnotations;
using System;

namespace business
{
    public class ImagemProduto : BaseModel
    {
        public Int64 ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }    
        [Display(Name = "Arquivo")]
        public string? ArquivoImagem { get; set; }
        public int WidthImagem { get; set; }     
    }
}