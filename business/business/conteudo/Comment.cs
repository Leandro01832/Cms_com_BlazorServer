
using business.business.sistema;

namespace  business.business.conteudo
{

    public class Comment : BaseModel
    {         
        public string? Comentar { get; set; }

          public Int64? ContentId { get; set; }
          public virtual Content? Content { get; set; }

          public string UserModelId { get; set; }
        public virtual UserModel UserModel { get; set; }

        public override string ToString()
        {
            return "Comentários";
        }
    }

} 