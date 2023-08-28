using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Classificacao : BaseModel
    {
        [Key, ForeignKey("Pagina")]
        public new Int64 Id { get; set; }
        public int preferencia1 { get; set; }
        public int preferencia2 { get; set; }
        public int preferencia3 { get; set; }
        public int preferencia4 { get; set; }
        public int preferencia5 { get; set; }
        public int preferencia6 { get; set; }
        public int preferencia7 { get; set; }
        public int preferencia8 { get; set; }
        public int preferencia9 { get; set; }
        public int preferencia10 { get; set; }

        public virtual Pagina? Pagina { get; set; }
    }
}
