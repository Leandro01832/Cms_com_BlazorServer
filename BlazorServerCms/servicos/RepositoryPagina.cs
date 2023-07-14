using BlazorServerCms.Data;
using business;
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
        public static string conexao = "Data Source=DESKTOP-7TI5J9C\\SQLEXPRESS;Initial Catalog=BlazorCms;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        public ApplicationDbContext Context =  new ApplicationDbContext(conexao);

        public RepositoryPagina(IConfiguration configuration, HttpClient http)
        {
            Configuration = configuration;
            Http = http;
        }

       static string path = Directory.GetCurrentDirectory();
        // public static string outroLivro = "https://localhost:5001";    

        //  public  static int? outroCapitulo = 1;  

        // public static Random randNum = new Random();

        // public static string[] livros = new string[10];

        //public static string Verificar(string url)
        // {     

        //       WebClient client = new WebClient();
        //       var html = client.DownloadString(url);                        

        //       if( html.Contains("<h1>Pagina não encontrada</h1>"))
        //       return null!;
        //       else 
        //       return html; 
        // }  

        //public static int RetornarCapitulos(string livro)
        //{
        //  int cap = 1;
        //   var url = livro + "/Renderizar/" + cap + "/" + 1 + "/1/user";   
        //   var html = "teste";

        //   while(html != null)
        //   {
        //      html = Verificar(url);
        //      if(html != null)
        //      {
        //          cap++;
        //          url = livro + "/Renderizar/" + cap + "/" + 1 + "/1/user";   
        //      }

        //   }
        //  return cap;
        //}  

        public int buscarCount()
        {
            SqlConnection con;
            SqlCommand cmd;
            var _TotalRegistros = 0;
            try
            {
                using (con = new SqlConnection(conexao))
                {
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Pagina", con);
                    con.Open();
                    _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString()!);
                    con.Close();
                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }
            return _TotalRegistros;
        }

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
        public string CodCss { get { return System.IO.File.ReadAllText(Path.Combine(path + "/wwwroot/Arquivotxt/DocCss.cshtml")); } }  
        //93 linhas
        public string CodCss2 { get { return System.IO.File.ReadAllText(Path.Combine(path + "/wwwroot/Arquivotxt/DocCssDiv.cshtml")); } }  

        //350 linhas
        public string CodigoBloco { get { return System.IO.File.ReadAllText(Path.Combine(path + "/wwwroot/Arquivotxt/DocBloco.cshtml")); } }
        
        //515 linhas
        public string CodigoProducao { get { return System.IO.File.ReadAllText(Path.Combine(path + "/wwwroot/Arquivotxt/DocProducao.cshtml")); } }
        
        //7 linhas
        public string CodigoMusic { get { return System.IO.File.ReadAllText(Path.Combine(path + "/wwwroot/Arquivotxt/music.cshtml")); } }

        // 30 linhas
        public string CodigoCarousel { get { return System.IO.File.ReadAllText(Path.Combine(path + "/wwwroot/Arquivotxt/Carousel.cshtml")); } }

        public string CodigoVideos { get { return System.IO.File.ReadAllText(Path.Combine(path + "/wwwroot/Arquivotxt/videos.txt")); } }


        public List<Pagina>? paginas { get; set; }
        public bool aguarde { get; set; } = false;

        public string buscarDominio()
        {
            var dominio = Configuration.GetConnectionString("dominio");
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

        public async Task<List<Pagina>> buscarCapitulo( int capitulo)
        {
            return await includes()
            .Where(p => p.Story!.PaginaPadraoLink == capitulo).ToListAsync();
        }

        public async Task buscarCapitulos()
        {
            var lista = await includes()
            .ToListAsync();

            paginas!.Clear();
            paginas.AddRange(lista);
        }

        public IIncludableQueryable<Pagina, List<Pagina>> includes()
        {
            return Context.Pagina!
             .Include(p => p.Produto)
             .ThenInclude(p => p!.Imagem)
             .Include(p => p.Produto)
             .ThenInclude(p => p!.Itens)
             .Include(p => p.Story)
             .ThenInclude(b => b!.Pagina)
             .Include(p => p.Story)
             .ThenInclude(b => b!.Filtro)

             .Include(b => b.SubStory).ThenInclude(b => b!.Pagina)
             .Include(b => b.SubStory).ThenInclude(b => b!.Grupo)
             .Include(b => b!.Grupo).ThenInclude(b => b!.Pagina)
             .Include(b => b!.Grupo).ThenInclude(b => b!.SubGrupo)
             .Include(b => b!.SubGrupo).ThenInclude(b => b!.Pagina)
             .Include(b => b!.SubGrupo).ThenInclude(b => b!.SubSubGrupo)
             .Include(b => b!.SubSubGrupo).ThenInclude(b => b!.Pagina)
             .Include(b => b!.SubSubGrupo).ThenInclude(b => b!.CamadaSeis)
             .Include(b => b!.CamadaSeis).ThenInclude(b => b!.Pagina)
             .Include(b => b!.CamadaSeis).ThenInclude(b => b!.CamadaSete)
             .Include(b => b!.CamadaSete).ThenInclude(b => b!.Pagina)
             .Include(b => b!.CamadaSete).ThenInclude(b => b!.CamadaOito)
             .Include(b => b!.CamadaOito).ThenInclude(b => b!.Pagina)
             .Include(b => b!.CamadaOito).ThenInclude(b => b!.CamadaNove)
             .Include(b => b!.CamadaNove).ThenInclude(b => b!.Pagina)
             .Include(b => b!.CamadaNove).ThenInclude(b => b!.CamadaDez)
             .Include(b => b!.CamadaDez).ThenInclude(b => b!.Pagina)!;
        }
    }
}
