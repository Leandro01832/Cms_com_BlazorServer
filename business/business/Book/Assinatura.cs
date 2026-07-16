using business.business.sistema;

namespace business.business.Book
{
    public class Assinatura : BaseModel
    {
        public  string? UserModelId { get; set; }
        public virtual UserModel? UserModel { get; set; }

        
        public virtual Livro? Livro { get; set; }


    }
}
