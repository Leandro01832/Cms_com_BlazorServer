using System.ComponentModel.DataAnnotations.Schema;
using business.business;
using business.business.Group;

namespace business.Group
{
    public class Story : BaseModel
    {

        public Story()
        {
            
        }

        public Story(string nome, Story padrao)
        {
            Content[] pages = null;
            if(this is PatternStory) pages = new Content[99999];
            if(this is SmallStory) pages = new Content[9999];
            if(this is ShortStory) pages = new Content[999];
            Pagina = new List<Content>();
            for (int i = 0; i < pages.Length; i++) 
            {
                string? codHtml = null;
                if (i == 0) codHtml = $"<p> Seja bem-vindo a story {this.Nome}  </p>";
                pages[i] = new Pagina(i + 1)
                {
                    Html = codHtml,
                    Titulo = Nome,
                    StoryId = this.Id
                };
            }
            Pagina.AddRange(pages);

            Nome = nome;
           
            if(padrao != null)
            padrao.Pagina.Add(new Pagina(padrao)
            {
                Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {this.Nome} </h1> </a>",
                Titulo = "capitulos",
            });
        }



        public Story(string nome, List<Story> stories, Story padrao)
        {

            PaginaPadraoLink = stories.Count;
            Nome = nome;
            Content[] pages = null;
            if (this is PatternStory) pages = new Content[99999];
            if (this is SmallStory) pages = new Content[9999];
            if (this is ShortStory) pages = new Content[999];
            Pagina = new List<Content>();
            for (int i = 0; i < pages.Length; i++)
            {
                string? codHtml = null;
                if (i == 0) codHtml = $"<p> Seja bem-vindo a story {this.Nome}  </p>";
                pages[i] = new Pagina(i + 1)
                {
                    Html = codHtml,
                    Titulo = Nome,
                    StoryId = this.Id
                };
            }
            Pagina.AddRange(pages);

            if (padrao != null)
            padrao.Pagina.Add(new Pagina(padrao)
            {
                Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {this.Nome} </h1> </a>",
                Titulo = "capitulos",
            });
        }





        private int paginaPadraoLink = 0;
        private int quantidade = 0;
        private int quantComentario = 0;


        private int modelo;
        public int Modelo 
        {
            get { if (this is PatternStory) return 1; else return 0; }
            set {  modelo = value; }
        }
        public virtual List<Filtro>? Filtro { get; set; }

        public string? Nome { get; set; }
        public string? Image { get; set; }
        public string? Descricao { get; set; }

        private List<Content>? pagina;
        public virtual List<Content>? Pagina 
        {
            get { return pagina.OrderBy(c => c.Id).ToList();  }
            set 
            {
                if(
                    value.Where(c => c is Pagina && this is PatternStory).ToList().Count < 99999 ||
                      value.Where(c => c is Pagina && this is SmallStory).ToList().Count < 9999 ||
                      value.Where(c => c is Pagina && this is ShortStory).ToList().Count < 999)

                pagina = value;
            }
        }
        public int PaginaPadraoLink
        {
            get
            {
                if (Nome == "Padrao") return 0;                
                return paginaPadraoLink;
            }
            set
            {   
                paginaPadraoLink = value;
            }
        }

        [NotMapped]
        public string CapituloComNome
        {
            get { return "Capitulo " + this.paginaPadraoLink + " - " + this.Nome; }
        } 

        [NotMapped]
        public int Quantidade
        {
            get { return quantidade; }
            set { quantidade = value; }
        }
       
        [NotMapped]
        public int QuantComentario
        {
            get { return quantComentario; }
            set { quantComentario = value; }
        }

    }
}
