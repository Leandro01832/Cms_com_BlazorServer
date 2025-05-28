using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business.Book
{
    public class Assinatura : BaseModel
    {
        public  string? UserModelId { get; set; }
        public virtual UserModel? UserModel { get; set; }

        
        public virtual Livro? Livro { get; set; }


    }
}
