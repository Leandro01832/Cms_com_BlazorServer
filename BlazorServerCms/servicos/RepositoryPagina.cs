using BlazorServerCms.Data;
using business;
using business.business;
using business.business.Group;
using business.Group;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NVelocity;
using NVelocity.App;
using System.Text;

namespace BlazorServerCms.servicos
{
    public class RepositoryPagina 
    {

        public IConfiguration Configuration { get; }
        public  HttpClient Http { get; }

        public Random random = new Random();

        public bool erro = false;
        public bool exibir = true;

        public int QuantMinutos { get; set; } = 30;
        public int quantSlidesCarregando { get; set; } = 90;

        public int dias { get; set; } = 10;  
        public int meta1 { get; set; } = 1;  
        public int meta2 { get; set; } = 1;  
        public int meta3 { get; set; } = 1;  
        public int meta4 { get; set; } = 1;  
        public int meta5 { get; set; } = 1;  
        public int meta6 { get; set; } = 1;  
        public int meta7 { get; set; } = 1;  
        public int meta8 { get; set; } = 1;  
        public int metaTime { get; set; } = 1;  

        public int? CapituloLivro { get; set; } = 1;

        public int diaCupom = 0;

        public string cupomDesconto = "";

        public static List<Content> Conteudo = new List<Content>();
        public static List<UserModel> UserModel = new List<UserModel>();
        public static List<Story> stories = new List<Story>();
        public static List<FiltroContentIndice> conteudoEmFiltro = new List<FiltroContentIndice>();

        public RepositoryPagina(IConfiguration configuration, HttpClient http)
        {
            Configuration = configuration;
            Http = http;
        }

       static string path = Directory.GetCurrentDirectory();
         


        public  async Task<string> Verificar(string url)
        {
            var html = await Http.GetStringAsync(url);

            if (html.Contains("<h1>Pagina não encontrada</h1>"))
                return null;
            else
                return html;
        }


        public static string Capa { get { return System.IO.File.ReadAllText(path + "/wwwroot/Arquivotxt/Capa.html"); } }  
        public  string textDefault {  get  { return System.IO.File.ReadAllText(path + Configuration.GetConnectionString("path")); } }


        //140 linhas



        public string buscarApiYoutube()
        {
            return Configuration.GetConnectionString("api_youtube");
        }

        public string buscarApiGemini()
        {
            return Configuration.GetConnectionString("api_gemini");

        }
        
        public string buscarApiGoogleAnalytics()
        {
            return Configuration.GetConnectionString("api_google_analytics");

        }


        public string buscarDominio()
        {
            var dominio = Configuration.GetConnectionString("dominio");
            return dominio;
        } 
        
        public int buscarCamada()
        {
            var camada = Configuration.GetConnectionString("camada");
            int cam = int.Parse(camada);
            return cam;
        }

        public string buscarEcommerce()
        {
            var dominio = Configuration.GetConnectionString("ecommerce");
            return dominio;
        }


        public async Task<List<PatternStory>> buscarPatternStory()
        {
            List<PatternStory> lista = new List<PatternStory>();
            var dom = buscarDominio();
            var text = await Http.GetStringAsync($"https://{dom}/Arquivotxt/stories.txt");

            var stories = text.Split('-');

            for (int i = 0; i < stories.Length; i++)
            {
                lista.Add(new PatternStory
                {
                    PaginaPadraoLink = i,
                    Nome = stories[i].TrimStart().TrimEnd()
                });
            }

            return lista;
        }


        public async Task<string> renderizarPagina(Content content)
        {
            var dom = buscarDominio();
            var text = await Http.GetStringAsync($"https://{dom}/Arquivotxt/default.html");
           
            var resultado = renderizar(content, text!);
            return resultado;
        }       
       

        public string renderizar(Content pagina, string TextoHtml)
        {     
            Velocity.Init();
            var Modelo = new
            {
                Pagina = pagina,
                titulo = pagina.Titulo
            };

            var velocitycontext = new VelocityContext();
            velocitycontext.Put("model", Modelo);
            var html = new StringBuilder();
            bool result = Velocity.Evaluate(velocitycontext, new StringWriter(html), "NomeParaCapturarLogError",
            new StringReader(TextoHtml));
            
            return html.ToString();
        }

        public IIncludableQueryable<Pagina, Story> includes()
        {
            DemoContextFactory db = new DemoContextFactory();
            ApplicationDbContext Contexto = db.CreateDbContext(null);
            return Contexto.Pagina!
             .Include(p => p.Story)!;
        }

    }

}
