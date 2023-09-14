using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Endereco : BaseModel
    {
        [Key, ForeignKey("Cliente")]
        public new int Id { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Rua { get; set; }
        public long Numero { get; set; }
        public string Cep { get; set; }
        public string Complemento { get; set; }
        public virtual Cliente Cliente { get; set; }
    }
}
