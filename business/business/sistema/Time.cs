using business.business.relacionamento;

namespace business.business.sistema
{
    public class Time : BaseModel
    {
       
        public string? nome { get; set; }        
        public int vendas { get; set; }
        public virtual List<UserModelTime> usuarios { get; set; }
        public virtual RelogioParede RelogioParede { get; set; }


    }
}
