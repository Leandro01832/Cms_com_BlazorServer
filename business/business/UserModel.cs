using System.ComponentModel.DataAnnotations.Schema;
using business.business.Book;
using Microsoft.AspNetCore.Identity;

namespace business.business
{
    public class UserModel : IdentityUser
    {
        public UserModel()
        {

        }


        public string? HashUserName { get; set; }
        public string? Compartilhar { get; set; } = null;
        public string? Image { get; set; }

        public int PontosPorDia { get; set; }

        public DateTime DataPontuacao { get; set; }

        public int Recorde { get; set; }

        public virtual  List<Assinatura> Assinatura { get; set; }

        public virtual List<UserModelLivro> Livro { get; set; }

        public virtual List<UserModelTime> Time { get; set; }

        public virtual List<UserModelFiltro> Pastas { get; set; }

        public virtual List<UserModelPastaSalva> PastaSalvas { get; set; }

        public virtual List<UserContent> conteudos { get; set; }
        public virtual List<UserModelContent> PageLiked { get; set; }

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

        public void incluirPasta(Filtro filtro)
        {
            if (this.Pastas == null) this.Pastas = new List<UserModelFiltro>();
            this.Pastas!.Add(new UserModelFiltro { Filtro = filtro, UserModel = this });
        }

        public void incluirPastaSalva(PastaSalva pasta)
        {
            if (this.PastaSalvas == null) this.PastaSalvas = new List<UserModelPastaSalva>();
            this.PastaSalvas!.Add(new UserModelPastaSalva { PastaSalva = pasta, UserModel = this });
        }

        public void curtir(Content page)
        {
            if (this.PageLiked == null) this.PageLiked = new List<UserModelContent>();
            this.PageLiked!.Add(new UserModelContent { Content = page, UserModel = this });
        }

       
    }
}
