using business.business.Book;
using Microsoft.AspNetCore.Identity;

namespace business.business
{
    public class UserModel : IdentityUser
    {
       
        public string? Image { get; set; }

        public int PontosPorDia { get; set; }

        public DateTime DataPontuacao { get; set; }

        public int Recorde { get; set; }

        public long? FiltroId { get; set; }
        public virtual Filtro Filtro { get; set; }

        public virtual List<UserModelLivro> Livro { get; set; }

        public virtual List<UserModelTime> Time { get; set; }

        public virtual List<UserModelFiltro> savedFolder { get; set; }

        public virtual List<ContentUser> conteudos { get; set; }
        public virtual List<UserModelPageLiked> PageLiked { get; set; }

        public void IncluiTime(Time time)
        {
            if (this.Time == null) this.Time = new List<UserModelTime>();
            this.Time!.Add(new UserModelTime { Time = time, UserModel = this });
        }
        
        public void IncluiLivro(Livro livro)
        {
            if (this.Livro == null) this.Livro = new List<UserModelLivro>();
            this.Livro!.Add(new UserModelLivro { Livro = livro, UserModel = this });
        }

        public void salvarPasta(Filtro filtro)
        {
            if (this.savedFolder == null) this.savedFolder = new List<UserModelFiltro>();
            this.savedFolder!.Add(new UserModelFiltro { Filtro = filtro, UserModel = this });
        }
        
        public void curtir(PageLiked page)
        {
            if (this.PageLiked == null) this.PageLiked = new List<UserModelPageLiked>();
            this.PageLiked!.Add(new UserModelPageLiked { PageLiked = page, UserModel = this });
        }
    }
}
