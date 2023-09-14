using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace business.business
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
