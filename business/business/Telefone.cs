using business.business;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business
{
    public class Telefone : BaseModel
    {
        [Key, ForeignKey("Cliente")]
        public new long Id { get; set; }
        [Required(ErrorMessage = "DDD é obrigatório")]
        public string? DDD_Celular { get; set; }
        [Required(ErrorMessage = "Celular é obrigatório")]
        public string? Celular { get; set; }
        public string? DDD_Telefone { get; set; }
        public string? Fone { get; set; }        
        public virtual Cliente Cliente { get; set; }
    }
}
