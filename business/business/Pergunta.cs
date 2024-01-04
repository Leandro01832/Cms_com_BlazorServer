using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class Pergunta : BaseModel
    {
        public long UserPreferencesId { get; set; }
        public virtual UserQuestions UserPreferences { get; set; }
        public string? Questao { get; set; }
        public int p1 { get; set; }
        public int p2 { get; set; }
        public int p3 { get; set; }
        public int p4 { get; set; }
        public int p5 { get; set; }
        public int p6 { get; set; }
        public int p7 { get; set; }
        public int p8 { get; set; }
        public int p9 { get; set; }
        public int p10 { get; set; }
    }
}
