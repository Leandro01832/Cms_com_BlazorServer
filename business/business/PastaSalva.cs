using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace business.business
{
    public class PastaSalva : BaseModel
    {
        [Key, ForeignKey("Filtro")]
        public new long Id { get; set; }
        public virtual Filtro Filtro { get; set; }

        public virtual List<UserModelPastaSalva> UserModel { get; set; }
    }
}
