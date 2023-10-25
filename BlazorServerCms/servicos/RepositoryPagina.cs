using BlazorServerCms.Data;
using business;
using business.business;
using business.Group;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NVelocity;
using NVelocity.App;
using System.Net;
using System.Text;

namespace BlazorServerCms.servicos
{
    public class RepositoryPagina 
    {
        public IConfiguration Configuration { get; }
        public  HttpClient Http { get; }
        public Random random = new Random();
        public List<Pagina>? paginas = new List<Pagina>();
        public bool aguarde { get; set; } = false;
        public int diaCupom = 0;
        public string cupomDesconto = "";

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

        private async Task<string> texto()
        {           
            return await Http.GetStringAsync("Arquivotxt/default.md");  

        }

        public static string Capa { get { return System.IO.File.ReadAllText(path + "/wwwroot/Arquivotxt/Capa.html"); } }  
        public  string textDefault {  get  { return System.IO.File.ReadAllText(path + Configuration.GetConnectionString("path")); } }
        //140 linhas

        

        public string buscarDominio()
        {
            var dominio = Configuration.GetConnectionString("dominio");
            return dominio;
        }

        public string buscarAdmin()
        {
            var dominio = Configuration.GetConnectionString("admin");
            return dominio;
        }

        public string buscarEcommerce()
        {
            var dominio = Configuration.GetConnectionString("ecommerce");
            return dominio;
        }


        public async Task<string> renderizarPagina(Pagina pagina)
        {
        var text = await Http.GetStringAsync("https://localhost:7224/Arquivotxt/default.html");
           
            var resultado = renderizar(pagina, text!);
            return resultado;
        }       
       

        public string renderizar(Pagina pagina, string TextoHtml)
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

        public IIncludableQueryable<Pagina, Pagina> includes()
        {
            DemoContextFactory db = new DemoContextFactory();
            ApplicationDbContext Contexto = db.CreateDbContext(null);
            return Contexto.Pagina!
             .Include(p => p.Produto)
             .ThenInclude(p => p!.Imagem)
             .Include(p => p.Filtro)!
             .ThenInclude(b => b!.Filtro)!
             .Include(p => p.Story)
             .ThenInclude(b => b!.Filtro)!
             .ThenInclude(b => b!.Pagina)!
             .ThenInclude(b => b!.Pagina)!;
        }
        
        

    }

}
