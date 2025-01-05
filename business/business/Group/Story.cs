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
            Nome = nome;
            if (padrao != null)
                Pagina = new List<Content>
            {
                new Pagina(1)
                {
                    Html = $"<p> Seja bem-vindo a story {this.Nome}  </p>",
                    Titulo = Nome,
                    Versiculo = 1,
                    StoryId = this.Id
                }
            };
            if(padrao != null)
            padrao.Pagina.Add(new Pagina(padrao)
            {
                Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {this.Nome} </h1> </a>",
                Titulo = "capitulos",
            });
        }

        public Story(string nome, int quantidade, Story padrao)
        {
            this.Nome = nome;
            if (padrao != null)
                this.Pagina = new List<Content>
            {
                new Pagina(1)
                {
                    Html = $"<p> Seja bem-vindo a story {this.Nome}  </p>",
                    Titulo = this.Nome,
                    Versiculo = 1,
                    StoryId = this.Id
                }
            };
            if (padrao != null)
            padrao.Pagina.Add(new Pagina(padrao)
            {
                Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {this.Nome} </h1> </a>",
                Titulo = "capitulos",
            });

            if (this.Modelo == 1)
            {
                for (var i = 0; i < quantidade; i++)
                {
                    int vers = this.Pagina.OfType<Pagina>().OrderBy(p => p.Id).Last().Versiculo + 1 + i;
                    var v = vers.ToString();
                    if (v[v.Length - 1] == '0' ||
                        v[v.Length - 1] == '9' ||
                        v[v.Length - 1] == '8')
                    {
                        this.Pagina
                        .Add(new ChangeContent(vers)
                        {
                            StoryId = this.Id,
                            Html = $"<p> conteudo {vers} </p>",
                            Titulo = "pagina " + vers
                        });
                    }
                    else if (v[v.Length - 1] == '7')
                    {
                        this.Pagina
                        .Add(new AdminContent(vers)
                        {
                            StoryId = this.Id,
                            Html = $"<p> conteudo {vers} </p>",
                            Titulo = "pagina " + vers
                        });
                    }
                    else
                    {
                        this.Pagina
                        .Add(new ProductContent(vers)
                        {
                            StoryId = this.Id,
                            Html = $"<p> conteudo {vers} </p>",
                            Titulo = "pagina " + vers
                        });
                    }

                }
            }
            else
            {
                for (var i = 0; i < quantidade; i++)
                {
                    int vers = this.Pagina.OfType<Pagina>().OrderBy(p => p.Id).Last().Versiculo + 1 + i;
                    this.Pagina
                       .Add(new Pagina(vers)
                       {
                           StoryId = this.Id,
                           Html = $"<p> conteudo {vers} </p>",
                           Titulo = "pagina " + vers
                       });
                }
            }
        }

        public Story(string nome, List<Story> stories, Story padrao)
        {

            PaginaPadraoLink = stories.Count;
            Nome = nome;
            if (padrao != null)
                Pagina = new List<Content>
            {
                new Pagina(1)
                {
                    Html = $"<p> Seja bem-vindo a story {this.Nome}  </p>",
                    Titulo = Nome,
                    Versiculo = 1,
                    StoryId = this.Id
                }
            };
            if (padrao != null)
            padrao.Pagina.Add(new Pagina(padrao)
            {
                Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {this.Nome} </h1> </a>",
                Titulo = "capitulos",
            });
        }

        public Story(string nome, List<Story> stories, List<Content> conteudos, Story padrao)
        {

            PaginaPadraoLink = stories.Count;
            Nome = nome;
            if (padrao != null)
                Pagina = new List<Content>
            {
                new Pagina(1)
                {
                    Html = $"<p> Seja bem-vindo a story {this.Nome}  </p>",
                    Titulo = Nome,
                    Versiculo = 1,
                    StoryId = this.Id
                }
            };
            if (padrao != null)
            padrao.Pagina.Add(new Pagina(padrao)
            {
                Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {this.Nome} </h1> </a>",
                Titulo = "capitulos",
            });

            if (this.Modelo == 1)
            {
                int i = 0;
                foreach (var c in conteudos.Where(c => !string.IsNullOrEmpty(c.Html)).ToList())
                {
                    int vers = this.Pagina.OfType<Pagina>().OrderBy(p => p.Id).Last().Versiculo + 1 + i;
                    var v = vers.ToString();
                    if (v[v.Length - 1] == '0' ||
                        v[v.Length - 1] == '9' ||
                        v[v.Length - 1] == '8')
                    {
                        this.Pagina
                        .Add(new ChangeContent(vers)
                        {
                            StoryId = this.Id,
                            Html = c.Html,
                            Titulo = "pagina " + vers
                        });
                    }
                    else if (v[v.Length - 1] == '7')
                    {
                        this.Pagina
                        .Add(new AdminContent(vers)
                        {
                            StoryId = this.Id,
                            Html = c.Html,
                            Titulo = "pagina " + vers
                        });
                    }
                    else
                    {
                        this.Pagina
                    .Add(new ProductContent(vers)
                    {
                        StoryId = this.Id,
                        Html = c.Html,
                        Titulo = "pagina " + vers
                    });
                    }
                    i++;
                    c.Id = 0;
                }
            }
            else
            {
                var i = 0;
                foreach (var c in conteudos.Where(c => !string.IsNullOrEmpty(c.Html)).ToList())
                {
                    int vers = this.Pagina.OfType<Pagina>().OrderBy(p => p.Id).Last().Versiculo + 1 + i;
                    this.Pagina
                       .Add(new Pagina(vers)
                       {
                           StoryId = this.Id,
                           Html = c.Html,
                           Titulo = "pagina " + vers
                       });
                    i++;
                    c.Id = 0;
                }
            }
        }

        public Story(string nome, List<Content> conteudos, Story padrao)
        {
            this.Nome = nome;
            if (padrao != null)
                this.Pagina = new List<Content>
            {
                new Pagina(1)
                {
                    Html = $"<p> Seja bem-vindo a story {this.Nome}  </p>",
                    Titulo = this.Nome,
                    Versiculo = 1,
                    StoryId = this.Id
                }
            };
            if (padrao != null)
            padrao.Pagina.Add(new Pagina(padrao)
            {
                Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {this.Nome} </h1> </a>",
                Titulo = "capitulos",
            });

            if (this.Modelo == 1)
            {
                int i = 0;
                foreach (var c in conteudos.Where(c => !string.IsNullOrEmpty(c.Html)).OrderBy(c => c.Id).ToList())
                {
                    int vers = this.Pagina.OfType<Pagina>().OrderBy(p => p.Id).Last().Versiculo + 1 + i;
                    var v = vers.ToString();
                    if (v[v.Length - 1] == '0' ||
                        v[v.Length - 1] == '9' ||
                        v[v.Length - 1] == '8')
                    {
                        this.Pagina
                        .Add(new ChangeContent(vers)
                        {
                            StoryId = this.Id,
                            Html = c.Html,
                            Titulo = "pagina " + vers
                        });
                    }
                    else if (v[v.Length - 1] == '7')
                    {
                        this.Pagina
                        .Add(new AdminContent(vers)
                        {
                            StoryId = this.Id,
                            Html = c.Html,
                            Titulo = "pagina " + vers
                        });
                    }
                    else
                    {
                            this.Pagina
                        .Add(new ProductContent(vers)
                        {
                            StoryId = this.Id,
                            Html = c.Html,
                            Titulo = "pagina " + vers
                        });
                    }
                    i++;
                    c.Id = 0;
                }
            }
            else
            {
                var i = 0;
                foreach (var c in conteudos.Where(c => !string.IsNullOrEmpty(c.Html)).OrderBy(c => c.Id).ToList())
                {
                    int vers = this.Pagina.OfType<Pagina>().OrderBy(p => p.Id).Last().Versiculo + 1 + i;
                    this.Pagina
                       .Add(new Pagina(vers)
                       {
                           StoryId = this.Id,
                           Html = c.Html,
                           Titulo = "pagina " + vers
                       });
                    i++;
                    c.Id = 0;
                }
            }
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
        public virtual List<Content>? Pagina { get; set; }
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
