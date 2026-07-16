using System.ComponentModel.DataAnnotations;

namespace business.business.ecommerce
{
    public class Cliente : BaseModel
    {
        [Display(Name = "Primeiro nome")]
        public string FirstName { get; set; }
        [Display(Name = "Segundo nome")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string UserName { get; set; }
        public string Cpf { get; set; }
        public virtual Telefone Telefone { get; set; }
        public virtual Endereco Endereco { get; set; }
    }
}
