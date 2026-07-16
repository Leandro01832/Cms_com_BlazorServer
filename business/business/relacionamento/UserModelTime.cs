using business.business.sistema;

namespace business.business.relacionamento
{
    public class UserModelTime
    {
        public string UserModelId { get; set; }
        public long TimeId { get; set; }
        public virtual UserModel UserModel { get; set; }
        public virtual Time Time { get; set; }

    }
}
